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
using M = Metaphor;
using IToken = antlr.IToken;

namespace Metaphor.Compiler
{
	public abstract class Literal : Expr
	{
		public Literal(IToken token)
			: base(token)
		{
		}
	}

	public class LitBool : Literal
	{
		public bool val;

		public LitBool(IToken token, bool val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			return new RValue(new M.LitBool(val));
		}
	}

	public class LitInt : Literal
	{
		public int val;

		public LitInt(IToken token, int val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			return new RValue(new M.LitInt(val));
		}
	}

	public class LitLong: Literal
	{
		public long val;

		public LitLong(IToken token, long val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			return new RValue(new M.LitLong(val));
		}
	}

	public class LitFloat : Literal
	{
		public float val;

		public LitFloat(IToken token, float val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class LitDouble : Literal
	{
		public double val;

		public LitDouble(IToken token, double val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			return new RValue(new M.LitDouble(val));
		}
	}

	public class LitDecimal : Literal
	{
		public decimal val;

		public LitDecimal(IToken token, decimal val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class LitChar : Literal
	{
		public char val;

		public LitChar(IToken token, char val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			return new RValue(new M.LitChar(val));
		}
	}

	public class LitString : Literal
	{
		public string val;

		public LitString(IToken token, string val)
			: base(token)
		{
			this.val = val;
		}

		public override Kind Compile(CompileState state)
		{
			return new RValue(new M.LitString(val));
		}
	}

	public class LitNull : Literal
	{
		public LitNull(IToken token)
			: base(token)
		{
		}

		public override Kind Compile(CompileState state)
		{
			return new RValue(new M.LitNull());
		}
	}
}