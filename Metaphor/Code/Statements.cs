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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Metaphor.Collections;

namespace Metaphor
{
	public abstract class Stmt: Code
	{
	    protected Stmt()
		{
		}

        public override MType GetMType()
        {
            return PrimType.Void;
        }
	}

	public partial class Empty : Stmt
	{
		public Empty()
		{
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
		}

		internal override void CodeGen(CodeGenState state)
		{
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.MakeCode(typeof(Empty));
		}

	}

	public partial class Block : Stmt
	{
		public Code code;

		public Block(Code code)
		{
			this.code = code;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			state.PushScope();
			code.CodeGenClosures(state);
			state.PopScope();
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.code.BeginScope();
			code.CodeGen(state);
			state.code.EndScope();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			code.CodeGenMeta(state);
			state.MakeCode(typeof(Block));
		}

	}

	public partial class DeclareLocal : Stmt
	{
		public LocalDecl decl;
		public Code init;

		public DeclareLocal(LocalDecl decl, Code init)
		{
			this.decl = decl;
			this.init = init;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			state.PushLocal(decl);
			decl.type.GenerateClosures(state);
			if (init != null)
			{
				init.CodeGenClosures(state);
			}
		}

		internal override void CodeGen(CodeGenState state)
		{
			int pos = state.DeclareLocal(decl.GetSystemType());
			decl.SetLocation(pos);
			if(init != null)
			{
				ObjectCreate objCreate = init as ObjectCreate;
				if (objCreate != null && decl.GetMType().IsValueType())
				{
					state.EmitLdloca(pos);
					objCreate.CodeGenAssignValueType(state);
				}
				else
				{
					init.CodeGen(state);
					state.EmitStloc(pos);
				}
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			decl.GenerateCode(state);
			decl.CodeGenMeta(state);
			Code.CodeGenMeta(state, init);
			state.MakeCode(typeof(DeclareLocal));
		}

	}

	public partial class If : Stmt
	{
		public Code cond;
		public Code ifTrue;
		public Code ifFalse;

		public If(Code cond, Code ifTrue, Code ifFalse)
		{
			this.cond = cond;
			this.ifTrue = ifTrue;
			this.ifFalse = ifFalse;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			cond.CodeGenClosures(state);
			ifTrue.CodeGenClosures(state);
			if(ifFalse != null) ifFalse.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			if(ifFalse != null)
			{
				Label falseBlock = state.code.DefineLabel();
				cond.CodeGenBranch(state, false, falseBlock);
				ifTrue.CodeGen(state);
				Label endIf = state.code.DefineLabel();
				if(!ifTrue.IsBranch()) state.code.Emit(OpCodes.Br, endIf);
				state.code.MarkLabel(falseBlock);
				ifFalse.CodeGen(state);
				state.code.MarkLabel(endIf);
			}
			else
			{
				Label endIf = state.code.DefineLabel();
				cond.CodeGenBranch(state, false, endIf);
				ifTrue.CodeGen(state);
				state.code.MarkLabel(endIf);
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			cond.CodeGenMeta(state);
			ifTrue.CodeGenMeta(state);
			Code.CodeGenMeta(state, ifFalse);
			state.MakeCode(typeof(If));
		}

	}

	public partial class Loop : Stmt
	{
		public bool preTest;
		public Code cond;
		public Code body;

		public Loop(bool preTest, Code cond, Code body)
		{
			this.preTest = preTest;
			this.cond = cond;
			this.body = body;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			cond.CodeGenClosures(state);
			body.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			if (preTest)
			{
				Label begin, end;
				state.PushLoop(out begin, out end);
				state.EmitLabel(begin);
				cond.CodeGenBranch(state, false, end);
				body.CodeGen(state);
				state.code.Emit(OpCodes.Br, begin);
				state.EmitLabel(end);
			}
			else
			{
				Label begin, end;
				state.PushLoop(out begin, out end);
				state.EmitLabel(begin);
				body.CodeGen(state);
				cond.CodeGenBranch(state, true, begin);
				state.EmitLabel(end);
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitBool(preTest);
			cond.CodeGenMeta(state);
			body.CodeGenMeta(state);
			state.MakeCode(typeof(Loop));
		}
	}

	public class Break : Branch
	{
		public Break()
		{
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitBreak();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.MakeCode(typeof(Break));
		}
	}

	public class Continue : Branch
	{
		public Continue()
		{
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitContinue();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.MakeCode(typeof(Continue));
		}
	}

	public class ForField : Stmt
	{
		private static int gensym = 0;

		public int sym;
		public bool isStatic;
		public LocalReflTypeVarDecl fieldType;
		public string name;
		public MType type;
		public Code body;

		[NonSerialized]
		internal int pos;

		public ForField(int sym, bool isStatic, LocalReflTypeVarDecl fieldType, string name, MType type)
		{
			this.sym = sym != -1 ? sym : gensym++;
			this.isStatic = isStatic;
			this.fieldType = fieldType;
			this.name = name;
			this.type = type;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			state.PushField(this);
			type.GenerateClosures(state);
			body.CodeGenClosures(state);
			state.PopField();
		}

		private static readonly MethodInfo getFields = typeof(MType).GetMethod("GetFields", new Type[] { typeof(bool) });
		private static readonly MethodInfo getEnumerator = typeof(MFieldInfo[]).GetMethod("GetEnumerator");
		private static readonly MethodInfo moveNext = typeof(IEnumerator).GetMethod("MoveNext");
		private static readonly MethodInfo getCurrent = typeof(IEnumerator).GetMethod("get_Current");
		private static readonly MethodInfo getFieldType = typeof(MFieldInfo).GetMethod("GetFieldType");

		internal override void CodeGen(CodeGenState state)
		{
			type.CodeGen(state);
			state.EmitBool(isStatic);
			state.code.Emit(OpCodes.Callvirt, getFields);
			state.code.Emit(OpCodes.Callvirt, getEnumerator);
			int enumPos = state.DeclareLocal(typeof(IEnumerator));
			state.EmitStloc(enumPos);

			Label begin, end;
			state.PushLoop(out begin, out end);
			state.code.MarkLabel(begin);
			state.EmitLdloc(enumPos);
			state.code.Emit(OpCodes.Callvirt, moveNext);
			state.code.Emit(OpCodes.Brfalse, end);
			state.EmitLdloc(enumPos);
			state.code.Emit(OpCodes.Callvirt, getCurrent);
			state.code.Emit(OpCodes.Castclass, typeof(MFieldInfo));
			pos = state.DeclareLocal(typeof(MFieldInfo));
			fieldType.Generate(delegate(ILGenerator code)
			{
				EmitHelper.EmitLdloc(code, pos);
				code.Emit(OpCodes.Callvirt, getFieldType);
			});
			state.EmitStloc(pos);
			body.CodeGen(state);
			state.code.Emit(OpCodes.Br, begin);
			state.code.MarkLabel(end);
			state.PopLoop();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

    public class WithType : Stmt
    {
        public LocalReflTypeVarDecl decl;
        public Code type;
        public Code body;

        [NonSerialized]
        internal int pos;

        public WithType(LocalReflTypeVarDecl decl, Code type, Code body)
        {
            this.decl = decl;
            this.type = type;
            this.body = body;
        }

        internal override void CodeGenClosures(TypeCheckState state)
        {
            type.CodeGenClosures(state);
            state.PushTypeVar(decl);
            body.CodeGenClosures(state);
            state.PopTypeVars();
        }

        private static readonly MethodInfo getFields = typeof(MType).GetMethod("GetFields", new Type[] { typeof(bool) });
        private static readonly MethodInfo getEnumerator = typeof(MFieldInfo[]).GetMethod("GetEnumerator");
        private static readonly MethodInfo moveNext = typeof(IEnumerator).GetMethod("MoveNext");
        private static readonly MethodInfo getCurrent = typeof(IEnumerator).GetMethod("get_Current");
        private static readonly MethodInfo getFieldType = typeof(MFieldInfo).GetMethod("GetFieldType");

        internal override void CodeGen(CodeGenState state)
        {
            int pos = state.DeclareLocal(typeof(MType));
            type.CodeGen(state);
            state.EmitStloc(pos);
            decl.Generate(delegate(ILGenerator code) { EmitHelper.EmitLdloc(code, pos); });
            body.CodeGen(state);
        }

        internal override void CodeGenMeta(CodeGenState state)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

	public enum AssignOp { Nop, Add, Sub, Mul, Div, Mod, And, Xor, Or, Shl, Shr, Semi }

	public enum AssignRet { None, Post, Pre }

	public partial class Assign : Stmt
	{
		public Code lhs;
		public AssignOp op;
		public Code rhs;
		public AssignRet ret = AssignRet.None;

		public Assign(Code lhs, AssignOp op, Code rhs)
		{
			this.lhs = lhs;
			this.op = op;
			this.rhs = rhs;
		}

		public override MType GetMType()
		{
			if (ret == AssignRet.None) return PrimType.Void;
			else return lhs.GetMType();
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			lhs.CodeGenClosures(state);
			rhs.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			ObjectCreate objCreate = rhs as ObjectCreate;
			int pos = -1;
			if (objCreate != null && lhs.GetMType().IsValueType())
			{
				lhs.CodeGenRef(state);
				if(ret == AssignRet.Post) 
				{
					state.code.Emit(OpCodes.Dup);
					Type sysType = lhs.GetMType().GetSystemType();
					state.code.Emit(OpCodes.Ldobj, sysType);
					pos = state.DeclareLocal(sysType);
					state.EmitStloc(pos);
				}
				
				objCreate.CodeGenAssignValueType(state);
				if(ret == AssignRet.Pre) lhs.CodeGen(state);
				if (ret == AssignRet.Post) state.EmitLdloc(pos);
			}
			lhs.CodeGenAssign(state, op, rhs, ret);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			lhs.CodeGenMeta(state);
			state.EmitInt((int)op);
			rhs.CodeGenMeta(state);
			state.MakeCode(typeof(Assign));
		}

		private static readonly MethodInfo semi = typeof(CodeGroup).GetMethod("MakeCode2", BindingFlags.Static | BindingFlags.Public);

		internal static void CodeGenAssignOp(ILGenerator code, AssignOp op)
		{
			switch (op)
			{
				case AssignOp.Nop:
					break;
				case AssignOp.Add:
					code.Emit(OpCodes.Add);
					break;
				case AssignOp.Sub:
					code.Emit(OpCodes.Sub);
					break;
				case AssignOp.Mul:
					code.Emit(OpCodes.Mul);
					break;
				case AssignOp.Div:
					code.Emit(OpCodes.Div);
					break;
				case AssignOp.Mod:
					code.Emit(OpCodes.Rem);
					break;
				case AssignOp.And:
					code.Emit(OpCodes.And);
					break;
				case AssignOp.Xor:
					code.Emit(OpCodes.Xor);
					break;
				case AssignOp.Or:
					code.Emit(OpCodes.Or);
					break;
				case AssignOp.Shl:
					code.Emit(OpCodes.Shl);
					break;
				case AssignOp.Shr:
					code.Emit(OpCodes.Shr);
					break;
				case AssignOp.Semi:
					code.Emit(OpCodes.Call, semi);
					break;
				default:
					throw new InvalidOperationException("Unknown value for enum AssignOp.");
			}
		}
	}

	public abstract partial class Branch : Stmt
	{
		public override bool IsBranch()
		{
			return true;
		}
	}

	public partial class Return : Branch
	{
		public Code retVal;

		public Return(Code retVal)
		{
			this.retVal = retVal;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			if (retVal != null) retVal.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			Code.CodeGen(state, retVal);
			state.EmitReturn();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			Code.CodeGenMeta(state, retVal);
			state.MakeCode(typeof(Return));
		}

	}

	public partial class Throw : Branch
	{
		public Code exc;

		public Throw(Code exc)
		{
			this.exc = exc;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			exc.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			exc.CodeGen(state);
			state.code.Emit(OpCodes.Throw);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			exc.CodeGenMeta(state);
			state.MakeCode(typeof(Throw));
		}

	}

	public class BaseConstructorInvoke : Stmt
	{
		public MConstructorInfo ctor;
		public Code[] args;

		public BaseConstructorInvoke(MConstructorInfo ctor, Code[] args)
		{
			this.ctor = ctor;
			this.args = args;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			Code.CodeGenClosures(state, args);
		}
		
		internal override void CodeGen(CodeGenState state)
		{
			state.EmitLdarg(0);
			foreach (Code arg in args) arg.CodeGen(state);
			ctor.CodeGenBase(state);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			ctor.CodeGenMeta(state);
			Code.CodeGenMeta(state, args);
			state.MakeCode(typeof(BaseConstructorInvoke));
		}

	}

	public partial class TypeIf: Stmt
	{
		public TypeVarDecl var;
		public TypeVarDecl[] patternVars;
		public MType pattern;
		public Code ifTrue;
		public Code ifFalse;

		public TypeIf(TypeVarDecl var, TypeVarDecl[] patternVars, MType pattern, Code ifTrue, Code ifFalse)
		{
			this.var = var;
			this.patternVars = patternVars;
			this.pattern = pattern;
			this.ifTrue = ifTrue;
			this.ifFalse = ifFalse;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			state.PushTypeVars(patternVars);
			pattern.GenerateClosures(state);
			ifTrue.CodeGenClosures(state);
			state.PopTypeVars();
			if (ifFalse != null) ifFalse.CodeGenClosures(state);
		}

		private static readonly MethodInfo isCompatible = typeof(TypeVarCon).GetMethod("IsCompatibleType");
		private static readonly MethodInfo isEqualTo = typeof(MType).GetMethod("IsEqualTo");
		//private static readonly MethodInfo isSubTypeOf = typeof(MType).GetMethod("IsSubTypeOf");
		//private static readonly MethodInfo isValueType = typeof(MType).GetMethod("IsValueType");
		//private static readonly MethodInfo hasDefaultConstructor = typeof(MType).GetMethod("HasDefaultConstrcutor");

		internal override void CodeGen(CodeGenState state)
		{
			PatternVar varPattern = pattern as PatternVar;
			if (varPattern != null)
			{
				int index = Array.IndexOf<TypeVarDecl>(patternVars, varPattern.decl);
				if (index == -1) throw new InvalidOperationException();
				varPattern.decl.Generate(delegate { var.CodeGen(state); });
				varPattern.decl.con.CodeGen(state);
				var.CodeGen(state);
				state.code.Emit(OpCodes.Call, isCompatible);
			}
			else if (!pattern.IsGeneric() || pattern is SystemType)
			{
				pattern.CodeGen(state);
				var.CodeGen(state);
				state.code.Emit(OpCodes.Callvirt, isEqualTo);
			}
			else
			{
				throw new NotImplementedException();
			}
			Label @else = state.code.DefineLabel();
			state.code.Emit(OpCodes.Brfalse, @else);

			//else
			//{
			//    TypeVarCon typeVarCon = con.Right;
			//    int numTests = 0;
			//    if (typeVarCon.superType != null) numTests++;
			//    if (typeVarCon.kind != TypeKind.Any) numTests++;
			//    if (typeVarCon.hasNew) numTests++;
			//    if (typeVarCon.interfaces != null) numTests += typeVarCon.interfaces.Count;

			//    if (typeVarCon.superType != null)
			//    {
			//        if (--numTests > 0) state.code.Emit(OpCodes.Dup);
			//        typeVarCon.superType.CodeGen(state);
			//        state.code.Emit(OpCodes.Callvirt, isSubTypeOf);
			//        state.code.Emit(OpCodes.Brfalse, @else);
			//    }
			//    if (typeVarCon.kind != TypeKind.Any)
			//    {
			//        if (--numTests > 0) state.code.Emit(OpCodes.Dup);
			//        state.code.Emit(OpCodes.Callvirt, isValueType);
			//        state.code.Emit(typeVarCon.kind == TypeKind.Class ? OpCodes.Brtrue : OpCodes.Brfalse, @else);
			//    }
			//    if (typeVarCon.hasNew)
			//    {
			//        if (--numTests > 0) state.code.Emit(OpCodes.Dup);
			//        state.code.Emit(OpCodes.Callvirt, hasDefaultConstructor);
			//        state.code.Emit(OpCodes.Brfalse, @else);
			//    }
			//    if (typeVarCon.interfaces != null)
			//    {
			//        foreach (MType iface in typeVarCon.interfaces)
			//        {
			//            if (--numTests > 0) state.code.Emit(OpCodes.Dup);
			//            state.code.Emit(OpCodes.Callvirt, isSubTypeOf);
			//            state.code.Emit(OpCodes.Brfalse, @else);
			//        }
			//    }
			//}

			ifTrue.CodeGen(state);
			
			if (ifFalse == null) state.code.MarkLabel(@else);
			else
			{
				Label end = state.code.DefineLabel();
				if (!ifTrue.IsBranch()) state.code.Emit(OpCodes.Br, end);
				state.code.MarkLabel(@else);
				ifFalse.CodeGen(state);
				state.code.MarkLabel(end);
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}

	}

	public partial class Pop : Stmt
	{
		public Code expr;

		public Pop(Code expr)
		{
			this.expr = expr;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			expr.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			Assign assignExpr = expr as Assign;
			if (assignExpr != null)
			{
				assignExpr.ret = AssignRet.None;
				assignExpr.CodeGen(state);
			}
			else
			{
				expr.CodeGen(state);
				state.EmitPop();
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			expr.CodeGenMeta(state);
			state.MakeCode(typeof(Pop));
		}
	}
}