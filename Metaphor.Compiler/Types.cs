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
using IToken = antlr.IToken;

namespace Metaphor.Compiler
{
	public abstract class Typ : Node
	{
		public MType mType;

	    protected Typ(IToken token)
			: base(token)
		{
		}

		public MType CompileFor(CompileState state)
		{
			MType mType = Compile(state);
			if (mType.GetLevelKind() > state.level) throw state.ThrowTypeError(this, "to do: write error message");
			return mType;
		}

		public MType CompileFor(CompileState state, int offset)
		{
			MType mType = Compile(state);
			if (mType.GetLevelKind() > state.level + offset) throw state.ThrowTypeError(this, "to do: write error message");
			return mType;
		}

		public abstract MType Compile(CompileState state);

		public static MType[] Compile(CompileState state, List<Typ> types)
		{
			int num = types.Count;
			MType[] mTypes = new MType[num];
			for (int i = 0; i < num; i++)
			{
				mTypes[i] = types[i].Compile(state);
				if (mTypes[i] == M.PrimType.Void) throw state.ThrowTypeError(types[i], "Cannot use 'void' as a type argument.");
			}
			return mTypes;
		}
	}

	public class PrimType : Typ
	{
		public PrimType(IToken token, M.PrimType primType)
			: base(token)
		{
			this.mType = primType;
		}

		public override MType Compile(CompileState state)
		{
			return mType;
		}

		public static PrimType CreateVoid(IToken token)
		{
			return new PrimType(token, M.PrimType.Void);
		}

		public static PrimType CreateInt(IToken token)
		{
			return new PrimType(token, M.PrimType.Int);
		}

		public static PrimType CreateLong(IToken token)
		{
			return new PrimType(token, M.PrimType.Long);
		}

		public static PrimType CreateFloat(IToken token)
		{
			return new PrimType(token, M.PrimType.Float);
		}

		public static PrimType CreateBool(IToken token)
		{
			return new PrimType(token, M.PrimType.Boolean);
		}

		public static PrimType CreateString(IToken token)
		{
			return new PrimType(token, M.PrimType.String);
		}

		public static PrimType CreateChar(IToken token)
		{
			return new PrimType(token, M.PrimType.Char);
		}

		public static PrimType CreateObject(IToken token)
		{
			return new PrimType(token, M.PrimType.Object);
		}

		public static PrimType CreateDecimal(IToken token)
		{
			return new PrimType(token, M.PrimType.Decimal);
		}

		public static PrimType CreateByte(IToken token)
		{
			return new PrimType(token, M.PrimType.Byte);
		}

		public static PrimType CreateShort(IToken token)
		{
			return new PrimType(token, M.PrimType.Short);
		}

		public static PrimType CreateDouble(IToken token)
		{
			return new PrimType(token, M.PrimType.Double);
		}
	}

	public class ClassType : Typ
	{
		public ClassType parent;
		public string name;
		public List<Typ> args;

		public ClassType(ClassType parent, Ident name, List<Typ> args)
			: base(parent != null ? parent.Token : name.Token)
		{
            Contract.Requires(name != null);
            this.parent = parent;
			this.name = name.Name;
			this.args = CheckNull<Typ>(args);
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(name != null);
	    }

	    public string CompileNamespace(CompileState state)
		{
			if (args.Count == 0)
			{
				if (parent == null)
				{
					if (state.NamespaceStartsWith(name)) return name;
				}
				else
				{
					string ns = parent.CompileNamespace(state);
					if (ns != null)
					{
						ns = string.Format("{0}.{1}", ns, name);
						if (state.NamespaceStartsWith(ns)) return ns;
					}
				}
			}
			return null;
		}

		public override MType Compile(CompileState state)
		{
			MType[] mArgs = Typ.Compile(state, args);

			if (parent == null)
			{
				mType = state.LookupType(null, name, mArgs);
				if (mType == null) throw state.ThrowTypeError(this, "Could not find a type named '{0}' with arity '{1}'.", name, args.Count);
			}
			else
			{
				string ns = parent.CompileNamespace(state);
				if (ns != null)
				{
					mType = state.LookupType(ns, name, mArgs);
					if (mType == null) throw state.ThrowTypeError(this, "Could not find a type named '{0}' with arity '{1}' in namespace '{2}'.", name, args.Count, ns);
				}
				else
				{
					MType parentMType = parent.Compile(state);
					mType = parentMType.GetNestedType(name, mArgs);
					if (mType == null) throw state.ThrowTypeError(this, "Could not find a type named '{0}' with arity '{1}' as a nested type of '{2}'.", name, args.Count, parent.mType);
				}
			}
			return mType;
		}
	}

	public class ArrayType : Typ
	{
		public Typ type;
		public int rank;

		public static ArrayType Create(Typ type, List<int> ranks)
		{
			Contract.Requires(type != null);
			Contract.Requires(ranks != null);
			Contract.Requires(ranks.Count != 0, nameof(ranks));
			
			ArrayType arrayType = new ArrayType(type, ranks[ranks.Count - 1]);
			for (int i = ranks.Count - 2; i >= 0; i++)
				arrayType = new ArrayType(arrayType, ranks[i]);
			return arrayType;
		}

		public ArrayType(Typ type, int rank)
			: base(type.Token)
		{
			Contract.Requires(type != null);
			Contract.Requires(rank > 0, nameof(rank));
			this.type = type;
			this.rank = rank;
		}

		public override MType Compile(CompileState state)
		{
			mType = type.Compile(state);
			mType = M.ArrayType.Create(mType, rank);
			return mType;
		}
	}
}