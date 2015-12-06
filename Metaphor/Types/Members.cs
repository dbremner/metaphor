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
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using JetBrains.Annotations;

namespace Metaphor
{
	[Serializable]
	public abstract class MMemberInfo
	{
	    protected MMemberInfo()
		{
		}

		public abstract string GetName();

		public abstract bool IsStatic();

		public abstract MType GetDeclaringType();

		public override string ToString()
		{
			return $"{GetDeclaringType()}::{GetName()}";
		}
	}

	[Serializable]
	public abstract partial class MFieldInfo : MMemberInfo
	{
	    protected MFieldInfo(): base()
		{
		}

		public abstract MType GetFieldType();

		internal virtual FieldInfo GetSystemField()
		{
			throw new Exception("System field not available.");
		}

		internal virtual void GenerateClosures(TypeCheckState state)
		{
			this.GetFieldType().GenerateClosures(state);
		}

		internal virtual void CodeGen(CodeGenState state)
		{
			if(!IsStatic()) state.code.Emit(OpCodes.Ldfld, GetSystemField());
			else state.code.Emit(OpCodes.Ldsfld, GetSystemField());
		}

		internal virtual void CodeGenAssign(CodeGenState state)
		{
			/// TODO emit is probably a better name for this method
			if(!IsStatic()) state.code.Emit(OpCodes.Stfld, GetSystemField());
			else state.code.Emit(OpCodes.Stsfld, GetSystemField());
		}

		internal virtual void CodeGenRef(CodeGenState state)
		{
			/// TODO emit is probably a better name for this method
			if(!IsStatic()) state.code.Emit(OpCodes.Ldflda, GetSystemField());
			else state.code.Emit(OpCodes.Ldsflda, GetSystemField());
		}

		internal virtual void CodeGenMeta(CodeGenState state)
		{
			state.EmitField(this);
			state.MakeType(typeof(SystemType.Field));
		}

		public override string  ToString()
		{
			return $"{GetFieldType()} {GetDeclaringType()}::{GetName()}";
 		}
	}

	[Serializable]
	public abstract class MMethodBase : MMemberInfo
	{
	    protected MMethodBase(): base()
		{
		}

		public abstract MType GetReturnType();

		public abstract ParamType[] GetParamTypes();

		protected static int ChooseBestOverload(MMethodBase[] methods, ParamType[] paramTypes, MType retType)
		{
			if (methods == null) return -1;

			int n = methods.Length;
			int goodness = -1;
			int selected = -1;
			for (int i = 0; i < n; i++)
			{
				int tmp = ParamType.GoodnessOfFit(paramTypes, methods[i].GetParamTypes());
				if (retType != null && tmp != -1 && methods[i].GetReturnType().IsEqualTo(retType)) tmp++;
				if (tmp > goodness)
				{
					goodness = tmp;
					selected = i;
				}
				else if (tmp == goodness)
				{
					// method choice is ambiguous but we won't complain about it
					//selected = -1;
				}
			}
			return selected;
		}
	}

	[Serializable]
	public abstract partial class MMethodInfo : MMethodBase
	{
	    protected MMethodInfo(): base()
		{
		}

		public virtual MType[] GetTypeArguments()
		{
			return MType.Empty;
		}

	    [CanBeNull]
	    public static MMethodInfo ChooseBestOverload(MMethodInfo[] methods, ParamType[] paramTypes, MType retType)
		{
			int i = MMethodBase.ChooseBestOverload(methods, paramTypes, retType);
			return i != -1 ? methods[i] : null;
		}

		internal virtual MethodInfo GetSystemMethod()
		{
			throw new Exception("System method not available.");
		}

		internal virtual void GenerateClosures(TypeCheckState state)
		{
			foreach (MType type in this.GetTypeArguments())
				type.GenerateClosures(state);
			foreach (ParamType paramType in this.GetParamTypes())
				paramType.type.GenerateClosures(state);
			this.GetReturnType().GenerateClosures(state);
		}

		//internal virtual Type[] CodeGen1(CodeGenState state)
		//{
		//    return Type.EmptyTypes;
		//}

		internal virtual void CodeGen(CodeGenState state)
		{
			MethodInfo methodInfo = GetSystemMethod();
			if (methodInfo.IsVirtual && !methodInfo.DeclaringType.IsValueType) state.code.Emit(OpCodes.Callvirt, methodInfo);
			else state.code.Emit(OpCodes.Call, methodInfo);
		}

		internal virtual void CodeGenRef(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldftn, GetSystemMethod());
		}

		internal virtual void CodeGenMeta(CodeGenState state)
		{
			state.EmitMethod(this);
			state.MakeType(typeof(SystemType.Method));
		}

		public override string  ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} {1}::{2}", GetReturnType(), GetDeclaringType(), GetName());
			MType[] typeArgs = GetTypeArguments();
			if(typeArgs.Length > 0)
			{
				sb.AppendFormat("<{0}", typeArgs[0]);
				for(int i = 1; i < typeArgs.Length; i++) sb.AppendFormat(",{1}", typeArgs[i]);
				sb.Append(">");
			}
			sb.Append("(");
			ParamType[] paramTypes = GetParamTypes();
			if(paramTypes.Length > 0)
			{
				sb.Append(paramTypes[0]);
				for(int i = 1; i < paramTypes.Length; i++) sb.AppendFormat(",{0}", paramTypes[i]);
			}
			sb.Append(")");
 			 return sb.ToString();
		}
	}

	[Serializable]
	public partial class MetaphorMethod : MMethodInfo
	{
		public MType declaringType;
		public MethodInfo methodInfo;
		public MType retType;
		public ParamType[] paramTypes;

		protected MetaphorMethod(MType declaringType, MethodInfo methodInfo, MType retType, ParamType[] paramTypes)
		{
			Contract.Requires(methodInfo != null);
			Contract.Requires(retType != null);
			Contract.Requires(paramTypes != null);
			this.declaringType = declaringType;
			this.methodInfo = methodInfo;
			this.retType = retType;
			this.paramTypes = paramTypes;
		}

		public static MetaphorMethod Create(MType declaringType, MethodInfo methodInfo, MType retType, ParamType[] paramTypes)
		{
			return new MetaphorMethod(declaringType, methodInfo, retType, paramTypes);
		}

  		public override MType GetDeclaringType()
		{
			return declaringType;
		}

		public override string GetName()
		{
			return methodInfo.Name;
		}

		public override bool IsStatic()
		{
			return methodInfo.IsStatic;
		}

		public override MType GetReturnType()
		{
			return retType;
		}

		public override ParamType[] GetParamTypes()
		{
			return paramTypes;
		}

		internal override MethodInfo GetSystemMethod()
		{
			return methodInfo;
		}

	}

	[Serializable]
	public abstract partial class MConstructorInfo : MMethodBase
	{
	    protected MConstructorInfo(): base()
		{
		}

		public override string GetName()
		{
			return ".ctor";
		}

		public override bool IsStatic()
		{
			return false;
		}

		public override MType GetReturnType()
		{
			return PrimType.Void;
		}

		public static MConstructorInfo ChooseBestOverload(MConstructorInfo[] methods, ParamType[] paramTypes)
		{
			int i = MMethodBase.ChooseBestOverload(methods, paramTypes, null);
			return i != -1 ? methods[i] : null;
		}

		internal abstract ConstructorInfo GetSystemConstructor();

		internal virtual void GenerateClosures(TypeCheckState state)
		{
			foreach (ParamType paramType in this.GetParamTypes())
				paramType.type.GenerateClosures(state);
		}

		internal virtual void CodeGen(CodeGenState state)
		{
			state.code.Emit(OpCodes.Newobj, GetSystemConstructor());
		}

		internal virtual void CodeGenBase(CodeGenState state)
		{
			state.code.Emit(OpCodes.Call, GetSystemConstructor());
		}

		internal virtual void CodeGenMeta(CodeGenState state)
		{
			state.EmitConstructor(this);
			state.MakeType(typeof(SystemType.Constructor));
		}

		public override string  ToString()
		{
			System.Text.StringBuilder sb = new StringBuilder();
			MType declaringType = GetDeclaringType();
			sb.AppendFormat("{1}::{2}", declaringType, declaringType.GetName());
			sb.Append("(");
			ParamType[] paramTypes = GetParamTypes();
			if(paramTypes.Length > 0)
			{
				sb.Append(paramTypes[0]);
				for(int i = 0; i < paramTypes.Length; i++) sb.AppendFormat(",{0}", paramTypes[i]);
			}
			sb.Append(")");
 			 return sb.ToString();
		}
	}

	[Serializable]
	public sealed class ValueTypeConstructor : MConstructorInfo
	{
		public MType valueType;

		public ValueTypeConstructor(MType valueType)
		{
			this.valueType = valueType;
		}

		public override MType GetDeclaringType()
		{
			return valueType;
		}

		public override ParamType[] GetParamTypes()
		{
			return ParamType.Empty;
		}

		internal override ConstructorInfo GetSystemConstructor()
		{
			throw new InvalidOperationException();
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			valueType.GenerateClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			Type sysType = valueType.GetSystemType();
			int pos = state.DeclareLocal(sysType);
			state.EmitLdloca(pos);
			state.code.Emit(OpCodes.Initobj, sysType);
			state.EmitLdloc(pos);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			valueType.CodeGenMeta(state);
			state.MakeCode(typeof(ValueTypeConstructor));
		}
	}

	[Serializable]
	public sealed class TypeVarConstructor : MConstructorInfo
	{
		public MType typeVar;

		public TypeVarConstructor(MType typeVar)
		{
			this.typeVar = typeVar;
		}

		public override MType GetDeclaringType()
		{
			return typeVar;
		}

		public override ParamType[] GetParamTypes()
		{
			return ParamType.Empty;
		}

		internal override ConstructorInfo GetSystemConstructor()
		{
			throw new InvalidOperationException();
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			typeVar.GenerateClosures(state);
		}

		private static readonly MethodInfo createInstance;
		private static readonly MethodInfo createInstanceGeneric;
		private static readonly MethodInfo getSystemType = typeof(MType).GetMethod("GetSystemType");

		static TypeVarConstructor()
		{
			MemberInfo[] members = typeof(Activator).GetMember("CreateInstance", MemberTypes.Method, BindingFlags.Public | BindingFlags.Static);
			foreach (MemberInfo member in members)
			{
				MethodInfo method = (MethodInfo)member;
				ParameterInfo[] paramTypes = method.GetParameters();
				if (paramTypes.Length == 0 && method.IsGenericMethodDefinition)
					{
						if (createInstanceGeneric == null) createInstanceGeneric = method;
						else throw new Exception();
					}
					else if(paramTypes.Length == 1 && paramTypes[0].ParameterType == typeof(Type))
					{
						if (createInstance == null) createInstance = method;
						else throw new Exception();
					}
			}
			if (createInstance == null || createInstanceGeneric == null) throw new Exception();
		}

		internal override void CodeGen(CodeGenState state)
		{
			TypeVar var = typeVar as TypeVar;
			if (var != null)
			{
				GenericTypeVarDecl genericDecl = var.decl.GetGenericTypeVarDecl();
				if (genericDecl != null)
				{
					Type sysType = genericDecl.GetSystemType();
					MethodInfo ctor = createInstanceGeneric.MakeGenericMethod(sysType);
					state.code.Emit(OpCodes.Call, ctor);
				}
				else
				{
					var.CodeGen(state);
					state.code.Emit(OpCodes.Callvirt, getSystemType);
					state.code.Emit(OpCodes.Call, createInstance);
				}
			}
			else
			{
				MConstructorInfo ctor = typeVar.GetConstructor(ParamType.Empty);
				state.code.Emit(OpCodes.Newobj, ctor.GetSystemConstructor());
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			typeVar.CodeGenMeta(state);
			state.MakeCode(typeof(TypeVarConstructor));
		}
	}
}
