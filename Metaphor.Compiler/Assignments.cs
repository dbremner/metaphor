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
using Metaphor.Collections;
using M = Metaphor;

namespace Metaphor.Compiler
{
	public abstract class Assignment : Expr
	{
		public Expr lhs;
		public Expr rhs;
		public AssignOp op;

	    protected Assignment(Expr lhs, Expr rhs, AssignOp op)
			: base(lhs.Token)
		{
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
			Contract.Requires(Enum.IsDefined(typeof (AssignOp), op));
			this.lhs = lhs;
			this.rhs = rhs;
			this.op = op;
		}

		public override Kind Compile(CompileState state)
		{
			Code mLhs = lhs.CompileLValue(state);
			MType mType = mLhs.GetMType();
			state.PushExpectedType(mType);
			Code mRhs = rhs.CompileRValue(state);
			state.PopExpectedType();
			if (!Code.MakeCompatible(ref mRhs, mType))
				throw state.ThrowTypeError(this, "Left-hand side of assignment has type '{0}' which is not compatible with the right-hand side's type '{1}'.", mType, mRhs.GetMType());
			return Value.FromCode(new M.Assign(mLhs, op, mRhs));
		}
	}

	public class Assign : Assignment
	{
		public Assign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Nop)
		{
		}
	}

	public class AddAssign : Assignment
	{
		public AddAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Add)
		{
		}
	}

	public class SubtractAssign : Assignment
	{
		public SubtractAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Sub)
		{
		}
	}

	public class MultiplyAssign : Assignment
	{
		public MultiplyAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Mul)
		{
		}
	}

	public class DivideAssign : Assignment
	{
		public DivideAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Div)
		{
		}
	}

	public class RemainderAssign : Assignment
	{
		public RemainderAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Mod)
		{
		}
	}

	public class AndAssign : Assignment
	{
		public AndAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.And)
		{
		}
	}
	
	public class XorAssign : Assignment
	{
		public XorAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Xor)
		{
		}
	}
	
	public class OrAssign : Assignment
	{
		public OrAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Or)
		{
		}
	}
	
	public class ShiftLeftAssign : Assignment
	{
		public ShiftLeftAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Shl)
		{
		}
	}

	public class ShiftRightAssign : Assignment
	{
		public ShiftRightAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Shr)
		{
		}
	}

	public class SemiAssign : Assignment
	{
		public SemiAssign(Expr lhs, Expr rhs)
			: base(lhs, rhs, AssignOp.Semi)
		{
		}
	}
}