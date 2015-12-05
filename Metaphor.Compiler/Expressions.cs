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
	public abstract class Expr : Node
	{
		//public M.Code mExpr;

	    protected Expr(IToken token)
			: base(token)
		{
		}

		public abstract Kind Compile(CompileState state);

		public Code CompileNoValue(CompileState state)
		{
			Kind kind = Compile(state);
			NoValue noValue = kind as NoValue;
			if (noValue != null) return noValue.GetNoValue();
			else throw state.ThrowTypeError(this, "A value expression was expected but a '{0}' was found.", kind);
		}

		public Code CompileRValue(CompileState state)
		{
			Kind kind = Compile(state);
			RValue rValue = kind as RValue;
			if (rValue != null) return rValue.GetRValue();
			else throw state.ThrowTypeError(this, "An r-value expression was expected but a '{0}' was found.", kind);
		}

		public Code CompileLValue(CompileState state)
		{
			Kind kind = Compile(state);
			LValue lValue = kind as LValue;
			if (lValue != null) return lValue.GetLValue();
			else throw state.ThrowTypeError(this, "An l-value expression was expected but a '{0}' was found.", kind);
		}

		public Code CompileAnyValue(CompileState state)
		{
			Kind kind = Compile(state);
			RValue rValue = kind as RValue;
			if (rValue != null) return rValue.GetRValue();
			else
			{
				NoValue noValue = kind as NoValue;
				if (noValue != null) return noValue.GetNoValue();
				else throw state.ThrowTypeError(this, "A value expression was expected but a '{0}' was found.", kind);
			}
		}

		public static Code[] CompileRValues(CompileState state, List<Expr> exprs)
		{
			int num = exprs.Count;
			Code[] mCode = new Code[num];
			for (int i = 0; i < num; i++) mCode[i] = exprs[i].CompileRValue(state);
			return mCode;
		}

		//public static M.Code[] Map(List<Expr> exprs)
		//{
		//    int num = exprs.Count;
		//    M.Code[] mExprs = new M.Code[num];
		//    for (int i = 0; i < num; i++) mExprs[i] = exprs[i].mExpr;
		//    return mExprs;
		//}

		//		public static M.MType[] MapType(Expr[] exprs)
		//		{
		//			int num = exprs.Length;
		//			M.MType[] mTypes = new M.MType[num];
		//			for(int i = 0; i < num; i++) mTypes[i] = exprs[i].mType;
		//			return mTypes;
		//		}

		public abstract class Kind
		{
		}

		public abstract class Value : Kind
		{
			public abstract MType GetMType();

			public static Value FromCode(Code code)
			{
				Contract.Requires(code != null);
				bool isVoid = code.GetMType() == M.PrimType.Void;
				bool isAssignable = code.IsAssignable();
				if (isVoid && !isAssignable) return new NoValue(code);
				else if (!isVoid && !isAssignable) return new RValue(code);
				else if (!isVoid && isAssignable) return new LValue(code);
				else throw new InvalidOperationException("a void value cannot be assignable");
			}

			public static MType[] GetMTypes<T>(ICollection<T> values) where T : Value
			{
				MType[] mTypes = new MType[values.Count];
				int index = 0;
				foreach (Value value in values) mTypes[index++] = value.GetMType();
				return mTypes;
			}
		}

		public class NoValue : Value
		{
			protected Code code;

			public NoValue(Code code)
			{
				this.code = code;
			}

			public override MType GetMType()
			{
				return code.GetMType();
			}

			public virtual Code GetNoValue()
			{
				return code;
			}

			public override string ToString()
			{
				return "void";
			}
		}

		public class RValue : NoValue
		{
			public RValue(Code code)
				: base(code)
			{
			}

			public override Code GetNoValue()
			{
				M.Assign assignCode =  code as M.Assign;
				if (assignCode != null)
				{
					assignCode.ret = AssignRet.None;
					return assignCode;
				}
				else return new Pop(base.GetNoValue());
			}

			public virtual Code GetRValue()
			{
				return base.GetNoValue();
			}

			public override string ToString()
			{
				return "r-value";
			}
		}

		public class LValue : RValue
		{
			protected Code codeRef;

			public LValue(Code codeRef)
				: base(codeRef)
			{
				this.codeRef = codeRef;
			}

			public virtual Code GetLValue()
			{
				return codeRef;
			}

			public override string ToString()
			{
				return "l-value";
			}
		}

		public class Namespace : Kind
		{
			public string @ns;

			public Namespace(string ns)
			{
				this.ns = ns;
			}

			public override string ToString()
			{
				return "namespace";
			}
		}

		public class Type : Kind
		{
			public M.MType mType;

			public Type(M.MType mType)
			{
				this.mType = mType;
			}

			public override string ToString()
			{
				return "type";
			}
		}

		public class MethodGroup : Kind
		{
			public string name;
			public M.Code expr;
			public M.MMethodInfo[] methods;

			public MethodGroup(string name, M.Code expr, M.MMethodInfo[] methods)
			{
			    Contract.Requires(name != null);
			    Contract.Requires(methods != null && methods.Length != 0);
			    this.name = name;
				this.expr = expr;
				this.methods = methods;
			}

		    [ContractInvariantMethod]
		    private void ObjectInvariant()
		    {
		        Contract.Invariant(name != null);
		        Contract.Invariant(methods != null);
                Contract.Invariant(methods.Length != 0);
		    }

		    public override string ToString()
			{
				return "method group";
			}
		}

		public class Function : Kind
		{
			public M.MType[] paramTypes;

			public Function(M.MType[] paramTypes)
			{
				this.paramTypes = paramTypes;
			}

			public override string ToString()
			{
				return "anonymous function";
			}
		}
	}

	[Flags]
	public enum Dir { In = 1, Out = 2, InOut = 3 }

	public class Arg : Node
	{
		public Expr expr;
		public Dir dir;
		public M.Code mArg;

		public Arg(Dir dir, Expr expr)
			: base(expr.Token)
		{
			Contract.Requires(expr != null);
			if (!Enum.IsDefined(typeof(Dir), dir)) throw new ArgumentOutOfRangeException(nameof(dir));
			this.dir = dir;
			this.expr = expr;
		}

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(expr != null);
        }


        public M.Code Compile(CompileState state)
		{
			if ((dir & Dir.Out) != 0) return new Ref(expr.CompileLValue(state));
			else return expr.CompileRValue(state);
		}

		public static M.Code[] Compile(CompileState state, List<Arg> args)
		{
			int num = args.Count;
			M.Code[] mArgs = new M.Code[num];
			for (int i = 0; i < num; i++) mArgs[i] = args[i].Compile(state);
			return mArgs;
		}
	}

	public class Parentheses : Expr
	{
		public Expr expr;

		public Parentheses(IToken token, Expr expr)
			: base(token)
		{
			this.expr = expr;
		}

		public override Kind Compile(CompileState state)
		{
			return expr.Compile(state);
		}
	}

	public class TypeOf : Expr
	{
		public Typ type;

		public TypeOf(IToken token, Typ type)
			: base(token)
		{
			this.type = type;
		}

		public override Kind Compile(CompileState state)
		{
			MType mType = type.CompileFor(state, 1);
			return Value.FromCode(new LitType(mType));
		}
	}

	public class DefaultOf : Expr
	{
		public Typ type;

		public DefaultOf(IToken token, Typ type)
			: base(token)
		{
			this.type = type;
		}

		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class FieldOf : Expr
	{
		public MemberAccess field;

		public FieldOf(IToken token, MemberAccess field)
			: base(token)
		{
			this.field = field;
		}

		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class MethodOf : Expr
	{
		public MemberAccess method;

		public MethodOf(IToken token, MemberAccess method)
			: base(token)
		{
			this.method = method;
		}

		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class Lift : Expr
	{
		public Expr expr;

		public Lift(IToken token, Expr expr)
			: base(token)
		{
		    Contract.Requires(expr != null);
		    this.expr = expr;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(expr != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			if (state.GetLevel() <= 0) throw state.ThrowTypeError(this, "Lift cannot occur at level 0.");
			state.LowerLevel();
			Code mExpr = expr.CompileRValue(state);
			state.RaiseLevel();
			return Value.FromCode(new M.Lift(mExpr));
		}
	}

	public class BaseMember : Expr
	{
		public BaseMember(IToken token, Ident name, List<Typ> typeArgs)
			: base(token)
		{
		}
		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class BaseIndexer : Expr
	{
		public BaseIndexer(IToken token, List<Expr> args)
			: base(token)
		{
		}
		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class This : Expr
	{
		public This(IToken token)
			: base(token)
		{
		}

		public override Kind Compile(CompileState state)
		{
			Code mThis = state.LookupThisVar();
			if (mThis != null) return Value.FromCode(mThis);
			else throw state.ThrowTypeError(this, "The 'this' parameter is not in scope.");
		}

	}

	public class Name : Expr
	{
		public string name;
		public List<Typ> typeArgs;

		public Name(Ident name, List<Typ> typeArgs)
			: base(name.Token)
		{
			Contract.Requires(name != null);
			this.name = name.Name;
			this.typeArgs = CheckNull<Typ>(typeArgs);
		}

		public override Kind Compile(CompileState state)
		{
			MType[] mTypeArgs = Typ.Compile(state, typeArgs);

			// is this identifier a local, param, or closure variable
			if (typeArgs.Count == 0)
			{
				Code mVar = state.LookupVar(name);
				if (mVar != null) return Value.FromCode(mVar);
			}

			// is identifier a member on this or the current class
			Code mThis = state.LookupThisVar();
			MType mType;
			bool notStatic = mThis != null;
			if (typeArgs.Count == 0)
			{
				MFieldInfo field = null;
				mType = state.GetCurrentClassNoLifting();
				if(notStatic) field = mType.GetField(name, false);
				if(field == null) field = mType.GetField(name, true);
				if (field != null)
				{
					int level = 0;
					M.Lift lift = mThis as M.Lift;
					while (lift != null) { level++; mThis = lift.expr; lift = mThis as M.Lift; }
					Code mCode = new FieldAccess(mThis, field);
					while (level > 0) { mCode = new M.Lift(mCode); level--; }
					return Value.FromCode(mCode);
				}
			}

			MMethodInfo[] methods;
			mType = state.GetCurrentClass();
			if(notStatic) methods = mType.GetMethods(name, mTypeArgs);
			else methods = mType.GetMethods(name, true, mTypeArgs);
			if (methods != null) return new MethodGroup(name, mThis, methods);

			MType nestedType = mType.GetNestedType(name, mTypeArgs);
			if (nestedType != null) return new Type(nestedType);

			// is identifier a type
			MType type = state.LookupType(null, name, mTypeArgs);
			if (type != null) return new Type(type);

			// is identifier a namespace
			if (typeArgs.Count == 0)
			{
				if (state.NamespaceStartsWith(name)) return new Namespace(name);
			}

			throw state.ThrowTypeError(this, "The identifier '{0}' could not be found.", name);
		}
	}

	public abstract class Increment : Expr
	{
		public Expr expr;
		public AssignOp op;
		public AssignRet ret;

	    protected Increment(Expr expr, AssignOp op, AssignRet ret)
			: base(expr.Token)
		{
			Contract.Requires(expr != null);
			this.expr = expr;
			this.op = op;
			this.ret = ret;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(expr != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			Code mExpr = expr.CompileLValue(state);
			if (!BinaryOp.IsNumeric(mExpr.GetMType())) throw state.ThrowTypeError(this, "The operand of increment must be a numeric type but was found to have type '{1}'.", mExpr.GetMType());
			M.Assign mAssign = new M.Assign(mExpr, op, new M.LitInt(1));
			mAssign.ret = ret;
			return Value.FromCode(mAssign);
		}
	}

	public class PostIncrement : Increment
	{
		public PostIncrement(Expr expr)
			: base(expr, AssignOp.Add, AssignRet.Post)
		{
		}
	}

	public class PostDecrement : Increment
	{
		public PostDecrement(Expr expr)
			: base(expr, AssignOp.Sub, AssignRet.Post)
		{
		}
	}

	public abstract class UnaryOp : Expr
	{
		public UnaryOpCode op;
		public Expr a;

	    protected UnaryOp(IToken token, Expr x)
			: base(token)
		{
		    Contract.Requires(x != null);
		    this.a = x;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(a != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			Code mExpr = a.CompileRValue(state);
			return Value.FromCode(CompilerOp(state, mExpr));
		}

		protected abstract Code CompilerOp(CompileState state, Code mExpr);
	}

	#region Unary ops
	public class Plus : UnaryOp
	{
		public Plus(IToken token, Expr x)
			: base(token, x)
		{
		}

		protected override Code CompilerOp(CompileState state, Code mExpr)
		{
			if (BinaryOp.IsNumeric(mExpr.GetMType()))
			{
				return mExpr;
			}
			else throw state.ThrowTypeError(this, "The operand of '+' must have a numeric type but was found to have type '{0}'.", mExpr.GetMType());
		}
	}

	public class Negate : UnaryOp
	{
		public Negate(IToken token, Expr x)
			: base(token, x)
		{
		}

		protected override Code CompilerOp(CompileState state, Code mExpr)
		{
			M.LitInt mLitInt = mExpr as M.LitInt;
			if (mLitInt != null)
			{
				return new M.LitInt(-mLitInt.val);
			}
			M.LitLong mLitLong = mExpr as M.LitLong;
			if (mLitLong != null)
			{
				return new M.LitLong(-mLitLong.val);
			}
			M.LitFloat mLitFloat = mExpr as M.LitFloat;
			if (mLitFloat != null)
			{
				return new M.LitFloat(-mLitFloat.val);
			}
			M.LitDouble mLitDouble = mExpr as M.LitDouble;
			if (mLitDouble != null)
			{
				return new M.LitDouble(-mLitDouble.val);
			}
			if (BinaryOp.IsNumeric(mExpr.GetMType()))
			{
				return new M.UnaryOp(UnaryOpCode.Neg, mExpr);
			}
			else throw state.ThrowTypeError(this, "The operand of '-' must have a numeric types but was found to have type '{0}'.", mExpr.GetMType());
		}
	}

	public class Not : UnaryOp
	{
		public Not(IToken token, Expr x)
			: base(token, x)
		{
		}

		protected override Code CompilerOp(CompileState state, Code mExpr)
		{
			if (BinaryOp.IsBoolean(mExpr.GetMType()))
			{
				return new M.UnaryOp(UnaryOpCode.Not, mExpr);
			}
			else throw state.ThrowTypeError(this, "The operand of '!' must be boolean type but was found to have type '{0}'.", mExpr.GetMType());
		}
	}

	public class Complement : UnaryOp
	{
		public Complement(IToken token, Expr x)
			: base(token, x)
		{
		}

		protected override Code CompilerOp(CompileState state, Code mExpr)
		{
			throw state.ThrowNotImplemented(this, "bitwise complement");
		}
	}

	public class PreIncrement : UnaryOp
	{
		public PreIncrement(IToken token, Expr x)
			: base(token, x)
		{
		}

		protected override Code CompilerOp(CompileState state, Code mExpr)
		{
			throw state.ThrowNotImplemented(this, "pre-increment");
		}

	}

	public class PreDecrement : UnaryOp
	{
		public PreDecrement(IToken token, Expr x)
			: base(token, x)
		{
		}

		protected override Code CompilerOp(CompileState state, Code mExpr)
		{
			throw state.ThrowNotImplemented(this, "post-increment");
		}

	}
	#endregion

	public abstract class BinaryOp : Expr
	{
		protected string name;
		public Expr a, b;

	    protected BinaryOp(string name, Expr x, Expr y)
			: base(x.Token)
		{
			Contract.Requires(x != null);
			Contract.Requires(y != null);
			this.name = name;
			this.a = x;
			this.b = y;
		}

		public override Kind Compile(CompileState state)
		{
			Code mExpr1 = a.CompileRValue(state);
			Code mExpr2 = b.CompileRValue(state);

			MType unifiedType = mExpr1.GetMType();
			if (!Code.MakeCompatible(ref mExpr2, unifiedType))
			{
				unifiedType = mExpr2.GetMType();
				if (!Code.MakeCompatible(ref mExpr1, unifiedType))
				{
					unifiedType = null;
					throw state.ThrowTypeError(this, "The operands of the binary operator '{0}' must have the compatible types but the left operand has type '{1}' and the right operand has type '{2}'.", name, mExpr1.GetMType(), mExpr2.GetMType());
				}
			}

			return Value.FromCode(CompileOp(state, mExpr1, mExpr2));
		}

		public static bool IsBoolean(MType operandType)
		{
			return operandType == M.PrimType.Boolean;
		}

		public static bool IsNumeric(MType operandType)
		{
			return operandType == M.PrimType.Int || operandType == M.PrimType.Long || operandType == M.PrimType.Float || operandType == M.PrimType.Double;
		}

		public static bool IsEqable(MType operandType)
		{
			return !operandType.IsValueType() || IsNumeric(operandType) || operandType == M.PrimType.Char;
		}

		protected abstract Code CompileOp(CompileState state, Code mExpr1, Code mExpr2);
	}

	#region Arithmetic ops
	public abstract class NumericOp : BinaryOp
	{
		private readonly BinaryArithmeticOpCode op;

		public NumericOp(BinaryArithmeticOpCode op, string name, Expr x, Expr y)
			: base(name, x, y)
		{
			this.op = op;
		}

		protected override Code CompileOp(CompileState state, Code mExpr1, Code mExpr2)
		{
			if (IsNumeric(mExpr1.GetMType()))
			{
				return new BinaryArithmeticOp(op, mExpr1, mExpr2);
			}
			else throw state.ThrowTypeError(this, "The operands of '{0}' must be have numeric types but where found to have type '{1}'.", name, mExpr1.GetMType());
		}
	}

	public class Multiply : NumericOp
	{
		public Multiply(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Mul, "*", x, y)
		{
		}
	}

	public class Divide : NumericOp
	{
		public Divide(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Div, "/", x, y)
		{
		}
	}

	public class Remainder : NumericOp
	{
		public Remainder(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Mod, "%", x, y)
		{
		}
	}

	public class Add : NumericOp
	{
		public Add(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Add, "+", x, y)
		{
		}
	}

	public class Subtract : NumericOp
	{
		public Subtract(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Sub, "-", x, y)
		{
		}
	}
	#endregion

	#region Shift ops
	public class ShiftLeft : NumericOp
	{
		public ShiftLeft(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Shl, "<<", x, y)
		{
		}
	}

	public class ShiftRight : NumericOp
	{
		public ShiftRight(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Shr, ">>", x, y)
		{
		}
	}
	#endregion

	#region Relational ops
	public abstract class RelationalOp : BinaryOp
	{
		private readonly BinaryRelationalOpCode op;

		public RelationalOp(BinaryRelationalOpCode op, string name, Expr x, Expr y)
			: base(name, x, y)
		{
			this.op = op;
		}

		protected override Code CompileOp(CompileState state, Code mExpr1, Code mExpr2)
		{
			if (IsNumeric(mExpr1.GetMType()))
			{
				return new BinaryRelationalOp(op, mExpr1, mExpr2);
			}
			else throw state.ThrowTypeError(this, "The operands of '{0}' must be have numeric types but where found to have type '{1}'.", name, mExpr1.GetMType());
		}
	}

	public class LessThan : RelationalOp
	{
		public LessThan(Expr x, Expr y)
			: base(BinaryRelationalOpCode.Lt, "<", x, y)
		{
		}
	}

	public class GreaterThan : RelationalOp
	{
		public GreaterThan(Expr x, Expr y)
			: base(BinaryRelationalOpCode.Gt, ">", x, y)
		{
		}
	}

	public class LessEqual : RelationalOp
	{
		public LessEqual(Expr x, Expr y)
			: base(BinaryRelationalOpCode.Le, "<=", x, y)
		{
		}
	}

	public class GreaterEqual : RelationalOp
	{
		public GreaterEqual(Expr x, Expr y)
			: base(BinaryRelationalOpCode.Ge, ">=", x, y)
		{
		}
	}
	#endregion

	public class Is : Expr
	{
		public Is(Expr expr, Typ type)
			: base(expr.Token)
		{
		}

		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class As : Expr
	{
		public Expr expr;
		public Typ type;

		public As(Expr expr, Typ type)
			: base(expr.Token)
		{
			this.expr = expr;
			this.type = type;
		}

		public override Kind Compile(CompileState state)
		{
			Code mExpr = expr.CompileRValue(state);
			if (mExpr.GetMType().IsValueType()) throw state.ThrowTypeError(this, "The expression of in the 'as' expression has type '{0}' but it must be a reference type.", mExpr.GetMType());
			MType mType = type.CompileFor(state);
			if (mType.IsValueType()) throw state.ThrowTypeError(this, "The type '{0}' in the 'as' expression must be a reference type.", mType);
			return Value.FromCode(new M.AsClass(mExpr, mType));
		}
	}

	#region Equality ops
	public abstract class EqualityOp : BinaryOp
	{
		private readonly BinaryEqualityOpCode op;

		public EqualityOp(BinaryEqualityOpCode op, string name, Expr x, Expr y)
			: base(name, x, y)
		{
			this.op = op;
		}

		protected override Code CompileOp(CompileState state, Code mExpr1, Code mExpr2)
		{
			if (IsEqable(mExpr1.GetMType()))
			{
				return new BinaryEqualityOp(op, mExpr1, mExpr2);
			}
			else throw state.ThrowTypeError(this, "The operands of '{0}' must have a reference or primitive type but where found to have type '{1}'.", name, mExpr1.GetMType());
		}
	}

	public class Equals : EqualityOp
	{
		public Equals(Expr x, Expr y)
			: base(BinaryEqualityOpCode.Eq, "==", x, y)
		{
		}
	}

	public class NotEqual : EqualityOp
	{
		public NotEqual(Expr x, Expr y)
			: base(BinaryEqualityOpCode.Ne, "!=", x, y)
		{
		}
	}
	#endregion

	#region Bitwise ops
	public class And : NumericOp
	{
		public And(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Add, "&", x, y)
		{
		}
	}

	public class Xor : NumericOp
	{
		public Xor(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Add, "^", x, y)
		{
		}
	}

	public class Or : NumericOp
	{
		public Or(Expr x, Expr y)
			: base(BinaryArithmeticOpCode.Add, "|", x, y)
		{
		}
	}
	#endregion

	#region Logical ops
	public abstract class LogicalOp : BinaryOp
	{
		private readonly BinaryLogicalOpCode op;

		public LogicalOp(BinaryLogicalOpCode op, string name, Expr x, Expr y)
			: base(name, x, y)
		{
			this.op = op;
			this.name = name;
		}

		protected override Code CompileOp(CompileState state, Code mExpr1, Code mExpr2)
		{
			if (IsBoolean(mExpr1.GetMType()))
			{
				return new BinaryLogicalOp(op, mExpr1, mExpr2);
			}
			else throw state.ThrowTypeError(this, "The operands of '{0}' must have a boolean type but where found to have type '{1}'.", name, mExpr1.GetMType());
		}
	}

	public class LogicalAnd : LogicalOp
	{
		public LogicalAnd(Expr x, Expr y)
			: base(BinaryLogicalOpCode.And, "&&", x, y)
		{
		}
	}

	public class LogicalOr : LogicalOp
	{
		public LogicalOr(Expr x, Expr y)
			: base(BinaryLogicalOpCode.Or, "||", x, y)
		{
		}
	}
	#endregion

	public class Conditional : Expr
	{
		public Conditional(Expr x, Expr y, Expr z)
			: base(x.Token)
		{
		}

		public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class MemberAccess : Expr
	{
		public Expr expr;
		public Typ predefinedType;
		public string name;
		public List<Typ> typeArgs;
		//public List<Arg> valueArgs;

		public MemberAccess(Expr expr, Ident name, List<Typ> typeArgs)
			: base(expr.Token)
		{
			Contract.Requires(expr != null);
			Contract.Requires(name != null);
			this.expr = expr;
			this.predefinedType = null;
			this.name = name.Name;
			this.typeArgs = typeArgs;
		}

		public MemberAccess(Typ predefinedType, Ident name, List<Typ> typeArgs)
			: base(predefinedType.Token)
		{
			Contract.Requires(predefinedType != null);
			Contract.Requires(name != null);
			this.expr = null;
			this.predefinedType = predefinedType;
			this.name = name.Name;
			this.typeArgs = typeArgs;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(expr != null);
	        Contract.Invariant(name != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			Kind exprKind;
			if (expr != null) exprKind = expr.Compile(state);
			else exprKind = new Type(predefinedType.Compile(state));
			MType[] mTypeArgs = null;
			if (typeArgs != null) mTypeArgs = Typ.Compile(state, typeArgs);

			if (exprKind is Namespace)
			{
				string ns = ((Namespace)exprKind).ns;
				MType type = state.LookupType(ns, name, mTypeArgs);
				if (type != null) return new Type(type);

				if (typeArgs == null)
				{
					ns = $"{ns}.{name}";
					if (state.NamespaceStartsWith(ns)) return new Namespace(ns);
					throw state.ThrowTypeError(this, "The namespace '{0}' could not be found.", ns);
				}
				else
					throw state.ThrowTypeError(this, "Cannot apply type arguments to the namespace '{0}'.", ns);
			}

			if (exprKind is Type)
			{
				MType type = ((Type)exprKind).mType;
				MType nestedType = type.GetNestedType(name, mTypeArgs != null ? mTypeArgs : M.MType.Empty);
				if (nestedType != null) return new Type(nestedType);
				Kind kind = FindMember(state, type, name, mTypeArgs, null);
				if (kind != null) return kind;
				else throw state.ThrowTypeError(this, "The type '{0}' does not have a static member named '{1}'.", type, name);
			}

			if (exprKind is MethodGroup)
			{
				MethodGroup mg = (MethodGroup)exprKind;
				throw state.ThrowTypeError(this, "Cannot access the member '{0}' on the method '{1}'.", name, mg.name);
			}

			if (exprKind is RValue)
			{
				RValue rValue = (RValue)exprKind;
				Kind kind = FindMember(state, rValue.GetMType(), name, mTypeArgs, rValue.GetRValue());
				if (kind != null) return kind;
				else throw state.ThrowTypeError(this, "The type '{0}' does not have an instance member named '{1}'.", rValue.GetMType(), name);
			}

			throw state.ThrowTypeError(this, "Cannot access a member on a '{0}'.", exprKind);
		}

		public static Kind FindMember(CompileState state, M.MType type, string name, MType[] typeArgs, Code mExpr)
		{
			bool isStatic = mExpr == null;
			if (typeArgs == null)
			{
				MFieldInfo field = state.LookupField(name, isStatic);
				if (field == null) field = type.GetField(name, isStatic);
				if (field != null) return Value.FromCode(new FieldAccess(mExpr, field));
			}

			MMethodInfo[] methods = type.GetMethods(name, isStatic, typeArgs != null ? typeArgs : MType.Empty);
			if (methods != null) return new MethodGroup(name, mExpr, methods);

			return null;
		}
	}

	public class ElementAccess : Expr
	{
		public Expr expr;
		public List<Expr> indices;

		public ElementAccess(Expr expr, List<Expr> exprs)
			: base(expr.Token)
		{
			Contract.Requires(expr != null);
			Contract.Requires(exprs != null);
			this.expr = expr;
			if (exprs.Count == 0) throw new ArgumentException("exprs.Count == 0", "exprs");
			this.indices = exprs;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(expr != null);
	        Contract.Invariant(indices != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			Code mExpr = expr.CompileRValue(state);
			MType mType = mExpr.GetMType();
			if (mType.IsArray())
			{
				int rank = mType.GetArrayRank();
				if (rank != indices.Count) throw state.ThrowTypeError(this, "The array type '{0}' has rank {1} but the element access supplies {2} indices.", mType, rank, indices.Count);
				if (indices.Count == 1)
				{
					Code mIndex = indices[0].CompileRValue(state);
					if (mIndex.GetMType() != M.PrimType.Int)
							throw state.ThrowTypeError(this, "Index number 1 in element access must have type 'int' but was found to have type '{0}'.", mIndex.GetMType());
					return Value.FromCode(new M.ElementAccess(mExpr, mIndex));
				}
				else throw state.ThrowNotImplemented(this, "multi-dimensional arrays");
			}
			else throw state.ThrowTypeError(this, "Element access on the type '{0}' is not an array type.", mExpr.GetMType());
		}
	}

	public class Invocation : Expr
	{
		public Expr expr;
		public List<Arg> args;

		public Invocation(Expr expr, List<Arg> args)
			: base(expr.Token)
		{
			Contract.Requires(expr != null);
			this.expr = expr;
			this.args = args != null ? args : new List<Arg>();
		}

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(expr != null);
        }


        public override Kind Compile(CompileState state)
		{
			Kind exprKind = expr.Compile(state);
			M.Code[] mArgs = Arg.Compile(state, args);

			if (exprKind is Namespace)
			{
				string ns = ((Namespace)exprKind).ns;
				throw state.ThrowTypeError(this, "Cannot invoke a namespace '{0}'.", ns);
			}

			if (exprKind is Type)
			{
				MType type = ((Type)exprKind).mType;
				throw state.ThrowTypeError(this, "Cannot invoke a type '{0}'.", type);
			}

			M.ParamType[] paramTypes = Code.GetParamTypes(mArgs);
			if (exprKind is MethodGroup)
			{
				MethodGroup mg = (MethodGroup)exprKind;
				MMethodInfo method = MMethodInfo.ChooseBestOverload(mg.methods, paramTypes, state.GetExpectedType());
				if (method != null)
				{
					return Value.FromCode(new MethodInvoke(method.IsStatic() ? null: mg.expr, method, DoBoxing(method.GetParamTypes(), mArgs)));
				}
				else throw state.ThrowTypeError(this, "No suitable overload of the method '{0}' could be found for argument types [{1}].", mg.name, ShowParamTypeList(paramTypes));
			}

			if (exprKind is RValue)
			{
				RValue rValue = (RValue)exprKind;
				MType mType = rValue.GetMType();
				if (mType.IsSubTypeOf(M.PrimType.Delegate))
				{
					MMethodInfo method = MMethodInfo.ChooseBestOverload(new MMethodInfo[] { DelegateType.GetInvokeMethod(mType)}, paramTypes, state.GetExpectedType());
					if (method != null)
					{
						return Value.FromCode(new MethodInvoke(rValue.GetRValue(), method, DoBoxing(method.GetParamTypes(), mArgs)));
					}
					else throw state.ThrowTypeError(this, "The delegate type '{0}' in the invocation is not compatible with the argument types [{1}].", mType, ShowParamTypeList(paramTypes));
				}
				else throw state.ThrowTypeError(this, "Cannot invoke the non-delegate type '{0}'.", mType);
			}

			throw state.ThrowTypeError(this, "Cannot perform an invocation on a '{0}'.", exprKind);
		}

		public static M.Code[] DoBoxing(M.ParamType[] paramTypes, M.Code[] valueArgs)
		{
			for (int i = 0; i < paramTypes.Length; i++)
				Code.MakeCompatible(ref valueArgs[i], paramTypes[i].type);
			return valueArgs;
		}

		public static string ShowParamTypeList(M.ParamType[] paramTypes)
		{
			if (paramTypes.Length == 0) return string.Empty;
			else
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.AppendFormat("'{0}'", paramTypes[0]);
				for (int i = 1; i < paramTypes.Length; i++)
					sb.AppendFormat(", '{0}'", paramTypes[i]);
				return sb.ToString();
			}
		}
	}

	public class ObjectCreation : Expr
	{
		public Typ type;
		public List<Arg> args;

		public ObjectCreation(IToken token, Typ type, List<Arg> args)
			: base(token)
		{
			Contract.Requires(type != null);
			this.type = type;
			this.args = args != null ? args : new List<Arg>();
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(type != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			MType mType = type.Compile(state);
			M.Code[] mArgs = Arg.Compile(state, args);
			M.ParamType[] paramTypes = Code.GetParamTypes(mArgs);
			MConstructorInfo ctor = MConstructorInfo.ChooseBestOverload(mType.GetConstructors(), paramTypes);
			if (ctor != null)
			{
				return Value.FromCode(new M.ObjectCreate(ctor, Invocation.DoBoxing(ctor.GetParamTypes(), mArgs)));
			}
			else throw state.ThrowTypeError(this, "No suitable constructor overload on type '{0}' with argument types [{1}] could be found.", mType, Invocation.ShowParamTypeList(paramTypes));
		}
	}

	public class ArrayCreation : Expr
	{
		public Typ elemType;
		public int rank;
		public List<Expr> dims;
		public ArrayInitialisation init;

		public ArrayCreation(IToken token, ArrayType type, List<Expr> dims)
			: base(token)
		{
			Contract.Requires(type != null);
			Contract.Requires(dims != null);
			this.elemType = type.type;
			this.rank = type.rank;
			this.dims = dims;
			this.init = null;
		}

		public ArrayCreation(IToken token, ArrayType type, ArrayInitialisation init)
			: base(token)
		{
			Contract.Requires(type != null);
			Contract.Requires(init != null);
			this.elemType = type.type;
			this.rank = type.rank; 
			this.dims = null;
			this.init = init;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(dims != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			MType mType = elemType.Compile(state);
			if (dims != null)
			{
				Code[] mDims = Expr.CompileRValues(state, dims);
				for (int i = 0; i < mDims.Length; i++)
					if (mDims[i].GetMType() != M.PrimType.Int)
						throw state.ThrowTypeError(this, "Index '{0}' in array construction must have type 'int' but was found to have type '{1}'.", i, mDims[i].GetMType());
				return Value.FromCode(new M.ArrayCreate(mType, mDims, null));
			}
			else
			{
				Code[] mExprs = Expr.CompileRValues(state, init.exprs);
				return Value.FromCode(new M.ArrayCreate(mType, new Code[] { new M.LitInt(mExprs.Length) }, mExprs));
			}
		}
	}

	public class ArrayInitialisation : Expr
	{
		public List<Expr> exprs;

		public ArrayInitialisation(List<Expr> exprs)
			: base(FirstToken<Expr>(exprs))
		{
		    Contract.Requires(exprs != null);
		    this.exprs = exprs;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(exprs != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			throw new NotImplementedException();
		}
	}

	public class Cast : Expr
	{
		public Typ type;
		public Expr expr;

		public Cast(IToken token, Typ type, Expr expr)
			: base(token)
		{
			Contract.Requires(type != null);
			Contract.Requires(expr != null);
			this.type = type;
			this.expr = expr;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(type != null);
	        Contract.Invariant(expr != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			MType destType = type.Compile(state);
			state.PushExpectedType(destType);
			Code mExpr = expr.CompileRValue(state);
			state.PopExpectedType();
			//MType srcType = mExpr.GetMType();
			if (mExpr is FunctionCreate) return Value.FromCode(mExpr);
			else if (mExpr.GetMType().IsValueType() && destType.IsValueType())
			{
				if (destType == M.PrimType.Int) return Value.FromCode(new M.Convert(mExpr, CILType.I4));
				else if (destType == M.PrimType.Long) return Value.FromCode(new M.Convert(mExpr, CILType.I8));
				else if (destType == M.PrimType.Float) return Value.FromCode(new M.Convert(mExpr, CILType.R4));
				else if (destType == M.PrimType.Double) return Value.FromCode(new M.Convert(mExpr, CILType.R8));
				else if (destType == M.PrimType.Short) return Value.FromCode(new M.Convert(mExpr, CILType.I2));
				else if (destType == M.PrimType.Byte) return Value.FromCode(new M.Convert(mExpr, CILType.U1));
				else throw state.ThrowTypeError(this, "Cannot cast a value type to another value type.");
			}
			else return Value.FromCode(new M.CastClass(destType, mExpr));
		}
	}

	public class Bracket : Expr
	{
		public List<Stmt> stmts;
		public Expr expr;

		public Bracket(IToken token, List<Stmt> stmts, Expr expr)
			: base(token)
		{
			this.stmts = CheckNull(stmts);
			this.expr = expr;
		}

		public override Kind Compile(CompileState state)
		{
			state.RaiseLevel();

			List<Code> mCode = new List<Code>();
			foreach(Stmt stmt in stmts)
			{
				Code mStmt = stmt.Compile(state);
				mCode.Add(mStmt);
			}
			if (expr != null)
			{
				CodeType codeType = state.GetExpectedType() as CodeType;
				if (codeType != null) state.PushExpectedType(LiftType.Promote(codeType.codeType));
				Code mExpr = expr.CompileRValue(state);
				if (codeType != null) state.PopExpectedType();
				mCode.Add(mExpr);
			}
			state.LowerLevel();

			return Value.FromCode(new M.Bracket(CodeGroup.MakeCode(mCode.ToArray())));
		}
	}

	//	public class BracketModule : Expr
	//	{
	//		public Class[] classes;
	//		public Delegate[] delegates;
	//		public Stmt[] stmts;
	//		public Expr expr;
	//
	//		public BracketModule(Class[] classes, Delegate[] delegates, Stmt[] stmts, Expr expr)
	//		{
	//			this.classes = classes;
	//			this.delegates = delegates;
	//			this.stmts = stmts;
	//			this.expr = expr;
	//		}
	//
	//		public override Kind Compile(CompileState state)
	//		{
	//			state.level++;
	//			M.MModuleBuilder module = new M.MModuleBuilder();
	//			state.modules.Push(module);
	//			foreach (Class @class in classes)
	//			{
	//				@class.CompileTypes(state);
	//				module.AddType(@class.mClass);
	//			}
	//			foreach (Delegate @delegate in delegates)
	//			{
	//				@delegate.CompileTypes(state);
	//				module.AddType(@delegate.mDelegate);
	//			}
	//			foreach (Class @class in classes) @class.CompileMembers(state);
	//			foreach (Class @class in classes) @class.CompileCode(state);
	//			state.modules.Pop();
	//			module.Done();
	//			state.level--;
	//			mExpr = new M.BracketModule(module);
	//			return new Value(mExpr.type);
	//		}
	//
	//	}

	public class EscapeExpr : Expr
	{
		public Expr expr;

		public EscapeExpr(IToken token, Expr expr)
			: base(token)
		{
		    Contract.Requires(expr != null);
		    this.expr = expr;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(expr != null);
	    }

	    public override Kind Compile(CompileState state)
		{
			if (state.level <= 0) throw state.ThrowTypeError(this, "Escape cannot occur at level 0.");
			state.LowerLevel();
			Code mExpr = expr.CompileAnyValue(state);
			state.RaiseLevel();
			M.CodeType codeType = mExpr.GetMType() as M.CodeType;
			if (codeType == null) throw state.ThrowTypeError(this, "Escape expression does not have a code type.");
			return Value.FromCode(new M.Escape(mExpr));
		}

	}

	public class Function : Expr
	{
		public string name;
		public List<Param> @params;
		public List<Stmt> stmts;

		public M.Function mFunc;

		public Function(IToken token, Ident name, List<Param> @params, List<Stmt> stmts)
			: base(token)
		{
			Contract.Requires(stmts != null);
			this.name = name != null ? name.Name : null;
			this.@params = CheckNull(@params);
			this.stmts = stmts;
		}

		public override Kind Compile(CompileState state)
		{
			MType delegateType = state.GetExpectedType();
			if (delegateType == null)
				throw state.ThrowTypeError(this, "Cannot imply the delegate type of the anonymous function.");

			ParamType[] paramTypes;
			MType retType;
			if (!DelegateType.OpenDelegateType(delegateType, out paramTypes, out retType))
				throw state.ThrowTypeError(this, "The anonymous function is expected to have type '{0}' but this is not a delegate type.", delegateType);

			string[] paramNames = @params.ConvertAll<string>(delegate(Param p) { return p.name; }).ToArray();
			if (paramTypes.Length != paramNames.Length)
				throw state.ThrowTypeError(this, "The delegate type '{0}' defines {1} parameters but the anonymous function defined {2} parameters.", delegateType, paramTypes.Length, paramNames.Length);

			M.Function mFunc = new M.Function(name, delegateType, paramNames);
			state.PushMethod(null, mFunc.@params, retType);
			if (name != null) state.PushLocal(mFunc);
			mFunc.stmt = Stmt.Compile(state, stmts);
			state.PopMethod();

			return Value.FromCode(new M.FunctionCreate(mFunc));
		}
	}
}