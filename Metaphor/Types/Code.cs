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
using System.Reflection;
using System.Reflection.Emit;

namespace Metaphor
{
	[Serializable]
	public partial class CodeType : MType
	{
		public MType codeType;
		public bool @ref;

		public CodeType(MType codeType, bool @ref)
		{
			Contract.Requires(codeType != null);
			if (@ref && codeType == PrimType.Void) throw new ArgumentException("Cannot create the type <|ref void|>.");
			this.codeType = codeType;
			this.@ref = @ref;
		}

		public static CodeType Create(MType codeType, bool @ref)
		{
			return new CodeType(codeType, @ref);
		}

		public override int GetLevelKind()
		{
			int tmp = codeType.GetLevelKind();
			return tmp > 0 ? tmp - 1 : 0;
		}

		public override string GetName()
		{
			//if(@ref) return string.Format("CodeRef<{0}>", codeType);
			//else return string.Format("Code<{0}>", codeType);
			return @ref ? "CodeRef" : "Code";
		}

		public override MType[] GetTypeArguments()
		{
			return new MType[] { codeType };
		}

		public override MType ReplaceTypeArguments(MType[] newTypeArgs)
		{
			return Create(newTypeArgs[0], @ref);
		}

		public override bool IsValueType()
		{
			return false;
		}

		public override bool IsGeneric()
		{
			return false;
		}

		protected override bool  IsEqualToInternal(MType type)
		{
			CodeType that = (CodeType)type;
 			return @ref == that.@ref && codeType.IsEqualTo(that.codeType);
		}

		public override MType GetSuperType()
		{
			if(@ref) return CodeType.Create(codeType, false);
			MType superType = codeType.GetSuperType();
			if (superType != null) return CodeType.Create(superType, false);
			else return SystemType.Create(typeof(Code));
		}

		protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
			methods.Add(RunMethod.Create(this));
			methods.Add(RunModuleMethod.Create(this));
			methods.Add(SaveMethod.Create(this));
			methods.Add(IldasmMethod.Create(this));
			PrimType.Object.GetMethods(methods, typeParams);
		}

		internal override void MakeLiteral(CodeGenState state)
		{
			state.MakeCode(typeof(LitCode));
		}

		public override Type GetSystemType()
		{
			return typeof(Code);
		}

		internal override void CodeGen(CodeGenState state)
		{
			codeType.CodeGen(state);
			state.EmitBool(@ref);
			state.MakeType(typeof(CodeType));
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			codeType.CodeGenMeta(state);
			state.EmitBool(@ref);
			state.MakeType(typeof(CodeType));
		}

		#region Methods
		public class RunMethod : MetaphorMethod
		{
			protected RunMethod(CodeType codeType) : base(codeType, typeof(Code).GetMethod("Run"), codeType.codeType, ParamType.Empty)
			{
			}

			public static RunMethod Create(CodeType codeType)
			{
				return new RunMethod(codeType);
			}

			internal override void CodeGen(CodeGenState state)
			{
				base.CodeGen(state);
				Type type = retType.GetSystemType();
				if (type == typeof(void)) state.code.Emit(OpCodes.Pop);
				else state.EmitCast(type);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(RunMethod));
			}

		}

		public class RunModuleMethod : MetaphorMethod
		{
			protected RunModuleMethod(CodeType codeType)
				: base(codeType, typeof(Code).GetMethod("RunModule"), codeType.codeType, ParamType.Empty)
			{
			}

			public static RunModuleMethod Create(CodeType codeType)
			{
				return new RunModuleMethod(codeType);
			}

			internal override void CodeGen(CodeGenState state)
			{
				base.CodeGen(state);
				Type type = retType.GetSystemType();
				if (type == typeof(void)) state.code.Emit(OpCodes.Pop);
				else state.EmitCast(type);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(RunModuleMethod));
			}

		}

		public abstract class SaveOrIldasmMethod : MetaphorMethod
		{
			private static ParamType[] saveParamTypes = new ParamType[] { ParamType.Create(PrimType.String, false) };

			protected SaveOrIldasmMethod(CodeType codeType, MethodInfo methodInfo): base(codeType, methodInfo, PrimType.Void, saveParamTypes)
			{
			}

		}

		public class SaveMethod : SaveOrIldasmMethod
		{
			protected SaveMethod(CodeType codeType): base(codeType, typeof(Code).GetMethod("Save"))
			{
			}

			public static SaveMethod Create(CodeType codeType)
			{
				return new SaveMethod(codeType);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(SaveMethod));
			}

		}

		public class IldasmMethod : SaveOrIldasmMethod
		{
			protected IldasmMethod(CodeType codeType): base(codeType, typeof(Code).GetMethod("Ildasm"))
			{
			}

			public static IldasmMethod Create(CodeType codeType)
			{
				return new IldasmMethod(codeType);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(IldasmMethod));
			}
		}
		#endregion
	}

	[Serializable]
	public partial class FixedArrayType : CodeType
	{
		public MType elemType;

		public FixedArrayType(MType elemType) : base(ArrayType.Create(elemType, 1), false)
		{
			this.elemType = elemType;
		}

		public static FixedArrayType Create(MType elemType)
		{
		    Contract.Requires(elemType != null);
		    return new FixedArrayType(elemType);
		}

		public override string GetName()
		{
			return "FixedArray";
		}

		public override MType[] GetTypeArguments()
		{
			return new MType[] { elemType };
		}

		public override MType ReplaceTypeArguments(MType[] newTypeArgs)
		{
			return Create(newTypeArgs[0]);
		}

		protected override bool IsEqualToInternal(MType type)
		{
			FixedArrayType that = (FixedArrayType)type;
			return elemType.IsEqualTo(that.elemType);
		}

		public override MType GetSuperType()
		{
			return CodeType.Create(ArrayType.Create(elemType, 1), false);
		}

		protected internal override void GetMethods(List<MMethodInfo> methods, MType[] typeParams)
		{
			if (typeParams.Length == 0)
			{
				methods.Add(CreateVoidMethod.Create(this));
				methods.Add(LengthMethod.Create(this));
				methods.Add(GetMethod.Create(this));
				methods.Add(ToArrayMethod.Create(this));
			}
			base.GetMethods(methods, typeParams);
		}

		internal override void CodeGen(CodeGenState state)
		{
			throw new NotImplementedException();
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			throw new NotImplementedException();
		}

		#region Methods
		public class CreateVoidMethod : MetaphorMethod
		{
			protected CreateVoidMethod(FixedArrayType fixedArrayType)
				: base(fixedArrayType, 
				typeof(FixedArray).GetMethod("Create"), 
				CodeType.Create(PrimType.Void, false), 
				new ParamType[] { 
					ParamType.Create(PrimType.Int, false), 
					ParamType.Create(PrimType.Delegate, false)})
			{
			}

			public static CreateVoidMethod Create(FixedArrayType fixedArrayType)
			{
				return new CreateVoidMethod(fixedArrayType);
			}

			public override bool IsStatic()
			{
				return true;
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(CreateVoidMethod));
			}
		}

		public class LengthMethod : MetaphorMethod
		{
			protected LengthMethod(FixedArrayType fixedArrayType)
				: base(fixedArrayType, typeof(FixedArray).GetMethod("Length"), PrimType.Int, ParamType.Empty)
			{
			}

			public static LengthMethod Create(FixedArrayType fixedArrayType)
			{
				return new LengthMethod(fixedArrayType);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(LengthMethod));
			}
		}

		public class GetMethod : MetaphorMethod
		{
			protected GetMethod(FixedArrayType fixedArrayType)
				: base(fixedArrayType, typeof(FixedArray).GetMethod("Get"), CodeType.Create(fixedArrayType.elemType, true), new ParamType[] { ParamType.Create(PrimType.Int, false)})
			{
			}

			public static GetMethod Create(FixedArrayType fixedArrayType)
			{
				return new GetMethod(fixedArrayType);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(GetMethod));
			}
		}

		public class ToArrayMethod : MetaphorMethod
		{
			protected ToArrayMethod(FixedArrayType fixedArrayType)
				: base(fixedArrayType, typeof(FixedArray).GetMethod("ToArray"), CodeType.Create(ArrayType.Create(fixedArrayType.elemType, 1), false), ParamType.Empty)
			{
			}

			public static ToArrayMethod Create(FixedArrayType fixedArrayType)
			{
				return new ToArrayMethod(fixedArrayType);
			}

			internal override void CodeGenMeta(CodeGenState state)
			{
				declaringType.CodeGenMeta(state);
				state.MakeType(typeof(ToArrayMethod));
			}
		}
		#endregion
	}
}