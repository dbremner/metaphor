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
using Metaphor.Collections;
using M = Metaphor;
using IToken = antlr.IToken;

namespace Metaphor.Compiler
{
	public abstract class Stmt : Node
	{
		public Stmt(IToken token)
			: base(token)
		{
		}

		public abstract Code Compile(CompileState state);

		public static M.Code Compile(CompileState state, List<Stmt> stmts)
		{
			List<Code> mStmts = new List<Code>();
			foreach (Stmt stmt in stmts)
				if(stmt != null)
					mStmts.Add(stmt.Compile(state));
			return CodeGroup.MakeCode(mStmts.ToArray());
		}
	}

	public class Label : Stmt
	{
		public Label(Ident name)
			: base(name.Token)
		{
			if (name == null) throw new ArgumentNullException("name");
		}

		public override Code Compile(CompileState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class Empty : Stmt
	{
		public Empty()
			: base(null)
		{
		}

		public override Code Compile(CompileState state)
		{
			return new M.Empty();
		}

	}

	public class Block : Stmt
	{
		public List<Stmt> stmts;

		public Block(List<Stmt> stmts)
			: base(FirstToken<Stmt>(stmts))
		{
			if (stmts == null) throw new ArgumentNullException("stmts");
			this.stmts = stmts;
		}

		public override Code Compile(CompileState state)
		{
			state.PushBlock();
			Code mStmt = Stmt.Compile(state, stmts);
			state.PopBlock();
			return mStmt;
		}

	}

	public class LocalDecl : Stmt
	{
		public Typ type;
		public string name;
		public Expr init;

		public static void Add(List<Stmt> stmts, Typ type, List<Tuple<Ident, Expr>> decls)
		{
            if (stmts == null) throw new ArgumentNullException("stmts");
            if (type == null) throw new ArgumentNullException("type");
            if (decls == null || decls.Count == 0) throw new ArgumentException("decls");
            foreach (Tuple<Ident, Expr> decl in decls)
                stmts.Add(new LocalDecl(type, decl.fst, decl.snd));
		}

		public LocalDecl(Typ type, Ident name, Expr init)
			: base(type.Token)
		{
            if (type == null) throw new ArgumentNullException("type");
			this.type = type;
            if (name == null) throw new ArgumentNullException("name");
			this.name = name.Name;
			this.init = init;
		}

		public override Code Compile(CompileState state)
		{
			MType mType = type.CompileFor(state);
			Code mExpr = null;
			if (init != null)
			{
				state.PushExpectedType(mType);
				mExpr = init.CompileRValue(state);
				state.PopExpectedType();
				if (!Code.MakeCompatible(ref mExpr, mType))
					throw state.ThrowTypeError(this, "Initialisation expression has type '{0}' which is not compatible with '{1}' the type of the local variable '{2}'.", mExpr.GetMType(), mType, name);
			}
			M.LocalDecl decl = new M.LocalDecl(name, mType);
			state.PushLocal(decl);
			return new M.DeclareLocal(decl, mExpr);
		}
	}

	public class ConstDecl : Stmt
	{
		public Typ type;
		public string name;
		public Expr init;

		public static void Add(List<Stmt> stmts, IToken token, Typ type, List<Tuple<Ident, Expr>> decls)
		{
			foreach (Tuple<Ident, Expr> decl in decls)
				stmts.Add(new ConstDecl(token, type, decl.fst.Name, decl.snd));
		}

		public ConstDecl(IToken token, Typ type, string name, Expr init)
			: base(token)
		{
			this.type = type;
			this.name = name;
			this.init = init;
		}

		public override Code Compile(CompileState state)
		{
			type.Compile(state);
			Code mExpr = init.CompileRValue(state);
			if (!(mExpr is M.Value))
				throw state.ThrowTypeError(this, "Initialisation expression of the constant '{0}' is not a constant.", name);
			if (!Code.MakeCompatible(ref mExpr, type.mType))
				throw state.ThrowTypeError(this, "The type of the Initialisation expression is '{0}' differs from the type of the constant declaration '{2}' which has type '{1}'.", mExpr.GetMType(), type.mType, name);
			state.PushConst(name, mExpr);
			return null;
		}
	}

	public class If : Stmt
	{
		public Expr cond;
		public Stmt ifTrue;
		public Stmt ifFalse;

		public If(IToken token, Expr cond, Stmt ifTrue, Stmt ifFalse)
			: base(token)
		{
			if (cond == null) throw new ArgumentNullException("cond");
			this.cond = cond;
			if (ifTrue == null) throw new ArgumentNullException("ifTrue");
			this.ifTrue = ifTrue;
			this.ifFalse = ifFalse;
		}

		public override Code Compile(CompileState state)
		{
			Code mCond = cond.CompileRValue(state);
			if (mCond.GetMType() != M.PrimType.Boolean) throw state.ThrowTypeError(this,"If condition must have boolean type.");
			Code mIfTrue = ifTrue.Compile(state);
			Code mIfFalse = null;
			if (ifFalse != null) mIfFalse = ifFalse.Compile(state);
			return new M.If(mCond, mIfTrue, mIfFalse);
		}

	}

	public class Switch : Stmt
	{
		public Expr expr;
		public List<Case> cases;

		public Switch(IToken token, Expr expr, List<Case> cases)
			: base(token)
		{
			if (expr == null) throw new ArgumentNullException("expr");
			this.expr = expr;
			if (cases == null) throw new ArgumentNullException("cases");
			this.cases = cases;
		}

		public override Code Compile(CompileState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class Case
	{
		public List<Literal> literals;
		public List<Stmt> stmts;

		public Case(IToken token, List<Literal> literals, List<Stmt> stmts)
		{
			this.literals = literals;
			if (stmts == null) throw new ArgumentNullException("stmts");
			this.stmts = stmts;
		}
	}

	public class While : Stmt
	{
		public Expr cond;
		public Stmt body;

		public While(IToken token, Expr cond, Stmt body)
			: base(token)
		{
			if (cond == null) throw new ArgumentNullException("cond");
			this.cond = cond;
			if (body == null) throw new ArgumentNullException("body");
			this.body = body;
		}

		public override Code Compile(CompileState state)
		{
			Code mCond = cond.CompileRValue(state);
			if (mCond.GetMType() != M.PrimType.Boolean) throw state.ThrowTypeError(this, "While condition must have boolean type.");
			Code mBody = body.Compile(state);
			return new M.Loop(true, mCond, mBody);
		}
	}

	public class Do : Stmt
	{
		public Expr cond;
		public Stmt body;

		public Do(IToken token, Stmt body, Expr cond)
			: base(token)
		{
			if (cond == null) throw new ArgumentNullException("cond");
			this.cond = cond;
			if (body == null) throw new ArgumentNullException("body");
			this.body = body;
		}

		public override Code Compile(CompileState state)
		{
			throw state.ThrowNotImplemented(this, "do loops");
		}
	}

	public class For : Stmt
	{
		public LocalDecl initVar;
		public List<Expr> initExprs;
		public Expr cond;
		public List<Expr> loop;
		public Stmt body;

		public For(IToken token, LocalDecl init, Expr cond, List<Expr> loop, Stmt body)
			: base(token)
		{
			this.initVar = init;
			this.initExprs = null;
			this.cond = cond;
			this.loop = loop;
			if (body == null) throw new ArgumentNullException("body");
			this.body = body;
		}

		public For(IToken token, List<Expr> init, Expr cond, List<Expr> loop, Stmt body)
			: base(token)
		{
			this.initVar = null;
			this.initExprs = init;
			this.cond = cond;
			this.loop = loop;
			if (body == null) throw new ArgumentNullException("body");
			this.body = body;
		}

		public override Code Compile(CompileState state)
		{
			state.PushBlock();
			List<Code> mInit = new List<Code>();
			if (initVar != null)
			{
				Code mInitVar = initVar.Compile(state);
				mInit.Add(mInitVar);
			}
			if (initExprs != null)
			{
				foreach (Expr expr in initExprs)
				{
					Code mInitExpr = expr.CompileNoValue(state);
					mInit.Add(mInitExpr);
				}
			}
			Code mCond = new M.Empty();
			if (cond != null)
			{
				mCond = cond.CompileRValue(state);
				if (mCond.GetMType() != M.PrimType.Boolean) 
					throw state.ThrowTypeError(this, "Condition in for loop must have boolean type.");
			}
			List<Code> mBody = new List<Code>();
			Code mCode = body.Compile(state);
			mBody.Add(mCode);
			if (loop != null) 
			{
				foreach (Expr expr in loop)
				{
					mCode = expr.CompileNoValue(state);
					mBody.Add(mCode);
				}
			}
			state.PopBlock();

			return CodeGroup.MakeCode(
				new CodeGroup(mInit.ToArray()),
				new Loop(true, mCond, CodeGroup.MakeCode(mBody.ToArray())));
		}
	}

	public class Foreach : Stmt
	{
		public Typ type;
		public string ident;
		public Expr expr;
		public Stmt body;

		public Foreach(IToken token, Typ type, Ident ident, Expr expr, Stmt body)
			: base(token)
		{
			if (type == null) throw new ArgumentNullException("type");
			this.type = type;
			if (ident == null) throw new ArgumentNullException("ident");
			this.ident = ident.Name;
			if (expr == null) throw new ArgumentNullException("expr");
			this.expr = expr;
			if (body == null) throw new ArgumentNullException("body");
			this.body = body;
		}

		public override Code Compile(CompileState state)
		{
			Code mExpr = expr.CompileRValue(state);
			MType elemType = type.Compile(state);
			MType enumerableType = GenericSystemType.Create(typeof(IEnumerable<>), new MType[] { elemType });
			if(!mExpr.GetMType().IsSubTypeOf(enumerableType))
				throw state.ThrowTypeError(this, "The expression in the foreach statement has type '{1}' which does not implement '{0}'.", enumerableType, mExpr.GetMType());

			MType enumType = GenericSystemType.Create(typeof(IEnumerator<>), new MType[] { elemType });
			M.LocalDecl e = new M.LocalDecl(ident, enumType);
			Code mInit = new MethodInvoke(mExpr, enumerableType.GetMethod("GetEnumerator", false, MType.Empty, ParamType.Empty), Code.Empty);
			Code mCond = new MethodInvoke(e.MakeVar(), enumType.GetMethod("MoveNext", false, MType.Empty, ParamType.Empty), Code.Empty);
			state.PushBlock();
			state.PushConst(ident, new MethodInvoke(e.MakeVar(), enumType.GetMethod("get_Current", false, MType.Empty, ParamType.Empty), Code.Empty));
			Code mBody = body.Compile(state);
			state.PopBlock();

			return CodeGroup.MakeCode(new DeclareLocal(e, mInit), new Loop(true, mCond, mBody));
		}
	}

	public class Break : Stmt
	{
		public Break(IToken token)
			: base(token)
		{
		}

		public override Code Compile(CompileState state)
		{
			return new M.Break();
		}
	}

	public class Continue : Stmt
	{
		public Continue(IToken token)
			: base(token)
		{
		}

		public override Code Compile(CompileState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class Goto : Stmt
	{
		public Goto(IToken token, Ident name)
			: base(token)
		{
		}

		public override Code Compile(CompileState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class Return : Stmt
	{
		public Expr expr;

		public Return(IToken token, Expr expr)
			: base(token)
		{
			this.expr = expr;
		}

		public override Code Compile(CompileState state)
		{
			MType expectedRetType = state.GetReturnType();
			if (expr == null)
			{
				if (expectedRetType != M.PrimType.Void)
					throw state.ThrowTypeError(this, "The return expression cannot return a value.");
				return new M.Return(null);
			}
			else
			{
				state.PushExpectedType(expectedRetType);
				Code mExpr = expr.CompileRValue(state);
				state.PopExpectedType();
				if (!Code.MakeCompatible(ref mExpr, expectedRetType))
					throw state.ThrowTypeError(this,"The return expression has type '{0}' which is not compatiable with the expected return type '{1}'.", mExpr.GetMType(), expectedRetType);
				return new M.Return(mExpr);
			}
		}
	}

	public class Throw : Stmt
	{
		public Expr expr;

		public Throw(IToken token, Expr expr)
			: base(token)
		{
			if (expr == null) throw new ArgumentNullException("exc");
			this.expr = expr;
		}

		public override Code Compile(CompileState state)
		{
			Code mExpr = expr.CompileRValue(state);
			//if (!excType.IsSubTypeOf(M.PrimType.Exception))
			//    throw new TypeException(this, state, "Must throw an exception.");
			return new M.Throw(mExpr);
		}
	}

	public class Try : Stmt
	{
		public List<Stmt> stmts;
		public List<Catch> catches;
		public List<Stmt> fin;

		public Try(IToken token, List<Stmt> stmts, List<Catch> catches, Finally fin)
			: base(token)
		{
			if (stmts == null) throw new ArgumentNullException("stmts");
			this.stmts = stmts;
			if (catches == null && fin == null) throw new ArgumentException("catches == null && fin == null");
			this.catches = catches;
			this.fin = fin.stmts;
		}

		public override Code Compile(CompileState state)
		{
			throw state.ThrowNotImplemented(this, "try/catch/finally blocks");
		}
	}

	public class Catch
	{
		public Typ typ;
		public Ident name;
		public List<Stmt> stmts;

		public Catch(IToken token, Typ typ, Ident name, List<Stmt> stmts)
		{
			if (typ == null && name != null) throw new ArgumentException("typ == null && name != null");
			if (stmts == null) throw new ArgumentNullException("stmts");
		}
	}

	public class Finally
	{
		public List<Stmt> stmts;

		public Finally(IToken token, List<Stmt> stmts)
		{
			if (stmts == null) throw new ArgumentNullException("stmts");
		}
	}

	public class TypeIf : Stmt
	{
		public List<Tuple<Ident, int>> patternVars;
		public string name;
		public List<TypeParam> constraints;
		public Typ pattern;
		public Stmt ifTrue;
		public Stmt ifFalse;

		public TypeIf(IToken token, List<Tuple<Ident, int>> patternVars, Ident ident, Typ pattern, List<TypeParam> constraints, Stmt ifTrue, Stmt ifFalse)
			: base(token)
		{
			this.patternVars = CheckNull<Tuple<Ident, int>>(patternVars);
			for (int i = 0; i < this.patternVars.Count; i++)
				if (this.patternVars[i].snd < 1) this.patternVars[i] = new Tuple<Ident, int>(this.patternVars[i].fst, 1);

			if (ident == null) throw new ArgumentNullException("name");
			this.name = ident.Name;
			if (pattern == null) throw new ArgumentNullException("type");
			this.pattern = pattern;
			this.constraints = CheckNull<TypeParam>(constraints);
			if (ifTrue == null) throw new ArgumentNullException("ifTrue");
			this.ifTrue = ifTrue;
			this.ifFalse = ifFalse;
		}

		public override Code Compile(CompileState state)
		{
			TypeVar typeVar = state.LookupTypeVar(name) as TypeVar;
			if (typeVar == null) throw state.ThrowTypeError(this, "The type varible '{0}' is not in scope.", name);

			TypeVarDecl[] decls = TypeParam.Compile(state, patternVars, constraints, TypeParam.Location.Stmt);
			state.PushTypeVars(decls);
			MType mType = pattern.CompileFor(state, 1);
			state.PushTypeSubst(typeVar.decl, mType);
			Code mIfTrue = ifTrue.Compile(state);
			state.PopTypeSubst();
			state.PopTypeVar();
			Code mIfFalse = null;
			if (ifFalse != null) mIfFalse = ifFalse.Compile(state);

			return new M.TypeIf(typeVar.decl, decls, Patternify(decls, mType), mIfTrue, mIfFalse);
		}

		private MType Patternify(TypeVarDecl[] decls, MType type)
		{
			TypeVar typeVar = type as TypeVar;
			if (typeVar != null && Array.IndexOf<TypeVarDecl>(decls, typeVar.decl) != -1) return new PatternVar((LocalReflTypeVarDecl) typeVar.decl);
			else
			{
				MType[] typeArgs = type.GetTypeArguments();
				for (int i = 0; i < typeArgs.Length; i++) typeArgs[i] = Patternify(decls, typeArgs[i]);
				return type.ReplaceTypeArguments(typeArgs);
			}
		}
	}

	public class WithType : Stmt
	{
		public string name;
		public Expr expr;
		public Stmt stmt;

		public WithType(IToken token, Ident ident, Expr expr, Stmt stmt)
			: base(token)
		{
			if (ident == null) throw new ArgumentNullException("ident");
			this.name = ident.Name;
			if (expr == null) throw new ArgumentNullException("expr");
			this.expr = expr;
			if (stmt == null) throw new ArgumentNullException("stmt");
			this.stmt = stmt;
		}

		public override Code Compile(CompileState state)
		{
            LocalReflTypeVarDecl decl = new M.LocalReflTypeVarDecl(name);
            Code type = expr.CompileRValue(state);
            state.PushTypeVar(decl);
            Code body = stmt.Compile(state);
            state.PopTypeVar();
            return new M.WithType(decl, type, body);
		}
	}

	public class ForMember : Stmt
	{
		public string name;
		public bool isStatic;
		public string retType;
		public List<Tuple<string,bool>> @params;
		public Typ type;
		public Stmt stmt;

		public ForMember(IToken token, bool isStatic, string retType, Ident ident, List<Tuple<string, bool>> @params, Typ type, Stmt stmt)
			: base(token)
		{
			if (ident == null) throw new ArgumentNullException("ident");
			this.name = ident.Name;
			this.isStatic = isStatic;
			if (retType == null) throw new ArgumentNullException("retType");
			this.retType = retType;
			if (@params != null)
			{
				this.@params = @params;
			}
			if (type == null) throw new ArgumentNullException("type");
			this.type = type;
			if (stmt == null) throw new ArgumentNullException("stmt");
			this.stmt = stmt;
		}

		public override Code Compile(CompileState state)
		{
			MType mType = type.CompileFor(state, 1);

			if (@params != null)
			{
				throw state.ThrowNotImplemented(this, "method patterns");
			}
			else
			{
				LocalReflTypeVarDecl fieldType = new M.LocalReflTypeVarDecl(retType);
				state.PushTypeVar(fieldType);
				ForField forField = new ForField(-1, isStatic, fieldType, name, mType);
				state.PushField(forField);
				forField.body = stmt.Compile(state);
				state.PopField();
				state.PopTypeVar();
				return forField;
			}
		}
	}

	public class EscapeStmt : Stmt
	{
		public List<Stmt> stmts;

		public EscapeStmt(IToken token, List<Stmt> stmts)
			: base(token)
		{
			this.stmts = CheckNull<Stmt>(stmts);
		}

		public override Code Compile(CompileState state)
		{
			if (state.level <= 0) throw state.ThrowTypeError(this, "Escape cannot occur at level 0.");
			state.LowerLevel();
			state.PushBlock();
			List<Code> mStmts = new List<Code>();
			foreach (Stmt stmt in stmts)
				if (stmt != null)
					mStmts.Add(stmt.Compile(state));
			mStmts.Add(new M.Empty());
			Code mStmt = CodeGroup.MakeCode(mStmts.ToArray());
			state.PopBlock();
			state.RaiseLevel();
			return new M.Escape(mStmt);
		}
	}

	public class Expression : Stmt
	{
		public Expr expr;

		public Expression(Expr expr)
			: base(expr.Token)
		{
			if (expr == null) throw new ArgumentNullException("expr");
			this.expr = expr;
		}

		public override Code Compile(CompileState state)
		{
			return expr.CompileNoValue(state);
		}
	}
}