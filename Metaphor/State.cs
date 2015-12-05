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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;

using Metaphor.Collections;

namespace Metaphor
{
	internal abstract class GenState
	{
		public CodeGenState CodeGen(CodeGenState cgState, Code code, List<Function> functions)
		{
			CodeGenState[] cgStates = new CodeGenState[functions.Count];
			for (int i = 0; i < functions.Count; i++)
				cgStates[i] = DefineFunction(functions[i]);

			if (cgState != null)
			{
				code.CodeGen(cgState);
				cgState.EndMethodBody();
			}
			for (int i = 0; i < functions.Count; i++)
			{
				functions[i].stmt.CodeGen(cgStates[i]);
				cgStates[i].EndMethodBody();
			}
			if (cgState != null) return cgState;
			else if (functions.Count > 0) return cgStates[0];
			else return null;
		}

		public CodeGenState DefineFunction(Function func)
		{
			List<Closure> closures = new List<Closure>();
			foreach (Closure closure in func.closures)
			{
				Function recFunc = closure.decl as Function;
				if (recFunc != null)
				{
					if (recFunc.closures.Count > 0) throw new NotImplementedException("Variable capturing between recursive functions.");
					else closure.inside.Set(recFunc.GetLocation());
				}
				else closures.Add(closure);
			}
			return DefineFunction(func, closures);
		}

		public abstract CodeGenState DefineFunction(Function function, List<Closure> closures);

		public static Type[] ExtractTypeArgs(TypeBuilder declaringScope, List<ClosureType> closureTypes, MethodBuilder method)
		{
			Type[] classTypeParams = null;
			if (declaringScope.IsGenericTypeDefinition) classTypeParams = declaringScope.GetGenericArguments();
			List<string> genericParamNames = new List<string>();
			List<ClosureType> filteredClosures = new List<ClosureType>();
			foreach (ClosureType closureType in closureTypes)
			{
				if (classTypeParams != null)
				{
					Type type = closureType.outside.GetSystemType();
					if (Array.IndexOf<Type>(classTypeParams, type) != -1)
					{
						closureType.inside.Set(closureType.outside);
						continue;
					}
					genericParamNames.Add(closureType.decl.name);
					filteredClosures.Add(closureType);
				}
			}
			if (genericParamNames.Count > 0)
			{
				GenericTypeParameterBuilder[] genericParams = method.DefineGenericParameters(genericParamNames.ToArray());
				int index = 0;
				Type[] typeArgs = new Type[genericParamNames.Count];
				foreach (ClosureType closureType in filteredClosures)
				{
					closureType.inside.Set(new GenericParamType(genericParams[index]));
					typeArgs[index] = closureType.outside.GetSystemType();
					index++;
				}
				return typeArgs;
			}
			else return null;
		}
	}

	internal sealed class ModuleGenState : GenState
	{
		private int count = 0;

		public AssemblyBuilder assembly;
		public ModuleBuilder module;
		public Stack<TypeBuilder> types;
		public MethodBuilder entryPoint;

		private GroupDictionary<int, TypeVarDecl> typeVars;

		public Set<object> cspStore;
		private TypeBuilder cspStoreType;
		private Dictionary<object, FieldInfo> cspFields;
		private List<TypeBuilder> closures;

		private TypeBuilder functions;

		//public int level;

		public ModuleGenState(AssemblyBuilder assembly, ModuleBuilder module)
		{
			this.assembly = assembly;
			this.module = module;
			this.types = new Stack<TypeBuilder>();
			this.typeVars = new GroupDictionary<int, TypeVarDecl>();
			this.cspStore = new Set<object>();
			this.closures = new List<TypeBuilder>();
		}

		public void SetEntryPoint(MethodBuilder method)
		{
			assembly.SetEntryPoint(method);
			entryPoint = method;
		}

		internal ModuleCodeGenState DefineMethod(MethodBaseBuilder method)
		{
			return new ModuleCodeGenState(this, method.GetCode());
		}

		public override CodeGenState DefineFunction(Function function, List<Closure> closures)
		{
			TypeBuilder declaringScope;
			if(types.Count > 0) declaringScope = types.Peek();
			else if(functions != null) declaringScope = functions;
			else declaringScope = functions = module.DefineType("Functions", TypeAttributes.Public | TypeAttributes.Sealed);

			if (closures.Count == 0)
			{
				MethodBuilder method = declaringScope.DefineMethod(
					string.Format("Function{0:d4}", count++),
					MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig);

				method.SetReturnType(function.retType.GetSystemType());
				method.SetParameters(ParamDecl.GetSystemTypes(function.@params));

				int pos = 0;
				foreach (ParamDecl param in function.@params) param.SetLocation(pos++);

				Type[] typeArgs = ExtractTypeArgs(declaringScope, function.closureTypes, method);
				if (typeArgs != null) function.SetLocation(null, method.MakeGenericMethod(typeArgs), null, closures);
				else function.SetLocation(null, method, null, closures);

				return new ModuleCodeGenState(this, method.GetILGenerator());
			}
			else if (closures.Count == 1)
			{
				MethodBuilder method = declaringScope.DefineMethod(
					string.Format("Closure{0:d4}", count++),
					MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig);

				method.SetReturnType(function.retType.GetSystemType());

				Type[] paramTypes = new Type[function.@params.Length + 1];
				paramTypes[0] = closures[0].type.GetSystemType();
				for (int i = 1; i < paramTypes.Length; i++) paramTypes[i] = function.@params[i - 1].GetSystemType();
				method.SetParameters(paramTypes);

				closures[0].inside.Set(new ParamLoc(0, closures[0].type.GetSystemType()));
				int pos = 1;
				foreach (ParamDecl param in function.@params) param.SetLocation(pos++);

				Type[] typeArgs = ExtractTypeArgs(declaringScope, function.closureTypes, method);
				if (typeArgs != null) function.SetLocation(null, method.MakeGenericMethod(typeArgs), null, closures);
				else function.SetLocation(null, method, null, closures);
				
				return new ModuleCodeGenState(this, method.GetILGenerator());
			}
			else
			{
				TypeBuilder closureType;
				ConstructorInfo closureCtor;
				MakeClosureClass(
					declaringScope, 
					string.Format("Closure{0:d4}", count++), 
					function.closureTypes, 
					closures, 
					out closureType, 
					out closureCtor);
				this.closures.Add(closureType);

				MethodBuilder method = closureType.DefineMethod(
					"Invoke",
					MethodAttributes.Public | MethodAttributes.HideBySig,
					CallingConventions.HasThis,
					function.retType.GetSystemType(),
					ParamDecl.GetSystemTypes(function.@params));

				int pos = 1;
				foreach (ParamDecl param in function.@params) param.SetLocation(pos++);
				function.SetLocation(closureCtor, method, null, closures);

				return new ModuleCodeGenState(this, method.GetILGenerator());
			}
		}

		internal ModuleCodeGenState DefineRun(Type returnType)
		{
			return DefineRun(returnType);
		}

		internal ModuleCodeGenState DefineRun(Type returnType, out MethodBuilder method)
		{
			method = module.DefineGlobalMethod(
				"Run",
				MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
				returnType,
				Type.EmptyTypes);

			return new ModuleCodeGenState(this, method.GetILGenerator());
		}

		//public void PushTypeVar(TypeVarDecl decl)
		//{
		//    typeVars.Mark();
		//    typeVars.Add(decl.sym, decl);
		//}

		//public void PushTypeVars(TypeVarDecl[] decls)
		//{
		//    typeVars.Mark();
		//    for (int i = 0; i < decls.Length; i++)
		//        typeVars.Add(decls[i].sym, decls[i]);
		//}

		//public void PopTypeVars()
		//{
		//    typeVars.Remove();
		//}

		//public TypeVarDecl LookupTypeVar(int sym)
		//{
		//    if (typeVars.ContainsKey(sym)) return typeVars[sym];
		//    else throw new Exception(string.Format("Type variable '{0}' is not declared in this scope.", sym));
		//}

		public void EmitCspObject(ILGenerator code, object obj)
		{
			if (!cspStore.Contains(obj)) throw new InvalidOperationException("CSP object index out of range.");

			if (cspStoreType == null)
			{
				cspStoreType = module.DefineType("CspStore", TypeAttributes.NotPublic | TypeAttributes.Sealed);
				cspFields = new Dictionary<object, FieldInfo>();
			}

			FieldInfo field;
			if (cspFields.ContainsKey(obj)) field = cspFields[obj];
			else
			{
				field = cspStoreType.DefineField(string.Format("CSP{0:d4}", cspFields.Count), obj.GetType(), FieldAttributes.Static | FieldAttributes.Assembly);
				cspFields.Add(obj, field);
			}

			code.Emit(OpCodes.Ldsfld, field);
		}

		internal void FinalizeCsp(bool persist)
		{
			foreach (TypeBuilder type in closures)
			{
				type.CreateType();
			}

			if (functions != null) functions.CreateType();

			if (cspStore.Count > 0)
			{
				if (persist)
				{
					IResourceWriter locals = module.DefineResource("CspLocal", null);
					IResourceWriter remotes = module.DefineResource("CspRemote", null);
					BinaryFormatter bin = new BinaryFormatter();
					foreach (object obj in cspStore)
					{
						Type type = obj.GetType();
						FieldInfo field = cspFields[obj];
						if (type.IsSubclassOf(typeof(MarshalByRefObject)))
						{
							ObjRef objRef = RemotingServices.Marshal((MarshalByRefObject)obj);
							MemoryStream ms = new MemoryStream();
							bin.Serialize(ms, objRef);
							ms.Close();
							remotes.AddResource(field.Name, ms.ToArray());
						}
						else if (type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
						{
							locals.AddResource(field.Name, obj);
						}
						else
						{
							throw new Exception(string.Format("Cannot marshal CSP object of type {0}.", type));
						}
					}
					ConstructorBuilder cctor = cspStoreType.DefineTypeInitializer();
					ILGenerator code = cctor.GetILGenerator();
					code.Emit(OpCodes.Call, typeof(Csp).GetMethod("RestoreCspObjects"));
					code.Emit(OpCodes.Ret);
					cspStoreType.CreateType();
					module.CreateGlobalFunctions();
				}
				else
				{
					MethodBuilder cctor = cspStoreType.DefineMethod("Initialize", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, typeof(void), new Type[] { typeof(object[]) });
					ILGenerator code = cctor.GetILGenerator();
					object[] objs = cspStore.ToArray();
					for (int index = 0; index < objs.Length; index++)
					{
						code.Emit(OpCodes.Ldarg_0);
						code.Emit(OpCodes.Ldc_I4, index);
						code.Emit(OpCodes.Ldelem_Ref);
						Type type = objs[index].GetType();
						code.Emit(OpCodes.Castclass, type);
						if (type.IsValueType) code.Emit(OpCodes.Unbox, type);
						FieldInfo field = cspFields[objs[index]];
						code.Emit(OpCodes.Stsfld, field);
					}
					code.Emit(OpCodes.Ret);
					Type t = cspStoreType.CreateType();
					module.CreateGlobalFunctions();
					MethodInfo initialize = t.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
					initialize.Invoke(null, new object[] { objs });
				}
			}
			else module.CreateGlobalFunctions();
		}

		private void MakeClosureClass(TypeBuilder declaringScope, string name, List<ClosureType> typeVars, List<Closure> vars, out TypeBuilder typeBuilder, out ConstructorInfo ctor)
		{
			typeBuilder = declaringScope.DefineNestedType(
				name,
				TypeAttributes.Sealed | TypeAttributes.NestedPublic | TypeAttributes.Serializable);
			
			Type type = typeBuilder;
			if (typeVars.Count > 0)
			{
				string[] names = new string[typeVars.Count];
				for (int i = 0; i < names.Length; i++) names[i] = "T" + i.ToString("d2");
				GenericTypeParameterBuilder[] genericParams = typeBuilder.DefineGenericParameters(names);
				Type[] typeArgs = new Type[genericParams.Length];
				for (int i = 0; i < genericParams.Length; i++)
				{
					typeArgs[i] = typeVars[i].outside.GetSystemType();
					typeVars[i].inside.Set(new GenericParamType(genericParams[i]));
				}
				type = typeBuilder.MakeGenericType(typeArgs);
			}

			Type[] types = new Type[vars.Count];
			for (int i = 0; i < types.Length; i++) types[i] = vars[i].type.GetSystemType();

			ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
				CallingConventions.HasThis,
				types);
			if (typeVars.Count > 0) ctor = TypeBuilder.GetConstructor(type, ctorBuilder);
			else ctor = ctorBuilder;

			ILGenerator code = ctorBuilder.GetILGenerator();
			code.Emit(OpCodes.Ldarg_0);
			code.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
			//fields = new FieldInfo[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				FieldBuilder fieldBuilder = typeBuilder.DefineField("field" + i.ToString("d4"), types[i], FieldAttributes.Private);
				code.Emit(OpCodes.Ldarg_0);
				code.Emit(OpCodes.Ldarg, i + 1);
				code.Emit(OpCodes.Stfld, fieldBuilder);
				if (typeVars.Count > 0) 
					vars[i].inside.Set(new FieldLoc(new ParamLoc(0, type), TypeBuilder.GetField(type, fieldBuilder)));
				else vars[i].inside.Set(new FieldLoc(new ParamLoc(0, type), fieldBuilder));
			}
			code.Emit(OpCodes.Ret);
		}
	}

	internal class TypeCheckState
	{
		protected int level = 0;

		// local variables and method/function parameters that are in scope
		protected GroupStack<VarDecl> vars = new GroupStack<VarDecl>();

		// the function definition that are may be in scope
		protected Function func = null;

		// closure varibles used by the current function
		protected List<Closure> closures = new List<Closure>();
		protected Dictionary<int, ClosureTypeVarDecl> closureTypes = new Dictionary<int, ClosureTypeVarDecl>();

		protected GroupStack<TypeVarDecl> typeVars = new GroupStack<TypeVarDecl>();
		
		// store for all CSP objects used in the current code generation
		// only the top level function/method has this
		protected Set<object> rootCspStore;
		protected bool hasCsp;

		protected List<Function> functions;

		protected TypeCheckState outer;

		public TypeCheckState(Set<object> rootCspStore, TypeCheckState outer)
		{
			this.rootCspStore = rootCspStore;
			this.outer = outer;
			if (outer != null) this.functions = outer.functions;
			else this.functions = new List<Function>();
			hasCsp = false;
		}

		public static TypeCheckState CreateRootState(Set<object> rootCspStore, ParamDecl[] @params)
		{
			TypeCheckState newState = new TypeCheckState(rootCspStore, null);
			if (@params != null)
				foreach (ParamDecl param in @params)
				{
					param.type.type.GenerateClosures(newState);
					newState.vars.Push(param);
				}
			return newState;
		}

		public bool IsTopMost()
		{
			return outer == null;
		}

		public TypeCheckState CreateNestedState(Function func)
		{
			TypeCheckState newState = new TypeCheckState(rootCspStore, this);
			newState.vars.Mark();
			func.delegateType.GenerateClosures(newState);
			newState.func = func;
			foreach (ParamDecl param in func.@params)
			{
				param.type.type.GenerateClosures(newState);
				newState.vars.Push(param);
			}
			return newState;
		}

		public int GetLevel()
		{
			return level;
		}

		public void RaiseLevel()
		{
			level++;
		}

		public void LowerLevel()
		{
			if (level == 0) throw new InvalidOperationException("Attempt to lower level at level 0.");
			else level--;
		}

		#region Type Variable Scope Management
		public void PushTypeVar(TypeVarDecl decl)
		{
			typeVars.Mark();
			decl.con.GenerateClosures(this);
			typeVars.Push(decl);
		}

		public void PushTypeVars(TypeVarDecl[] decls)
		{
			typeVars.Mark();
			foreach(TypeVarDecl decl in decls)
			{
				decl.con.GenerateClosures(this);
				typeVars.Push(decl);
			}
		}

		public void PopTypeVars()
		{
			typeVars.Pop();
		}

		//public TypeVarDecl LookupTypeVar(int sym)
		//{
		//    TypeVarDecl decl = LookupTypeVarInternal(sym);
		//    if (decl != null) return decl;
		//    else throw new Exception(string.Format("Type variable '{0}' is not declared in this scope.", sym));
		//}

		//protected virtual TypeVarDecl LookupTypeVarInternal(int sym)
		//{
		//    if (typeVars.ContainsKey(sym)) return typeVars[sym];
		//    if (closureTypes.ContainsKey(sym)) return closureTypes[sym];
		//    if (outer != null)
		//    {
				
		//        TypeVarDecl decl = outer.LookupTypeVarInternal(sym);
		//        if (decl != null)
		//        {
		//            ClosureTypeVarDecl closureDecl = new ClosureTypeVarDecl(decl);
		//            closureTypes.Add(closureDecl.sym, closureDecl);
		//            return closureDecl;
		//        }
		//    }
		//    return null;
		//}

		public List<ClosureTypeVarDecl> GetClosureTypes()
		{
			return new List<ClosureTypeVarDecl>(closureTypes.Values);
		}
		#endregion

		#region Member Management
		private Stack<ForField> fields = new Stack<ForField>();

		public void PushField(ForField decl)
		{
			PushTypeVar(decl.fieldType);
			fields.Push(decl);
		}


		public void PopField()
		{
			fields.Pop();
			PopTypeVars();
		}

		public ForField LookupField(int sym)
		{
			ForField decl = LookupFieldInternal(sym);
			if (decl != null) return decl;
			else throw new Exception(string.Format("Reflected field '{0}' is not declared in this scope.", sym));
		}

		protected ForField LookupFieldInternal(int sym)
		{
			foreach (ForField field in fields)
				if (field.sym == sym) return field;
			if (outer != null)
			{
				//if (closureTypes.ContainsKey(sym)) return closureTypes[sym];
				ForField decl = outer.LookupFieldInternal(sym);
				if (decl != null)
				{
					//ClosureTypeVarDecl closureDecl = new ClosureTypeVarDecl(decl);
					//closureTypes.Add(closureDecl.sym, closureDecl);
					//return closureDecl;
					throw new NotImplementedException("capture of reflected fields");
				}
			}
			return null;
		}
		#endregion

		#region Variable Scope Management
		public void PushScope()
		{
			vars.Mark();
		}

		public void PushMethod(ThisDecl @this, ParamDecl[] @params)
		{
			vars.Mark();
			if (@this != null)
			{
				@this.type.GenerateClosures(this);
				vars.Push(@this);
			}
			foreach (ParamDecl param in @params)
			{
				param.type.type.GenerateClosures(this);
				vars.Push(param);
			}
		}

		public void PushLocal(LocalDecl decl)
		{
			decl.type.GenerateClosures(this);
			vars.Push(decl);
		}

		public Location LookupVar(VarDecl decl)
		{
			Location loc = LookupVarInternal(decl);
			if (loc != null) return loc;
			else throw new Exception(string.Format("Local variable '{0}' is not declared in this scope.", decl.name));
		}

		protected virtual Location LookupVarInternal(VarDecl decl)
		{
			if (func == decl || vars.Contains(decl)) return decl.GetLocation();
			foreach (Closure closure in closures)
				if (closure.decl == decl) return closure.inside;
			if (outer != null)
			{
				Location outside = outer.LookupVarInternal(decl);
				if (outside != null)
				{
					Closure closure = new Closure(decl, decl.GetMType(), outside);
					closures.Add(closure);
					return closure.inside;
				}
			}
			return null;
		}

		public void PopScope()
		{
			vars.Pop();
		}

		public void PopMethod()
		{
			func = null;
			vars.Clear();
			functions.Clear();
		}

		public bool ContainsClosures()
		{
			return closures.Count > 0;
		}

		public List<Closure> GetClosures()
		{
			return closures;
		}

		public void AddCspObject(object obj)
		{
			hasCsp = true;
			if(!rootCspStore.Contains(obj))
				rootCspStore.Add(obj);
		}

		public bool ContainsCspObjects()
		{
			return hasCsp;
		}

		public bool AddFunction(Function func)
		{
			foreach (Function func2 in functions)
				if (func == func2) return false;
			functions.Add(func);
			return true;
		}

		public List<Function> GetFunctions()
		{
			return functions;
		}
		#endregion
	}

	internal abstract class CodeGenState
	{
		// the staging level
		internal int level = 0;

		// the buffer to write IL instructions
		internal ILGenerator code;

		// the return label
		protected Label returnLabel;

		protected CodeGenState(ILGenerator code)
		{
			this.code = code;
			this.returnLabel = code.DefineLabel();
		}

		#region Loop Management
		// the continue and break labels for the nested loops inside this function/method
		private readonly Stack<Collections.Tuple<Label, Label>> loops = new Stack<Collections.Tuple<Label, Label>>();

		public void PushLoop(out Label @continue, out Label @break)
		{
			@continue = code.DefineLabel();
			@break = code.DefineLabel();
			loops.Push(new Collections.Tuple<Label, Label>(@continue, @break));
		}

		public void PopLoop()
		{
			loops.Pop();
		}
		
		public void EndMethodBody()
		{
			code.MarkLabel(returnLabel);
			code.Emit(OpCodes.Ret);
		}
		#endregion

		#region Emit helpers
		public void EmitInt(int i)
		{
			switch(i)
			{
				case -1: code.Emit(OpCodes.Ldc_I4_M1); break;
				case 0: code.Emit(OpCodes.Ldc_I4_0); break;
				case 1: code.Emit(OpCodes.Ldc_I4_1); break;
				case 2: code.Emit(OpCodes.Ldc_I4_2); break;
				case 3: code.Emit(OpCodes.Ldc_I4_3); break;
				case 4: code.Emit(OpCodes.Ldc_I4_4); break;
				case 5: code.Emit(OpCodes.Ldc_I4_5); break;
				case 6: code.Emit(OpCodes.Ldc_I4_6); break;
				case 7: code.Emit(OpCodes.Ldc_I4_7); break;
				case 8: code.Emit(OpCodes.Ldc_I4_8); break;
				default: code.Emit(OpCodes.Ldc_I4, i); break;
			}
		}

		public void EmitBool(bool b)
		{
			code.Emit(b ? OpCodes.Ldc_I4_1: OpCodes.Ldc_I4_0);
		}

		public void EmitString(string s)
		{
			if(s != null) code.Emit(OpCodes.Ldstr, s);
			else code.Emit(OpCodes.Ldnull);
		}

		public void EmitNull()
		{
			code.Emit(OpCodes.Ldnull);
		}

		public void EmitPop()
		{
			code.Emit(OpCodes.Pop);
		}

		public void EmitLdarg(int pos)
		{
			if (pos == 0) code.Emit(OpCodes.Ldarg_0);
			else if (pos == 1) code.Emit(OpCodes.Ldarg_1);
			else if (pos == 2) code.Emit(OpCodes.Ldarg_2);
			else if (pos == 3) code.Emit(OpCodes.Ldarg_3);
			else if (pos < 256) code.Emit(OpCodes.Ldarg_S, pos);
			else code.Emit(OpCodes.Ldarg, pos);
		}

		public void EmitStarg(int pos)
		{
			if (pos < 256) code.Emit(OpCodes.Starg_S, pos);
			else code.Emit(OpCodes.Starg, pos);
		}

		public void EmitLdarga(int pos)
		{
			if (pos < 256) code.Emit(OpCodes.Ldarga_S, pos);
			else code.Emit(OpCodes.Ldarga, pos);
		}

		public int DeclareLocal(Type type)
		{
			return code.DeclareLocal(type).LocalIndex;
		}

		public void EmitLdloc(int pos)
		{
			if (pos == 0) code.Emit(OpCodes.Ldloc_0);
			else if (pos == 1) code.Emit(OpCodes.Ldloc_1);
			else if (pos == 2) code.Emit(OpCodes.Ldloc_2);
			else if (pos == 3) code.Emit(OpCodes.Ldloc_3);
			else if (pos < 256) code.Emit(OpCodes.Ldloc_S, pos);
			else code.Emit(OpCodes.Ldloc, pos);
		}

		public void EmitStloc(int pos)
		{
			if (pos == 0) code.Emit(OpCodes.Stloc_0);
			else if (pos == 1) code.Emit(OpCodes.Stloc_1);
			else if (pos == 2) code.Emit(OpCodes.Stloc_2);
			else if (pos == 3) code.Emit(OpCodes.Stloc_3);
			else if (pos < 256) code.Emit(OpCodes.Stloc_S, pos);
			else code.Emit(OpCodes.Stloc, pos);
		}

		public void EmitLdloca(int pos)
		{
			if (pos < 256) code.Emit(OpCodes.Ldloca_S, pos);
			else code.Emit(OpCodes.Ldloca, pos);
		}

		public void EmitLdind(CILType cilType)
		{
			switch (cilType)
			{
				case CILType.I1:
					code.Emit(OpCodes.Ldind_I1); 
					break;
				case CILType.I2:
					code.Emit(OpCodes.Ldind_I2); 
					break;
				case CILType.I4:
					code.Emit(OpCodes.Ldind_I4); 
					break;
				case CILType.I8:
					code.Emit(OpCodes.Ldind_I8); 
					break;
				case CILType.R4:
					code.Emit(OpCodes.Ldind_R4); 
					break;
				case CILType.R8:
					code.Emit(OpCodes.Ldind_R8); 
					break;
				case CILType.Ref:
					code.Emit(OpCodes.Ldind_Ref); 
					break;
				default:
					throw new Exception("Unknown CIL type.");
			}
		}

		public void EmitStind(CILType cilType)
		{
			switch (cilType)
			{
				case CILType.I1:
					code.Emit(OpCodes.Stind_I1);
					break;
				case CILType.I2:
					code.Emit(OpCodes.Stind_I2);
					break;
				case CILType.I4:
					code.Emit(OpCodes.Stind_I4);
					break;
				case CILType.I8:
					code.Emit(OpCodes.Stind_I8);
					break;
				case CILType.R4:
					code.Emit(OpCodes.Stind_R4);
					break;
				case CILType.R8:
					code.Emit(OpCodes.Stind_R8);
					break;
				case CILType.Ref:
					code.Emit(OpCodes.Stind_Ref);
					break;
				default:
					throw new Exception("Unknown CIL type.");
			}
		}

		public void EmitLdelem(MType type)
		{
			switch (type.GetCILType())
			{
				case CILType.I1:
					code.Emit(OpCodes.Ldelem_I1);
					break;
				case CILType.I2:
					code.Emit(OpCodes.Ldelem_I2);
					break;
				case CILType.I4:
					code.Emit(OpCodes.Ldelem_I4);
					break;
				case CILType.I8:
					code.Emit(OpCodes.Ldelem_I8);
					break;
				case CILType.R4:
					code.Emit(OpCodes.Ldelem_R4);
					break;
				case CILType.R8:
					code.Emit(OpCodes.Ldelem_R8);
					break;
				case CILType.Ref:
					//code.Emit(OpCodes.Ldelem_Ref);
					code.Emit(OpCodes.Ldelem, type.GetSystemType());
					break;
				default:
					throw new Exception("Unknown CIL type.");
			}
		}

		public void EmitStelem(MType type)
		{
			switch(type.GetCILType())
			{
				case CILType.I1:
					code.Emit(OpCodes.Stelem_I1);
					break;
				case CILType.I2:
					code.Emit(OpCodes.Stelem_I2);
					break;
				case CILType.I4 :
					code.Emit(OpCodes.Stelem_I4); 
					break;
				case CILType.I8:
					code.Emit(OpCodes.Stelem_I8);
					break;
				case CILType.R4:
					code.Emit(OpCodes.Stelem_R4);
					break;
				case CILType.R8:
					code.Emit(OpCodes.Stelem_R8);
					break;
				case CILType.Ref :
					code.Emit(OpCodes.Stelem, type.GetSystemType()); 
					break;
				default :
					throw new Exception("Unknown CIL type.");
			}
		}

		public void EmitReturn()
		{
			code.Emit(OpCodes.Br, returnLabel);
		}

		public void EmitContinue()
		{
		    Collections.Tuple<Label, Label> labels = loops.Peek();
			code.Emit(OpCodes.Br, labels.fst);
		}

		public void EmitBreak()
		{
		    Collections.Tuple<Label, Label> labels = loops.Peek();
			code.Emit(OpCodes.Br, labels.snd);
		}

		public void EmitLabel(Label label)
		{
			code.MarkLabel(label);
		}

		public void EmitCondJump(Label label)
		{
			code.Emit(OpCodes.Brtrue, label);
		}

		public void EmitGoto(Label label)
		{
			code.Emit(OpCodes.Br, label);
		}

//
//		public void LoadTypeFactory()
//		{
//			PropertyInfo propInfo = typeof(TypeFactory).GetProperty("Current");
//			MethodInfo methodInfo = propInfo.GetGetMethod();
//			code.Emit(OpCodes.Call, methodInfo);
//		}

		public void MakeCode(Type type)
		{
			ConstructorInfo ctorInfo = type.GetConstructors()[0];
			code.Emit(OpCodes.Newobj, ctorInfo);
		}

		public void MakeCode(Type type, string name)
		{
			MethodInfo methodInfo = type.GetMethod(name, BindingFlags.Public | BindingFlags.Static);
			code.Emit(OpCodes.Call, methodInfo);
		}

		public void MakeType(Type type)
		{
			MethodInfo methodInfo = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
			code.Emit(OpCodes.Call, methodInfo);
		}

		private static readonly MethodInfo getTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
		public void EmitType(Type type)
		{
			code.Emit(OpCodes.Ldtoken, type);
			code.Emit(OpCodes.Call, getTypeFromHandle);
		}

		public void EmitTypes(Type[] types)
		{
			int num = types.Length;
			EmitInt(num);
			code.Emit(OpCodes.Newarr, typeof(Type));
			for(int i = 0; i < num; i++)
			{
				code.Emit(OpCodes.Dup);
				EmitInt(i);
				EmitType(types[i]);
				code.Emit(OpCodes.Stelem_Ref);
			}
		}

		private static readonly MethodInfo getMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });
		private static readonly MethodInfo getGenericMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });
		public void EmitMethod(MMethodInfo method)
		{
			code.Emit(OpCodes.Ldtoken, method.GetSystemMethod());
			MType type = method.GetDeclaringType();
			if (type.IsGeneric())
			{
				code.Emit(OpCodes.Ldtoken, type.GetSystemType());
				code.Emit(OpCodes.Call, getGenericMethodFromHandle);
			}
			else code.Emit(OpCodes.Call, getMethodFromHandle);
			code.Emit(OpCodes.Castclass, typeof(MethodInfo));
		}

		private static readonly MethodInfo getFieldFromHandle = typeof(FieldInfo).GetMethod("GetFieldFromHandle", new Type[] { typeof(RuntimeFieldHandle) });
		private static readonly MethodInfo getGenericFieldFromHandle = typeof(FieldInfo).GetMethod("GetFieldFromHandle", new Type[] { typeof(RuntimeFieldHandle), typeof(RuntimeTypeHandle) });
		public void EmitField(MFieldInfo field)
		{
			code.Emit(OpCodes.Ldtoken, field.GetSystemField());
			MType type = field.GetDeclaringType();
			if (type.IsGeneric())
			{
				code.Emit(OpCodes.Ldtoken, type.GetSystemType());
				code.Emit(OpCodes.Call, getGenericFieldFromHandle);
			}
			else code.Emit(OpCodes.Call, getFieldFromHandle);
		}

		public void EmitConstructor(MConstructorInfo ctor)
		{
			code.Emit(OpCodes.Ldtoken, ctor.GetSystemConstructor());
			MType type = ctor.GetDeclaringType();
			if (type.IsGeneric())
			{
				code.Emit(OpCodes.Ldtoken, type.GetSystemType());
				code.Emit(OpCodes.Call, getGenericMethodFromHandle);
			}
			else code.Emit(OpCodes.Call, getMethodFromHandle);
			code.Emit(OpCodes.Castclass, typeof(ConstructorInfo));
		}

		public void CallBaseCtor(MType baseType)
		{
			code.Emit(OpCodes.Ldarg_0);
			Type type = baseType.GetSystemType();
			ConstructorInfo ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
			while(ctor == null)
			{
				type = type.BaseType;
				type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
			}
			code.Emit(OpCodes.Call, ctor);
		}

		public void EmitCast(Type type)
		{
			if(type.IsValueType)
			{
				code.Emit(OpCodes.Unbox, type);
				code.Emit(OpCodes.Ldobj, type);
			}
			else code.Emit(OpCodes.Castclass, type);
		}

		public void CreateDelegate(Type delegateType)
		{
			code.Emit(OpCodes.Newobj, delegateType.GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
		}

		public void InvokeDelegate(Type delegateType)
		{
			code.Emit(OpCodes.Call, delegateType.GetMethod("Invoke"));
		}
#endregion

		//public abstract void EmitClosureVar(int pos);

		//public abstract void EmitClosureVarAssign(int pos, AssignOp op, Code rhs, AssignRet ret);

		//public abstract void EmitClosureVarRef(int pos);

		public abstract void EmitCspStore();

		public abstract void EmitCspObject(object obj);
	}

	internal sealed class ModuleCodeGenState : CodeGenState
	{
		private ModuleGenState module;


		public ModuleCodeGenState(ModuleGenState module, ILGenerator code)
			: base(code)
		{
			this.module = module;
		}

		//public override void EmitClosureVar(int pos)
		//{
		//    if (closure == null) throw new InvalidOperationException();
		//    code.Emit(OpCodes.Ldarg_0);
		//    if(closure.fields != null) code.Emit(OpCodes.Ldfld, closure.fields[pos]);
		//}

		//public override void EmitClosureVarAssign(int pos, AssignOp op, Code rhs, AssignRet ret)
		//{
		//    if (closure == null) throw new InvalidOperationException();
		//    FieldInfo field = closure.fields != null ? closure.fields[pos] : null;
		//    if (ret == AssignRet.Post)
		//    {
		//        code.Emit(OpCodes.Ldarg_0);
		//        if (field != null) code.Emit(OpCodes.Ldfld, field);
		//    }
		//    if (field != null)
		//    {
		//        code.Emit(OpCodes.Ldarg_0);
		//        if (ret == AssignRet.Pre) code.Emit(OpCodes.Dup);
		//    }
		//    if (op != AssignOp.Nop)
		//    {
		//        if (field == null) code.Emit(OpCodes.Ldarg_0);
		//        else
		//        {
		//            code.Emit(OpCodes.Dup);
		//            code.Emit(OpCodes.Ldfld, field);
		//        }
		//    }
		//    rhs.CodeGen(this);
		//    Assign.CodeGenAssignOp(this, op);
		//    if (field == null) code.Emit(OpCodes.Starg_S, 0);
		//    else code.Emit(OpCodes.Stfld, field);
		//    if (ret == AssignRet.Pre)
		//    {
		//        if (field == null) code.Emit(OpCodes.Ldc_I4_0);
		//        else code.Emit(OpCodes.Ldfld, field);
		//    }
		//}

		//public override void EmitClosureVarRef(int pos)
		//{
		//    if (closure == null) throw new InvalidOperationException();
		//    FieldInfo field = closure.fields != null ? closure.fields[pos] : null;
		//    if (field == null) code.Emit(OpCodes.Ldarga_S, 0);
		//    else
		//    {
		//        code.Emit(OpCodes.Ldarg_0);
		//        code.Emit(OpCodes.Ldflda, field);
		//    }
		//}

		public override void EmitCspStore()
		{
			// when generating code in a module there is no explicit CSP Store object
			// CSP objects are stored in global fields of the module
			// hence when we want to load the CSP Store object we don't have to do anything
		}

		public override void EmitCspObject(object obj)
		{
			module.EmitCspObject(code, obj);
		}
	}

	internal sealed class DynamicMethodGenState : GenState
	{
		private static int id = 0;

		private Module module;

		public Set<object> cspObjects;
		private VirtualTuple cspStoreType;
		private Dictionary<object, int> cspFields;

		public DynamicMethodGenState(Module module)
		{
			this.module = module;
			this.cspObjects = new Set<object>();
		}

		public object MakeCspStore()
		{
			if (cspObjects.Count > 0)
			{
				object[] objs = cspObjects.ToArray();
				Type[] cspTypes = new Type[objs.Length];
				for (int i = 0; i < cspTypes.Length; i++)
					cspTypes[i] = objs[i].GetType();
				cspStoreType = new VirtualTuple(cspTypes);

				cspFields = new Dictionary<object, int>();
				for (int i = 0; i < cspTypes.Length; i++)
					cspFields.Add(objs[i], i);

				return cspStoreType.Create(objs);
			}
			else
			{
				cspStoreType = null;
				return null;
			}
		}

		public override CodeGenState DefineFunction(Function func, List<Closure> closures)
		{
			if (func.closureTypes.Count > 0) throw new InvalidOperationException("Cannot compile a function as a DynamicMethod when it captures type variables.");

			DynamicMethod dm;
			List<Type> closureTypes = new List<Type>();
			if (func.hasCsp) closureTypes.Add(cspStoreType.GetTupleType());
			foreach (Closure closure in closures) closureTypes.Add(closure.type.GetSystemType());
			Location cspStore = null;

			VirtualTuple closureParam = null;
			if (closureTypes.Count == 0)
			{
				dm = new DynamicMethod(
					string.Format("Function{0:d4}", id++),
					func.retType.GetSystemType(),
					ParamDecl.GetSystemTypes(func.@params),
					module, true);

				int pos = 0;
				foreach (ParamDecl param in func.@params) param.SetLocation(pos++);
				func.SetLocation(null, dm, null, null);
			}
			else
			{
				closureParam = new VirtualTuple(closureTypes.ToArray());
				Type[] paramTypes = new Type[func.@params.Length + 1];
				paramTypes[0] = closureParam.GetTupleType();
				for (int i = 1; i < paramTypes.Length; i++) paramTypes[i] = func.@params[i - 1].GetSystemType();

				dm = new DynamicMethod(
					string.Format("Function{0:d4}", id++),
					func.retType.GetSystemType(),
					paramTypes,
					module, true);

				if (func.hasCsp)
				{
					if (closures.Count == 0) cspStore = new ParamLoc(0, cspStoreType.GetTupleType());
					else cspStore = new FieldLoc(new ParamLoc(0, closureParam.GetTupleType()), closureParam.GetFieldInfo(0));
				}
				else if (closures.Count == 1) closures[0].inside.Set(new ParamLoc(0, closures[0].type.GetSystemType()));
				int pos = func.hasCsp ? 1 : 0;
				if (func.hasCsp || closures.Count > 1)
					foreach (Closure decl in closures)
					{
						decl.inside.Set(new FieldLoc(new ParamLoc(0, closureParam.GetTupleType()), closureParam.GetFieldInfo(pos++)));
					}
				pos = 1;
				foreach (ParamDecl param in func.@params) param.SetLocation(pos++);
				func.SetLocation(closureParam.GetConstructorInfo(), dm, cspStore, closures);
			}

			return new DynamicMethodCodeGenState(this, cspStore, dm);
		}

		public DynamicMethodCodeGenState DefineRun(Type returnType)
		{
			Type[] paramTypes;
			if (cspStoreType != null) paramTypes = new Type[] { cspStoreType.GetTupleType() };
			else paramTypes = Type.EmptyTypes;

			return new DynamicMethodCodeGenState(this, 
				cspStoreType != null ? new ParamLoc(0, cspStoreType.GetTupleType()): null,
				new DynamicMethod(
					string.Format("Function{0:d4}", id++),
					returnType,
					paramTypes,
					module, true));
		}

		public void EmitCspObject(ILGenerator code, object obj)
		{
			if (!cspFields.ContainsKey(obj)) throw new InvalidOperationException("CSP object index out of range.");
			FieldInfo field = cspStoreType.GetFieldInfo(cspFields[obj]);
			if(field != null) code.Emit(OpCodes.Ldfld, field);
		}
	}

	internal sealed class DynamicMethodCodeGenState : CodeGenState
	{
		private DynamicMethodGenState module;
		private Location cspStore;
		public DynamicMethod dm;

		public DynamicMethodCodeGenState(DynamicMethodGenState module, Location cspStore, DynamicMethod dm)
			: base(dm.GetILGenerator())
		{
			this.module = module;
			this.cspStore = cspStore;
			this.dm = dm;
		}

		public override void EmitCspStore()
		{
			cspStore.CodeGen(code);
		}

		public override void EmitCspObject(object obj)
		{
			cspStore.CodeGen(code);
			module.EmitCspObject(code, obj);
		}
	}

	public static class Csp
	{
		public static object LoadGcHandle(IntPtr ptr)
		{
			return ((GCHandle)ptr).Target;
		}

		public static void RestoreCspObjects()
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			Type cspClass = assembly.GetType("CspStore");
			using (System.IO.Stream rs = assembly.GetManifestResourceStream("CspLocal"))
			{
				if (rs != null)
				{
					ResourceReader rr = new ResourceReader(rs);
					IDictionaryEnumerator e = rr.GetEnumerator();
					while (e.MoveNext())
					{
						FieldInfo cspField = cspClass.GetField((string)e.Key, BindingFlags.NonPublic | BindingFlags.Static);
						cspField.SetValue(null, e.Value);
					}
					rr.Close();
				}
			}
			using (System.IO.Stream rs = assembly.GetManifestResourceStream("CspRemote"))
			{
				if (rs != null)
				{
					BinaryFormatter bin = new BinaryFormatter();
					ResourceReader rr = new ResourceReader(rs);
					IDictionaryEnumerator e = rr.GetEnumerator();
					while (e.MoveNext())
					{
						MemoryStream ms = new MemoryStream((byte[])e.Value);
						object obj = bin.Deserialize(ms);
						ms.Close();
						FieldInfo cspField = cspClass.GetField((string)e.Key, BindingFlags.NonPublic | BindingFlags.Static);
						cspField.SetValue(null, obj);
					}
					rr.Close();
				}
			}
		}
	}

	internal sealed class ClosureType
	{
		public TypeVarDecl decl;
		public LocationType outside;
		public DeferredType inside;
	}

	internal abstract class LocationType
	{
		public abstract Type GetSystemType();
	}

	internal sealed class DeferredType : LocationType
	{
		private LocationType deferredType;

		public DeferredType()
		{
		}

		public void Set(LocationType deferredType)
		{
			this.deferredType = deferredType;
		}

		public override Type GetSystemType()
		{
			return deferredType.GetSystemType();
		}
	}

	internal sealed class GenericParamType : LocationType
	{
		private GenericTypeParameterBuilder genericParam;

		public GenericParamType(GenericTypeParameterBuilder genericParam)
		{
			this.genericParam = genericParam;
		}

		public override Type GetSystemType()
		{
			return genericParam;
		}
	}

	internal sealed class Closure
	{
		public object decl;
		public MType type;
		public Location outside;
		public DeferredLoc inside;

		public Closure(object decl, MType type, Location outside)
		{
			this.decl = decl;
			this.type = type;
			this.outside = outside;
			this.inside = new DeferredLoc();
		}

		public Closure(Closure closure)
			: this(closure.decl, closure.type, closure.inside)
		{
		}
	}

	internal delegate void CodeGen(ILGenerator code);

	internal abstract class Location
	{
		public abstract Type GetSystemType();

		public abstract void CodeGen(ILGenerator code);

		public abstract void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret);

		public abstract void CodeGenRef(ILGenerator code);

		public virtual void CodeGenCall(ILGenerator code, CodeGen args)
		{
			MethodInfo invokeMethod = GetSystemType().GetMethod("Invoke");
			if (invokeMethod == null) throw new InvalidOperationException("The location is not a delegate type.");
			CodeGen(code);
			args(code);
			code.Emit(OpCodes.Call, invokeMethod);
		}

		public static CILType GetCILType(Type type)
		{
			if (type.IsValueType)
			{
				if (type == typeof(Int32)) return CILType.I4;
				else if (type == typeof(Int64)) return CILType.I8;
				else if (type == typeof(Double)) return CILType.R8;
				else if (type == typeof(Single)) return CILType.R4;
				else if (type == typeof(Int16)) return CILType.I2;
				else if (type == typeof(Byte)) return CILType.U1;
				else if (type == typeof(IntPtr)) return CILType.I;
				else if (type == typeof(UInt32)) return CILType.U4;
				else if (type == typeof(UInt16)) return CILType.U2;
				else if (type == typeof(void)) return CILType.Void;
				else if (type == typeof(UInt32)) return CILType.U8;
				else if (type == typeof(SByte)) return CILType.I1;
				else if (type == typeof(UIntPtr)) return CILType.U;
				else return CILType.ValueType;
			}
			else return CILType.Ref;
		}
	}

	internal sealed class DeferredLoc : Location
	{
		private Location deferredLoc;

		public DeferredLoc()
		{
		}

		public void Set(Location deferredLoc)
		{
			this.deferredLoc = deferredLoc;
		}

		public override Type GetSystemType()
		{
			if (deferredLoc != null) return deferredLoc.GetSystemType();
			else throw new InvalidOperationException("Deferred location not set.");
		}

		public override void CodeGen(ILGenerator code)
		{
			if (deferredLoc != null) deferredLoc.CodeGen(code);
			else throw new InvalidOperationException("Deferred location not set.");
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			if (deferredLoc != null) deferredLoc.CodeGenAssign(code, op, rhs, ret);
			else throw new InvalidOperationException("Deferred location not set.");
		}

		public override void CodeGenRef(ILGenerator code)
		{
			if (deferredLoc != null) deferredLoc.CodeGenRef(code);
			else throw new InvalidOperationException("Deferred location not set.");
		}

		public override void CodeGenCall(ILGenerator code, CodeGen args)
		{
			if (deferredLoc != null) deferredLoc.CodeGenCall(code, args);
			else throw new InvalidOperationException("Deferred location not set.");
		}
	}

	internal sealed class ParamLoc : Location
	{
		private int pos;
		private Type type;

		public ParamLoc(int pos, Type type)
		{
			this.pos = pos;
			this.type = type;
		}

		public override Type GetSystemType()
		{
			return type;
		}

		public override void CodeGen(ILGenerator code)
		{
			if (pos == 0) code.Emit(OpCodes.Ldarg_0);
			else if (pos == 1) code.Emit(OpCodes.Ldarg_1);
			else if (pos == 2) code.Emit(OpCodes.Ldarg_2);
			else if (pos == 3) code.Emit(OpCodes.Ldarg_3);
			else if (pos < 256) code.Emit(OpCodes.Ldarg_S, pos);
			else code.Emit(OpCodes.Ldarg, pos);
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			if (op != AssignOp.Nop || ret == AssignRet.Post) CodeGen(code);
			if (op != AssignOp.Nop && ret == AssignRet.Post) code.Emit(OpCodes.Dup);
			rhs(code);
			Assign.CodeGenAssignOp(code, op);
			if (ret == AssignRet.Pre) code.Emit(OpCodes.Dup);
			if (pos < 256) code.Emit(OpCodes.Starg_S, pos);
			else code.Emit(OpCodes.Starg, pos);
		}

		public override void CodeGenRef(ILGenerator code)
		{
			if (pos < 256) code.Emit(OpCodes.Ldarga_S, pos);
			else code.Emit(OpCodes.Ldarga, pos);
		}
	}

	internal class AddrLoc : Location
	{
		private Location loc;
		private OpCode ldInstr, stInstr;
		private Type type;

		public AddrLoc(Location loc)
		{
			this.loc = loc;
			this.type = loc.GetSystemType().GetElementType();
			if (type == null) throw new InvalidOperationException("The type of 'loc' is not a pointer.");
			switch (GetCILType(type))
			{
				case CILType.I:
					this.ldInstr = OpCodes.Ldind_I;
					this.stInstr = OpCodes.Stind_I;
					break;
				case CILType.I1:
					this.ldInstr = OpCodes.Ldind_I1;
					this.stInstr = OpCodes.Stind_I1;
					break;
				case CILType.I2:
					this.ldInstr = OpCodes.Ldind_I2;
					this.stInstr = OpCodes.Stind_I2;
					break;
				case CILType.I4:
					this.ldInstr = OpCodes.Ldind_I4;
					this.stInstr = OpCodes.Stind_I4;
					break;
				case CILType.I8:
					this.ldInstr = OpCodes.Ldind_I8;
					this.stInstr = OpCodes.Stind_I8;
					break;
				case CILType.R4:
					this.ldInstr = OpCodes.Ldind_R4;
					this.stInstr = OpCodes.Stind_R4;
					break;
				case CILType.R8:
					this.ldInstr = OpCodes.Ldind_R8;
					this.stInstr = OpCodes.Stind_R8;
					break;
				case CILType.Ref:
					this.ldInstr = OpCodes.Ldind_Ref;
					this.stInstr = OpCodes.Stind_Ref;
					break;
				case CILType.U1:
					this.ldInstr = OpCodes.Ldind_U1;
					this.stInstr = OpCodes.Stobj;
					break;
				case CILType.U2:
					this.ldInstr = OpCodes.Ldind_U2;
					this.stInstr = OpCodes.Stobj;
					break;
				case CILType.U4:
					this.ldInstr = OpCodes.Ldind_U4;
					this.stInstr = OpCodes.Stobj;
					break;
				case CILType.ValueType:
					this.ldInstr = OpCodes.Ldobj;
					this.stInstr = OpCodes.Stobj;
					break;
				default:
					throw new InvalidOperationException();
			}
			this.type = type;
		}

		public override Type GetSystemType()
		{
			return type;
		}

		public override void CodeGen(ILGenerator code)
		{
			loc.CodeGen(code);
			if (ldInstr != OpCodes.Ldobj) code.Emit(ldInstr);
			else code.Emit(OpCodes.Ldobj, type);
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			if (ret == AssignRet.Post) CodeGen(code);
			loc.CodeGen(code);
			if (ret == AssignRet.Pre) code.Emit(OpCodes.Dup);
			if (op != AssignOp.Nop)
			{
				code.Emit(OpCodes.Dup);
				if (ldInstr == OpCodes.Ldobj) code.Emit(ldInstr);
				else code.Emit(OpCodes.Ldobj, type);
			}
			rhs(code);
			Assign.CodeGenAssignOp(code, op);
			if (stInstr == OpCodes.Stobj) code.Emit(stInstr);
			else code.Emit(OpCodes.Stobj, type);
			if (ret == AssignRet.Pre)
			{
				if (ldInstr == OpCodes.Ldobj) code.Emit(ldInstr);
				else code.Emit(OpCodes.Ldobj, type);
			}
		}

		public override void CodeGenRef(ILGenerator code)
		{
			loc.CodeGen(code);
		}
	}

	internal class LocalLoc : Location
	{
		private int pos;
		private Type type;

		public LocalLoc(int pos, Type type)
		{
			this.pos = pos;
			this.type = type;
		}

		public override Type GetSystemType()
		{
			return type;
		}

		public override void CodeGen(ILGenerator code)
		{
			if (pos == 0) code.Emit(OpCodes.Ldloc_0);
			else if (pos == 1) code.Emit(OpCodes.Ldloc_1);
			else if (pos == 2) code.Emit(OpCodes.Ldloc_2);
			else if (pos == 3) code.Emit(OpCodes.Ldloc_3);
			else if (pos < 256) code.Emit(OpCodes.Ldloc_S, pos);
			else code.Emit(OpCodes.Ldloc, pos);
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			if (op != AssignOp.Nop || ret == AssignRet.Post) CodeGen(code);
			if (op != AssignOp.Nop && ret == AssignRet.Post) code.Emit(OpCodes.Dup);
			rhs(code);
			Assign.CodeGenAssignOp(code, op);
			if (ret == AssignRet.Pre) code.Emit(OpCodes.Dup);
			if (pos == 0) code.Emit(OpCodes.Stloc_0);
			else if (pos == 1) code.Emit(OpCodes.Stloc_1);
			else if (pos == 2) code.Emit(OpCodes.Stloc_2);
			else if (pos == 3) code.Emit(OpCodes.Stloc_3);
			else if (pos < 256) code.Emit(OpCodes.Stloc_S, pos);
			else code.Emit(OpCodes.Stloc, pos);
		}

		public override void CodeGenRef(ILGenerator code)
		{
			if (pos < 256) code.Emit(OpCodes.Ldloca_S, pos);
			else code.Emit(OpCodes.Ldloca, pos);
		}
	}

	internal class FieldLoc : Location
	{
		private Location loc;
		private FieldInfo field;

		public FieldLoc(Location loc, FieldInfo field)
		{
			this.loc = loc;
			this.field = field;
		}

		public override Type GetSystemType()
		{
			return field.FieldType;
		}

		public override void CodeGen(ILGenerator code)
		{
			loc.CodeGen(code);
			code.Emit(OpCodes.Ldfld, field);
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void CodeGenRef(ILGenerator code)
		{
			loc.CodeGen(code);
			code.Emit(OpCodes.Ldflda, field);
		}
	}

	internal class ArrayLoc : Location
	{
		private Location loc;
		private OpCode instr;
		private Type type;
		private int index;

		public ArrayLoc(Location loc, Type type, int index)
		{
			this.loc = loc;
			switch (GetCILType(type))
			{
				case CILType.I:
					this.instr = OpCodes.Ldelem_I;
					break;
				case CILType.I1:
					this.instr = OpCodes.Ldelem_I1;
					break;
				case CILType.I2:
					this.instr = OpCodes.Ldelem_I2;
					break;
				case CILType.I4:
					this.instr = OpCodes.Ldelem_I4;
					break;
				case CILType.I8:
					this.instr = OpCodes.Ldelem_I8;
					break;
				case CILType.R4:
					this.instr = OpCodes.Ldelem_R4;
					break;
				case CILType.R8:
					this.instr = OpCodes.Ldelem_R8;
					break;
				case CILType.Ref:
					this.instr = OpCodes.Ldelem_Ref;
					break;
				case CILType.U1:
					this.instr = OpCodes.Ldelem_U1;
					break;
				case CILType.U2:
					this.instr = OpCodes.Ldelem_U2;
					break;
				case CILType.U4:
					this.instr = OpCodes.Ldelem_U4;
					break;
				case CILType.ValueType:
					this.instr = OpCodes.Ldelem;
					break;
				default:
					throw new InvalidOperationException();
			}
			this.index = index;
			this.type = type;
		}

		public override Type GetSystemType()
		{
			return type;
		}

		public override void CodeGen(ILGenerator code)
		{
			loc.CodeGen(code);
			EmitHelper.EmitInt(code, index);
			if (instr != OpCodes.Ldelem) code.Emit(instr);
			else code.Emit(OpCodes.Ldelem, type);
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void CodeGenRef(ILGenerator code)
		{
			loc.CodeGen(code);
			EmitHelper.EmitInt(code, index);
			code.Emit(OpCodes.Ldelema, type);
		}
	}

	internal class GlobalLoc : Location
	{
		private static SortedDictionary<FieldInfo, GlobalLoc> globalLocs = new SortedDictionary<FieldInfo, GlobalLoc>();

		private FieldInfo field;

		private GlobalLoc(FieldInfo field)
		{
			this.field = field;
		}

		public static GlobalLoc Get(FieldInfo field)
		{
			if (globalLocs.ContainsKey(field)) return globalLocs[field];
			else
			{
				GlobalLoc globalLoc = new GlobalLoc(field);
				globalLocs.Add(field, globalLoc);
				return globalLoc;
			}
		}

		public override Type GetSystemType()
		{
			return field.FieldType;
		}

		public override void CodeGen(ILGenerator code)
		{
			code.Emit(OpCodes.Ldsfld, field);
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void CodeGenRef(ILGenerator code)
		{
			code.Emit(OpCodes.Ldsflda, field);
		}
	}

	internal class FunctionLoc : Location
	{
		private ConstructorInfo delegateCtor;
		private ConstructorInfo closureCtor;
		private MethodInfo invokeMethod;
		private List<Location> closures;

		public FunctionLoc(ConstructorInfo delegateCtor, ConstructorInfo closureCtor, MethodInfo invokeMethod, List<Location> closures)
		{
			this.delegateCtor = delegateCtor;
			this.closureCtor = closureCtor;
			this.invokeMethod = invokeMethod;
			this.closures = closures;
		}

		public override Type GetSystemType()
		{
			return delegateCtor.DeclaringType;
		}

		public override void CodeGen(ILGenerator code)
		{
			if(closures != null)
				foreach (Location closure in closures) closure.CodeGen(code);
			if (closureCtor != null) code.Emit(OpCodes.Newobj, closureCtor);
			else if (closures != null && closures.Count == 1)
			{
				Type type = closures[0].GetSystemType();
				if (type.IsValueType) code.Emit(OpCodes.Box, type);
			}
			else if (closures == null || closures.Count == 0) code.Emit(OpCodes.Ldnull);
			code.Emit(OpCodes.Ldftn, invokeMethod);
			code.Emit(OpCodes.Newobj, delegateCtor);
		}

		public override void CodeGenAssign(ILGenerator code, AssignOp op, CodeGen rhs, AssignRet ret)
		{
			throw new InvalidOperationException("Cannot assign to a function.");
		}

		public override void CodeGenRef(ILGenerator code)
		{
			throw new InvalidOperationException("Cannot get address of a function.");
		}
		
		public override void CodeGenCall(ILGenerator code, CodeGen args)
		{
			if (closures != null)
				foreach (Location closure in closures) closure.CodeGen(code);
			if (closureCtor != null) code.Emit(OpCodes.Newobj, closureCtor);
			args(code);
			code.Emit(OpCodes.Call, invokeMethod);
		}
	}

	internal static class EmitHelper
	{
		public static void EmitInt(ILGenerator code, int value)
		{
			switch (value)
			{
				case -1: code.Emit(OpCodes.Ldc_I4_M1); break;
				case 0: code.Emit(OpCodes.Ldc_I4_0); break;
				case 1: code.Emit(OpCodes.Ldc_I4_1); break;
				case 2: code.Emit(OpCodes.Ldc_I4_2); break;
				case 3: code.Emit(OpCodes.Ldc_I4_3); break;
				case 4: code.Emit(OpCodes.Ldc_I4_4); break;
				case 5: code.Emit(OpCodes.Ldc_I4_5); break;
				case 6: code.Emit(OpCodes.Ldc_I4_6); break;
				case 7: code.Emit(OpCodes.Ldc_I4_7); break;
				case 8: code.Emit(OpCodes.Ldc_I4_8); break;
				default: code.Emit(OpCodes.Ldc_I4, value); break;
			}
		}

		public static void EmitLdarg(ILGenerator code, int pos)
		{
			if (pos == 0) code.Emit(OpCodes.Ldarg_0);
			else if (pos == 1) code.Emit(OpCodes.Ldarg_1);
			else if (pos == 2) code.Emit(OpCodes.Ldarg_2);
			else if (pos == 3) code.Emit(OpCodes.Ldarg_3);
			else if (pos < 256) code.Emit(OpCodes.Ldarg_S, pos);
			else code.Emit(OpCodes.Ldarg, pos);
		}

		public static void EmitStarg(ILGenerator code, int pos)
		{
			if (pos < 256) code.Emit(OpCodes.Starg_S, pos);
			else code.Emit(OpCodes.Starg, pos);
		}

		public static void EmitLdarga(ILGenerator code, int pos)
		{
			if (pos < 256) code.Emit(OpCodes.Ldarga_S, pos);
			else code.Emit(OpCodes.Ldarga, pos);
		}

		public static void EmitLdloc(ILGenerator code, int pos)
		{
			if (pos == 0) code.Emit(OpCodes.Ldloc_0);
			else if (pos == 1) code.Emit(OpCodes.Ldloc_1);
			else if (pos == 2) code.Emit(OpCodes.Ldloc_2);
			else if (pos == 3) code.Emit(OpCodes.Ldloc_3);
			else if (pos < 256) code.Emit(OpCodes.Ldloc_S, pos);
			else code.Emit(OpCodes.Ldloc, pos);
		}

		public static void EmitStloc(ILGenerator code, int pos)
		{
			if (pos == 0) code.Emit(OpCodes.Stloc_0);
			else if (pos == 1) code.Emit(OpCodes.Stloc_1);
			else if (pos == 2) code.Emit(OpCodes.Stloc_2);
			else if (pos == 3) code.Emit(OpCodes.Stloc_3);
			else if (pos < 256) code.Emit(OpCodes.Stloc_S, pos);
			else code.Emit(OpCodes.Stloc, pos);
		}

		public static void EmitLdloca(ILGenerator code, int pos)
		{
			if (pos < 256) code.Emit(OpCodes.Ldloca_S, pos);
			else code.Emit(OpCodes.Ldloca, pos);
		}

		public static void EmitStind(ILGenerator code, Type type)
		{
			if (type.IsValueType)
			{
				if (type == typeof(Int32)) code.Emit(OpCodes.Stind_I4);
				else if (type == typeof(Int64)) code.Emit(OpCodes.Stind_I8);
				else if (type == typeof(Double)) code.Emit(OpCodes.Stind_R8);
				else if (type == typeof(Single)) code.Emit(OpCodes.Stind_R4);
				else if (type == typeof(Int16)) code.Emit(OpCodes.Stind_I2);
				else if (type == typeof(IntPtr)) code.Emit(OpCodes.Stind_I);
				else if (type == typeof(SByte)) code.Emit(OpCodes.Stind_I1);
				else code.Emit(OpCodes.Stobj, type);
			}
			else code.Emit(OpCodes.Stind_Ref);
		}

		public static void EmitLdind(ILGenerator code, Type type)
		{
			if (type.IsValueType)
			{
				if (type == typeof(Int32)) code.Emit(OpCodes.Ldind_I4);
				else if (type == typeof(Int64)) code.Emit(OpCodes.Ldind_I8);
				else if (type == typeof(Double)) code.Emit(OpCodes.Ldind_R8);
				else if (type == typeof(Single)) code.Emit(OpCodes.Ldind_R4);
				else if (type == typeof(Int16)) code.Emit(OpCodes.Ldind_I2);
				else if (type == typeof(IntPtr)) code.Emit(OpCodes.Ldind_I);
				else if (type == typeof(Byte)) code.Emit(OpCodes.Ldind_U1);
				else if (type == typeof(UInt16)) code.Emit(OpCodes.Ldind_U2);
				else if (type == typeof(UInt32)) code.Emit(OpCodes.Ldind_U4);
				else if (type == typeof(SByte)) code.Emit(OpCodes.Ldind_I1);
				else code.Emit(OpCodes.Ldobj, type);
			}
			else code.Emit(OpCodes.Ldind_Ref);
		}
	}
}
