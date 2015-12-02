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
	[Serializable]
	public partial class ArrayType : MType
	{
		public MType elementType;
		public int rank;

		public ArrayType(MType elementType, int rank)
		{
			if (elementType == null) throw new ArgumentNullException("elementType");
			this.elementType = elementType;
			if (rank <= 0) throw new ArgumentOutOfRangeException("rank", "cannot be non-positive");
			this.rank = rank;
		}

		public static MType Create(MType elementType, int rank)
		{
			if (!(elementType is SystemType)) return new ArrayType(elementType, rank);
			else return rank == 1 ? SystemType.Create(elementType.GetSystemType().MakeArrayType()) : SystemType.Create(elementType.GetSystemType().MakeArrayType(rank));
		}

		public override int GetLevelKind()
		{
			return elementType.GetLevelKind();
		}

		public override MType[] GetTypeArguments()
		{
			return new MType[] { elementType };
		}

		public override MType ReplaceTypeArguments(MType[] newTypeArgs)
		{
			return Create(newTypeArgs[0], rank);
		}

		public override bool IsArray()
		{
			return true;
		}

		public override MType GetElementType()
		{
			return elementType;
		}

		public override int GetArrayRank()
		{
			return rank;
		}

		public override bool IsGeneric()
		{
			return false;
		}

		public override bool IsValueType()
		{
			return false;
		}

		protected override bool IsEqualToInternal(MType type)
		{
			ArrayType that = (ArrayType)type;
			return rank == that.rank && elementType.IsEqualTo(that.elementType);
		}

		public override MType GetSuperType()
		{
//			MType superElementType = elementType.GetSuperType();
//			//if(superElementType != null) return TypeFactory.Current.GetArrayType(superElementType, rank);
//			if (superElementType != null) return new ArrayType(superElementType, rank);
//			else return PrimType.Array;
			return PrimType.Array;
		}

		public override MType[] GetInterfaces()
		{
			return new MType[] { GenericSystemType.Create(typeof(IEnumerable<>), new MType[] { elementType }) };
		}

//		public override MFieldInfo GetField(string name, bool isStatic)
//		{
//			if(name == "Length" && !isStatic)
//			{
//				return new ArrayLength();
//			}
//			else
//				return base.GetField(name, isStatic);
//		}
//
//		public override MConstructorInfo[] GetConstructors()
//		{
//			return new MConstructorInfo[] { new ArrayConstructor(rank) };
//		}


		protected internal override void GetFields(List<MFieldInfo> fields)
		{
			fields.Add(Length.Create());
			base.GetFields(fields);
		}

		protected internal override void GetConstructors(List<MConstructorInfo> ctors)
		{
			ctors.Add(ArrayConstructor.Create(rank));
			base.GetConstructors(ctors);
		}

		public override bool IsSubTypeOf(MType type2)
		{
			ArrayType arrayType = type2 as ArrayType;
			if (arrayType != null) return elementType.IsSubTypeOf(arrayType.elementType);
			else return base.IsSubTypeOf(type2);
		}

		public override Type GetSystemType()
		{
			return rank == 1 ? elementType.GetSystemType().MakeArrayType() : elementType.GetSystemType().MakeArrayType(rank);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			elementType.CodeGenMeta(state);
			state.EmitInt(rank);
			state.MakeType(typeof(ArrayType));
		}

//        public override MType Subst(int sym, MType type)
//        {
//            MType substElemType = elementType.Subst(sym, type);
////			return TypeFactory.Current.GetArrayType(substElemType, rank);
//            if (substElemType != elementType) return ArrayType.Create(substElemType, rank);
//            else return this;
//        }

		public partial class ArrayConstructor : MConstructorInfo
		{
			public int rank;

			protected ArrayConstructor(int rank)
			{
				this.rank = rank;
			}

			public static ArrayConstructor Create(int rank)
			{
				return new ArrayConstructor(rank);
			}

			public override MType GetDeclaringType()
			{
				throw new NotImplementedException();
			}

			public override ParamType[] GetParamTypes()
			{
				ParamType[] paramTypes = new ParamType[rank];
				for (int i = 0; i < rank; i++)
					paramTypes[i] = ParamType.Create(PrimType.Int, false);
				return paramTypes;
			}

			internal override ConstructorInfo GetSystemConstructor()
			{
				throw new Exception("Should never be called.");
			}

		}

		public class Length : MFieldInfo
		{
			protected Length()
			{
			}

			public static Length Create()
			{
				return new Length();
			}

			public override string GetName()
			{
				return "Length";
			}

			public override bool IsStatic()
			{
				return false;
			}

			public override MType GetDeclaringType()
			{
				throw new NotImplementedException();
			}

			public override MType GetFieldType()
			{
				return PrimType.Int;
			}

			internal override void CodeGen(CodeGenState state)
			{
				state.code.Emit(OpCodes.Ldlen);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				state.MakeType(typeof(Length));
			}
		}
	}
}