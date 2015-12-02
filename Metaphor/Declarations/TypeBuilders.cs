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
using System.Reflection;
using System.Reflection.Emit;
using Metaphor.Collections;

namespace Metaphor
{
	public abstract class MTypeBuilder
	{
		private static int genSym = 0;

		public int sym;
		public string name;
		public string ns;
		public TypeAttributes attr;
		public TypeVarDecl[] @params;
		public MType baseType;
		public List<MType> interfaces;
		public List<MTypeBuilder> types;
		public List<MFieldBuilder> fields;
		public List<MConstructorBuilder> ctors;
		public List<MMethodBuilder> methods;

		[NonSerialized]
		public TypeBuilder typeBuilder;

		public MTypeBuilder(int sym, string name, string ns, TypeAttributes attr, TypeVarDecl[] @params)
		{
			this.sym = sym == -1 ? genSym++ : sym;
			this.name = name;
			this.ns = ns;
			this.attr = attr;
			this.@params = @params;
			this.baseType = PrimType.Object;
			this.types = new List<MTypeBuilder>();
			this.fields = new List<MFieldBuilder>();
			this.methods = new List<MMethodBuilder>();
			this.ctors = new List<MConstructorBuilder>();

			foreach (TypeVarDecl decl in @params)
				if (decl.GetGenericTypeVarDecl() == null)
					throw new NotImplementedException("type variables on classes with ~'s");
		}

		public void AddType(MTypeBuilder type)
		{
			//MModuleBuilder.allTypes.Push(type);
			types.Add(type);
		}

		public void AddField(MFieldBuilder field)
		{
			field.declaringType = this;
			//MModuleBuilder.allFields.Push(field);
			fields.Add(field);
		}

		public void AddConstructor(MConstructorBuilder ctor)
		{
			ctor.declaringType = this;
			//MModuleBuilder.allCtors.Push(ctor);
			ctors.Add(ctor);
		}

		public void AddMethod(MMethodBuilder method)
		{
			method.declaringType = this;
			//MModuleBuilder.allMethods.Push(method);
			methods.Add(method);
		}

		public MType[] MakeFormalArgs()
		{
			MType[] args = new MType[@params.Length];
			for (int i = 0; i < @params.Length; i++)
				args[i] = @params[i].MakeVar();
			return args;
		}

		public MType MakeType()
		{
			return MakeType(MakeFormalArgs());
		}

		public MType MakeType(MType[] args)
		{
			return Type.Create(this, args);
		}

		internal virtual void GenerateClosures(TypeCheckState state)
		{
			state.PushTypeVars(@params);
			baseType.GenerateClosures(state);
			//foreach(MType iface in interfaces) iface.GenerateClosures(state);
			foreach(MTypeBuilder type in types) type.GenerateClosures(state);
			foreach(MFieldBuilder field in fields) field.GenerateClosures(state);
			foreach(MConstructorBuilder ctor in ctors) ctor.GenerateClosures(state);
			foreach(MMethodBuilder method in methods) method.GenerateClosures(state);
		}

		internal virtual void GenerateTypes(ModuleGenState state)
		{
			if (state.types.Count == 0)
			{
				string fullName = ns != null ? string.Format("{0}.{1}", ns, name) : name;
				if (@params.Length > 0) fullName = string.Format("{0}`{1}", fullName, @params.Length);
				typeBuilder = state.module.DefineType(fullName, attr);
			}
			else
			{
				TypeBuilder outerClass = state.types.Peek();
				typeBuilder = outerClass.DefineNestedType(name, attr);
			}

			List<string> typeParamNames = new List<string>();
			foreach (TypeVarDecl param in @params)
			{
				if (param.levelKind == 0) typeParamNames.Add(param.name);
				else throw new NotImplementedException();
			}
			if (typeParamNames.Count > 0)
			{
				GenericTypeParameterBuilder[] types = typeBuilder.DefineGenericParameters(typeParamNames.ToArray());
				int i = 0;
				foreach (TypeVarDecl param in @params)
					if (param.levelKind == 0) ((GenericTypeVarDecl) param).Generate(types[i++]);
			}

			state.types.Push(typeBuilder);
			foreach (MTypeBuilder type in types) type.GenerateTypes(state);
			state.types.Pop();

			typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(System.Type.EmptyTypes), new object[] { }));
		}

		internal virtual void GenerateMembers(ModuleGenState state)
		{
			//TypeVarDecl.PushVars(@params);
			typeBuilder.SetParent(baseType.GetSystemType());

			state.types.Push(typeBuilder);
			foreach (MFieldBuilder field in fields) field.GenerateMembers(state);
			foreach (MMethodBuilder method in methods) method.GenerateMembers(state);
			foreach (MConstructorBuilder ctor in ctors) ctor.GenerateMembers(state);
			foreach (MTypeBuilder type in types) type.GenerateMembers(state);
			state.types.Pop();
			//TypeVarDecl.PopVars(@params.Length);
		}

		internal virtual void GenerateCode(ModuleGenState state)
		{
			//TypeVarDecl.PushVars(@params);
			state.types.Push(typeBuilder);
			foreach (MMethodBuilder method in methods) method.GenerateCode(state);
			foreach (MConstructorBuilder ctor in ctors) ctor.GenerateCode(state);
			foreach (MTypeBuilder type in types) type.GenerateCode(state);
			state.types.Pop();
			//TypeVarDecl.PopVars(@params.Length);
		}

		internal virtual void GenerateFinal(ModuleGenState state)
		{
			typeBuilder.CreateType();
			foreach (MTypeBuilder type in types) type.GenerateFinal(state);
		}

		internal virtual void GenerateTypesMeta(CodeGenState state)
		{
			state.EmitInt(sym);
			state.EmitString(name);
			state.EmitString(ns);
			TypeVarDecl.CodeGenMeta(state, @params);
			state.MakeCode(GetType());

			for (int i = 0; i < types.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				types[i].GenerateTypesMeta(state);
				state.code.Emit(OpCodes.Call, typeof(MTypeBuilder).GetMethod("AddType"));
			}
		}

		internal virtual void GenerateMembersMeta(CodeGenState state)
		{
			state.code.Emit(OpCodes.Dup);
			baseType.CodeGenMeta(state);
			state.code.Emit(OpCodes.Stfld, typeof(MTypeBuilder).GetField("baseType"));

			for (int i = 0; i < fields.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				fields[i].GenerateMembersMeta(state);
				state.code.Emit(OpCodes.Call, typeof(MTypeBuilder).GetMethod("AddField"));
			}

			for (int i = 0; i < ctors.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				ctors[i].GenerateMembersMeta(state);
				state.code.Emit(OpCodes.Call, typeof(MTypeBuilder).GetMethod("AddConstructor"));
			}

			for (int i = 0; i < methods.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				methods[i].GenerateMembersMeta(state);
				state.code.Emit(OpCodes.Call, typeof(MTypeBuilder).GetMethod("AddMethod"));
			}

			for (int i = 0; i < types.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.code.Emit(OpCodes.Ldfld, typeof(MTypeBuilder).GetField("types"));
				state.EmitInt(i);
				state.code.Emit(OpCodes.Call, typeof(List<MTypeBuilder>).GetMethod("get_Item"));
				types[i].GenerateMembersMeta(state);
			}

			state.code.Emit(OpCodes.Pop);
		}

		internal virtual void GenerateCodeMeta(CodeGenState state)
		{
			for (int i = 0; i < ctors.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.code.Emit(OpCodes.Ldfld, typeof(MTypeBuilder).GetField("ctors"));
				state.EmitInt(i);
				state.code.Emit(OpCodes.Call, typeof(List<MConstructorBuilder>).GetMethod("get_Item"));
				ctors[i].GenerateCodeMeta(state);
			}

			for (int i = 0; i < methods.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.code.Emit(OpCodes.Ldfld, typeof(MTypeBuilder).GetField("methods"));
				state.EmitInt(i);
				state.code.Emit(OpCodes.Call, typeof(List<MMethodBuilder>).GetMethod("get_Item"));
				methods[i].GenerateCodeMeta(state);
			}

			for (int i = 0; i < types.Count; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.code.Emit(OpCodes.Ldfld, typeof(MTypeBuilder).GetField("types"));
				state.EmitInt(i);
				state.code.Emit(OpCodes.Call, typeof(List<MTypeBuilder>).GetMethod("get_Item"));
				types[i].GenerateCodeMeta(state);
			}

			state.code.Emit(OpCodes.Pop);
		}

		internal static MType Subst(TypeVarDecl[] classTypeParams, MType[] classTypeArgs, TypeVarDecl[] methodTypeParams, MType[] methodTypeArgs, MType substType)
		{
			if ((classTypeParams == null || classTypeParams.Length == 0) && (methodTypeParams == null || methodTypeParams.Length == 0)) return substType;

			TypeVar typeVar = substType as TypeVar;
			if (typeVar != null)
			{
				int index = 0;
				if (methodTypeParams != null)
					foreach (TypeVarDecl decl in methodTypeParams)
					{
						if (decl == typeVar.decl) return methodTypeArgs[index];
						index++;
					}
				index = 0;
				if (classTypeParams != null)
					foreach (TypeVarDecl decl in classTypeParams)
					{
						if (decl == typeVar.decl) return classTypeArgs[index];
						index++;
					}
				throw new NotImplementedException();
				//MType substSuperType = Subst(typeVar.superType);
				//MType[] substInterfaces = Array.ConvertAll<MType, MType>(typeVar.interfaces, new Converter<MType, MType>(Subst));
				//return TypeVar.Create(typeVar.sym, substSuperType, substInterfaces, typeVar.hasNew);
			}

			MType[] substTypeArgs = substType.GetTypeArguments();
			if (substTypeArgs.Length == 0) return substType;
			else
			{
				MType[] substSubstTypeArgs = new MType[substTypeArgs.Length];
				for (int i = 0; i < substTypeArgs.Length; i++)
					substSubstTypeArgs[i] = Subst(classTypeParams, classTypeArgs, methodTypeParams, methodTypeArgs, substTypeArgs[i]);
				return substType.ReplaceTypeArguments(substSubstTypeArgs);
			}
		}

		internal static uint GetTypeParamMask(TypeVarDecl[] @params)
		{
			uint mask = 0;
			for (int i = 0; i < @params.Length; i++)
				if (@params[i].levelKind > 0)
					mask |= (uint)1 << i;
			return mask;
		}

		protected class Type : MType
		{
			public MTypeBuilder type;
			public MType[] typeArgs;
			protected int levelKind;

			protected Type(MTypeBuilder type, MType[] args, int levelKind)
			{
				this.type = type;
				this.typeArgs = args;
				this.levelKind = levelKind;
			}

			public static MType Create(MTypeBuilder type, MType[] args)
			{
				if (type.@params.Length != args.Length) throw new ArgumentException(string.Format("'type' has {0} type parameter(s) but {1} type(s) was supplied in 'args'", type.@params.Length, args.Length));

				int levelKind = 0;
				for (int i = 0; i < args.Length; i++)
				{
					int tmp = args[i].GetLevelKind() - type.@params[i].levelKind;
					if (tmp > levelKind) levelKind = tmp;
				}
				return new Type(type, args, levelKind);
			}

			public override int GetLevelKind()
			{
				return levelKind;
			}

			public override MType[] GetTypeArguments()
			{
				return typeArgs;
			}

			public override string GetName()
			{
				return type.name;
			}

			protected override bool IsEqualToInternal(MType type)
			{
				Type that = (Type)type;
				return this.type == that.type && MType.AreEqualTo(typeArgs, that.typeArgs);
			}

			public override MType GetSuperType()
			{
				return Subst(type.@params, typeArgs, null, null, type.baseType);
			}

			public override MType GetNestedType(string name, MType[] args)
			{
				foreach (MTypeBuilder nestedType in type.types)
					if (nestedType.name == name)
						return nestedType.MakeType(args);
				return null;
			}

			#region Member lookup
			protected internal override void GetFields(List<MFieldInfo> fields)
			{
				foreach (MFieldBuilder field in type.fields)
					fields.Add(field.MakeField(typeArgs));
				base.GetFields(fields);
			}

			protected internal override void GetConstructors(List<MConstructorInfo> ctors)
			{
				foreach (MConstructorBuilder ctor in type.ctors)
					ctors.Add(ctor.MakeConstructor(typeArgs));
			}

			protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
			{
				foreach (MMethodBuilder method in type.methods)
					if (method.typeParams.Length == typeParams.Length)
						methods.Add(method.MakeMethod(typeArgs, typeParams));
				base.GetMethods(methods, typeParams);
			}
			#endregion

			public override System.Type GetSystemType()
			{
				System.Type systemType = type.typeBuilder;
				System.Type[] systemTypeArgs = TypeVarDecl.GetGenericTypes(type.@params, typeArgs).ToArray();
				if (systemTypeArgs.Length != 0)
					systemType = systemType.MakeGenericType(systemTypeArgs);
				return systemType;
			}

			internal override void CodeGen(CodeGenState state)
			{
				if (type.typeBuilder != null)
				{
					state.EmitType(type.typeBuilder);
					if (typeArgs.Length != 0)
					{
						MType.CodeGen(state, typeArgs);
						state.MakeType(typeof(GenericSystemType));
					}
					else
					{
						
						state.MakeType(typeof(SystemType));
					}
				}
				else throw new NotImplementedException();
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				//should do something intelligent with levels (and similiarly for other builders)
				if (type.typeBuilder != null)
				{
					state.EmitType(type.typeBuilder);
					if (typeArgs.Length == 0) 
						state.MakeType(typeof(SystemType));
					else
					{
						MType.CodeGenMeta(state, typeArgs);
						state.MakeType(typeof(GenericSystemType));
					}
				}
				else
				{
					state.EmitInt(type.sym);
					MType.CodeGen(state, typeArgs);
					state.code.Emit(OpCodes.Call, typeof(MModuleBuilder).GetMethod("CreateType", BindingFlags.Public | BindingFlags.Static));
				}
			}

			public override MType ReplaceTypeArguments(MType[] newTypeArgs)
			{
				return Create(type, newTypeArgs);
			}
		}
	}

	public class ClassBuilder : MTypeBuilder
	{
		public ClassBuilder(int sym, string name, string ns, TypeAttributes attr, TypeVarDecl[] @params)
			: base(sym, name, ns, attr | TypeAttributes.BeforeFieldInit, @params)
		{
		}
	}

	public class StructBuilder : MTypeBuilder
	{
		public StructBuilder(int sym, string name, string ns, TypeAttributes attr, TypeVarDecl[] @params)
			: base(sym, name, ns, attr | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed | TypeAttributes.SequentialLayout, @params)
		{
			baseType = PrimType.Value;
		}
	}

	public class DelegateBuilder : MTypeBuilder
	{
		public MType returnType;
		public ParamType[] paramTypes;

		public DelegateBuilder(int sym, string name, string ns, TypeAttributes attr, TypeVarDecl[] @params)
			: base(sym, name, ns, attr | TypeAttributes.Sealed, @params)
		{
			this.baseType = PrimType.Delegate;
		}

		public void AddDelegateMembers(MType retType, ParamType[] paramTypes)
		{
			this.returnType = retType;
			this.paramTypes = paramTypes;

			this.AddConstructor(
					new ConstructorBuilder(
						-1, 
						new ThisDecl(MakeType()),
						new ParamDecl[] {
							new MethodParamDecl(null, ParamType.Create(PrimType.Object, false)),
							new MethodParamDecl(null, ParamType.Create(PrimType.Native, false))}));

			ParamDecl[] paramDecls = new ParamDecl[paramTypes.Length];
			for (int i = 0; i < paramTypes.Length; i++)
				paramDecls[i] = new MethodParamDecl(null, paramTypes[i]);

			this.AddMethod(
				new InvokeMethodBuilder(
					-1,
					new ThisDecl(MakeType()),
					paramDecls,
					retType));
		}

		public class ConstructorBuilder : MConstructorBuilder
		{
			public ConstructorBuilder(int sym, ThisDecl @this, ParamDecl[] @params)
				: base(sym, MethodAttributes.Public, @this, @params)
			{
			}

			internal override void GenerateMembers(ModuleGenState state)
			{
				ctorBuilder = declaringType.typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, new System.Type[] { typeof(object), typeof(IntPtr) });
				ctorBuilder.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.Runtime);
			}

			internal override void GenerateCode(ModuleGenState state)
			{
				throw new InvalidOperationException("cannot generate code for delegate constructor");
			}
		}

		public class InvokeMethodBuilder : MMethodBuilder
		{
			public InvokeMethodBuilder(int sym, ThisDecl @this, ParamDecl[] @params, MType retType)
				: base(sym, "Invoke", MethodAttributes.Public, TypeVarDecl.Empty, @this, @params, retType)
			{
			}

			internal override void GenerateMembers(ModuleGenState state)
			{
				methodBuilder = declaringType.typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis);
				methodBuilder.SetReturnType(retType.GetSystemType());
				methodBuilder.SetParameters(ParamDecl.GetSystemTypes(@params));
				methodBuilder.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.Runtime);
			}

			internal override void GenerateCode(ModuleGenState state)
			{
				throw new InvalidOperationException("cannot generate code for delegate invoke method");
			}
		}

		internal override void GenerateCode(ModuleGenState state)
		{
		}
	}
}
