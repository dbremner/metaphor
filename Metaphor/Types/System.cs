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
using System.Runtime.Serialization;

namespace Metaphor
{
	[Serializable]
	public partial class SystemType : MType, ISerializable
	{
		public Type systemType;

		protected SystemType(Type systemType)
		{
			this.systemType = systemType;
		}

		public static SystemType Create(Type systemType)
		{
			if (systemType == typeof(void)) return PrimType.Void;
			else if (systemType == typeof(object)) return PrimType.Object;
			else if (systemType == typeof(bool)) return PrimType.Boolean;
			else if (systemType == typeof(int)) return PrimType.Int;
			else if (systemType == typeof(long)) return PrimType.Long;
			else if (systemType == typeof(float)) return PrimType.Float;
			else if (systemType == typeof(double)) return PrimType.Double;
			else if (systemType == typeof(string)) return PrimType.String;
			else if (systemType == typeof(char)) return PrimType.Char;
			else if (systemType == typeof(Array)) return PrimType.Array;
			else if (systemType == typeof(MulticastDelegate)) return PrimType.Delegate;
			else if (systemType == typeof(IntPtr)) return PrimType.Native;
			else if (systemType == typeof(ValueType)) return PrimType.Value;
			else if (systemType == typeof(byte)) return PrimType.Byte;
			else if (systemType == typeof(short)) return PrimType.Short;
			else if (systemType == typeof(decimal)) return PrimType.Decimal;
			else return new SystemType(systemType);
		}

		public static MType[] Creates(Type[] systemTypes)
		{
			int n = systemTypes.Length;
			MType[] mTypes = new MType[n];
			for (int i = 0; i < n; i++) mTypes[i] = Create(systemTypes[i]);
			return mTypes;
		}

		public override string GetName()
		{
			return systemType.Name;
		}

		public override MType[] GetTypeArguments()
		{
			if (systemType.IsArray) return new MType[] { Create(systemType.GetElementType()) };
			else if (systemType.IsGenericType) return Creates(systemType.GetGenericArguments());
			else return base.GetTypeArguments();
		}

		public override MType ReplaceTypeArguments(MType[] newTypeArgs)
		{
			if (!systemType.IsGenericType) return this;
			else return GenericSystemType.Create(systemType.GetGenericTypeDefinition(), newTypeArgs);
		}

		public override bool IsArray()
		{
			return systemType.IsArray;
		}

		public override MType GetElementType()
		{
			if (systemType.IsArray) return Create(systemType.GetElementType());
			else return base.GetElementType();
		}

		public override int GetArrayRank()
		{
			if (systemType.IsArray) return systemType.GetArrayRank();
			else return base.GetArrayRank();
		}

		public override bool IsGeneric()
		{
			return systemType.IsGenericType;
		}

		public override bool IsValueType()
		{
			return systemType.IsValueType;
		}

		public override bool IsInterface()
		{
			return systemType.IsInterface;
		}

		protected override bool IsEqualToInternal(MType type)
		{
			SystemType that = (SystemType)type;
			return systemType == that.systemType;
		}

		public override MType GetSuperType()
		{
			Type superType = systemType.BaseType;
			//return superType != null ? TypeFactory.Current.GetClrType(superType) : null;
			return superType != null ? SystemType.Create(superType) : null;
		}

		public override MType[] GetInterfaces()
		{
			Type[] interfaces = systemType.GetInterfaces();
			MType[] mInterfaces = new MType[interfaces.Length];
			for (int i = 0; i < interfaces.Length; i++)
				mInterfaces[i] = SystemType.Create(interfaces[i]);
			return mInterfaces;
		}

		public override MType GetNestedType(string name, MType[] args)
		{
			Type nestedType = systemType.GetNestedType(name, BindingFlags.Public);
			if (nestedType != null)
				if (!nestedType.IsGenericTypeDefinition && args.Length == 0)
					return SystemType.Create(nestedType);
				else if (nestedType.IsGenericTypeDefinition && nestedType.GetGenericArguments().Length == args.Length)
					return GenericSystemType.Create(nestedType, args);
			return null;
		}

		protected internal override void GetFields(List<MFieldInfo> fields)
		{
			if (systemType.IsArray)
				fields.Add(ArrayType.Length.Create());
			foreach (FieldInfo fieldInfo in systemType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
				fields.Add(Field.Create(fieldInfo));
		}

		protected internal override void GetConstructors(List<MConstructorInfo> ctors)
		{
			foreach (ConstructorInfo ctorInfo in systemType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				ctors.Add(Constructor.Create(ctorInfo));
			base.GetConstructors(ctors);
		}

		private static MethodInfo getType = typeof(object).GetMethod("GetType");

		protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
			foreach (MethodInfo methodInfo in systemType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
				//if (methodInfo != getType)
				//{
				if (!methodInfo.IsGenericMethodDefinition && typeParams.Length == 0)
					methods.Add(Method.Create(methodInfo));
				else if (methodInfo.GetGenericArguments().Length == typeParams.Length)
					methods.Add(GenericMethod.Create(methodInfo, typeParams));
			//}

			//if (typeParams.Length == 0)
			//{
			//    foreach (PropertyInfo propertyInfo in systemType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
			//    {
			//        MethodInfo methodInfo;
			//        methodInfo = propertyInfo.GetGetMethod();
			//        if (methodInfo != null) methods.Add(SystemMethod.Create(methodInfo));
			//        methodInfo = propertyInfo.GetSetMethod();
			//        if (methodInfo != null) methods.Add(SystemMethod.Create(methodInfo));
			//    }
			//}

			if (IsInterface())
			{
				foreach (MType Iface in GetInterfaces())
					Iface.GetMethods(methods, typeParams);
			}
		}

		public override int GetHashCode()
		{
			return systemType.GetHashCode();
		}

		public override Type GetSystemType()
		{
			return systemType;
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitType(systemType);
			state.MakeType(typeof(SystemType));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("systemType", systemType);
			info.SetType(typeof(ObjRef));
		}

		public static MType Subst(MType[] classTypeArgs, MType[] methodTypeArgs, Type systemType)
		{
			Contract.Requires(systemType != null);
			if (systemType.IsGenericTypeDefinition) throw new ArgumentException("IsGenericTypeDefinition", "type");

			if (systemType.IsGenericParameter)
			{
				if (systemType.DeclaringMethod != null)
					return methodTypeArgs[systemType.GenericParameterPosition];
				else
					return classTypeArgs[systemType.GenericParameterPosition];
			}
			else if (systemType.IsArray && systemType.ContainsGenericParameters)
			{
				return ArrayType.Create(
					Subst(classTypeArgs, methodTypeArgs, systemType.GetElementType()),
					systemType.GetArrayRank());
			}
			else if (!systemType.ContainsGenericParameters)
			{
				return SystemType.Create(systemType);
			}
			else
			{
				Type[] systemTypeArgs = systemType.GetGenericArguments();
				MType[] typeArgs = new MType[systemTypeArgs.Length];
				for (int i = 0; i < systemTypeArgs.Length; i++)
					typeArgs[i] = Subst(classTypeArgs, methodTypeArgs, systemTypeArgs[i]);
				return GenericSystemType.Create(systemType.GetGenericTypeDefinition(), typeArgs);
			}
		}

		internal static MType GetMTypeFromParameter(MType[] classTypeArgs, MType[] methodTypeArgs, ParameterInfo param)
		{
		/*	object[] attrs = param.GetCustomAttributes(typeof(CodeTypeAttribute), false);
			if (attrs != null && attrs.Length != 0)
			{
				Type codeType = ((CodeTypeAttribute)attrs[0]).GetCodeType();
				MType mType = Subst(classTypeArgs, methodTypeArgs, codeType);
				return CodeType.Create(mType, param.ParameterType == typeof(CodeRef));
			}
			else */return Subst(classTypeArgs, methodTypeArgs, param.ParameterType);
		}

		internal static MType GetMTypeFromField(MType[] classTypeArgs, FieldInfo field)
		{
			/*object[] attrs = field.GetCustomAttributes(typeof(CodeTypeAttribute), false);
			if (attrs != null && attrs.Length != 0)
			{
				Type codeType = ((CodeTypeAttribute)attrs[0]).GetCodeType();
				MType mType = Subst(classTypeArgs, null, codeType);
				return CodeType.Create(mType, field.FieldType == typeof(CodeRef));
			}
			else */return Subst(classTypeArgs, null, field.FieldType);
		}

		[Serializable]
		public partial class Field : MFieldInfo
		{
			public FieldInfo systemField;

			protected Field(FieldInfo systemField)
			{
				this.systemField = systemField;
			}

			public static Field Create(FieldInfo systemField)
			{
				return new Field(systemField);
			}

			public override string GetName()
			{
				return systemField.Name;
			}

			public override bool IsStatic()
			{
				return systemField.IsStatic;
			}

			public override MType GetDeclaringType()
			{
				return SystemType.Create(systemField.DeclaringType);
			}

			public override MType GetFieldType()
			{
				return SystemType.Create(systemField.FieldType);
			}

			internal override FieldInfo GetSystemField()
			{
				return systemField;
			}

			internal override void CodeGen(CodeGenState state)
			{
				if(systemField.IsLiteral)
				{
					object value = systemField.GetRawConstantValue();
					Type fieldType = systemField.FieldType;
					if(fieldType.BaseType == typeof(Enum)) fieldType = Enum.GetUnderlyingType(systemField.FieldType);
					if (fieldType== typeof(Int32))
						state.EmitInt((int)value);
					else throw new NotImplementedException("field constants not of type int");
				}
				else base.CodeGen(state);
			}
		}

		[Serializable]
		public partial class Constructor : MConstructorInfo
		{
			public ConstructorInfo systemCtor;

			protected Constructor(ConstructorInfo systemCtor)
			{
				this.systemCtor = systemCtor;
			}

			public static Constructor Create(ConstructorInfo systemCtor)
			{
				return new Constructor(systemCtor);
			}

			public override MType GetDeclaringType()
			{
				return SystemType.Create(systemCtor.DeclaringType);
			}

			public override ParamType[] GetParamTypes()
			{
				ParameterInfo[] @params = systemCtor.GetParameters();
				int num = @params.Length;
				ParamType[] mParamTypes = new ParamType[num];
				for (int i = 0; i < num; i++)
					mParamTypes[i] = ParamType.Create(SystemType.Create(@params[i].ParameterType), @params[i].IsOut);
				return mParamTypes;
			}

			internal override ConstructorInfo GetSystemConstructor()
			{
				return systemCtor;
			}
		}

		[Serializable]
		public partial class Method : MMethodInfo
		{
			public MethodInfo systemMethod;

			protected Method(MethodInfo systemMethod)
			{
				Contract.Requires(systemMethod != null);
				if (systemMethod.IsGenericMethodDefinition) throw new ArgumentException("IsGenericMethodDefinition", "systemMethod");
				this.systemMethod = systemMethod;
			}

			public static Method Create(MethodInfo systemMethod)
			{
				return new Method(systemMethod);
			}

			public override string GetName()
			{
				return systemMethod.Name;
			}

			public override bool IsStatic()
			{
				return systemMethod.IsStatic;
			}

			public override MType GetDeclaringType()
			{
				return SystemType.Create(systemMethod.DeclaringType);
			}

			public override MType GetReturnType()
			{
				return GetMTypeFromParameter(null, null, systemMethod.ReturnParameter);
			}

			public override ParamType[] GetParamTypes()
			{
				ParameterInfo[] @params = systemMethod.GetParameters();
				ParamType[] paramTypes = new ParamType[@params.Length];
				for (int i = 0; i < @params.Length; i++)
				{
					MType mType = GetMTypeFromParameter(null, null, @params[i]);
					paramTypes[i] = ParamType.Create(mType, @params[i].IsOut);
				}
				return paramTypes;
			}

			internal override MethodInfo GetSystemMethod()
			{
				return systemMethod;
			}
		}

		[Serializable]
		public partial class GenericMethod : MMethodInfo
		{
			public MethodInfo genericSystemMethod;
			public MType[] typeArgs;

			protected GenericMethod(MethodInfo genericSystemMethod, MType[] typeArgs)
			{
				Contract.Requires(genericSystemMethod != null);
				Contract.Requires(typeArgs != null);
				//if (!genericSystemMethod.IsGenericMethodDefinition) throw new ArgumentException("!IsGenericMethodDefinition", "genericSystemMethod");
				this.genericSystemMethod = genericSystemMethod;
				if (typeArgs.Length != genericSystemMethod.GetGenericArguments().Length) throw new ArgumentException(string.Format("genericSystemMethod has {0} type parameter(s) but {1} type(s) was supplied in typeArgs", genericSystemMethod.GetGenericArguments().Length, typeArgs.Length));
				this.typeArgs = typeArgs;
			}

			public static GenericMethod Create(MethodInfo genericSystemMethod, MType[] typeArgs)
			{
				return new GenericMethod(genericSystemMethod, typeArgs);
			}

			public override string GetName()
			{
				return genericSystemMethod.Name;
			}

			public override bool IsStatic()
			{
				return genericSystemMethod.IsStatic;
			}

			public override MType GetDeclaringType()
			{
				return SystemType.Create(genericSystemMethod.DeclaringType);
			}

			public override MType GetReturnType()
			{
				return GetMTypeFromParameter(null, typeArgs, genericSystemMethod.ReturnParameter);
			}

			public override ParamType[] GetParamTypes()
			{
				ParameterInfo[] @params = genericSystemMethod.GetParameters();
				ParamType[] paramTypes = new ParamType[@params.Length];
				for (int i = 0; i < @params.Length; i++)
				{
					MType mType = GetMTypeFromParameter(null, typeArgs, @params[i]);
					paramTypes[i] = ParamType.Create(mType, @params[i].IsOut);
				}
				return paramTypes;
			}

			internal override MethodInfo GetSystemMethod()
			{
				return genericSystemMethod;
			}
		}

		[Serializable]
		public class ObjRef : IObjectReference
		{
			private readonly Type systemType;

			protected ObjRef(SerializationInfo info, StreamingContext context)
			{
				systemType = (Type)info.GetValue("systemType", typeof(Type));
			}

			public object GetRealObject(StreamingContext context)
			{
				//return TypeFactory.Current.GetClrType(systemType);
				return SystemType.Create(systemType);
			}
		}
	}

	[Serializable]
	public partial class GenericSystemType : MType
	{
		public Type genericSystemType;
		public MType[] typeArgs;
		protected int levelKind;

		protected GenericSystemType(Type genericSystemType, MType[] typeArgs, int levelKind)
		{
			this.genericSystemType = genericSystemType;
			this.typeArgs = typeArgs;
			this.levelKind = levelKind;
		}

		public static MType Create(Type genericSystemType, MType[] typeArgs)
		{
			if (!genericSystemType.IsGenericTypeDefinition) throw new ArgumentException("!IsGenericTypeDefinition", "genericSystemType");
			if (genericSystemType.GetGenericArguments().Length != typeArgs.Length) throw new ArgumentException(string.Format("'genericSystemType' has {0} type parameter(s) but {1} type(s) was supplied in 'typeArgs'", genericSystemType.GetGenericArguments().Length, typeArgs.Length));
			bool allSystemTypes = true;
			int levelKind = 0;
			for (int i = 0; i < typeArgs.Length; i++)
			{
				int tmp = typeArgs[i].GetLevelKind();
				if (tmp > levelKind) levelKind = tmp;
				if (!(typeArgs[i] is SystemType)) allSystemTypes = false;
			}
			if (!allSystemTypes) return new GenericSystemType(genericSystemType, typeArgs, levelKind);
			else
			{
				Type[] systemTypeArgs = MType.GetSystemTypes(typeArgs);
				return SystemType.Create(genericSystemType.MakeGenericType(systemTypeArgs));
			}
		}

		public static GenericSystemType CreateGeneric(Type genericSystemType, MType[] typeArgs)
		{
			if (!genericSystemType.IsGenericTypeDefinition) throw new ArgumentException("!IsGenericTypeDefinition", "genericSystemType");
			if (genericSystemType.GetGenericArguments().Length != typeArgs.Length) throw new ArgumentException(string.Format("'genericSystemType' has {0} type parameter(s) but {1} type(s) was supplied in 'typeArgs'", genericSystemType.GetGenericArguments().Length, typeArgs.Length));
			int levelKind = 0;
			for (int i = 0; i < typeArgs.Length; i++)
			{
				int tmp = typeArgs[i].GetLevelKind();
				if (tmp > levelKind) levelKind = tmp;
			}
			return new GenericSystemType(genericSystemType, typeArgs, levelKind);
		}

		public override string GetName()
		{
			return genericSystemType.Name;
		}

		public override MType[] GetTypeArguments()
		{
			return typeArgs;
		}

		public override MType ReplaceTypeArguments(MType[] newTypeArgs)
		{
			return Create(genericSystemType, newTypeArgs);
		}

		public override bool IsGeneric()
		{
			return true;
		}

		public override bool IsValueType()
		{
			return genericSystemType.IsValueType;
		}

		public override bool IsInterface()
		{
			return genericSystemType.IsInterface;
		}

		protected override bool IsEqualToInternal(MType type)
		{
			GenericSystemType that = (GenericSystemType)type;
			return genericSystemType == that.genericSystemType && MType.AreEqualTo(typeArgs, that.typeArgs);
		}

		public override MType GetSuperType()
		{
			Type superType = genericSystemType.BaseType;
			return superType != null ? Subst(typeArgs, null, superType) : null;
		}

		public override MType[] GetInterfaces()
		{
			Type[] interfaces = genericSystemType.GetInterfaces();
			MType[] mInterfaces = new MType[interfaces.Length];
			for (int i = 0; i < interfaces.Length; i++)
				mInterfaces[i] = Subst(typeArgs, null, interfaces[i]);
			return mInterfaces;
		}

		public override MType GetNestedType(string name, MType[] args)
		{
			// I don't think nested types can capture type variables
			Type nestedType = genericSystemType.GetNestedType(name, BindingFlags.Public);
			if (nestedType != null)
				if (!nestedType.IsGenericTypeDefinition && args.Length == 0)
					return SystemType.Create(nestedType);
				else if (nestedType.IsGenericTypeDefinition && nestedType.GetGenericArguments().Length == args.Length)
					return GenericSystemType.Create(nestedType, args);
			return null;
		}

		protected internal override void GetFields(List<MFieldInfo> fields)
		{
			foreach (FieldInfo fieldInfo in genericSystemType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
				fields.Add(Field.Create(this, fieldInfo));
		}

		protected internal override void GetConstructors(List<MConstructorInfo> ctors)
		{
			foreach (ConstructorInfo ctorInfo in genericSystemType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				ctors.Add(Constructor.Create(this, ctorInfo));
		}

		private static MethodInfo getType = typeof(object).GetMethod("GetType");

		protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
			foreach (MethodInfo methodInfo in genericSystemType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
				//if (methodInfo != getType)
				if (!methodInfo.IsGenericMethod && typeParams.Length == 0) 
					methods.Add(Method.Create(this, methodInfo));
				else if (methodInfo.GetGenericArguments().Length == typeParams.Length)
					methods.Add(GenericMethod.Create(this, methodInfo, typeParams));

			//if (typeParams.Length == 0)
			//    foreach (PropertyInfo propertyInfo in genericSystemType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
			//    {
			//        MethodInfo methodInfo;
			//        methodInfo = propertyInfo.GetGetMethod();
			//        if (methodInfo != null) methods.Add(Method.Create(this, methodInfo, typeParams));
			//        methodInfo = propertyInfo.GetSetMethod();
			//        if (methodInfo != null) methods.Add(Method.Create(this, methodInfo, typeParams));
			//    }

			if (IsInterface())
			{
				foreach (MType iface in GetInterfaces())
					iface.GetMethods(methods, typeParams);
			}
		}

		public override Type GetSystemType()
		{
			Type[] systemTypeArgs = MType.GetSystemTypes(typeArgs);
			return genericSystemType.MakeGenericType(systemTypeArgs);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}

		public static MType Subst(MType[] classTypeArgs, MType[] methodTypeArgs, Type systemType)
		{
			return SystemType.Subst(classTypeArgs, methodTypeArgs, systemType);
		}

//		public void GetObjectData(SerializationInfo info, StreamingContext context)
//		{
//			info.AddValue("systemType", systemType);
//			info.SetType(typeof(SystemTypeRef));
//		}

		[Serializable]
		public partial class Field : MFieldInfo
		{
			public GenericSystemType declaringType;
			public FieldInfo systemField;

			protected Field(GenericSystemType declaringType, FieldInfo systemField)
			{
				Contract.Requires(declaringType != null);
				Contract.Requires(systemField != null);
				this.declaringType = declaringType;
				this.systemField = systemField;
			}

			public static MFieldInfo Create(GenericSystemType declaringType, FieldInfo systemField)
			{
				return new Field(declaringType, systemField);
			}

			public override MType GetDeclaringType()
			{
				return declaringType;
			}

			public override string GetName()
			{
				return systemField.Name;
			}

			public override bool IsStatic()
			{
				return systemField.IsStatic;
			}

			public override MType GetFieldType()
			{
				return Subst(declaringType.typeArgs, null, systemField.FieldType);
			}

			internal override FieldInfo GetSystemField()
			{
				Type declaringSystemType = declaringType.GetSystemType();
				throw new NotImplementedException();
				// doesn't work
				//return TypeBuilder.GetField(declaringSystemType, systemField);
			}
		}

		[Serializable]
		public partial class Constructor : MConstructorInfo
		{
			public GenericSystemType declaringType;
			public ConstructorInfo systemCtor;

			protected Constructor(GenericSystemType declaringType, ConstructorInfo systemCtor)
			{
				this.declaringType = declaringType;
				this.systemCtor = systemCtor;
			}

			public static MConstructorInfo Create(GenericSystemType declaringType, ConstructorInfo systemCtor)
			{
				return new Constructor(declaringType, systemCtor);
			}

			public override MType GetDeclaringType()
			{
				return declaringType;
			}

			public override ParamType[] GetParamTypes()
			{
				ParameterInfo[] @params = systemCtor.GetParameters();
				ParamType[] paramTypes = new ParamType[@params.Length];
				for (int i = 0; i < @params.Length; i++)
					paramTypes[i] = ParamType.Create(Subst(declaringType.typeArgs, null, @params[i].ParameterType), @params[i].IsOut);
				return paramTypes;
			}

			internal override ConstructorInfo GetSystemConstructor()
			{
				Type declaringSystemType = declaringType.GetSystemType();
				throw new NotImplementedException();
				// doesn't work
				//return TypeBuilder.GetConstructor(declaringSystemType, systemCtor);
			}
		}

		[Serializable]
		public partial class Method : MMethodInfo
		{
			public GenericSystemType declaringType;
			public MethodInfo systemMethod;

			protected Method(GenericSystemType declaringType, MethodInfo systemMethod)
			{
				Contract.Requires(declaringType != null);
				Contract.Requires(systemMethod != null);
				this.declaringType = declaringType;
				if (systemMethod.IsGenericMethodDefinition) throw new ArgumentException("IsGenericMethodDefinition", "systemMethod");
				this.systemMethod = systemMethod;
			}

			public static MMethodInfo Create(GenericSystemType declaringType, MethodInfo systemMethod)
			{
				return new Method(declaringType, systemMethod);
			}

			public static MMethodInfo CreateFromToken(GenericSystemType declaringType, int genericSystemMethodToken)
			{
				return Create(
					declaringType,
					(MethodInfo)declaringType.genericSystemType.Module.ResolveMethod(genericSystemMethodToken));
			}

			public static MMethodInfo CreateFromHandle(GenericSystemType declaringType, RuntimeMethodHandle genericSystemMethodHandle)
			{
				return Create(
					declaringType,
					(MethodInfo)MethodInfo.GetMethodFromHandle(genericSystemMethodHandle, declaringType.GetSystemType().TypeHandle));
			}

			public static MMethodInfo CreateFromName(GenericSystemType declaringType, string genericSystemMethodName)
			{
				return Create(
					declaringType,
					declaringType.GetSystemType().GetMethod(genericSystemMethodName));
			}

			public override string GetName()
			{
				return systemMethod.Name;
			}

			public override bool IsStatic()
			{
				return systemMethod.IsStatic;
			}

			public override MType GetDeclaringType()
			{
				return declaringType;
			}

			public override MType GetReturnType()
			{
				return Subst(declaringType.typeArgs, null, systemMethod.ReturnType);
			}

			public override ParamType[] GetParamTypes()
			{
				ParameterInfo[] @params = systemMethod.GetParameters();
				ParamType[] paramTypes = new ParamType[@params.Length];
				for (int i = 0; i < @params.Length; i++)
					paramTypes[i] = ParamType.Create(Subst(declaringType.typeArgs, null, @params[i].ParameterType), @params[i].IsOut);
				return paramTypes;
			}

			internal override MethodInfo GetSystemMethod()
			{
				try { return (MethodInfo)MethodBase.GetMethodFromHandle(systemMethod.MethodHandle, declaringType.GetSystemType().TypeHandle); }
				catch
				{
					Type declaringSystemType = declaringType.GetSystemType();
					try { return TypeBuilder.GetMethod(declaringSystemType, systemMethod); }
					catch (Exception e) { throw new Exception("Problem with Reflection.Emit.", e); }
				}
				//Type declaringSystemType = declaringType.GetSystemType();
				//try { return TypeBuilder.GetMethod(declaringSystemType, systemMethod); }
				//catch (ArgumentException e)
				//{
				//    if (e.Message == "'type' must contain a TypeBuilder as a generic argument.")
				//        throw new Exception("yeah, there's a problem with System.Reflection");
				//    else throw e;
				//}
			}
		}

		[Serializable]
		public partial class GenericMethod : MMethodInfo
		{
			public GenericSystemType declaringType;
			public MethodInfo genericSystemMethod;
			public MType[] typeArgs;

			protected GenericMethod(GenericSystemType declaringType, MethodInfo genericSystemMethod, MType[] typeArgs)
			{
				Contract.Requires(declaringType != null);
				Contract.Requires(genericSystemMethod != null);
				Contract.Requires(typeArgs != null);
				this.declaringType = declaringType;
				if (!genericSystemMethod.IsGenericMethodDefinition) throw new ArgumentException("!IsGenericMethodDefinition", "genericSystemMethod");
				this.genericSystemMethod = genericSystemMethod;
				if (typeArgs.Length != genericSystemMethod.GetGenericArguments().Length) throw new ArgumentException(string.Format("genericSystemMethod has {0} type parameter(s) but {1} type(s) was supplied in typeArgs", genericSystemMethod.GetGenericArguments().Length, typeArgs.Length));
				this.typeArgs = typeArgs;
			}

			public static GenericMethod Create(GenericSystemType declaringType, MethodInfo genericSystemMethod, MType[] typeArgs)
			{
				return new GenericMethod(declaringType, genericSystemMethod, typeArgs);
			}

			public static GenericMethod CreateFromToken(GenericSystemType declaringType, int genericSystemMethodToken, MType[] typeArgs)
			{
				return Create(
					declaringType,
					(MethodInfo)declaringType.genericSystemType.Module.ResolveMethod(genericSystemMethodToken),
					typeArgs);
			}

			public static MMethodInfo CreateFromHandle(GenericSystemType declaringType, RuntimeMethodHandle genericSystemMethodHandle, MType[] typeArgs)
			{
				return Create(
					declaringType,
					(MethodInfo)MethodInfo.GetMethodFromHandle(genericSystemMethodHandle, declaringType.GetSystemType().TypeHandle),
					typeArgs);
			}

			public static MMethodInfo CreateFromName(GenericSystemType declaringType, string genericSystemMethodName, MType[] typeArgs)
			{
				return Create(
					declaringType,
					declaringType.GetSystemType().GetMethod(genericSystemMethodName),
					typeArgs);
			}

			public override string GetName()
			{
				return genericSystemMethod.Name;
			}

			public override bool IsStatic()
			{
				return genericSystemMethod.IsStatic;
			}

			public override MType GetDeclaringType()
			{
				return declaringType;
			}

			public override MType GetReturnType()
			{
				return SystemType.GetMTypeFromParameter(declaringType.typeArgs, typeArgs, genericSystemMethod.ReturnParameter);
			}

			public override ParamType[] GetParamTypes()
			{
				ParameterInfo[] @params = genericSystemMethod.GetParameters();
				ParamType[] paramTypes = new ParamType[@params.Length];
				for (int i = 0; i < @params.Length; i++)
				{
					MType mType = SystemType.GetMTypeFromParameter(declaringType.typeArgs, typeArgs, @params[i]);
					paramTypes[i] = ParamType.Create(mType, @params[i].IsOut);
				}
				return paramTypes;
			}

			internal override MethodInfo GetSystemMethod()
			{
				Type declaringSystemType = declaringType.GetSystemType();
				throw new NotImplementedException();
				// doesn't work
				//MethodInfo systemMethod = TypeBuilder.GetMethod(declaringSystemType, genericSystemMethod);
				//Type[] systemTypeArgs = MType.GetSystemTypes(typeArgs);
				//return systemMethod.MakeGenericMethod(systemTypeArgs);
			}
		}

	}

}