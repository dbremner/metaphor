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

namespace Metaphor
{
	public sealed class RefelctedField : MFieldInfo
	{
		public int sym;
		public MType declaringType;
		public bool isStatic;
		public MType fieldType;
		public string name;

		[NonSerialized]
		private ForField field;

		public RefelctedField(int sym, MType declaringType, bool isStatic, MType fieldType, string name)
		{
			this.sym = sym;
			this.declaringType = declaringType;
			this.isStatic = isStatic;
			this.fieldType = fieldType;
			this.name = name;
		}

		public override MType GetDeclaringType()
		{
			return declaringType;
		}

		public override bool IsStatic()
		{
			return isStatic;
		}

		public override MType GetFieldType()
		{
			return fieldType;
		}

		public override string GetName()
		{
			return name;
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			field = state.LookupField(sym);
		}

		internal override FieldInfo GetSystemField()
		{
			return base.GetSystemField();
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitLdloc(field.pos);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt(sym);
			state.EmitBool(isStatic);
			fieldType.CodeGenMeta(state);
			state.EmitString(name);
			state.MakeCode(typeof(RefelctedField));
		}
	}

	public sealed class LiftField : MFieldInfo
	{
		public MFieldInfo field;

		public LiftField(MFieldInfo field)
		{
			this.field = field;
		}

		public override MType GetDeclaringType()
		{
			return field.GetDeclaringType();
		}

		public override bool IsStatic()
		{
			return field.IsStatic();
		}

		public override MType GetFieldType()
		{
			return field.GetFieldType();
		}

		public override string GetName()
		{
			return field.GetName();
		}

		internal override void GenerateClosures(TypeCheckState state)
		{
			state.LowerLevel();
			field.GenerateClosures(state);
			state.RaiseLevel();
		}

		internal override FieldInfo GetSystemField()
		{
			throw new InvalidOperationException("Lift called at stage 0.");
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			if (state.level == 1)
			{
				state.level = 0;
				field.CodeGen(state);
				state.level = 1;
			}
			else
			{
				state.level--;
				field.CodeGenMeta(state);
				state.level++;
				state.MakeCode(typeof(LiftField));
			}
		}
	}
	//[Serializable]
	//public partial class ModuleType : MType
	//{
	//    protected ModuleType()
	//    {
	//    }

	//    public static ModuleType Create()
	//    {
	//        return new ModuleType();
	//    }

	//    protected override bool IsEqualToInternal(MType type)
	//    {
	//        return type is ModuleType;
	//    }

	//    public override MType GetSuperType()
	//    {
	//        return PrimType.Object;
	//    }

	//    protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
	//    {
	//        //methods.Add(new NonGenericMethodInfo(MetaphorMethod.Create(this, typeof(MModuleBuilder).GetMethod("Save", BindingFlags.Public | BindingFlags.Instance, null, new Type[] {typeof(string)}, null), PrimType.Void, new ParamType[] { ParamType.Create(PrimType.String, false) })));
	//    }

	//    internal override Type GetLiteralType()
	//    {
	//        throw new NotImplementedException();
	//    }

	//    internal override Type GetSystemType()
	//    {
	//        return typeof(MModuleBuilder);
	//    }

	//    internal override void CodeGenMeta(CodeGenState state)
	//    {
	//        throw new NotImplementedException();
	//    }

	//}

	//[Serializable]
	//public partial class TypeType : MType
	//{
	//    public MType typeOf;

	//    protected TypeType(MType typeOf)
	//    {
	//        this.typeOf = typeOf;
	//    }

	//    public static TypeType Create(MType typeOf)
	//    {
	//        return new TypeType(typeOf);
	//    }

	//    protected override bool IsEqualToInternal(MType type)
	//    {
	//        TypeType that = (TypeType)type;
	//        return typeOf.IsEqualTo(that.typeOf);
	//    }

	//    public override MType GetSuperType()
	//    {
	//        return PrimType.Object;
	//    }

	//    protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
	//    {
	//        //TODO should have Create method
	//        //methods.Add(new NonGenericMethodInfo(new GetFieldsMethod(this)));
	//        //SystemType.Create(typeof(MType)).GetMethods(methods);
	//    }

	//    internal override Type GetLiteralType()
	//    {
	//        return typeof(LitType);
	//    }

	//    internal override Type GetSystemType()
	//    {
	//        return typeof(MType);
	//    }

	//    public override MType Subst(int sym, MType type)
	//    {
	//        MType substTypeOf = typeOf.Subst(sym, type);
	//        if (substTypeOf != typeOf) return TypeType.Create(substTypeOf);
	//        else return this;
	//    }

	//    public override MType Unpack(int index, TypeVar var)
	//    {
	//        MType unpackTypeOf = typeOf.Unpack(index, var);
	//        if (unpackTypeOf != typeOf) return TypeType.Create(unpackTypeOf);
	//        else return this;
	//    }

	//    internal override void CodeGenMeta(CodeGenState state)
	//    {
	//        typeOf.CodeGenMeta(state);
	//        state.MakeType(typeof(TypeType));
	//    }

	//    public class GetFieldsMethod : MetaphorMethod
	//    {
	//        public GetFieldsMethod(TypeType declaringType): base(declaringType, typeof(MType).GetMethod("GetInstanceFields"), ArrayType.Create(ExistsType.Create(PrimType.Object, false, FieldType.Create(declaringType.typeOf, ExistsType.Vars[0])), 1), ParamType.Empty)
	//        {
	//        }

	//        public override string GetName()
	//        {
	//            return "GetFields";
	//        }

	//    }

	//}

	//[Serializable]
	//public partial class FieldType : MType
	//{
	//    public MType declaringType;
	//    public MType fieldType;

	//    protected FieldType(MType declaringType, MType fieldType)
	//    {
	//        this.declaringType = declaringType;
	//        this.fieldType = fieldType;
	//    }

	//    public static FieldType Create(MType declaringType, MType fieldType)
	//    {
	//        return new FieldType(declaringType, fieldType);
	//    }

	//    protected override bool IsEqualToInternal(MType type)
	//    {
	//        FieldType that = (FieldType)type;
	//        return declaringType.IsEqualTo(that.declaringType) && fieldType.IsEqualTo(that.fieldType);
	//    }

	//    public override MType GetSuperType()
	//    {
	//        return PrimType.Object;
	//    }

	//    protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
	//    {
	//        //TODO should have Create method
	//        //methods.Add(new NonGenericMethodInfo(new GetFieldTypeMethod(this)));
	//    }

	//    internal override Type GetLiteralType()
	//    {
	//        throw new NotImplementedException();
	//    }

	//    internal override Type GetSystemType()
	//    {
	//        return typeof(MFieldInfo);
	//    }

	//    public override MType Subst(int sym, MType type)
	//    {
	//        MType substDeclaringType = declaringType.Subst(sym, type);
	//        MType substFieldType = fieldType.Subst(sym, type);
	//        if (substDeclaringType != declaringType || substFieldType != fieldType) return FieldType.Create(substDeclaringType, substFieldType);
	//        else return this;
	//    }

	//    public override MType Unpack(int index, TypeVar var)
	//    {
	//        MType unpackDeclaringType = declaringType.Unpack(index, var);
	//        MType unpackFieldType = fieldType.Unpack(index, var);
	//        if (unpackDeclaringType != declaringType || unpackFieldType != fieldType) return FieldType.Create(unpackDeclaringType, unpackFieldType);
	//        else return this;
	//    }

	//    internal override void CodeGenMeta(CodeGenState state)
	//    {
	//        declaringType.CodeGenMeta(state);
	//        fieldType.CodeGenMeta(state);
	//        state.MakeCode(typeof(FieldType));
	//    }

	//    public class GetFieldTypeMethod : MetaphorMethod
	//    {
	//        public GetFieldTypeMethod(FieldType fieldType):base(fieldType, typeof(MFieldInfo).GetMethod("GetFieldType"), TypeType.Create(fieldType.declaringType), ParamType.Empty)
	//        {
	//        }

	//    }

	//}

}