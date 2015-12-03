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
	public enum CILType { Void, I1, I2, I4, I8, I, R4, R8, Ref, U, U1, U2, U4, U8, ValueType };

	//[Serializable]
	public abstract partial class MType: ISerializable
	{
		//protected int levelKind;
		
		protected MType(/*int levelKind*/)
		{
			//if (levelKind < 0) throw new ArgumentOutOfRangeException("levelKind", "cannot be negative");
			//this.levelKind = levelKind;
		}

		public static MType GetMType(object obj)
		{
			return SystemType.Create(obj.GetType());
		}

		public virtual int GetLevelKind()
		{
			return 0;
		}

		//public static int GetLevelKind(MType[] types)
		//{
		//    int levelKind = 0;
		//    for (int i = 0; i < types.Length; i++)
		//        levelKind = Math.Max(levelKind, types[i].GetLevelKind());
		//    return levelKind;
		//}

		public abstract MType GetSuperType();

		public virtual MType[] GetInterfaces()
		{
			return MType.Empty;
		}

		public virtual MType[] GetTypeArguments()
		{
			return MType.Empty;
		}

		public virtual MType ReplaceTypeArguments(MType[] newTypeArgs)
		{
			if (GetTypeArguments().Length == 0) return this;
			else throw new NotImplementedException();
		}

		public virtual string GetName()
		{
			return null;
		}

		public virtual bool IsGeneric()
		{
			return GetTypeArguments().Length > 0;
		}

		public virtual bool IsArray()
		{
			return false;
		}

		public virtual MType GetElementType()
		{
			if (IsArray()) return GetTypeArguments()[0];
			else throw new InvalidOperationException("Type is not an array.");
		}

		public virtual int GetArrayRank()
		{
			throw new InvalidOperationException("Type is not an array.");
		}

		public virtual bool IsValueType()
		{
			MType superType = GetSuperType();
			return superType == PrimType.Value;
		}

		public virtual bool IsInterface()
		{
			return false;
		}

		public virtual bool HasDefaultConstructor()
		{
			return GetConstructor(ParamType.Empty) != null;
		}

		#region Filtered member methods
		public virtual MType GetNestedType(string name, MType[] args)
		{
			MType superType = GetSuperType();
			return superType != null ? superType.GetNestedType(name, args) : null;
		}

		public MFieldInfo GetField(string name, bool isStatic)
		{
			foreach (MFieldInfo field in GetFields())
				if (field.IsStatic() == isStatic && field.GetName() == name)
					return field;
			return null;
		}

		public MFieldInfo[] GetFields(bool isStatic)
		{
			List<MFieldInfo> fields = new List<MFieldInfo>();
			foreach (MFieldInfo field in GetFields())
				if (field.IsStatic() == isStatic) fields.Add(field);
			return fields.ToArray();
		}

		public MConstructorInfo GetConstructor(ParamType[] paramTypes)
		{
			return MConstructorInfo.ChooseBestOverload(GetConstructors(), paramTypes);
		}

		public MMethodInfo GetMethod(string name, bool isStatic, MType[] typeParams, ParamType[] paramTypes)
		{
			return MMethodInfo.ChooseBestOverload(GetMethods(name, isStatic, typeParams), paramTypes, null);
		}

		public MMethodInfo[] GetMethods(string name, bool isStatic, MType[] typeParams)
		{
			List<MMethodInfo> methods = new List<MMethodInfo>();
			foreach (MMethodInfo method in GetMethods(typeParams))
				if (method.IsStatic() == isStatic && method.GetName() == name)
					methods.Add(method);
			return methods.Count > 0 ? methods.ToArray(): null;
		}

		public MMethodInfo[] GetMethods(string name, MType[] typeParams)
		{
			List<MMethodInfo> methods = new List<MMethodInfo>();
			foreach (MMethodInfo method in GetMethods(typeParams))
				if (method.GetName() == name)
					methods.Add(method);
			return methods.Count > 0 ? methods.ToArray() : null;
		}
		#endregion

		#region All member methods
		public MFieldInfo[] GetFields()
		{
			List<MFieldInfo> fields = new List<MFieldInfo>();
			GetFields(fields);
			return fields.ToArray();
		}

		protected internal virtual void GetFields(List<MFieldInfo> fields)
		{
			MType superType = GetSuperType();
			if(superType != null) superType.GetFields(fields);
		}

		public MConstructorInfo[] GetConstructors()
		{
			List<MConstructorInfo> ctors = new List<MConstructorInfo>();
			GetConstructors(ctors);
			return ctors.ToArray();
		}

		protected internal virtual void GetConstructors(List<MConstructorInfo> ctors)
		{
			if (IsValueType()) ctors.Add(new ValueTypeConstructor(this));
		}

		public MMethodInfo[] GetMethods(MType[] typeParams)
		{
			List<MMethodInfo> methods = new List<MMethodInfo>();
			GetMethods(methods, typeParams);
			return methods.ToArray();
		}

		protected internal virtual void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
		    Contract.Ensure(Contract.Result<Metaphor.MType>() != null);
            MType superType = GetSuperType();
			if (superType != null) superType.GetMethods(methods, typeParams);
		}

		//protected class GetTypeMethod : MetaphorMethod
		//{
		//    protected GetTypeMethod(MType type): base(type, typeof(MType).GetMethod("GetMType", BindingFlags.Public | BindingFlags.Static), ExistsType.Create(type, false, TypeType.Create(ExistsType.Vars[0])), ParamType.Empty)
		//    {
		//    }

		//    public static GetTypeMethod Create(MType type)
		//    {
		//        return new GetTypeMethod(type);
		//    }

		//    public override string GetName()
		//    {
		//        return "GetType";
		//    }

		//    public override bool IsStatic()
		//    {
		//        return false;
		//    }

		//}
		#endregion

		#region Comparision methods
		public bool IsEqualTo(MType type)
		{
			MType type0 = this;
			while (type0 is LiftType) type0 = ((LiftType)type0).type;
			while (type is LiftType) type = ((LiftType)type).type;

			return type0.GetType() == type.GetType() ? type0.IsEqualToInternal(type) : false;
		}

		protected abstract bool IsEqualToInternal(MType type);

		public virtual bool IsSubTypeOf(MType type2)
		{
			while (type2 is LiftType) type2 = ((LiftType)type2).type;
			if (!object.ReferenceEquals(type2, null))
			{
				//if (type2 is ExistsType)
				//{
				//    //TODO something better than this
				//    return true;
				//}
				//if (type2 is EscapeType) return IsSubTypeOf(((EscapeType)type2).type);
				if (type2.IsInterface())
				{
					Stack<MType> stack = new Stack<MType>();
					stack.Push(this);
					while (stack.Count > 0)
					{
						MType @interface = stack.Pop();
						if (@interface.IsEqualTo(type2)) return true;
						foreach (MType i in @interface.GetInterfaces()) stack.Push(i);
					}
					return false;
				}
				for (MType type1 = this; !object.ReferenceEquals(type1, null); type1 = type1.GetSuperType())
					if (type1.IsEqualTo(type2)) return true;
			}
			return false;
		}

		#region System comparison methods
		public override bool Equals(object obj)
		{
			MType that = obj as MType;
			return that != null ? IsEqualTo(that) : false;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(MType type1, MType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? type1.IsEqualTo(type2) : object.ReferenceEquals(type1, type2);
		}

		public static bool operator !=(MType type1, MType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? !type1.IsEqualTo(type2) : !object.ReferenceEquals(type1, type2);
		}

		public static bool operator <=(MType type1, MType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? type1.IsSubTypeOf(type2) : false;
		}

		public static bool operator >(MType type1, MType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? !type1.IsSubTypeOf(type2) : false;
		}

		public static bool operator <(MType type1, MType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? type1.IsSubTypeOf(type2) && !type1.IsEqualTo(type2) : false;
		}

		public static bool operator >=(MType type1, MType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? !type1.IsSubTypeOf(type2) || type1.IsEqualTo(type2) : false;
		}
		#endregion
		#endregion

		#region Code generation methods
		internal virtual CILType GetCILType()
		{
			return CILType.Ref;
		}

		private static readonly MethodInfo fromObject = typeof(LitObj).GetMethod("FromObject", BindingFlags.Static | BindingFlags.Public);
		
		internal virtual void MakeLiteral(CodeGenState state)
		{
			state.code.Emit(OpCodes.Call, fromObject);
		}

		public abstract Type GetSystemType();

		internal virtual void GenerateClosures(TypeCheckState state)
		{
			foreach (MType type in GetTypeArguments())
				type.GenerateClosures(state);
		}

		private static readonly MethodInfo getSystemType = typeof(MType).GetMethod("GetSystemType");
		internal virtual void CodeGenSystemType(CodeGenState state)
		{
			state.EmitType(GetSystemType());
		}

		internal virtual void CodeGen(CodeGenState state)
		{
			state.EmitType(GetSystemType());
			state.MakeType(typeof(SystemType));
		}

		internal virtual void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}

		#endregion

		//#region Type manipulation methods
		//public virtual MType Subst(int sym, MType type)
		//{
		//    return this;
		//}

		//public virtual MType Unpack(int index, TypeVar var)
		//{
		//    return this;
		//}
		//#endregion

		#region Comparison array methods
		public static MType[] Empty = new MType[] {};

		public static int GetHashCode(MType[] types)
		{
			int hash = 0;
			foreach (MType type in types) hash ^= type.GetHashCode();
			return hash;
		}

		public static bool AreEqualTo(MType[] types1, MType[] types2)
		{
			if (types1.Length == types2.Length)
			{
				int n = types1.Length;
				for (int i = 0; i < n; i++)
					if (!types1[i].IsEqualTo(types2[i])) return false;
				return true;
			}
			else return false;
		}
		
		public static bool AreSubtypesOf(MType[] types1, MType[] types2)
		{
			if (types1.Length == types2.Length)
			{
				int n = types1.Length;
				for (int i = 0; i < n; i++)
					if (!types1[i].IsSubTypeOf(types2[i])) return false;
				return true;
			}
			else return false;
		}
		#endregion

		#region Code generation array methods
		internal static Type[] GetSystemTypes(MType[] mTypes)
		{
			Type[] types = new Type[mTypes.Length];
			for (int i = 0; i < mTypes.Length; i++)
				if (mTypes[i].GetLevelKind() == 0)
					types[i] = mTypes[i].GetSystemType();
			return types;
		}

		internal static void CodeGen<T>(CodeGenState state, T[] types) where T : MType
		{
			int length = types.Length;
			state.EmitInt(length);
			state.code.Emit(OpCodes.Newarr, typeof(T));
			for (int i = 0; i < length; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitInt(i);
				types[i].CodeGen(state);
				state.code.Emit(OpCodes.Stelem_Ref);
			}
		}

		internal static void CodeGenMeta(CodeGenState state, MType[] types)
		{
			int length = types.Length;
			state.EmitInt(length);
			state.code.Emit(OpCodes.Newarr, typeof(MType));
			for (int i = 0; i < length; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitInt(i);
				types[i].CodeGenMeta(state);
				state.code.Emit(OpCodes.Stelem_Ref);
			}
		}
		#endregion

		#region Serialization methods
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(MTypeSurrogate));
			return;
		}

		public class MTypeSurrogate: IObjectReference
		{
			object IObjectReference.GetRealObject(StreamingContext context)
			{
				return PrimType.Int;
			}
		}
		#endregion

		public override string ToString()
		{
			MType[] typeArgs = GetTypeArguments();
			if(typeArgs.Length == 0) return GetName();
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (IsArray())
			{
				sb.AppendFormat("{0}[", typeArgs[0]);
				int rank = GetArrayRank();
				for (int i = 1; i < rank; i++) sb.Append(",");
				sb.Append("]");
			}
			else
			{
				sb.AppendFormat("{0}<{1}", GetName(), typeArgs[0]);
				for (int i = 1; i < typeArgs.Length; i++)
					sb.AppendFormat(",{0}", typeArgs[i]);
				sb.Append(">");
			}
			return sb.ToString();
		}
	}

	[Serializable]
	public class ParamType
	{
		public MType type;
		public bool @ref;

		protected ParamType(MType type, bool @ref)
		{
			this.type = type;
			this.@ref = @ref;
		}

		public static ParamType Create(MType type, bool @ref)
		{
			return new ParamType(type, @ref);
		}

		internal Type GetSystemType()
		{
			Type baseType = type.GetSystemType();
			if(@ref) baseType = baseType.MakeByRefType();
			return baseType;
		}

		public bool IsEqualTo(ParamType that)
		{
			return this.@ref == that.@ref && this.type.IsEqualTo(that.type);
		}

		public bool IsSubTypeOf(ParamType that)
		{
			return (this.@ref == false && that.@ref == false && this.type.IsSubTypeOf(that.type)) || (this.@ref == true && that.@ref == true && this.type.IsEqualTo(that.type));
		}

		#region System comparison methods
		public override bool Equals(object obj)
		{
			ParamType that = obj as ParamType;
			return that != null ? IsEqualTo(that) : false;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(ParamType type1, ParamType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? type1.IsEqualTo(type2) : object.ReferenceEquals(type1, type2);
		}

		public static bool operator !=(ParamType type1, ParamType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? !type1.IsEqualTo(type2) : !object.ReferenceEquals(type1, type2);
		}

		public static bool operator <=(ParamType type1, ParamType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? type1.IsSubTypeOf(type2) : false;
		}

		public static bool operator >(ParamType type1, ParamType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? !type1.IsSubTypeOf(type2) : false;
		}

		public static bool operator <(ParamType type1, ParamType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? type1.IsSubTypeOf(type2) && !type1.IsEqualTo(type2) : false;
		}

		public static bool operator >=(ParamType type1, ParamType type2)
		{
			return !object.ReferenceEquals(type1, null) && !object.ReferenceEquals(type2, null) ? !type1.IsSubTypeOf(type2) || type1.IsEqualTo(type2) : false;
		}
		#endregion

		//public ParamType Subst(int sym, MType type)
		//{
		//    MType substType = this.type.Subst(sym, type);
		//    return this.type != substType ? new ParamType(substType, @ref): this;
		//}

		internal void CodeGenMeta(CodeGenState state)
		{
			type.CodeGenMeta(state);
			state.EmitBool(@ref);
			state.MakeType(typeof(ParamType));
		}

		public static ParamType[] Empty = new ParamType[] {};
		public static ParamType[] DelegateCtor = new ParamType[] { new ParamType(PrimType.Object, false), new ParamType(PrimType.Native, false) };

		internal static Type[] GetSystemTypes(ParamType[] paramTypes)
		{
			int num = paramTypes.Length;
			Type[] types = new Type[num];
			for(int i = 0; i < num; i++) types[i] = paramTypes[i].GetSystemType();
			return types;
		}

		internal static void CodeGen(CodeGenState state, ParamType[] paramTypes)
		{
			int length = paramTypes.Length;
			state.EmitInt(length);
			state.code.Emit(OpCodes.Newarr, typeof(ParamType));
			for (int i = 0; i < length; i++)
			{
				state.code.Emit(OpCodes.Dup);
				state.EmitInt(i);
				paramTypes[i].CodeGenMeta(state);
				state.code.Emit(OpCodes.Stelem_Ref);
			}
		}

		public static bool AreEqualTo(ParamType[] types1, ParamType[] types2)
		{
			if (types1.Length == types2.Length)
			{
				int n = types1.Length;
				for (int i = 0; i < n; i++)
					if (!types1[i].IsEqualTo(types2[i])) return false;
				return true;
			}
			else return false;
		}

		public static bool AreSubtypesOf(ParamType[] types1, ParamType[] types2)
		{
			if (types1.Length == types2.Length)
			{
				int n = types1.Length;
				for (int i = 0; i < n; i++)
					if (!types1[i].IsSubTypeOf(types2[i])) return false;
				return true;
			}
			else return false;
		}

		public static int GoodnessOfFit(ParamType[] types1, ParamType[] types2)
		{
			if (types1.Length == types2.Length)
			{
				int n = types1.Length;
				int count = 0;
				for (int i = 0; i < n; i++)
				{
					if (types1[i].IsEqualTo(types2[i])) count++;
					else if (types1[i].@ref || types2[i].@ref || !Code.IsCompatible(types1[i].type, types2[i].type)) return -1;
				}
				return count;
			}
			else return -1;
		}

		public override string ToString()
		{
			if (!@ref) return type.ToString();
			else return "ref " + type.ToString();
		}

//		public override bool Equals(object obj)
//		{
//			if(obj is ParamType)
//			{
//				ParamType that = (ParamType) obj;
//				return this.type == that.type && this.@ref == that.@ref;
//			}
//			else return false;
//		}
//
//		public override int GetHashCode()
//		{
//			return type.GetHashCode();
//		}
//
//		public override string ToString()
//		{
//			if(!@ref) return type.ToString();
//			else
//				return "ref " + type.ToString();
//		}
//
//		public static bool operator==(ParamType a, ParamType b)
//		{
//			return a.type == b.type && a.@ref == b.@ref;
//		}
//
//		public static bool operator!=(ParamType a, ParamType b)
//		{
//			return a.type != b.type || a.@ref != b.@ref;
//		}

	}

	public class TypeVar : MType
	{
		public TypeVarDecl decl;

		//[NonSerialized]
		//internal TypeVarDecl implDecl;
		
		protected TypeVar(TypeVarDecl decl)
		{
			this.decl = decl;
		}

		public static TypeVar Create(TypeVarDecl decl)
		{
			return new TypeVar(decl);
		}

#region stuff
		public override string GetName()
		{
			return decl.name;
		}

		public override int GetLevelKind()
		{
			return decl.levelKind;
		}

		public override bool IsArray()
		{
			return decl.con.superType != null && decl.con.superType.IsArray();
		}

		public override int GetArrayRank()
		{
			if (IsArray()) return decl.con.superType.GetArrayRank();
			else return base.GetArrayRank();
		}

		public override bool IsValueType()
		{
			return decl.con.kind == TypeKind.Struct || decl.con.superType != null && decl.con.superType.IsValueType();
		}

		protected override bool IsEqualToInternal(MType type)
		{
			TypeVar that = (TypeVar)type;
			return decl == that.decl;
		}

		public override MType GetSuperType()
		{
			if (decl.con.superType != null) return decl.con.superType;
			else if (decl.con.kind == TypeKind.Struct) return PrimType.Value;
			else return PrimType.Object;
		}

		protected internal override void GetFields(List<MFieldInfo> fields)
		{
			GetSuperType().GetFields(fields);
		}

		protected internal override void GetConstructors(List<MConstructorInfo> ctors)
		{
			if (decl.con.hasNew) ctors.Add(new TypeVarConstructor(this));
			base.GetConstructors(ctors);
		}

		protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
			GetSuperType().GetMethods(methods, typeParams);
			if (decl.con.interfaces != null)
				foreach (MType iface in decl.con.interfaces)
					iface.GetMethods(methods, typeParams);
		}

		private static readonly MethodInfo makeLiteral = typeof(LitObj).GetMethod("MakeLiteral");

		internal override void MakeLiteral(CodeGenState state)
		{
			GenericTypeVarDecl genericDecl = decl as GenericTypeVarDecl;
			if (genericDecl != null)
			{
				MethodInfo makeLiteralThis = makeLiteral.MakeGenericMethod(genericDecl.genericParam);
				state.code.Emit(OpCodes.Call, makeLiteralThis);
			}
			else throw new InvalidOperationException();
		}

		//public override MType Subst(int sym, MType type)
		//{
		//    return this.sym == sym ? type : this;
		//}
#endregion

		public override Type GetSystemType()
		{
			return decl.GetSystemType();
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			if (state.GetLevel() == 0)
			{
				//implDecl = state.LookupTypeVar(sym);
			}
		}

		internal override void CodeGenSystemType(CodeGenState state)
		{
			decl.CodeGenSystemType(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			decl.CodeGen(state);
			//state.MakeType(typeof(SystemType));
		}

  		internal override void CodeGenMeta(CodeGenState state)
		{
			decl.CodeGenMeta(state);
			state.MakeType(typeof(TypeVar));
		}
	}

	public class PatternVar : MType
	{
		public LocalReflTypeVarDecl decl;

		//[NonSerialized]
		//internal TypeVarDecl implDecl;

		public PatternVar(LocalReflTypeVarDecl decl)
		{
			this.decl = decl;
		}

		#region stuff
		public override string GetName()
		{
			return decl.name;
		}

		public override int GetLevelKind()
		{
			return decl.levelKind;
		}

		public override bool IsArray()
		{
			return decl.con.superType != null && decl.con.superType.IsArray();
		}

		public override int GetArrayRank()
		{
			if (IsArray()) return decl.con.superType.GetArrayRank();
			else return base.GetArrayRank();
		}

		public override bool IsValueType()
		{
			return decl.con.kind == TypeKind.Struct || decl.con.superType != null && decl.con.superType.IsValueType();
		}

		protected override bool IsEqualToInternal(MType type)
		{
			TypeVar that = (TypeVar)type;
			return decl == that.decl;
		}

		public override MType GetSuperType()
		{
			if (decl.con.superType != null) return decl.con.superType;
			else if (decl.con.kind == TypeKind.Struct) return PrimType.Value;
			else return PrimType.Object;
		}

		protected internal override void GetFields(List<MFieldInfo> fields)
		{
			GetSuperType().GetFields(fields);
		}

		protected internal override void GetConstructors(List<MConstructorInfo> ctors)
		{
			if (decl.con.hasNew) ctors.Add(new TypeVarConstructor(this));
			base.GetConstructors(ctors);
		}

		protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
			GetSuperType().GetMethods(methods, typeParams);
			if (decl.con.interfaces != null)
				foreach (MType iface in decl.con.interfaces)
					iface.GetMethods(methods, typeParams);
		}

		internal override void MakeLiteral(CodeGenState state)
		{
			throw new Exception("Should never be called.");
		}

		//public override MType Subst(int sym, MType type)
		//{
		//    return this.sym == sym ? type : this;
		//}
		#endregion

		public override Type GetSystemType()
		{
			throw new InvalidOperationException();
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			if (state.GetLevel() == 0)
			{
				//implDecl = state.LookupTypeVar(sym);
			}
		}

		internal override void CodeGen(CodeGenState state)
		{
			decl.CodeGenMeta(state);
			state.MakeType(typeof(PatternVar));
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			decl.CodeGenMeta(state);
			state.MakeType(typeof(PatternVar));
		}
	}

	public class LiftType : MType
	{
		internal MType type;

		public LiftType(MType type)
		{
			this.type = type;
		}

		public static MType Promote(MType type)
		{
			if(type is TypeVar || type is LiftType) return new LiftType(type);
			MType[] typeArgs = type.GetTypeArguments();
			if (typeArgs.Length == 0) return type;
			else
			{
				MType[] liftedTypeArgs = new MType[typeArgs.Length];
				for (int i = 0; i < typeArgs.Length; i++) liftedTypeArgs[i] = Promote(typeArgs[i]);
				return type.ReplaceTypeArguments(liftedTypeArgs);
			}
		}

		public static MType[] Promote(MType[] types)
		{
			MType[] liftedTypes = new MType[types.Length];
			for (int i = 0; i < types.Length; i++) liftedTypes[i] = Promote(types[i]);
			return liftedTypes;
		}

		public override int GetLevelKind()
		{
			int tmp = type.GetLevelKind();
			return tmp > 0 ? tmp - 1 : 0;
		}

		public override MType GetSuperType()
		{
			MType superType = type.GetSuperType();
			return superType != null ? Promote(superType) : null;
		}

		public override MType[] GetInterfaces()
		{
			return Promote(type.GetInterfaces());
		}

		public override MType[] GetTypeArguments()
		{
			return Promote(type.GetTypeArguments());
		}

		public override MType ReplaceTypeArguments(MType[] newTypeArgs)
		{
			if (base.GetTypeArguments().Length == 0) return this;
			else throw new NotImplementedException();
		}

		public override string GetName()
		{
			return type.GetName();
		}

		public override bool IsArray()
		{
			return type.IsArray();
		}

		public override MType GetElementType()
		{
			return Promote(type.GetElementType());
		}

		public override int GetArrayRank()
		{
			return type.GetArrayRank();
		}

		public override bool IsGeneric()
		{
			return type.IsGeneric();
		}

		public override bool IsInterface()
		{
			return type.IsInterface();
		}

		public override bool IsValueType()
		{
			return type.IsValueType();
		}

		public override bool HasDefaultConstructor()
		{
			return type.HasDefaultConstructor();
		}

		#region reflection methods
		protected internal override void GetConstructors(List<MConstructorInfo> ctors)
		{
			type.GetConstructors(ctors);
			for (int i = 0; i < ctors.Count; i++)
			{
				TypeVarConstructor typeVarCtor = ctors[i] as TypeVarConstructor;
				if (typeVarCtor != null)
				{
					ctors[i] = new TypeVarConstructor(Promote(typeVarCtor.typeVar));
					continue;
				}
				ValueTypeConstructor valueTypeCtor = ctors[i] as ValueTypeConstructor;
				if (valueTypeCtor != null)
				{
					ctors[i] = new ValueTypeConstructor(Promote(valueTypeCtor.valueType));
					continue;
				}
			}
		}

		protected internal override void GetFields(List<MFieldInfo> fields)
		{
			type.GetFields(fields);
		}

		protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
			type.GetMethods(methods, typeParams);
		}

		public override MType GetNestedType(string name, MType[] args)
		{
			return type.GetNestedType(name, args);
		}

		internal override CILType GetCILType()
		{
			return type.GetCILType();
		}

		internal override void MakeLiteral(CodeGenState state)
		{
			type.MakeLiteral(state);
		}
		#endregion

		protected override bool IsEqualToInternal(MType type)
		{
			throw new InvalidOperationException();
		}

		public override bool IsSubTypeOf(MType type2)
		{
			return type.IsSubTypeOf(type2);
		}

		public override Type GetSystemType()
		{
			return type.GetSystemType();
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			state.LowerLevel();
			type.GenerateClosures(state);
			state.RaiseLevel();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			if (state.level == 1)
			{
				state.level = 0;
				type.CodeGen(state);
				state.level = 1;
			}
			else
			{
				state.level--;
				type.CodeGenMeta(state);
				state.level++;
				state.MakeCode(typeof(LiftType));
			}
		}
	}
}