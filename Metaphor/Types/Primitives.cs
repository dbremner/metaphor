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
using System.Runtime.Serialization;

namespace Metaphor
{
	public class NullType : MType
	{
		public static NullType Null = new NullType();

		protected NullType()
		{
		}

		public override string GetName()
		{
			return "<null>";
		}

		public override MType GetSuperType()
		{
			return null;
		}

		protected override bool IsEqualToInternal(MType type)
		{
			return true;
		}

		public override bool IsSubTypeOf(MType type2)
		{
			return !type2.IsValueType();
		}

		internal override void MakeLiteral(CodeGenState state)
		{
			state.MakeCode(typeof(LitNull));
		}

		public override Type GetSystemType()
		{
			throw new Exception("Should never be called.");
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldsfld, typeof(NullType).GetField("Null", BindingFlags.Public | BindingFlags.Static));
		}

	}

	[Serializable]
	public partial class PrimType : SystemType
	{
		public CILType cilType;
		public Type literalType;
		public FieldInfo field;

		protected PrimType(Type systemType, CILType cilType, Type literalType, FieldInfo field) : base(systemType)
		{
			this.cilType = cilType;
			this.literalType = literalType;
			this.field = field;
		}

		public override bool IsGeneric()
		{
			return false;
		}

		internal override CILType GetCILType()
		{
			return cilType;
		}

		internal override void MakeLiteral(CodeGenState state)
		{
			state.MakeCode(literalType);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldsfld, field);
		}

		protected class VoidType : PrimType
		{
			public VoidType() : base(typeof(void), CILType.Void, null, typeof(PrimType).GetField("Void", BindingFlags.Public | BindingFlags.Static))
			{
			}

			public override MType GetSuperType()
			{
				return null;
			}

		}

		public static PrimType Void = new VoidType();
		public static PrimType Object = new PrimType(typeof(object), CILType.Ref, typeof(LitObj), typeof(PrimType).GetField("Object", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Boolean = new PrimType(typeof(bool), CILType.I4, typeof(LitBool), typeof(PrimType).GetField("Boolean", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Int = new PrimType(typeof(int), CILType.I4, typeof(LitInt), typeof(PrimType).GetField("Int", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Long = new PrimType(typeof(long), CILType.I8, typeof(LitLong), typeof(PrimType).GetField("Long", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Float = new PrimType(typeof(float), CILType.R4, typeof(LitFloat), typeof(PrimType).GetField("Float", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Double = new PrimType(typeof(double), CILType.R8, typeof(LitDouble), typeof(PrimType).GetField("Double", BindingFlags.Public | BindingFlags.Static));
		public static PrimType String = new PrimType(typeof(string), CILType.Ref, typeof(LitString), typeof(PrimType).GetField("String", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Char = new PrimType(typeof(char), CILType.I4, typeof(LitChar), typeof(PrimType).GetField("Char", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Array = new PrimType(typeof(Array), CILType.Ref, typeof(LitObj), typeof(PrimType).GetField("Array", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Delegate = new PrimType(typeof(MulticastDelegate), CILType.Ref, typeof(LitObj), typeof(PrimType).GetField("Delegate", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Native = new PrimType(typeof(IntPtr), CILType.I4, typeof(LitInt), typeof(PrimType).GetField("Native", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Value = new PrimType(typeof(ValueType), CILType.Void, typeof(LitObj), typeof(PrimType).GetField("Value", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Byte = new PrimType(typeof(byte), CILType.U1, typeof(LitByte), typeof(PrimType).GetField("Byte", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Short = new PrimType(typeof(short), CILType.I2, typeof(LitShort), typeof(PrimType).GetField("Short", BindingFlags.Public | BindingFlags.Static));
		public static PrimType Decimal = new PrimType(typeof(decimal), CILType.ValueType, typeof(LitDecimal), typeof(PrimType).GetField("Decimal", BindingFlags.Public | BindingFlags.Static));
	}

}