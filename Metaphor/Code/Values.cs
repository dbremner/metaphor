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
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Metaphor.Collections;

namespace Metaphor
{
	[Serializable]
	public abstract class Value: Code
	{
        private readonly MType type;

	    protected Value(MType type)
		{
            this.type = type;
		}

        public override MType GetMType()
        {
            return type;
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
		}
	}

	[Serializable]
	public partial class LitByte : Value
	{
		public byte val;

		public LitByte(byte val)
			: base(PrimType.Byte)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitInt(val);
			state.code.Emit(OpCodes.Conv_U1);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt(val);
			state.code.Emit(OpCodes.Conv_U1);
			state.MakeCode(typeof(LitByte));
		}
	}

	[Serializable]
	public partial class LitShort : Value
	{
		public short val;

		public LitShort(short val)
			: base(PrimType.Short)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitInt(val);
			state.code.Emit(OpCodes.Conv_I2);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt(val);
			state.code.Emit(OpCodes.Conv_I2);
			state.MakeCode(typeof(LitShort));
		}
	}

	[Serializable]
	public partial class LitInt: Value
	{
		public int val;

		public LitInt(int val) : base(PrimType.Int)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitInt(val);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt(val);
			state.MakeCode(typeof(LitInt));
		}
	}

	[Serializable]
	public partial class LitLong: Value
	{
		public long val;

		public LitLong(long val) : base(PrimType.Long)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldc_I8, val);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldc_I8, val);
			state.MakeCode(typeof(LitLong));
		}

	}

	[Serializable]
	public partial class LitFloat : Value
	{
		public float val;

		public LitFloat(float val)
			: base(PrimType.Float)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldc_R4, val);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldc_R4, val);
			state.MakeCode(typeof(LitFloat));
		}
	}

	[Serializable]
	public partial class LitDouble : Value
	{
		public double val;

		public LitDouble(double val) : base(PrimType.Double)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldc_R8, val);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.code.Emit(OpCodes.Ldc_R8, val);
			state.MakeCode(typeof(LitDouble));
		}
	}

	[Serializable]
	public partial class LitDecimal : Value
	{
		public decimal val;

		public LitDecimal(decimal val)
			: base(PrimType.Decimal)
		{
			this.val = val;
		}

		private static readonly FieldInfo one = typeof(decimal).GetField("One", BindingFlags.Public | BindingFlags.Static);
		private static readonly FieldInfo zero = typeof(decimal).GetField("Zero", BindingFlags.Public | BindingFlags.Static);
		private static readonly FieldInfo minusOne = typeof(decimal).GetField("MinusOne", BindingFlags.Public | BindingFlags.Static);
		private static readonly ConstructorInfo ctor = typeof(decimal).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) });

		internal override void CodeGen(CodeGenState state)
		{
			if (val == 1.0m) state.code.Emit(OpCodes.Ldsfld, one);
			else if (val == 0.0m) state.code.Emit(OpCodes.Ldsfld, zero);
			else if (val == -1.0m) state.code.Emit(OpCodes.Ldsfld, minusOne);
			else
			{
				int[] bits = decimal.GetBits(val);
				state.EmitInt(bits[0]);
				state.EmitInt(bits[1]);
				state.EmitInt(bits[2]);
				state.EmitBool(bits[3] > 0);
				state.EmitInt(bits[3] >> 16 & 0xFF);
				state.code.Emit(OpCodes.Conv_U1);
				state.code.Emit(OpCodes.Newobj, ctor);
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			CodeGen(state);
			state.MakeCode(typeof(LitDecimal));
		}
	}

	[Serializable]
	public partial class LitBool: Value
	{
		public bool val;

		public LitBool(bool val) : base(PrimType.Boolean)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitBool(val);
		}

		internal override void CodeGenBranch(CodeGenState state, bool when, Label where)
		{
			if (val == when) state.code.Emit(OpCodes.Br, where);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitBool(val);
			state.MakeCode(typeof(LitBool));
		}

	}

	[Serializable]
	public partial class LitString: Value
	{
		public string val;

		public LitString(string val) : base(PrimType.String)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitString(val);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitString(val);
			state.MakeCode(typeof(LitString));
		}
	}

	[Serializable]
	public partial class LitChar : Value
	{
		public char val;

		public LitChar(char val) : base(PrimType.Char)
		{
			this.val = val;
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitInt(val);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt(val);
			state.MakeCode(typeof(LitChar));
		}

	}

	[Serializable]
	public partial class LitNull: Value
	{
		public LitNull() : base(NullType.Null)
		{
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitNull();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.MakeCode(typeof(LitNull));
		}
	}

	[Serializable]
	public partial class LitObj: Value
	{
		internal object obj;

		public LitObj(object obj) : base(SystemType.Create(obj.GetType()))
		{
			this.obj = obj;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			if(state.GetLevel() == 0)
				state.AddCspObject(obj);
		}

		internal override void CodeGen(CodeGenState state)
		{
			if (obj != null) state.EmitCspObject(obj);
			else state.EmitNull();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}

		public static Code FromObject(object obj)
		{
			if (obj == null) return new LitNull();
			else return new LitObj(obj);
		}

		public static Code MakeLiteral<T>(T tObj)
		{
			Type type = typeof(T);
			object obj = (object)tObj;
			if(obj == null) return new LitNull();
			else if (type == typeof(bool)) return new LitBool((bool)obj);
			else if (type == typeof(char)) return new LitChar((char)obj);
			else if (type == typeof(double)) return new LitDouble((double)obj);
			else if (type == typeof(float)) return new LitFloat((float)obj);
			else if (type == typeof(int)) return new LitInt((int)obj);
			else if (type == typeof(long)) return new LitLong((long)obj);
			else if (type == typeof(string)) return new LitString((string)obj);
			else return new LitObj(obj);
		}
	}

	[Serializable]
	public partial class LitCode : Value
	{
		public Code code;

		public LitCode(Code code) : base(CodeType.Create(code.GetMType(), false))
		{
			this.code = code;
		}

		internal override void CodeGen(CodeGenState state)
		{
			throw new NotImplementedException();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}

	}

	[Serializable]
	public partial class LitType: Value
	{
		public MType typeType;

		public LitType(MType typeType) : base(SystemType.Create(typeof(Type)))
		{
			this.typeType = typeType;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			typeType.GenerateClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			typeType.CodeGenSystemType(state);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			typeType.CodeGenMeta(state);
			state.MakeCode(typeof(LitType));
		}

	}

//	public partial class LitField: Value
//	{
//		public MFieldInfo field;
//
//		public LitField(MFieldInfo field)
//		{
//			this.field = field;
//		}
//
//		public override void CodeGen(CodeGenState state)
//		{
//			state.MakeField(field.GetSystemField());
//		}
//
//		public override void CodeGenMeta(CodeGenState state)
//		{
//			field.CodeGenMeta(state);
//			state.MakeCode(typeof(LitField));
//		}
//	}
//
//	public partial class LitConstructor: Value
//	{
//		public MConstructorInfo ctor;
//
//		public LitConstructor(MConstructorInfo ctor)
//		{
//			this.ctor = ctor;
//		}
//
//		public override void CodeGen(CodeGenState state)
//		{
//			throw new NotImplementedException();
////			state.MakeConstructor(ctor.GetSystemConstructor());
//		}
//
//		public override void CodeGenMeta(CodeGenState state)
//		{
//			ctor.CodeGenMeta(state);
//			state.MakeCode(typeof(LitConstructor));
//		}
//	}

}


