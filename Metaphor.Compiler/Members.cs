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
	public abstract class Member : Node
	{
		public Modifier mods;
		public string name;

		public Member(IToken token, Modifier mods, Ident name)
			: base(token)
		{
			this.mods = mods;
			if (name == null) throw new ArgumentNullException("name");
			this.name = name.Name;
		}

		public abstract void CompileMembers(CompileState state, M.MTypeBuilder mClass);

		public abstract void CompileCode(CompileState state);
	}

	public class Field : Member
	{
		public Typ fieldType;
		public Expr initExpr;

		internal MFieldBuilder mField;

		public Field(Modifier mods, Typ type, Ident name, Expr init)
			: base(type.Token, mods, name)
		{
			this.fieldType = type;
			this.initExpr = init;
		}

		public override void CompileMembers(CompileState state, M.MTypeBuilder mClass)
		{
			MType mFieldType = fieldType.Compile(state);
			mField = new MFieldBuilder(-1, name, Modifiers.GetFieldAttribures(state, mods), (mods & Modifier.Static) != 0, mFieldType);
			mClass.AddField(mField);
		}

		public override void CompileCode(CompileState state)
		{
		}		
	}

	public class Constructor : Member
	{
		public List<Param> @params;
		public CtorInit ctorInit;
		public List<Stmt> body;

		internal List<Field> fieldInits;

		private MConstructorBuilder mCtor;

		public Constructor(Modifier mods, Ident name, List<Param> @params, CtorInit ctorInit, List<Stmt> body)
			: base(name.Token, mods, name)
		{
			this.@params = CheckNull<Param>(@params);
			this.ctorInit = ctorInit;
			if (body == null) throw new ArgumentNullException("body");
			this.body = body;
		}

		public override void CompileMembers(CompileState state, M.MTypeBuilder mClass)
		{
			if(name != mClass.name) throw state.ThrowTypeError(this, "Constructor '{0}' must have the same name as its type '{0}'.", name, mClass.name);
			if ((mods & Modifier.Static) != 0)
			{
				if (@params.Count != 0) throw state.ThrowTypeError(this, "Static constructor cannot have parameters.");
				if (ctorInit != null) throw state.ThrowTypeError(this, "Static constructor cannot have an initialiser.");
				mCtor = new MStaticConstructorBuilder(-1);
				mClass.AddConstructor(mCtor);
			}
			else
			{
				ThisDecl mThis = new ThisDecl(mClass.MakeType());
				ParamDecl[] mParams = Param.Compile(state, @params);
				mCtor = new MConstructorBuilder(-1, Modifiers.GetMethodAttribures(state, mods), mThis, mParams);
				mClass.AddConstructor(mCtor);
			}
		}

		public override void CompileCode(CompileState state)
		{
			state.PushMethod(mCtor.@this, mCtor.@params, M.PrimType.Void);
			List<Code> code = new List<Code>();
			if ((mods & Modifier.Static) == 0)
			{
				if (ctorInit != null)
				{
					code.Add(ctorInit.CompileCode(state));
				}
				else
				{
					code.Add(CtorInit.CompileCodeDefault(state, @params.Count != 0));
				}
				foreach (Field field in fieldInits)
				{
					MFieldInfo mField = field.mField.MakeField(state.GetCurrentClass().GetTypeArguments());
					M.Code mThis = state.LookupThisVar();
					Code mExpr = field.initExpr.CompileRValue(state);
					if (!Code.MakeCompatible(ref mExpr, mField.GetFieldType()))
						state.ThrowTypeError(this, "The type of the field '{0}' is '{1}' but the initialisation expression has type '{2}' which is not assignment compatible.", name, mField.GetFieldType(), mExpr.GetMType());
					code.Add(new M.Assign(new FieldAccess(mThis, mField), AssignOp.Nop, mExpr));
				}
			}
			else
			{
				if (ctorInit != null)
					state.ThrowTypeError(this, "Static constructor cannot contain base constructor call.");
				foreach (Field field in fieldInits)
				{
					MFieldInfo mField = field.mField.MakeField(state.GetCurrentClass().GetTypeArguments());
					Code mExpr = field.initExpr.CompileRValue(state);
					if (!Code.MakeCompatible(ref mExpr, mField.GetFieldType()))
						state.ThrowTypeError(this, "The type of the field '{0}' is '{1}' but the initialisation expression has type '{2}' which is not assignment compatible.", name, mField.GetFieldType(), mExpr.GetMType());
					code.Add(new M.Assign(new FieldAccess(null, mField), AssignOp.Nop, mExpr));
				}
			}
			code.Add(Stmt.Compile(state, body));
			state.PopMethod();
			mCtor.stmt = CodeGroup.MakeCode(code.ToArray());
		}
	}

	public class CtorInit : Node
	{
		public bool onBase;
		public List<Arg> exprs;

		public CtorInit(IToken token, bool onBase, List<Arg> exprs)
			: base(token)
		{
			this.onBase = onBase;
			this.exprs = CheckNull<Arg>(exprs);
		}

		public Code CompileCode(CompileState state)
		{
			M.Code[] mArgs = Arg.Compile(state, exprs);
			MType mClass = state.GetCurrentClass();
			if (onBase) mClass = mClass.GetSuperType();
			MConstructorInfo ctor = mClass.GetConstructor(Code.GetParamTypes(mArgs));
			return new M.BaseConstructorInvoke(ctor, mArgs);
		}

		public static Code CompileCodeDefault(CompileState state, bool checkThis)
		{
			MType mClass = state.GetCurrentClass();
			MConstructorInfo ctor = null;
			if(checkThis)
				ctor = mClass.GetConstructor(new Metaphor.ParamType[] {});
			if(ctor == null)
			{
				mClass = mClass.GetSuperType();
				ctor = mClass.GetConstructor(new Metaphor.ParamType[] {});
			}
			if(ctor == null)
			{
				state.ThrowTypeError(null, "No default base constructors on the type '{0}'; must specify one to call.", mClass.GetName());
			}
			return new M.BaseConstructorInvoke(ctor, Code.Empty);
		}
	}

	public class Method : Member
	{
		public Typ returnType;
		public List<Tuple<Ident, int>> typeParamNames;
		public List<Param> @params;
		public List<TypeParam> typeParams;
		public List<Stmt> body;

		private MMethodBuilder mMethod;

		public Method(Modifier mods, MethodHeader head, List<Stmt> body)
			: base(head.type.Token, mods, head.name)
		{
			if (head.type == null) throw new ArgumentNullException("head.type");
			this.returnType = head.type;
			this.typeParamNames = CheckNull<Tuple<Ident, int>>(head.typeParamNames);
			this.@params = CheckNull<Param>(head.@params);
			this.typeParams = CheckNull<TypeParam>(head.typeParams);
			this.body = body;
		}

		public override void CompileMembers(CompileState state, M.MTypeBuilder mClass)
		{
			TypeVarDecl[] typeVars = TypeParam.Compile(state, typeParamNames, typeParams, TypeParam.Location.Method);
			state.PushTypeVars(typeVars);
			ThisDecl mThis = (mods & Modifier.Static) != 0 ? null : new ThisDecl(mClass.MakeType());
			ParamDecl[] mParams = Param.Compile(state, @params);
			MType mRetType = returnType.Compile(state);
			state.PopTypeVar();
			mMethod = new MMethodBuilder(-1, name, Modifiers.GetMethodAttribures(state, mods), typeVars, mThis, mParams, mRetType);
			mClass.AddMethod(mMethod);
		}

		public override void CompileCode(CompileState state)
		{
			if ((mods & Modifier.Abstract) != 0 && body == null) return;
			else if ((mods & Modifier.Abstract) == 0 && body != null)
			{
				state.PushTypeVars(mMethod.typeParams);
				state.PushMethod(mMethod.@this, mMethod.@params, mMethod.retType);
				Code code = Stmt.Compile(state, body);
				state.PopMethod();
				state.PopTypeVar();
				mMethod.stmt = code;
			}
			else if ((mods & Modifier.Abstract) == 0 && body == null)
				throw state.ThrowTypeError(this, "The method '{0}' does not have a body but is not marked abstract.", name);
			else throw state.ThrowTypeError(this, "The abstract method '{0}' cannot have a body.", name);
		}
	}

	public class MethodHeader
	{
		public Typ type;
		public Ident name;
		public List<Tuple<Ident, int>> typeParamNames;
		public List<Param> @params;
		public List<TypeParam> typeParams;

		public MethodHeader(Typ type, Ident name, List<Tuple<Ident, int>> typeParamNames, List<Param> @params, List<TypeParam> typeParams)
		{
			this.type = type;
			this.name = name;
			this.typeParamNames = typeParamNames;
			this.@params = @params;
			this.typeParams = typeParams;
		}
	}

	public class NestedType : Member
	{
		public NestedType(TypeDecl decl)
			: base(decl.Token, decl.mods, new Ident(null, decl.name))
		{
		}

		public override void CompileMembers(CompileState state, MTypeBuilder mClass)
		{
			state.ThrowNotImplemented(this, "nested types");
		}

		public override void CompileCode(CompileState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}