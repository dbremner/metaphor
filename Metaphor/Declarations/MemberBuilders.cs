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
using JetBrains.Annotations;
using Metaphor.Collections;

namespace Metaphor
{
	public abstract class MemberBuilder
	{
		private static int genSym = 0;

		public int sym;
		public MTypeBuilder declaringType;

	    protected MemberBuilder(int sym)
		{
			this.sym = sym == -1 ? genSym++ : sym;
			// declaringType is set in the method MTypeBuilder::Add when the member is added to a type
			this.declaringType = null;
		}
	}

	public partial class MFieldBuilder : MemberBuilder
	{
		public string name;
		public FieldAttributes attr;
		public bool isStatic;
		public MType type;

		[NonSerialized]
		public FieldBuilder fieldBuilder;

		public MFieldBuilder(int sym, string name, FieldAttributes attr, bool isStatic, MType type)
			: base(sym)
		{
			this.name = name;
			this.attr = attr;
			this.isStatic = isStatic;
			this.type = type;
		}

		public virtual MFieldInfo MakeField(MType[] typeArgs)
		{
			return Field.Create(typeArgs, this);
		}

		internal void GenerateClosures(TypeCheckState state)
		{
			type.GenerateClosures(state);
		}

		internal void GenerateMembers(ModuleGenState state)
		{
			if (isStatic)
				fieldBuilder = declaringType.typeBuilder.DefineField(name, type.GetSystemType(), attr | FieldAttributes.Static);
			else
				fieldBuilder = declaringType.typeBuilder.DefineField(name, type.GetSystemType(), attr);
		}

		internal void GenerateMembersMeta(CodeGenState state)
		{
			state.EmitInt(sym);
			state.EmitString(name);
			state.EmitBool(isStatic);
			type.CodeGenMeta(state);
			state.MakeCode(typeof(MFieldBuilder));
		}

		protected partial class Field : MFieldInfo
		{
			public MType[] typeArgs;
			public MFieldBuilder field;

			protected Field(MType[] typeArgs, MFieldBuilder field)
			{
				this.typeArgs = typeArgs;
				this.field = field;
			}

			public static MFieldInfo Create(MType[] typeArgs, MFieldBuilder field)
			{
				return new Field(typeArgs, field);
			}

			public override MType GetDeclaringType()
			{
				return field.declaringType.MakeType(typeArgs);
			}

			public override string GetName()
			{
				return field.name;
			}

			public override bool IsStatic()
			{
				return field.isStatic;
			}

			public override MType GetFieldType()
			{
				return MTypeBuilder.Subst(field.declaringType.@params, typeArgs, null, null, field.type);
			}

			internal override FieldInfo GetSystemField()
			{
				Type systemClassType = field.declaringType.typeBuilder;
				FieldInfo systemField = field.fieldBuilder;
				Type[] systemTypeArgs = TypeVarDecl.GetGenericTypes(field.declaringType.@params, typeArgs).ToArray();
				if (systemTypeArgs.Length != 0)
				{
					systemClassType = systemClassType.MakeGenericType(systemTypeArgs);
					systemField = TypeBuilder.GetField(systemClassType, systemField);
				}
				return systemField;
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				if (field.fieldBuilder != null)
				{
					state.EmitField(this);
					state.MakeType(typeof(SystemType.Field));
				}
				else
				{
					state.EmitInt(field.sym);
					MType.CodeGen<MType>(state, typeArgs);
					state.code.Emit(OpCodes.Call, typeof(MModuleBuilder).GetMethod("CreateField", BindingFlags.Public | BindingFlags.Static));
				}
			}
		}
	}

	public abstract class MethodBaseBuilder : MemberBuilder
	{
		public MethodAttributes attr;
		public ThisDecl @this;
	    [CanBeNull] public ParamDecl[] @params;
		public MType retType;
		public Code stmt;

		[NonSerialized]
		protected List<Function> functions;

	    protected MethodBaseBuilder(int sym, MethodAttributes attr, ThisDecl @this, [CanBeNull] ParamDecl[] @params, MType retType)
			: base(sym)
		{
			this.attr = attr;
			this.@this = @this;
			this.@params = @params;
			this.retType = retType;
		}

		internal abstract ILGenerator GetCode();

		internal virtual void GenerateClosures(TypeCheckState state)
		{
			state.PushMethod(@this, @params);
			retType.GenerateClosures(state);
			if (stmt != null)
			{
				stmt.CodeGenClosures(state);
				if (state.ContainsClosures()) throw new InvalidOperationException("A method should never have closure variables.");
				if (state.ContainsCspObjects()) throw new NotImplementedException("CSP objects in a method.");
				functions = state.GetFunctions();
			}
			state.PopMethod();
		}

		internal abstract void GenerateMembers(ModuleGenState state);

		internal abstract void GenerateCode(ModuleGenState state);

		internal abstract void GenerateMembersMeta(CodeGenState state);

		internal virtual void GenerateCodeMeta(CodeGenState state)
		{
			stmt.CodeGenMeta(state);
			state.code.Emit(OpCodes.Stfld, typeof(MethodBaseBuilder).GetField("stmt"));
		}

	}

	public partial class MStaticConstructorBuilder : MConstructorBuilder
	{
		public MStaticConstructorBuilder(int sym)
			: base(sym, MethodAttributes.Static, null, new ParamDecl[] { })
		{
		}

		internal override ILGenerator GetCode()
		{
			return ctorBuilder.GetILGenerator();
		}

		internal override void GenerateMembers(ModuleGenState state)
		{
			TypeBuilder declaringScope = declaringType.typeBuilder;
			ctorBuilder = declaringScope.DefineTypeInitializer();
		}

		internal override void GenerateCode(ModuleGenState state)
		{
			CodeGenState cgState = state.DefineMethod(this);
			state.CodeGen(cgState, stmt, functions);
			functions = null;
		}

		internal override void GenerateMembersMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public partial class MConstructorBuilder : MethodBaseBuilder
	{
		[NonSerialized]
		public ConstructorBuilder ctorBuilder;

		public MConstructorBuilder(int sym, MethodAttributes attr, ThisDecl @this, [CanBeNull] ParamDecl[] @params)
			: base(sym, attr, @this, @params, PrimType.Void)
		{
		}

		public virtual MConstructorInfo MakeConstructor(MType[] typeArgs)
		{
			return Constructor.Create(typeArgs, this);
		}

		internal override ILGenerator GetCode()
		{
			return ctorBuilder.GetILGenerator();
		}

		private List<Collections.Tuple<int, FieldBuilder>> stagedTypeVars;

		internal override void GenerateMembers(ModuleGenState state)
		{
			int paramPos = 1;
			TypeBuilder declaringScope = declaringType.typeBuilder;
			List<Collections.Tuple<Type, string>> paramTypes = new List<Collections.Tuple<Type, string>>();
			stagedTypeVars = new List<Collections.Tuple<int, FieldBuilder>>();
			@this.SetLocation();
			foreach (ParamDecl param in @params)
			{
				param.SetLocation(paramPos++);
				paramTypes.Add(new Collections.Tuple<Type, string>(param.GetSystemType(), param.name));
			}
			foreach (TypeVarDecl typeParam in declaringType.@params)
			{
				if (typeParam.levelKind > 0)
				{
					paramTypes.Add(new Collections.Tuple<Type, string>(typeof(MType), typeParam.name));
					FieldBuilder field = declaringScope.DefineField(typeParam.name, typeof(MType), FieldAttributes.Private);
					stagedTypeVars.Add(new Collections.Tuple<int, FieldBuilder>(paramPos++, field));
					((ClassReflTypeVarDecl) typeParam).Generate(field);
				}
			}
			ctorBuilder = declaringScope.DefineConstructor(attr | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, Collections.Tuple<Type, string>.MapFst(paramTypes.ToArray()));
			int paramIndex = 1;
			foreach (Collections.Tuple<Type, string> paramType in paramTypes)
				ctorBuilder.DefineParameter(paramIndex++, ParameterAttributes.None, paramType.snd);
		}

		internal override void GenerateCode(ModuleGenState state)
		{
			CodeGenState cgState = state.DefineMethod(this);
			foreach (Collections.Tuple<int, FieldBuilder> stagedTypeVar in stagedTypeVars)
			{
				cgState.EmitLdarg(0);
				cgState.EmitLdarg(stagedTypeVar.fst);
				cgState.code.Emit(OpCodes.Stfld, stagedTypeVar.snd);
			}
			state.CodeGen(cgState, stmt, functions);
			functions = null;
		}

		internal override void GenerateMembersMeta(CodeGenState state)
		{
			state.EmitInt(sym);
			@this.GenerateCode(state);
			VarDecl.CodeGenMeta<ParamDecl>(state, @params);
			state.MakeCode(typeof(MConstructorBuilder));
		}

		protected partial class Constructor : MConstructorInfo
		{
			public MType[] typeArgs;
			public MConstructorBuilder ctor;

			protected Constructor(MType[] typeArgs, MConstructorBuilder ctor)
			{
				this.typeArgs = typeArgs;
				this.ctor = ctor;
			}

			public static MConstructorInfo Create(MType[] typeArgs, MConstructorBuilder ctor)
			{
				return new Constructor(typeArgs, ctor);
			}

			public override MType GetDeclaringType()
			{
				return ctor.declaringType.MakeType(typeArgs);
			}

			public override ParamType[] GetParamTypes()
			{
				ParamType[] paramTypes = ParamDecl.GetParamTypes(ctor.@params);
				for (int i = 0; i < paramTypes.Length; i++)
					paramTypes[i] = SubstParamType(paramTypes[i]);
				return paramTypes;
			}

			internal override ConstructorInfo GetSystemConstructor()
			{
				Type systemClassType = ctor.declaringType.typeBuilder;
				ConstructorInfo systemCtor = ctor.ctorBuilder;
				Type[] systemTypeArgs = TypeVarDecl.GetGenericTypes(ctor.declaringType.@params, typeArgs).ToArray();
				if (systemTypeArgs.Length != 0)
				{
					systemClassType = systemClassType.MakeGenericType(systemTypeArgs);
					systemCtor = TypeBuilder.GetConstructor(systemClassType, systemCtor);
				}
				return systemCtor;
			}

			internal override void CodeGen(CodeGenState state)
			{
				foreach (MType mTypeArg in TypeVarDecl.GetReflectionTypes(ctor.declaringType.@params, typeArgs))
				{
					mTypeArg.CodeGen(state);
				}
				base.CodeGen(state);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				if (ctor.ctorBuilder != null)
				{
					state.EmitConstructor(this);
					state.MakeType(typeof(SystemType.Constructor));
				}
				else
				{
					state.EmitInt(ctor.sym);
					MType.CodeGen<MType>(state, typeArgs);
					state.code.Emit(OpCodes.Call, typeof(MModuleBuilder).GetMethod("CreateConstructor", BindingFlags.Public | BindingFlags.Static));
				}
			}

			protected ParamType SubstParamType(ParamType type)
			{
				return ParamType.Create(MTypeBuilder.Subst(ctor.declaringType.@params, typeArgs, null, null, type.type), type.@ref);
			}
		}
	}

	public partial class MMethodBuilder : MethodBaseBuilder
	{
		public string name;
		public TypeVarDecl[] typeParams;

		[NonSerialized]
		public MethodBuilder methodBuilder;

		public MMethodBuilder(int sym, string name, MethodAttributes attr, TypeVarDecl[] typeParams, ThisDecl @this, ParamDecl[] @params, MType retType)
			: base(sym, attr, @this, @params, retType)
		{
			this.name = name;
			this.typeParams = typeParams;
			this.retType = retType;
		}

		public virtual MMethodInfo MakeMethod(MType[] typeArgs, MType[] methodArgs)
		{
			return Method.Create(typeArgs, this, methodArgs);
		}

		internal override ILGenerator GetCode()
		{
			return methodBuilder.GetILGenerator();
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			state.PushTypeVars(typeParams);
			base.GenerateClosures(state);
			state.PopTypeVars();
		}

		internal override void GenerateMembers(ModuleGenState state)
		{
			bool isStatic = @this == null;
			int paramPos = isStatic ? 0 : 1;
			TypeBuilder declaringScope = declaringType.typeBuilder;
			if (@this != null) methodBuilder = declaringScope.DefineMethod(name, attr, CallingConventions.HasThis);
			else
			{
				methodBuilder = declaringScope.DefineMethod(name, attr | MethodAttributes.Static, CallingConventions.Standard);
				//if (name == "Main") state.assembly.SetEntryPoint(methodBuilder);
			}

			//TypeVarDecl.PushVars(typeParams);
			List<string> typeParamNames = new List<string>();
			List<Collections.Tuple<Type, string>> paramTypes = new List<Collections.Tuple<Type, string>>();
			foreach (TypeVarDecl typeParam in typeParams)
			{
				if (typeParam.levelKind == 0) typeParamNames.Add(typeParam.name);
			}
			if (typeParamNames.Count > 0)
			{
				GenericTypeParameterBuilder[] types = methodBuilder.DefineGenericParameters(typeParamNames.ToArray());
				int typeParamPos = 0;
				foreach (TypeVarDecl typeParam in typeParams)
				{
					if (typeParam.levelKind == 0) ((GenericTypeVarDecl)typeParam).Generate(types[typeParamPos++]);
				}
			}

			methodBuilder.SetReturnType(retType.GetSystemType());

			if (@this != null) @this.SetLocation();
			foreach (ParamDecl param in @params)
			{
				param.SetLocation(paramPos++);
				paramTypes.Add(new Collections.Tuple<Type, string>(param.GetSystemType(), param.name));
			}
			foreach (TypeVarDecl typeParam in typeParams)
			{
				if (typeParam.levelKind > 0)
				{
					paramTypes.Add(new Collections.Tuple<Type, string>(typeof(MType), typeParam.name));
					((MethodReflTypeVarDecl) typeParam).Generate(paramPos++);
				}
			}
			//TypeVarDecl.PopVars(typeParams.Length);

			methodBuilder.SetParameters(Collections.Tuple<Type, string>.MapFst(paramTypes.ToArray()));
			int paramIndex = 1;
			foreach (Collections.Tuple<Type, string> paramType in paramTypes)
			{
				ParameterBuilder p = methodBuilder.DefineParameter(paramIndex, ParameterAttributes.None, paramType.snd);
				//if (paramIndex <= @params.Length)
				//{
				//    CodeType codeType = @params[paramIndex - 1].GetMType() as CodeType;
				//    if (codeType != null)
				//        p.SetCustomAttribute(new CustomAttributeBuilder(CodeTypeAttribute.ctor, new object[] { codeType.codeType.GetSystemType() }));
				//}
				paramIndex++;
			}
			if (name == "Main") state.SetEntryPoint(methodBuilder);
		}

		internal override void GenerateCode(ModuleGenState state)
		{
			if (stmt != null)
			{
				CodeGenState cgState = state.DefineMethod(this);
				state.CodeGen(cgState, stmt, functions);
				functions = null;
			}
		}

		internal override void GenerateMembersMeta(CodeGenState state)
		{
			state.EmitInt(sym);
			state.EmitString(name);
			TypeVarDecl.CodeGenMeta(state, typeParams);
			@this.GenerateCode(state);
			VarDecl.CodeGenMeta<ParamDecl>(state, @params);
			retType.CodeGenMeta(state);
			state.MakeCode(typeof(MMethodBuilder));
		}

		protected partial class Method : MMethodInfo
		{
			public MType[] classTypeArgs;
			public MMethodBuilder method;
			public MType[] typeArgs;

			protected Method(MType[] classTypeArgs, MMethodBuilder method, MType[] typeArgs)
			{
				this.classTypeArgs = classTypeArgs;
				this.method = method;
				this.typeArgs = typeArgs;
			}

			public static MMethodInfo Create(MType[] classTypeArgs, MMethodBuilder method, MType[] typeArgs)
			{
				if (method.declaringType.@params.Length != classTypeArgs.Length) throw new ArgumentException("Class type parameter arity mismatch.");
				if (method.typeParams.Length != typeArgs.Length) throw new ArgumentException("Method type parameter arity mismatch.");

				int levelKind = method.declaringType.MakeType(classTypeArgs).GetLevelKind();
				for (int i = 0; i < typeArgs.Length; i++)
				{
					int tmp = typeArgs[i].GetLevelKind() - method.typeParams[i].levelKind;
					if (tmp > levelKind) throw new Exception();
				}
				return new Method(classTypeArgs, method, typeArgs);
			}

			public override MType GetDeclaringType()
			{
				return method.declaringType.MakeType(classTypeArgs);
			}

			public override string GetName()
			{
				return method.name;
			}

			public override bool IsStatic()
			{
				return method.@this == null;
			}

			public override MType GetReturnType()
			{
				return MTypeBuilder.Subst(method.declaringType.@params, classTypeArgs, method.typeParams, typeArgs, method.retType);
			}

			public override ParamType[] GetParamTypes()
			{
				ParamType[] paramTypes = ParamDecl.GetParamTypes(method.@params);
				for (int i = 0; i < paramTypes.Length; i++)
					paramTypes[i] = SubstParamType(paramTypes[i]);
				return paramTypes;
			}

			internal override MethodInfo GetSystemMethod()
			{
				Type systemClassType = method.declaringType.typeBuilder;
				MethodInfo systemMethod = method.methodBuilder;
				Type[] systemClassTypeArgs = TypeVarDecl.GetGenericTypes(method.declaringType.@params, classTypeArgs).ToArray();
				if (systemClassTypeArgs.Length != 0)
				{
					systemClassType = systemClassType.MakeGenericType(systemClassTypeArgs);
					systemMethod = TypeBuilder.GetMethod(systemClassType, systemMethod);
				}
				Type[] systemTypeArgs = TypeVarDecl.GetGenericTypes(method.typeParams, typeArgs).ToArray();
				if(systemTypeArgs.Length != 0)
				{
					systemMethod = systemMethod.MakeGenericMethod(systemTypeArgs);
				}
				return systemMethod;
			}

			internal override void CodeGen(CodeGenState state)
			{
				foreach (MType typeArg in TypeVarDecl.GetReflectionTypes(method.typeParams, typeArgs))
						typeArg.CodeGen(state);
				base.CodeGen(state);
			}

			private static readonly MethodInfo getMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });

			internal override void CodeGenMeta(CodeGenState state)
			{
				if (method.methodBuilder != null)
				{
					if (classTypeArgs.Length != 0)
					{
						state.EmitType(method.declaringType.typeBuilder);
						MType.CodeGen(state, classTypeArgs);
						state.MakeCode(typeof(GenericSystemType), "CreateGeneric");
				
						//state.code.Emit(OpCodes.Ldtoken, method.methodBuilder);
						//state.EmitInt(method.methodBuilder.GetToken().Token);
						state.EmitString(method.methodBuilder.Name);
						
						if (typeArgs.Length != 0)
						{
							MType.CodeGenMeta(state, typeArgs);
							state.MakeCode(typeof(GenericSystemType.GenericMethod), "CreateFromName");
						}
						else
						{
							state.MakeCode(typeof(GenericSystemType.Method), "CreateFromName");
						}
					}
					else
					{
						state.code.Emit(OpCodes.Ldtoken, method.methodBuilder);
						state.code.Emit(OpCodes.Call, getMethodFromHandle);
						state.code.Emit(OpCodes.Castclass, typeof(MethodInfo));
						if (typeArgs.Length != 0)
						{
							MType.CodeGenMeta(state, typeArgs);
							state.MakeType(typeof(SystemType.GenericMethod));
						}
						else
						{
							state.MakeType(typeof(SystemType.Method));
						}
					}
				}
				else
				{
					state.EmitInt(method.sym);
					MType.CodeGen<MType>(state, classTypeArgs);
					MType.CodeGen<MType>(state, typeArgs);
					state.code.Emit(OpCodes.Call, typeof(MModuleBuilder).GetMethod("CreateMethod", BindingFlags.Public | BindingFlags.Static));
				}
			}

			protected ParamType SubstParamType(ParamType type)
			{
				return ParamType.Create(MTypeBuilder.Subst(method.declaringType.@params, classTypeArgs, method.typeParams, typeArgs, type.type), type.@ref);
			}
		}
	}
}