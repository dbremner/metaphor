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
	public partial class Lift : Code
	{
		public Code expr;

		public Lift(Code expr)
		{
			this.expr = expr;
		}

        public override MType GetMType()
        {
            return LiftType.Promote(expr.GetMType());
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			state.LowerLevel();
			expr.CodeGenClosures(state);
			state.RaiseLevel();
		}

		internal override void CodeGen(CodeGenState state)
		{
			throw new InvalidOperationException("Lift called at stage 0.");
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			if(state.level == 1)
			{
				state.level = 0;
				expr.CodeGen(state);
				state.level = 1;
				expr.GetMType().MakeLiteral(state);
			}
			else
			{
				state.level--;
				expr.CodeGenMeta(state);
				state.level++;
				state.MakeCode(typeof(Lift));
			}
		}

	}

	public partial class Box : Code
	{
		public Code expr;

		public Box(Code expr)
		{
			this.expr = expr;
		}

        public override MType GetMType()
        {
            return expr.GetMType();
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			expr.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			expr.CodeGen(state);
			state.code.Emit(OpCodes.Box, GetMType().GetSystemType());
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
				expr.CodeGenMeta(state);
				state.MakeCode(typeof(Box));
		}
	}

	public class Convert : Code
	{
		public Code expr;
		public CILType target;

		public Convert(Code expr, CILType target)
		{
			this.expr = expr;
			this.target = target;
		}

        public override MType GetMType()
        {
			switch (target)
			{
				case CILType.U1: return PrimType.Byte;
				case CILType.I2: return PrimType.Short;
				case CILType.I4: return PrimType.Int;
				case CILType.I8: return PrimType.Long;
				case CILType.R4: return PrimType.Float;
				case CILType.R8: return PrimType.Double;
				default: throw new Exception(string.Format("Cannot convert to '{0}' or not implemented.", target));
			}
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			expr.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			expr.CodeGen(state);
			switch (target)
			{
				case CILType.U1:
					state.code.Emit(OpCodes.Conv_U1);
					break;
				case CILType.I2:
					state.code.Emit(OpCodes.Conv_I2);
					break;
				case CILType.I4:
					state.code.Emit(OpCodes.Conv_I4);
					break;
				case CILType.I8:
					state.code.Emit(OpCodes.Conv_I8);
					break;
				case CILType.R4:
					state.code.Emit(OpCodes.Conv_R4);
					break;
				case CILType.R8:
					state.code.Emit(OpCodes.Conv_R8);
					break;
				default:
					throw new Exception(string.Format("Cannot convert to '{0}' or not implemented.", target));
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			expr.CodeGenMeta(state);
			state.EmitInt((int)target);
			state.MakeCode(typeof(Convert));
		}
	}
	
#region Variables
	public partial class Ref : Code
	{
		public Code expr;

		public Ref(Code expr)
		{
			this.expr = expr;
		}

		public override MType GetMType()
		{
			return expr.GetMType();
		}

		public override ParamType GetParamType()
		{
			return ParamType.Create(expr.GetMType(), true);
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			expr.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			Code.CodeGenRef(state, expr);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			expr.CodeGenMeta(state);
			state.MakeCode(typeof(Ref));
		}
	}

	public partial class Var : Code
	{
		public VarDecl decl;

		[NonSerializedAttribute]
		private Location location;

		public Var(VarDecl decl)
		{
			this.decl = decl;
		}

        public override MType GetMType()
        {
			return decl.GetMType();
        }

		public override bool IsAssignable()
		{
			return true;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			if(state.GetLevel() == 0) location = state.LookupVar(decl);
		}

		internal override void CodeGen(CodeGenState state)
		{
			location.CodeGen(state.code);
		}

		internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		{
			location.CodeGenAssign(state.code, op, delegate { rhs.CodeGen(state); }, ret);
		}

		internal override void CodeGenRef(CodeGenState state)
		{
			location.CodeGenRef(state.code);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			decl.CodeGenMeta(state);
			state.MakeCode(typeof(Var));
		}
	}
#endregion
#region Operators
	public enum UnaryOpCode { Nop, Not, Neg }

	public partial class UnaryOp : Code
	{
		public UnaryOpCode op;
		public Code a;

		public UnaryOp(UnaryOpCode op, Code a)
		{
			this.op = op;
			this.a = a;
		}

        public override MType GetMType()
        {
            return a.GetMType();
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			a.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			a.CodeGen(state);
			if (op == UnaryOpCode.Nop) { }
			else if (op == UnaryOpCode.Not)
			{
				state.code.Emit(OpCodes.Ldc_I4_0);
				state.code.Emit(OpCodes.Ceq);
			}
			else if (op == UnaryOpCode.Neg)
				state.code.Emit(OpCodes.Neg);
			else throw new Exception();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt((int)op);
			a.CodeGenMeta(state);
			state.MakeCode(typeof(UnaryOp));
		}
	}

	public abstract class BinaryOp : Code
	{
		public Code a, b;

		public BinaryOp(Code a, Code b)
		{
			this.a = a;
			this.b = b;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			a.CodeGenClosures(state);
			b.CodeGenClosures(state);
		}
	}

	public enum BinaryEqualityOpCode { Ne, Eq };

	public partial class BinaryEqualityOp : BinaryOp
	{
		public BinaryEqualityOpCode op;

		public BinaryEqualityOp(BinaryEqualityOpCode op, Code a, Code b) : base(a, b)
		{
			this.op = op;
		}

        public override MType GetMType()
        {
            return PrimType.Boolean;
        }

		internal override void CodeGen(CodeGenState state)
		{
			a.CodeGen(state);
			b.CodeGen(state);
			switch (op)
			{
				case BinaryEqualityOpCode.Eq:
					state.code.Emit(OpCodes.Ceq);
					break;
				case BinaryEqualityOpCode.Ne:
					state.code.Emit(OpCodes.Ceq);
					state.code.Emit(OpCodes.Ldc_I4_0);
					state.code.Emit(OpCodes.Ceq);
					break;
				default:
					throw new Exception();
			}
		}

		internal override void CodeGenBranch(CodeGenState state, bool when, Label where)
		{
			a.CodeGen(state);
			b.CodeGen(state);
			switch (op)
			{
				case BinaryEqualityOpCode.Eq:
					if(when) state.code.Emit(OpCodes.Beq, where);
					else state.code.Emit(OpCodes.Bne_Un, where);
					break;
				case BinaryEqualityOpCode.Ne:
					if (when) state.code.Emit(OpCodes.Bne_Un, where);
					else state.code.Emit(OpCodes.Beq, where);
					break;
				default:
					throw new Exception();
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt((int)op);
			a.CodeGenMeta(state);
			b.CodeGenMeta(state);
			state.MakeCode(typeof(BinaryEqualityOp));
		}

	}

	public enum BinaryRelationalOpCode { Lt, Le, Ge, Gt };

	public partial class BinaryRelationalOp : BinaryOp
	{
		public BinaryRelationalOpCode op;

		public BinaryRelationalOp(BinaryRelationalOpCode op, Code a, Code b): base(a, b)
		{
			this.op = op;
		}

        public override MType GetMType()
        {
            return PrimType.Boolean;
        }

		internal override void CodeGen(CodeGenState state)
		{
			a.CodeGen(state);
			b.CodeGen(state);
			switch (op)
			{
				case BinaryRelationalOpCode.Lt:
					state.code.Emit(OpCodes.Clt);
					break;
				case BinaryRelationalOpCode.Le:
					state.code.Emit(OpCodes.Cgt);
					state.code.Emit(OpCodes.Ldc_I4_0);
					state.code.Emit(OpCodes.Ceq);
					break;
				case BinaryRelationalOpCode.Ge:
					state.code.Emit(OpCodes.Clt);
					state.code.Emit(OpCodes.Ldc_I4_0);
					state.code.Emit(OpCodes.Ceq);
					break;
				case BinaryRelationalOpCode.Gt:
					state.code.Emit(OpCodes.Cgt);
					break;
				default:
					throw new Exception();
			}
		}

		internal override void CodeGenBranch(CodeGenState state, bool when, Label where)
		{
			a.CodeGen(state);
			b.CodeGen(state);
			switch (op)
			{
				case BinaryRelationalOpCode.Lt:
					if (when) state.code.Emit(OpCodes.Blt, where);
					else state.code.Emit(OpCodes.Bge, where);
					break;
				case BinaryRelationalOpCode.Le:
					if (when) state.code.Emit(OpCodes.Ble, where);
					else state.code.Emit(OpCodes.Bgt, where);
					break;
				case BinaryRelationalOpCode.Ge:
					if (when) state.code.Emit(OpCodes.Bge, where);
					else state.code.Emit(OpCodes.Blt, where);
					break;
				case BinaryRelationalOpCode.Gt:
					if (when) state.code.Emit(OpCodes.Bgt, where);
					else state.code.Emit(OpCodes.Ble, where);
					break;
				default:
					throw new Exception();
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt((int)op);
			a.CodeGenMeta(state);
			b.CodeGenMeta(state);
			state.MakeCode(typeof(BinaryRelationalOp));
		}
	}

	public enum BinaryLogicalOpCode { Or, And };

	public partial class BinaryLogicalOp : BinaryOp
	{
		public BinaryLogicalOpCode op;

		public BinaryLogicalOp(BinaryLogicalOpCode op, Code a, Code b) : base(a, b)
		{
			this.op = op;
		}

        public override MType GetMType()
        {
            return PrimType.Boolean;
        }

		internal override void CodeGen(CodeGenState state)
		{
			a.CodeGen(state);
			Label snd = state.code.DefineLabel();
			Label end = state.code.DefineLabel();
			switch (op)
			{
				case BinaryLogicalOpCode.Or:
					state.code.Emit(OpCodes.Brfalse, snd);
					state.code.Emit(OpCodes.Ldc_I4_1);
					state.code.Emit(OpCodes.Br, end);
					break;
				case BinaryLogicalOpCode.And:
					state.code.Emit(OpCodes.Brtrue, snd);
					state.code.Emit(OpCodes.Ldc_I4_0);
					state.code.Emit(OpCodes.Br, end);
					break;
				default:
					throw new Exception();
			}
			state.code.MarkLabel(snd);
			b.CodeGen(state);
			state.code.MarkLabel(end);
		}

		internal override void  CodeGenBranch(CodeGenState state, bool when, Label where)
		{
			Label snd = state.code.DefineLabel();
			Label end = state.code.DefineLabel();
			switch (op)
			{
				case BinaryLogicalOpCode.Or:
					a.CodeGenBranch(state, false, snd);
					state.code.Emit(OpCodes.Brfalse, snd);
					if (when) state.code.Emit(OpCodes.Br, where);
					else state.code.Emit(OpCodes.Br, end);
					break;
				case BinaryLogicalOpCode.And:
					a.CodeGenBranch(state, true, snd);
					if (!when) state.code.Emit(OpCodes.Br, where);
					else state.code.Emit(OpCodes.Br, end);
					break;
				default:
					throw new Exception();
			}
			state.code.MarkLabel(snd);
			b.CodeGenBranch(state, when, where);
			state.code.MarkLabel(end);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt((int)op);
			a.CodeGenMeta(state);
			b.CodeGenMeta(state);
			state.MakeCode(typeof(BinaryLogicalOp));
		}
	}

	public enum BinaryArithmeticOpCode { Sub, Add, Mod, Div, Mul, Shl, Shr };

	public partial class BinaryArithmeticOp : BinaryOp
	{
		public BinaryArithmeticOpCode op;

		public BinaryArithmeticOp(BinaryArithmeticOpCode op, Code a, Code b) : base(a, b)
		{
			this.op = op;
		}

        public override MType GetMType()
        {
            return a.GetMType();
        }

		internal override void CodeGen(CodeGenState state)
		{
			a.CodeGen(state);
			b.CodeGen(state);
			switch (op)
			{
				case BinaryArithmeticOpCode.Sub:
					state.code.Emit(OpCodes.Sub);
					break;
				case BinaryArithmeticOpCode.Add:
					state.code.Emit(OpCodes.Add);
					break;
				case BinaryArithmeticOpCode.Mod:
					state.code.Emit(OpCodes.Rem);
					break;
				case BinaryArithmeticOpCode.Div:
					state.code.Emit(OpCodes.Div);
					break;
				case BinaryArithmeticOpCode.Mul:
					state.code.Emit(OpCodes.Mul);
					break;
				case BinaryArithmeticOpCode.Shl:
					state.code.Emit(OpCodes.Shl);
					break;
				case BinaryArithmeticOpCode.Shr:
					state.code.Emit(OpCodes.Shr);
					break;
				default:
					throw new Exception();
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.EmitInt((int)op);
			a.CodeGenMeta(state);
			b.CodeGenMeta(state);
			state.MakeCode(typeof(BinaryArithmeticOp));
		}
	}
#endregion
#region Member access
	public partial class FieldAccess : Code
	{
		public Code expr;
		public MFieldInfo field;

		public FieldAccess(Code expr, MFieldInfo field)
		{
			this.expr = expr;
			this.field = field;
		}

        public override MType GetMType()
        {
            return field.GetFieldType();
        }

		public override bool IsAssignable()
		{
			return true;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			if (expr != null) expr.CodeGenClosures(state);
			field.GenerateClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			if (expr != null)
			{
				MType exprType = expr.GetMType();
				if (exprType.IsValueType() || exprType is TypeVar) Code.CodeGenRef(state, expr);
				else expr.CodeGen(state);
			}
			field.CodeGen(state);
		}

		internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		{
			int pos = -1;
			if (expr != null)
			{
				MType exprType = expr.GetMType();
				if (exprType.IsValueType() || exprType is TypeVar) Code.CodeGenRef(state, expr);
				else expr.CodeGen(state);
			}
			if (op != AssignOp.Nop || ret == AssignRet.Post)
			{
				state.code.Emit(OpCodes.Dup);
				field.CodeGen(state);
				if (ret == AssignRet.Post)
				{
					state.code.Emit(OpCodes.Dup);
					pos = state.DeclareLocal(field.GetFieldType().GetSystemType());
					state.EmitStloc(pos);
				}
			}
			rhs.CodeGen(state);
			Assign.CodeGenAssignOp(state.code, op);
			if (ret == AssignRet.Pre)
			{
				state.code.Emit(OpCodes.Dup);
				pos = state.DeclareLocal(field.GetFieldType().GetSystemType());
				state.EmitStloc(pos);
			}
			field.CodeGenAssign(state);
			if (ret != AssignRet.None) state.EmitLdloc(pos);
		}

		internal override void CodeGenRef(CodeGenState state)
		{
			if (expr != null)
			{
				MType exprType = expr.GetMType();
				if (exprType.IsValueType() || exprType is TypeVar) Code.CodeGenRef(state, expr);
				else expr.CodeGen(state);
			}
			field.CodeGenRef(state);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			Code.CodeGenMeta(state, expr);
			field.CodeGenMeta(state);
			state.MakeCode(typeof(FieldAccess));
		}

	}

	[Serializable]
	public partial class MethodInvoke : Code
	{
		public Code expr;
		public MMethodInfo method;
		public Code[] args;

		public MethodInvoke(Code expr, MMethodInfo method, Code[] args)
		{
			if (expr == null && !method.IsStatic()) throw new ArgumentNullException("expr");
			this.expr = expr;
			if (method == null) throw new ArgumentNullException("method");
			this.method = method;
			if (args == null) throw new ArgumentNullException("args");
			this.args = args;
		}

        public override MType GetMType()
        {
            return method.GetReturnType();
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			LitObj obj = expr as LitObj;
			if (obj != null)
			{
				MulticastDelegate del = obj.obj as MulticastDelegate;
				if (del != null && method.GetName() == "Invoke")
				{
					if (del.Target != null) new LitObj(del.Target).CodeGenClosures(state);
				}
				else expr.CodeGenClosures(state);
			}
			else if (expr != null) expr.CodeGenClosures(state);
			Code.CodeGenClosures(state, args);
			method.GenerateClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			FunctionCreate fc = expr as FunctionCreate;
			if (fc != null)
			{
				fc.func.GetLocation().CodeGenCall(state.code, delegate { foreach (Code arg in args) arg.CodeGen(state); });
				return;
			}

			Var v = expr as Var;
			if (v != null)
			{
				Function func = v.decl as Function;
				if (func != null)
				{
					func.GetLocation().CodeGenCall(state.code, delegate { foreach (Code arg in args) arg.CodeGen(state); });
					return;
				}
			}

			LitObj obj = expr as LitObj;
			if (obj != null)
			{
				MulticastDelegate del = obj.obj as MulticastDelegate;
				if (del != null && method.GetName() == "Invoke")
				{
					object target = del.Target;
					Type targetType = null;
					if(target != null)
					{
						targetType = target.GetType();
						if (targetType.IsValueType || targetType.IsGenericParameter) Code.CodeGenRef(state, new LitObj(target));
						else new LitObj(target).CodeGen(state);
					}
					foreach (Code arg in args) arg.CodeGen(state);
					if (targetType != null && targetType.IsGenericParameter) state.code.Emit(OpCodes.Constrained, targetType);
					state.code.Emit(OpCodes.Call, del.Method);
					return;
				}
			}

			MType exprType = null;
			if (expr != null)
			{
				exprType = expr.GetMType();
				if (exprType.IsValueType() || exprType is TypeVar) Code.CodeGenRef(state, expr);
				else expr.CodeGen(state);
			}
			//Type[] types = method.CodeGen1(state);
			foreach (Code arg in args) arg.CodeGen(state);
			if (expr != null && exprType is TypeVar) state.code.Emit(OpCodes.Constrained, exprType.GetSystemType());
			method.CodeGen(state);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			if (expr == null)
			{
				state.EmitNull();
				method.CodeGenMeta(state);
			}
			else
			{
				expr.CodeGenMeta(state);
				if(method.GetDeclaringType().IsInterface())
				{
					state.code.Emit(OpCodes.Dup);
					method.CodeGenMeta(state);
					state.code.Emit(OpCodes.Call, applyInterfaceMethod);
				}
				else method.CodeGenMeta(state);
			}
			Code.CodeGenMeta(state, args);
			state.MakeCode(typeof(MethodInvoke));
		}

		private static readonly MethodInfo applyInterfaceMethod = typeof(MethodInvoke).GetMethod("ApplyInterfaceMethod", BindingFlags.Public | BindingFlags.Static);

		public static MMethodInfo ApplyInterfaceMethod(Code expr, MMethodInfo method)
		{
			MMethodInfo nonInterfaceMethod = expr.GetMType().GetMethod(method.GetName(), false, method.GetTypeArguments(), method.GetParamTypes());
			return nonInterfaceMethod != null ? nonInterfaceMethod : method;
		}

	}

	public partial class ElementAccess : Code
	{
		public Code expr;
		public Code index;
        public MType elementType;

		public ElementAccess(Code expr, Code index)
		{
			if (expr == null) throw new ArgumentNullException("expr");
			this.expr = expr;
			if (index == null) throw new ArgumentNullException("index");
			this.index = index;
			MType arrayType = expr.GetMType();
			if (!arrayType.IsArray()) throw new ArgumentException("not an array", "expr");
			this.elementType = arrayType.GetElementType();
		}

        public override MType GetMType()
        {
            return elementType;
        }

		public override bool IsAssignable()
		{
			return true;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			expr.CodeGenClosures(state);
			index.CodeGenClosures(state);
			elementType.GenerateClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			expr.CodeGen(state);
			index.CodeGen(state);
			state.EmitLdelem(elementType);
		}

		internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		{
			if (ret != AssignRet.None) throw new NotImplementedException("pre or post array element assignment");

			expr.CodeGen(state);
			index.CodeGen(state);
			if (op != AssignOp.Nop)
			{
				Type elementSystemType = elementType.GetSystemType();
				state.code.Emit(OpCodes.Ldelema, elementSystemType);
				state.code.Emit(OpCodes.Dup);
				state.code.Emit(OpCodes.Ldobj, elementSystemType);
				rhs.CodeGen(state);
				Assign.CodeGenAssignOp(state.code, op);
				state.code.Emit(OpCodes.Stobj, elementSystemType);
			}
			else
			{
				rhs.CodeGen(state);
				state.EmitStelem(elementType);
			}
		}

		internal override void CodeGenRef(CodeGenState state)
		{
			expr.CodeGen(state);
			index.CodeGen(state);
			state.code.Emit(OpCodes.Ldelema, elementType.GetSystemType());
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			expr.CodeGenMeta(state);
			index.CodeGenMeta(state);
			state.MakeCode(typeof(ElementAccess));
		}

	}

	//public delegate CodeVoid FixedArrayCont<~T>(FixedArray<T> fixedArray);
	//
	//public class FixedArray<~T> : Code<T[]>
	//{
	//    public static CodeVoid Create(int n, FixedArrayCont<T> k)
	//    {
	//        return <|? fixedArray = ?; ~k(<|fixedArray|>);|>;
	//    }
	//    public int Length();
	//    public CodeRef<T> Get(int index);
	//    public CodeVoid FromArray(Code<T[]> array);
	//    public Code<T[]> ToArray();
	//}

	public delegate Code FixedArrayCont(FixedArray fixedArray);

	public class FixedArrayCreate : Code
	{
		public MType elementType;
		public int length;
		public Code init;
		public Code k;

		[NonSerialized]
		internal int pos;

		public FixedArrayCreate()
		{
		}

		public override MType GetMType()
		{
			return k.GetMType();
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			elementType.GenerateClosures(state);
			init.CodeGenClosures(state);
			k.CodeGenClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			Type systemElementType = elementType.GetSystemType();
			pos = state.DeclareLocal(systemElementType.MakeByRefType());
			init.CodeGen(state);
			state.EmitInt(length);
			state.code.Emit(OpCodes.Call, checkArray.MakeGenericMethod(systemElementType));
			state.code.Emit(OpCodes.Ldc_I4_0);
			state.code.Emit(OpCodes.Ldelema, systemElementType);
			state.EmitStloc(pos);
			k.CodeGen(state);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private static readonly MethodInfo checkArray = typeof(FixedArrayCreate).GetMethod("CheckArray", BindingFlags.Public | BindingFlags.Static);

		public static T[] CheckArray<T>(T[] array, int length)
		{
			if (array.Length < length) throw new IndexOutOfRangeException();
			return array;
		}
	}

	public class FixedArray : Code
	{
		private FixedArrayCreate fixedArray;

		public static Code Create(int length, Code array, FixedArrayCont k)
		{
			FixedArrayCreate tmp = new FixedArrayCreate();
			FixedArray result = new FixedArray(tmp);
			if (!array.GetMType().IsArray()) throw new ArgumentException("'array' is not an array type.");
			tmp.elementType = array.GetMType().GetElementType();
			tmp.length = length;
			tmp.init = array;
			tmp.k = k(result);
			return tmp;
		}

		private FixedArray(FixedArrayCreate fixedArray)
		{
			this.fixedArray = fixedArray;
		}

		public int Length()
		{
			return fixedArray.length;
		}

		public Code Get(int index)
		{
			return new FixedElementAccess(fixedArray, index);
		}

		public Code FromArray(Code array)
		{
			throw new NotImplementedException();
		}

		public Code ToArray()
		{
			return this;
		}

		public override MType GetMType()
		{
			return new FixedArrayType(fixedArray.elementType);
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.EmitLdloc(fixedArray.pos);
			state.EmitLdind(CILType.Ref);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public partial class FixedElementAccess : Code
	{
		public FixedArrayCreate fixedArray;
		public int index;

		public FixedElementAccess(FixedArrayCreate fixedArray, int index)
		{
			if (fixedArray == null) throw new ArgumentNullException("fixedArray");
			this.fixedArray = fixedArray;
			if (index < 0 || index >= fixedArray.length) throw new ArgumentOutOfRangeException("index");
			this.index = index;
		}

		public override MType GetMType()
		{
			return fixedArray.elementType;
		}

		public override bool IsAssignable()
		{
			return true;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			//expr.CodeGenClosures(state);
			//index.CodeGenClosures(state);
			//elementType.GenerateClosures(state);
		}

		private void CodeGenAddr(CodeGenState state)
		{
			Type systemElementType = fixedArray.elementType.GetSystemType();
			state.EmitLdloc(fixedArray.pos);
			state.EmitInt(index);
			state.EmitInt(Marshal.SizeOf(systemElementType));
			state.code.Emit(OpCodes.Mul);
			state.code.Emit(OpCodes.Add);
		}

		internal override void CodeGen(CodeGenState state)
		{
			CodeGenRef(state);
			EmitHelper.EmitLdind(state.code, fixedArray.elementType.GetSystemType());
		}

		internal override void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		{
			if (ret != AssignRet.None) throw new NotImplementedException("pre or post array element assignment");
	
			CodeGenRef(state);
			Type systemElementType = fixedArray.elementType.GetSystemType();
			if (op != AssignOp.Nop)
			{
				state.code.Emit(OpCodes.Dup);
				EmitHelper.EmitLdind(state.code, systemElementType);
				rhs.CodeGen(state);
				Assign.CodeGenAssignOp(state.code, op);
				EmitHelper.EmitStind(state.code, systemElementType);
			}
			else
			{
				rhs.CodeGen(state);
				EmitHelper.EmitStind(state.code, systemElementType);
			}
		}

		internal override void CodeGenRef(CodeGenState state)
		{
			Type systemElementType = fixedArray.elementType.GetSystemType();
			state.EmitLdloc(fixedArray.pos);
			state.EmitInt(Marshal.SizeOf(systemElementType) * index);
			state.code.Emit(OpCodes.Add);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			fixedArray.CodeGenMeta(state);
			state.EmitInt(index);
			state.MakeCode(typeof(FixedElementAccess));
		}

	}
#endregion
#region Object creation
	public partial class ObjectCreate : Code
	{
		public MConstructorInfo ctor;
		public Code[] args;

		public ObjectCreate(MConstructorInfo ctor, Code[] args)
		{
			this.ctor = ctor;
			this.args = args;
		}

        public override MType GetMType()
        {
            return ctor.GetDeclaringType();
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			Code.CodeGenClosures(state, args);
			ctor.GenerateClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			foreach (Code arg in args) arg.CodeGen(state);
			ctor.CodeGen(state);
		}

		internal void CodeGenAssignValueType(CodeGenState state)
		{
			if (ctor is ValueTypeConstructor)
			{
				state.code.Emit(OpCodes.Initobj, ctor.GetDeclaringType().GetSystemType());
			}
			else
			{
				foreach (Code arg in args) arg.CodeGen(state);
				state.code.Emit(OpCodes.Call, ctor.GetSystemConstructor());
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			ctor.CodeGenMeta(state);
			Code.CodeGenMeta(state, args);
			state.MakeCode(typeof(ObjectCreate));
		}
	}

	public partial class ArrayCreate : Code
	{
		public MType elementType;
		public Code[] dims;
		public Code[] init;

		public ArrayCreate(MType elementType, Code[] dims, Code[] init)
		{
			this.elementType = elementType;
			this.dims = dims;
			this.init = init;
		}

        public override MType GetMType()
        {
            return ArrayType.Create(elementType, dims.Length);
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			elementType.GenerateClosures(state);
			Code.CodeGenClosures(state, dims);
			if(init != null) Code.CodeGenClosures(state, init);
		}

		internal override void CodeGen(CodeGenState state)
		{
			foreach(Code dim in dims) dim.CodeGen(state);
			state.code.Emit(OpCodes.Newarr, elementType.GetSystemType());
			if (init != null)
			{
				for (int i = 0; i < init.Length; i++)
				{
					state.code.Emit(OpCodes.Dup);
					state.EmitInt(i);
					init[i].CodeGen(state);
					state.EmitStelem(elementType);
				}
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			elementType.CodeGenMeta(state);
			Code.CodeGenMeta(state, dims);
			Code.CodeGenMeta(state, init);
			state.MakeCode(typeof(ArrayCreate));
		}

	}

	public partial class DelegateCreate : Code
	{
        MType delegateType;
		public Code expr;
		public MMethodInfo method;

		public DelegateCreate(MType delegateType, Code expr, MMethodInfo method)
		{
            this.delegateType = delegateType;
			this.expr = expr;
			this.method = method;
		}

        public override MType GetMType()
        {
            return delegateType;
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			delegateType.GenerateClosures(state);
			if(expr != null) expr.CodeGenClosures(state);
			method.GenerateClosures(state);
		}

		internal override void CodeGen(CodeGenState state)
		{
			expr.CodeGen(state);
			method.CodeGenRef(state);
			state.CreateDelegate(delegateType.GetSystemType());
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}
	}

	public partial class FunctionCreate : Code
	{
		public Function func;

		public FunctionCreate(Function func)
		{
			this.func = func;
		}

        public override MType GetMType()
        {
			return func.delegateType;
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			if (state.GetLevel() == 0)
			{
				TypeCheckState newState = state.CreateNestedState(func);
				func.retType.GenerateClosures(newState);
				func.GenerateClosures(newState);
			}
			else
			{
				func.retType.GenerateClosures(state);
				func.stmt.CodeGenClosures(state);
			}
		}

		internal override void CodeGen(CodeGenState state)
		{
			func.GetLocation().CodeGen(state.code);		
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			func.GenerateCode(state);
			func.CodeGenMeta(state);
			state.MakeCode(typeof(FunctionCreate));
		}
	}

	public partial class CastClass : Code
	{
		public Code expr;
        public MType target;

		public CastClass(MType target, Code expr)
		{
			this.expr = expr;
            this.target = target;
		}

        public override MType GetMType()
        {
            return target;
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			expr.CodeGenClosures(state);
			target.GenerateClosures(state);
		}

		private static readonly MethodInfo castCode = typeof(CastClass).GetMethod("CastCode");

		public static Code CastCode(Code code, MType target)
		{
			CodeType codeTarget = target as CodeType;
			if (code != null && codeTarget != null)
			{
				if (codeTarget.@ref)
				{
					if (code.IsAssignable() && code.GetMType().IsEqualTo(codeTarget.codeType)) return code;
				}
				else
				{
					if (code.GetMType().IsSubTypeOf(codeTarget.codeType)) return code;
				}
			}
			throw new InvalidCastException(string.Format("Cannot cast a code object of type '{0}' to '{1}'.",
				CodeType.Create(code.GetMType(), code.IsAssignable()),
				target));
		}

		internal override void CodeGen(CodeGenState state)
		{
			expr.CodeGen(state);
			Type sysType = target.GetSystemType();
			state.code.Emit(OpCodes.Castclass, sysType);
			if (target.IsValueType())
			{
				state.code.Emit(OpCodes.Unbox, sysType);
				state.code.Emit(OpCodes.Ldobj, sysType);
			}
			else if (target is CodeType)
			{
				target.CodeGen(state);
				state.code.Emit(OpCodes.Call, castCode);
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			target.CodeGenMeta(state);
			expr.CodeGenMeta(state);
			state.MakeCode(typeof(CastClass));
		}

	}

	public partial class AsClass : Code
	{
		public Code expr;
		public MType type;

		public AsClass(Code expr, MType type)
		{
			this.expr = expr;
			this.type = type;
		}

		public override MType GetMType()
		{
			return type;
		}

		internal override void CodeGenClosures(TypeCheckState state)
		{
			expr.CodeGenClosures(state);
			type.GenerateClosures(state);
		}

		private static readonly MethodInfo asCode = typeof(AsClass).GetMethod("AsCode");

		public static Code AsCode(Code code, MType type)
		{
			CodeType codeType = type as CodeType;
			if (code != null && codeType != null)
			{
				if (codeType.@ref)
				{
					if (code.IsAssignable() && code.GetMType().IsEqualTo(codeType.codeType)) return code;
				}
				else
				{
					if (code.GetMType().IsSubTypeOf(codeType.codeType)) return code;
				}
			}
			return null;
		}

		internal override void CodeGen(CodeGenState state)
		{
			expr.CodeGen(state);
			Type sysType = type.GetSystemType();
			state.code.Emit(OpCodes.Isinst, sysType);
			if (type is CodeType)
			{
				type.CodeGen(state);
				state.code.Emit(OpCodes.Call, asCode);
			}
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			type.CodeGenMeta(state);
			expr.CodeGenMeta(state);
			state.MakeCode(typeof(AsClass));
		}
	}

	public partial class Bracket : Code
	{
		//public MTypeBuilder[] types;
		public Code code;

		public Bracket(/*MTypeBuilder[] types, */Code code)
		{
			//this.types = types;
			this.code = code;
		}

        public override MType GetMType()
        {
            return CodeType.Create(code.GetMType(), code.IsAssignable());
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			state.RaiseLevel();
			code.CodeGenClosures(state);
			state.LowerLevel();
		}

		internal override void CodeGen(CodeGenState state)
		{
			state.level = 1;
			code.CodeGenMeta(state);
			state.level = 0;
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			state.level++;
			code.CodeGenMeta(state);
			state.level--;
			state.MakeCode(typeof(Bracket));
		}
	}

    //public partial class BracketModule : Expr
    //{
    //    public MModuleBuilder module;

    //    public BracketModule(MModuleBuilder module) : base(ModuleType.Create())
    //    {
    //        this.module = module;
    //    }

    //    internal override void CodeGen(CodeGenState state)
    //    {
    //        state.level = 1;
    //        module.CodeGenMeta(state);
    //        state.level = 0;
    //    }

    //    internal override void CodeGenMeta(CodeGenState state)
    //    {
    //        state.level++;
    //        module.CodeGenMeta(state);
    //        state.level--;
    //        state.MakeCode(typeof(BracketModule));
    //    }

    //}

	public partial class Escape : Code
	{
		public Code code;

		public Escape(Code code)
		{
			this.code = code;
		}

		public override bool IsAssignable()
		{
			CodeType type = code.GetMType() as CodeType;
			if (type != null) return type.@ref;
			else return false;
		}

        public override MType GetMType()
        {
			CodeType type = code.GetMType() as CodeType;
			if (type != null) return type.codeType;
			else return PrimType.Void;
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			state.LowerLevel();
			code.CodeGenClosures(state);
			state.RaiseLevel();
		}

		internal override void CodeGen(CodeGenState state)
		{
			throw new InvalidOperationException("Escape called at stage 0.");
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			if(state.level == 1)
			{
				state.level = 0;
				code.CodeGen(state);
				if (!(code.GetMType() is CodeType)) state.MakeCode(typeof(Empty));
				state.level = 1;
			}
			else
			{
				state.level--;
				code.CodeGenMeta(state);
				state.level++;
				state.MakeCode(typeof(Escape));
			}
		}

	}
#endregion
}