/*
 * Copyright (c) 2006, Gregory Neverov
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
 * 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. 
 * 3. Neither the name of the author nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using Metaphor.Collections;

namespace Metaphor
{
	public static class Heap
	{
		internal static Stack<MTypeBuilder> allTypes = new Stack<MTypeBuilder>();
		internal static Stack<MFieldBuilder> allFields = new Stack<MFieldBuilder>();
		internal static Stack<MConstructorBuilder> allCtors = new Stack<MConstructorBuilder>();
		internal static Stack<MMethodBuilder> allMethods = new Stack<MMethodBuilder>();

		private static Stack<Tuple<int, int, int, int>> marks = new Stack<Tuple<int, int, int, int>>();

		internal static void Push()
		{
			marks.Push(new Tuple<int, int, int, int>(allTypes.Count, allFields.Count, allCtors.Count, allMethods.Count));
		}

		internal static void Pop()
		{
			Tuple<int, int, int, int> mark = marks.Pop();
			while (allTypes.Count > mark.fst) allTypes.Pop();
			while (allFields.Count > mark.snd) allFields.Pop();
			while (allCtors.Count > mark.trd) allCtors.Pop();
			while (allMethods.Count > mark.fth) allMethods.Pop();
		}

		public static MType CreateType(int sym, MType[] args)
		{
			foreach (MTypeBuilder type in allTypes)
				if (type.sym == sym) return type.MakeType(args);
			throw new Exception("Type not found.");
		}

		public static MFieldInfo CreateField(int sym, MType[] typeArgs)
		{
			foreach (MFieldBuilder field in allFields)
				if (field.sym == sym) return field.MakeField(typeArgs);
			throw new Exception("Field not found.");
		}

		public static MConstructorInfo CreateConstructor(int sym, MType[] typeArgs)
		{
			foreach (MConstructorBuilder ctor in allCtors)
				if (ctor.sym == sym) return ctor.MakeConstructor(typeArgs);
			throw new Exception("Constructor not found.");
		}

		public static MMethodInfo CreateMethod(int sym, MType[] typeArgs, MType[] methodArgs)
		{
			foreach (MMethodBuilder method in allMethods)
				if (method.sym == sym) return method.MakeMethod(typeArgs, methodArgs);
			throw new Exception("Method not found.");
		}

	}

	#region Top level
	public class MModuleBuilder
	{
		public List<MTypeBuilder> types;

		[NonSerialized]
		protected ModuleBuilder moduleBuilder;

		public MModuleBuilder()
		{
			this.types = new List<MTypeBuilder>();
		}

		public void AddTypeBuilder(MTypeBuilder type)
		{
			//MModuleBuilder.allTypes.Push(type);
			types.Add(type);
		}

		public MTypeBuilder LookupTypeBuilder(string name)
		{
			foreach (MTypeBuilder type in types)
				if (type.name == name) return type;
			return null;
		}

		public void Save(string name)
		{
			Save(name, string.Format(".\\{0}.exe", name));
		}

		public void Save(string name, string fileName)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = name;
			string dir = Path.GetDirectoryName(fileName);
			AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save, dir != string.Empty ? dir: null);
			ModuleBuilder module = assembly.DefineDynamicModule(name, Path.GetFileName(fileName));
			ModuleGenState state = new ModuleGenState(assembly, module);
			this.GenerateClosures(new TypeCheckState(state.cspStore, null));
			this.GenerateTypes(state);
			this.GenerateMembers(state);
			this.GenerateCode(state);
			this.GenerateFinal(state);
			state.FinalizeCsp(true);
			assembly.Save(Path.GetFileName(fileName));
		}

		public object Run()
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MetaphorRun";
			AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder module = assembly.DefineDynamicModule("MetaphorRun");
			ModuleGenState state = new ModuleGenState(assembly, module);
			this.GenerateClosures(new TypeCheckState(state.cspStore, null));
			this.GenerateTypes(state);
			this.GenerateMembers(state);
			this.GenerateCode(state);
			this.GenerateFinal(state);
			state.FinalizeCsp(false);
			if (state.entryPoint != null)
			{
				MethodBuilder main = state.entryPoint;
				Type mainType = module.GetType(main.DeclaringType.FullName);
				//ParameterInfo[] paramInfos = main.GetParameters();
				//Type[] paramTypes = new Type[paramInfos.Length];
				//for(int i = 0; i < paramTypes.Length; i++)
				//    paramTypes[i] = paramInfos[i].ParameterType;
				MethodInfo mainMethod = mainType.GetMethod(main.Name, Type.EmptyTypes);
				if(mainMethod != null) return mainMethod.Invoke(null, null);
				mainMethod = mainType.GetMethod(main.Name, new Type[] { typeof(string[]) });
				if (mainMethod != null) return mainMethod.Invoke(null, new object[] { new string[] { } });
				throw new Exception("Invalid arguments on main method.");
			}
			else return null;
		}

		internal void GenerateClosures(TypeCheckState state)
		{
			foreach (MTypeBuilder type in types) type.GenerateClosures(state);
		}

		internal void GenerateTypes(ModuleGenState state)
		{
			moduleBuilder = state.module;
			foreach (MTypeBuilder type in types) type.GenerateTypes(state);
		}

		internal void GenerateMembers(ModuleGenState state)
		{
			foreach (MTypeBuilder type in types) type.GenerateMembers(state);
		}

		internal void GenerateCode(ModuleGenState state)
		{
			foreach (MTypeBuilder type in types) type.GenerateCode(state);
		}

		internal void GenerateFinal(ModuleGenState state)
		{
			foreach (MTypeBuilder type in types) type.GenerateFinal(state);
		}

		internal void CodeGenMeta(CodeGenState state)
		{
			state.MakeCode(typeof(MModuleBuilder));
			GenerateTypesMeta(state);
			GenerateMembersMeta(state);
			GenerateCodeMeta(state);
			state.code.Emit(OpCodes.Dup);
			state.code.Emit(OpCodes.Call, typeof(MModuleBuilder).GetMethod("Done"));
		}

		internal void GenerateTypesMeta(CodeGenState state)
		{
			for(int i = 0; i < types.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				types[i].GenerateTypesMeta(state);
				state.code.Emit(OpCodes.Call, typeof(MModuleBuilder).GetMethod("AddType"));
			}
		}

		internal void GenerateMembersMeta(CodeGenState state)
		{
			for (int i = 0; i < types.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.code.Emit(OpCodes.Ldfld, typeof(MModuleBuilder).GetField("types"));
				state.EmitInt(i);
				state.code.Emit(OpCodes.Call, typeof(List<MTypeBuilder>).GetMethod("get_Item"));
				types[i].GenerateMembersMeta(state);
			}
		}

		internal void GenerateCodeMeta(CodeGenState state)
		{
			for (int i = 0; i < types.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.code.Emit(OpCodes.Ldfld, typeof(MModuleBuilder).GetField("types"));
				state.EmitInt(i);
				state.code.Emit(OpCodes.Call, typeof(List<MTypeBuilder>).GetMethod("get_Item"));
				types[i].GenerateCodeMeta(state);
			}
		}

	}
	#endregion
	#region Type Variables
	public enum TypeKind { Any, Class, Struct };

	public sealed class TypeVarCon
	{
		public MType superType = null;
		public List<MType> interfaces = null;
		public TypeKind kind = TypeKind.Any;
		public bool hasNew = false;

		public TypeVarCon()
		{
		}

		internal void GenerateClosures(TypeCheckState state)
		{
			if(superType != null) superType.GenerateClosures(state);
			if (interfaces != null)
				foreach (MType iface in interfaces) iface.GenerateClosures(state);
		}

		public bool IsCompatibleType(MType type)
		{
			if (type == PrimType.Void) return false;
			if (superType != null && !type.IsSubTypeOf(superType)) return false;
			if (kind == TypeKind.Class && type.IsValueType()) return false;
			else if (kind == TypeKind.Struct && !type.IsValueType()) return false;
			if (hasNew && !type.HasDefaultConstructor()) return false;
			if (interfaces != null)
				foreach (MType iface in interfaces)
					if (!type.IsSubTypeOf(iface)) return false;
			return true;
		}

		public bool IsValid()
		{
			return
				(superType == null || kind == TypeKind.Any) &&
				(superType == null || !superType.IsValueType() || !hasNew) &&
				(kind != TypeKind.Struct || !hasNew);
		}

		public static TypeVarCon Combine(TypeVarCon con1, TypeVarCon con2)
		{
			TypeVarCon con = new TypeVarCon();
			if (con1.superType != null)
			{
				if (con2.superType != null)
				{
					if (con1.superType.IsSubTypeOf(con2.superType)) con.superType = con1.superType;
					else if (con2.superType.IsSubTypeOf(con1.superType)) con.superType = con2.superType;
					else return null;
				}
				else con.superType = con1.superType;
			}
			else if (con2.superType != null) con.superType = con2.superType;

			if (con1.interfaces != null || con2.interfaces != null)
			{
				con.interfaces = new List<MType>();
				if (con1.interfaces != null)
					foreach (MType iface in con1.interfaces)
						con.interfaces.Add(iface);
				if (con2.interfaces != null)
					foreach (MType iface in con2.interfaces)
						con.interfaces.Add(iface);
			}

			if (con1.kind != TypeKind.Any)
			{
				if (con2.kind == TypeKind.Any || con2.kind == con1.kind) con.kind = con1.kind;
				else return null;
			}
			else con.kind = con2.kind;

			con.hasNew = con1.hasNew || con2.hasNew;

			return con;
		}

		private static readonly FieldInfo superTypeField = typeof(TypeVarCon).GetField("superType");
		private static readonly FieldInfo interfacesField = typeof(TypeVarCon).GetField("interfaces");
		private static readonly FieldInfo kindField = typeof(TypeVarCon).GetField("kind");
		private static readonly FieldInfo hasNewField = typeof(TypeVarCon).GetField("hasNew");

		internal void CodeGen(CodeGenState state)
		{
			state.MakeCode(typeof(TypeVarCon));
			if (superType != null)
			{
				state.code.Emit(OpCodes.Dup);
				superType.CodeGen(state);
				state.code.Emit(OpCodes.Stfld, superTypeField);
			}
			if (interfaces != null)
			{
				state.code.Emit(OpCodes.Dup);
				MType.CodeGen(state, interfaces.ToArray());
				state.code.Emit(OpCodes.Stfld, interfacesField);
			}
			if (kind != TypeKind.Any)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitInt((int)kind);
				state.code.Emit(OpCodes.Stfld, kindField);
			}
			if (hasNew)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitBool(true);
				state.code.Emit(OpCodes.Stfld, hasNewField);
			}
		}

		internal void CodeGenMeta(CodeGenState state)
		{
			state.MakeCode(typeof(TypeVarCon));
			if(superType != null)
			{
				state.code.Emit(OpCodes.Dup);
				superType.CodeGenMeta(state);
				state.code.Emit(OpCodes.Stfld, superTypeField);
			}
			if (interfaces != null)
			{
				state.code.Emit(OpCodes.Dup);
				MType.CodeGenMeta(state, interfaces.ToArray());
				state.code.Emit(OpCodes.Stfld, interfacesField);
			}
			if (kind != TypeKind.Any)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitInt((int)kind);
				state.code.Emit(OpCodes.Stfld, kindField);
			}
			if (hasNew)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitBool(true);
				state.code.Emit(OpCodes.Stfld, hasNewField);
			}
		}

		public static TypeVarCon FromGenericParameter(Type genericParam)
		{
			if (!genericParam.IsGenericParameter) throw new ArgumentException("!genericParam.IsGenericParameter");
			TypeVarCon con = new TypeVarCon();
			foreach (Type type in genericParam.GetGenericParameterConstraints())
			{
				if (!type.IsInterface) con.superType = SystemType.Create(type);
				else
				{
					if (con.interfaces == null) con.interfaces = new List<MType>();
					con.interfaces.Add(SystemType.Create(type));
				}
			}
			if ((genericParam.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0) con.kind = TypeKind.Class;
			if ((genericParam.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0) con.kind = TypeKind.Struct;
			if ((genericParam.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) != 0) con.hasNew = true;
			return con;
		}
	}

	public abstract class TypeVarDecl
	{
		public string name;
		public int levelKind;
		public TypeVarCon con;

		public static readonly TypeVarDecl[] Empty = new TypeVarDecl[] { };

		public TypeVarDecl(string name, int levelKind)
		{
			this.name = name;
			this.levelKind = levelKind;
			this.con = new TypeVarCon();
		}

		public TypeVar MakeVar()
		{
			return TypeVar.Create(this);
		}

		public bool IsCompatible(MType type)
		{
			return type.GetLevelKind() <= levelKind && con.IsCompatibleType(type);
		}

		internal abstract Type GetSystemType();

		internal virtual void CodeGenSystemType(CodeGenState state)
		{
			CodeGen(state);
			state.code.Emit(OpCodes.Callvirt, typeof(MType).GetMethod("GetSystemType"));
		}

		internal abstract void CodeGen(CodeGenState state);

		internal abstract void CodeGenMeta(CodeGenState state);

		internal static void CodeGenMeta(CodeGenState state, TypeVarDecl[] decls)
		{
			int num = decls.Length;
			state.EmitInt(num);
			state.code.Emit(OpCodes.Newarr, typeof(TypeVarDecl));
			for (int i = 0; i < num; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitInt(i);
				decls[i].CodeGenMeta(state);
				state.code.Emit(OpCodes.Stelem_Ref);
			}
		}

		internal static List<Type> GetGenericTypes(TypeVarDecl[] decls, MType[] args)
		{
			List<Type> result = new List<Type>();
			for (int i = 0; i < decls.Length; i++)
			{
				if (decls[i].levelKind == 0)
				{
					result.Add(args[i].GetSystemType());
				}
			}
			return result;
		}
				
		internal static List<MType> GetReflectionTypes(TypeVarDecl[] decls, MType[] args)
		{
			List<MType> result = new List<MType>();
			for (int i = 0; i < decls.Length; i++)
			{
				if (decls[i].levelKind > 0)
				{
					result.Add(args[i]);
				}
			}
			return result;
		}

		internal virtual GenericTypeVarDecl GetGenericTypeVarDecl()
		{
			return null;
		}
	}

	public class ClosureTypeVarDecl : TypeVarDecl
	{
		// the type variable is captured from an outer scope and is accessed from a closure environment
		[NonSerialized]
		internal TypeVarDecl decl;

		[NonSerialized]
		internal GenericTypeParameterBuilder genericParam;

		[NonSerialized]
		internal FieldInfo field;

		public ClosureTypeVarDecl(TypeVarDecl decl) : base(decl.name, decl.levelKind)
		{
			this.decl = decl;
		}

		internal override Type GetSystemType()
		{
			if (genericParam != null) return genericParam;
			else throw new InvalidOperationException();
		}

		internal override void CodeGen(CodeGenState state)
		{
			if (genericParam != null) state.EmitType(genericParam);
			else
			{
				state.EmitLdarg(0);
				if(field != null) state.code.Emit(OpCodes.Ldfld, field);
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		internal override GenericTypeVarDecl GetGenericTypeVarDecl()
		{
			TypeVarDecl decl = this.decl;
			while (decl is ClosureTypeVarDecl)
				decl = ((ClosureTypeVarDecl)decl).decl;
			return decl as GenericTypeVarDecl;
		}
	}

	public class GenericTypeVarDecl : TypeVarDecl
	{
		internal bool onMethod;

		// if the type variable has levelKind == 0 then it is represent as a generic type parameter
		[NonSerialized]
		internal GenericTypeParameterBuilder genericParam;

		public GenericTypeVarDecl(string name, bool onMethod)
			: base(name, 0)
		{
			this.onMethod = onMethod;
		}

		internal override Type GetSystemType()
		{
			return genericParam;
		}

		internal override void CodeGenSystemType(CodeGenState state)
		{
			state.EmitType(genericParam);
		}

		internal void Generate(GenericTypeParameterBuilder typeVar)
		{
			this.genericParam = typeVar;
			GenericParameterAttributes attrs = GenericParameterAttributes.None;
			if (con.hasNew) attrs |= GenericParameterAttributes.DefaultConstructorConstraint;
			if (con.kind == TypeKind.Class) attrs |= GenericParameterAttributes.ReferenceTypeConstraint;
			if (con.kind == TypeKind.Struct) attrs |= GenericParameterAttributes.NotNullableValueTypeConstraint;
			typeVar.SetGenericParameterAttributes(attrs);
			if (con.superType != null) typeVar.SetBaseTypeConstraint(con.superType.GetSystemType());
			if (con.interfaces != null) typeVar.SetInterfaceConstraints(MType.GetSystemTypes(con.interfaces.ToArray()));
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitType(genericParam);
			state.MakeType(typeof(SystemType));
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		internal override GenericTypeVarDecl GetGenericTypeVarDecl()
		{
			return this;
		}
	}

	public class ClassReflTypeVarDecl : TypeVarDecl
	{
		// if the type variable is defined on a class and has levelKind > 0 then it is represent as an instance field on the defining class
		[NonSerialized]
		internal FieldBuilder field;

		public ClassReflTypeVarDecl(string name)
			: base(name, 1)
		{
			if (levelKind == 0) throw new InvalidOperationException("levelKind == 0");
		}

		internal void Generate(FieldBuilder field)
		{
			this.field = field;
		}

		internal override Type GetSystemType()
		{
			throw new InvalidOperationException("levelKind > 0");
		}

		internal override void CodeGen(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class MethodReflTypeVarDecl : TypeVarDecl
	{
		// if the type variable is defined on a method and has levelKind > 0 then it is represent as a value parameter on the defining method
		[NonSerialized]
		internal int paramPos;

		public MethodReflTypeVarDecl(string name) : base(name, 1)
		{
			if (levelKind == 0) throw new InvalidOperationException("levelKind == 0");
		}

		internal void Generate(int position)
		{
			this.paramPos = position;
		}

		internal override Type GetSystemType()
		{
			throw new InvalidOperationException("levelKind > 0");
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitLdarg(paramPos);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	//internal delegate void CodeGen(CodeGenState state);

	public class LocalReflTypeVarDecl : TypeVarDecl
	{
		[NonSerialized]
		internal CodeGen codegen;

		public LocalReflTypeVarDecl(string name)
			: base(name, 1)
		{
			if (levelKind == 0) throw new InvalidOperationException("levelKind == 0");
		}

		internal void Generate(CodeGen codegen)
		{
			this.codegen = codegen;
		}

		internal override Type GetSystemType()
		{
			throw new InvalidOperationException("levelKind > 0");
		}

		internal override void CodeGen(CodeGenState state)
		{
			codegen(state.code);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitString(name);
			state.MakeCode(typeof(LocalReflTypeVarDecl));
		}
	}
	#endregion
	#region Variables
	public abstract partial class VarDecl
	{
		//private static int genSym = 0;

		//public int sym;
		public string name;

		//[NonSerialized]
		//internal int newSym = -1;

		[NonSerialized]
		internal /*AND protected*/ DeferredLoc loc;

		public VarDecl(/*int sym, */string name)
		{
			//this.sym = sym == -1 ? genSym++ : sym;
			this.name = name;
		}

		public virtual VarDecl GetVarDecl()
		{
			return this;
		}

		internal virtual Location GetLocation()
		{
			if (loc == null) loc = new DeferredLoc();
			return loc;
		}

		public abstract MType GetMType();

		internal virtual Type GetSystemType()
		{
			return GetMType().GetSystemType();
		}

		internal static Type[] GetSystemTypes(VarDecl[] decls)
		{
			int num = decls.Length;
			Type[] types = new Type[num];
			for(int i = 0; i < num; i++) types[i] = decls[i].GetSystemType();
			return types;
		}

		public virtual Code MakeVar()
		{
			return new Var(this);
		}

		//internal abstract void CodeGen(CodeGenState state);

		//internal abstract void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret);

		//internal abstract void CodeGenRef(CodeGenState state);

		internal abstract void CodeGenMeta(CodeGenState state);

		internal abstract void GenerateCode(CodeGenState state);

		internal static void CodeGenMeta<T>(CodeGenState state, T[] decls) where T: VarDecl
		{
			int num = decls.Length;
			state.EmitInt(num);
			state.code.Emit(OpCodes.Newarr, typeof(T));
			for(int i = 0; i < num; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitInt(i);
				decls[i].GenerateCode(state);
				state.code.Emit(OpCodes.Stelem_Ref);
			}
		}

	}

	public abstract class ParamDecl : VarDecl
	{
		public ParamType type;

		public ParamDecl(string name, ParamType type): base(name)
		{
			this.type = type;
		}

		internal void SetLocation(int pos)
		{
			if (loc == null) loc = new DeferredLoc();
			ParamLoc paramLoc = new ParamLoc(pos, type.GetSystemType());
			if (!type.@ref) loc.Set(paramLoc);
			else loc.Set(new AddrLoc(paramLoc));
		}

		public override MType GetMType()
		{
			return type.type;
		}

		internal override Type GetSystemType()
		{
			return type.GetSystemType();
		}

		//internal override void CodeGen(CodeGenState state)
		//{
		//    state.EmitLdarg(pos);
		//    if(type.@ref) state.EmitLdind(type.type.GetCILType());
		//}

		//internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		//{
		//    if (!type.@ref)
		//    {
		//        if (op != AssignOp.Nop || ret == AssignRet.Post) state.EmitLdarg(pos);
		//        if (op != AssignOp.Nop && ret == AssignRet.Post) state.code.Emit(OpCodes.Dup);
		//        rhs.CodeGen(state);
		//        Assign.CodeGenAssignOp(state, op);
		//        if (ret == AssignRet.Pre) state.code.Emit(OpCodes.Dup);
		//        state.EmitStarg(pos);
		//    }
		//    else
		//    {
		//        CILType cilType = type.type.GetCILType();
		//        if (ret == AssignRet.Post)
		//        {
		//            state.EmitLdarg(pos);
		//            state.EmitLdind(cilType);
		//        }
		//        state.EmitLdarg(pos);
		//        if (ret == AssignRet.Pre) state.code.Emit(OpCodes.Dup);
		//        if (op != AssignOp.Nop)
		//        {
		//            state.code.Emit(OpCodes.Dup);
		//            state.EmitLdind(cilType);
		//        }
		//        rhs.CodeGen(state);
		//        Assign.CodeGenAssignOp(state, op);
		//        state.EmitStind(cilType);
		//        if (ret == AssignRet.Pre) state.EmitLdind(cilType);
		//    }
		//}

		//internal override void CodeGenRef(CodeGenState state)
		//{
		//    if(type.@ref) state.EmitLdarg(pos);
		//    else state.EmitLdarga(pos);
		//}

		public static ParamType[] GetParamTypes(ParamDecl[] decls)
		{
			int num = decls.Length;
			ParamType[] types = new ParamType[num];
			for(int i = 0; i < num; i++) types[i] = decls[i].type;
			return types;
		}
	}

	public class MethodParamDecl : ParamDecl
	{
		public MethodParamDecl(string name, ParamType type)
			: base(name, type)
		{
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}

		internal override void GenerateCode(CodeGenState state)
		{
			throw new NotImplementedException();
		}
	}

	public class FunctionParamDecl : ParamDecl
	{
		public Function func;
		public int paramPos;
		
		public FunctionParamDecl(string name, ParamType type, Function func, int paramPos)
			: base(name, type)
		{
			this.func = func;
			this.paramPos = paramPos;
		}

		private static readonly FieldInfo paramsField = typeof(Function).GetField("params");

		internal override void CodeGenMeta(CodeGenState state)
		{
			func.CodeGenMeta(state);
			state.code.Emit(OpCodes.Ldfld, paramsField);
			state.EmitInt(paramPos);
			state.code.Emit(OpCodes.Ldelem_Ref);
		}

		internal override void GenerateCode(CodeGenState state)
		{
			throw new InvalidOperationException();
		}
	}

	public partial class ThisDecl : VarDecl
	{
		public MType type;

		[NonSerialized]
		internal int pos;

		public ThisDecl(MType type)
			: base("this")
		{
			this.type = type;
		}

		public override MType GetMType()
		{
			return type;
		}

		internal void SetLocation()
		{
			if (loc == null) loc = new DeferredLoc();
			loc.Set(new ParamLoc(0, type.GetSystemType()));
		}

		//internal override void CodeGen(CodeGenState state)
		//{
		//    state.code.Emit(OpCodes.Ldarg_0);
		//}

		//internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		//{
		//    throw new Exception("This parameter cannot be assigned.");
		//}

		//internal override void CodeGenRef(CodeGenState state)
		//{
		//    state.code.Emit(OpCodes.Ldarga_S, 0);
		//}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitLdloc(pos);
		}

		internal override void GenerateCode(CodeGenState state)
		{
			type.CodeGenMeta(state);
			state.MakeCode(typeof(ThisDecl));
			pos = state.DeclareLocal(typeof(ParamDecl));
			state.EmitStloc(pos);
		}
	}

	public partial class LocalDecl : VarDecl
	{
		public MType type;

		[NonSerialized]
		protected int localPos;

		public LocalDecl(string name, MType type): base(name)
		{
			this.type = type;
		}

		public override MType GetMType()
		{
			return type;
		}

		public void SetLocation(int pos)
		{
			if (loc == null) loc = new DeferredLoc();
			loc.Set(new LocalLoc(pos, type.GetSystemType()));
		}

		//internal override void CodeGen(CodeGenState state)
		//{
		//    state.EmitLdloc(pos);
		//}

		//internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		//{
		//    if (op != AssignOp.Nop || ret == AssignRet.Post) state.EmitLdloc(pos);
		//    if (op != AssignOp.Nop && ret == AssignRet.Post) state.code.Emit(OpCodes.Dup);
		//    rhs.CodeGen(state);
		//    Assign.CodeGenAssignOp(state, op);
		//    if (ret == AssignRet.Pre) state.code.Emit(OpCodes.Dup);
		//    state.EmitStloc(pos);
		//}

		//internal override void CodeGenRef(CodeGenState state)
		//{
		//    state.EmitLdloca(pos);
		//}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitLdloc(localPos);
		}

		internal override void GenerateCode(CodeGenState state)
		{
			state.EmitString(name);
			type.CodeGenMeta(state);
			state.MakeCode(typeof(LocalDecl));
			localPos = state.DeclareLocal(typeof(LocalDecl));
			state.EmitStloc(localPos);
		}

	}
	#endregion

	public partial class Function : VarDecl
	{
		public MType delegateType;
		public ParamDecl[] @params;
		public MType retType;
		public Code stmt;

		[NonSerialized]
		internal int pos;

		[NonSerialized]
		internal List<Closure> closures;
		[NonSerialized]
		internal bool hasCsp;
		[NonSerialized]
		internal List<ClosureType> closureTypes;
		[NonSerialized]
		internal List<Function> subFunctions;

		//[NonSerialized]
		//internal MethodInfo method;
		//[NonSerialized]
		//internal ConstructorInfo closureCtor;

		public Function(string name, MType delegateType, string[] paramNames): base(name)
		{
			if (delegateType == null) throw new ArgumentNullException("delegateType");
			this.delegateType = delegateType;

			ParamType[] paramTypes;
			if (!DelegateType.OpenDelegateType(delegateType, out paramTypes, out retType)) throw new ArgumentException("not a delegate type", "delegateType");
			
			@params = new ParamDecl[paramTypes.Length];
			for(int i = 0; i < paramTypes.Length; i++)
			{
				string paramName = null;
				if (paramNames != null && i < paramNames.Length) paramName = paramNames[i];
				@params[i] = new FunctionParamDecl(paramName, paramTypes[i], this, i);
			}
		}

		#region VarDecl
		public override MType GetMType()
		{
			return delegateType;
		}

		//internal override void CodeGen(CodeGenState state)
		//{
		//    CodeGenDelegate(state);
		//}

		//internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		//{
		//    throw new InvalidOperationException();
		//}

		//internal override void CodeGenRef(CodeGenState state)
		//{
		//    throw new InvalidOperationException();
		//}
		#endregion

		internal void GenerateClosures(TypeCheckState state)
		{
			if (state.AddFunction(this))
			{
				stmt.CodeGenClosures(state);
				hasCsp = state.ContainsCspObjects();
				closures = state.GetClosures();
				closureTypes = new List<ClosureType>(); // state.GetClosureTypes();
				subFunctions = state.GetFunctions();
			}
		}

		internal void SetLocation(ConstructorInfo closureCtor, MethodInfo invokeMethod, Location cspStore, List<Closure> closures)
		{
			List<Location> locs = null;
            if (cspStore != null)
            {
                locs = new List<Location>();
                locs.Add(cspStore);
            }
            if (closures != null)
            {
                if (locs == null) locs = new List<Location>();
                foreach (Closure closure in closures)
                    locs.Add(closure.outside);
            }
			if (loc == null) loc = new DeferredLoc();
			loc.Set(new FunctionLoc(DelegateType.GetConstructor(delegateType).GetSystemConstructor(), closureCtor, invokeMethod, locs));
		}

		//internal void CodeGenCall(CodeGenState state, Arg[] args)
		//{
		//    if(hasCsp) state.EmitCspStore();
		//    foreach (Closure c in closures) c.outside.CodeGen(state.code);
		//    if (closureCtor != null) state.code.Emit(OpCodes.Newobj, closureCtor);
		//    Arg.CodeGen(state, args);
		//    state.code.Emit(OpCodes.Call, method);
		//}

		//internal void CodeGenDelegate(CodeGenState state)
		//{
		//    if (hasCsp) state.EmitCspStore();
		//    foreach (Closure c in closures) c.outside.CodeGen(state.code);
		//    if(closureCtor != null) state.code.Emit(OpCodes.Newobj, closureCtor);
		//    else if (closures.Count == 1)
		//    {
		//        Type t = closures[0].type.GetSystemType();
		//        if (t.IsValueType) state.code.Emit(OpCodes.Box, t);
		//    }
		//    else if (closures.Count == 0 && (state is ModuleCodeGenState || !hasCsp && state is DynamicMethodCodeGenState)) state.EmitNull();
		//    state.code.Emit(OpCodes.Ldftn, method);
		//    state.code.Emit(OpCodes.Newobj, DelegateType.GetConstructor(delegateType).GetSystemConstructor());
		//}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitLdloc(pos);
		}

		private static readonly FieldInfo stmtField = typeof(Function).GetField("stmt");

		internal override void GenerateCode(CodeGenState state)
		{
			state.EmitString(name);
			delegateType.CodeGenMeta(state);
			if (@params.Length > 0 && @params[0].name != null)
			{
				state.EmitInt(@params.Length);
				state.code.Emit(OpCodes.Newarr, typeof(string));
				for (int i = 0; i < @params.Length; i++)
				{
					state.code.Emit(OpCodes.Dup);
					state.EmitInt(i);
					state.EmitString(@params[i].name);
					state.code.Emit(OpCodes.Stelem_Ref);
				}
			}
			else state.EmitNull();
			state.MakeCode(typeof(Function));
			pos = state.DeclareLocal(typeof(Function));
			state.EmitStloc(pos);
			state.EmitLdloc(pos);
			stmt.CodeGenMeta(state);
			state.code.Emit(OpCodes.Stfld, stmtField);
		}
	}

}