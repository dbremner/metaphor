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
	[Serializable]
	public abstract partial class Code
	{
	    protected Code()
		{
		}

		public static readonly Code[] Empty = new Code[] { };

        public abstract MType GetMType();

		public virtual ParamType GetParamType()
		{
			return ParamType.Create(GetMType(), false);
		}

		public virtual bool IsAssignable()
		{
			return false;
		}

		public virtual bool IsBranch()
		{
			return false;
		}

		public static bool MakeCompatible(ref Code code, MType destType)
		{
			MType srcType = code.GetMType();
			if (srcType.IsSubTypeOf(destType))
			{
				// destType cannot be a value type since value types have no subtypes, unless destType equals srcType
				if (srcType.IsValueType() && !destType.IsValueType()) code = new Box(code);
				return true;
			}
			CodeType srcCodeType = srcType as CodeType;
			if (srcCodeType != null)
			{
				CodeType destCodeType = destType as CodeType;
                if (destCodeType != null && destCodeType.codeType == PrimType.Void && srcCodeType.codeType != PrimType.Void)
				{
					code = new Pop(code);
					return true;
				}
			}
			else if (destType.IsEqualTo(PrimType.Int) && srcType.IsEqualTo(PrimType.Short))
			{
				code = new Convert(code, CILType.I4);
				return true;
			}
			else if (destType.IsEqualTo(PrimType.Long) && (srcType.IsEqualTo(PrimType.Int) || srcType.IsEqualTo(PrimType.Short)))
			{
				code = new Convert(code, CILType.I8);
				return true;
			}
			else if (destType.IsEqualTo(PrimType.Double) && srcType.IsEqualTo(PrimType.Float))
			{
				code = new Convert(code, CILType.R8);
				return true;
			}
			return false;
		}

		public static bool IsCompatible(MType srcType, MType destType)
		{
			if (srcType.IsSubTypeOf(destType)) return true;
			else if (destType.IsEqualTo(PrimType.Int) && srcType.IsEqualTo(PrimType.Short)) return true;
			else if (destType.IsEqualTo(PrimType.Long) && (srcType.IsEqualTo(PrimType.Int) || srcType.IsEqualTo(PrimType.Short))) return true;
			else if (destType.IsEqualTo(PrimType.Double) && srcType.IsEqualTo(PrimType.Float)) return true;
			else return false;
		}

		internal abstract void CodeGenClosures(TypeCheckState state);

		internal abstract void CodeGen(CodeGenState state);

		internal virtual void CodeGenBranch(CodeGenState state, bool when, Label where)
		{
			if (GetMType() == PrimType.Boolean)
			{
				CodeGen(state);
				if (when) state.code.Emit(OpCodes.Brtrue, where);
				else state.code.Emit(OpCodes.Brfalse, where);
			}
			else throw new InvalidOperationException("Cannot generate condition branch on a non-boolean value.");
		}

		internal virtual void CodeGenAssign(CodeGenState state, AssignOp op, Code rhs, AssignRet ret)
		{
			throw new InvalidOperationException("Code expression is not assignable.");
		}

		internal virtual void CodeGenRef(CodeGenState state)
		{
			throw new InvalidOperationException("Code expression is not referencable.");
		}

		internal abstract void CodeGenMeta(CodeGenState state);

		internal static void CodeGenClosures(TypeCheckState state, Code[] codes)
		{
			for (int i = 0; i < codes.Length; i++)
				codes[i].CodeGenClosures(state);
		}

		internal static void CodeGen(CodeGenState state, Code code)
		{
			if(code != null) code.CodeGen(state);
		}

		internal static void CodeGenRef(CodeGenState state, Code code)
		{
			if (code.IsAssignable()) code.CodeGenRef(state);
			else
			{
				int pos = state.DeclareLocal(code.GetMType().GetSystemType());
				code.CodeGen(state);
				state.EmitStloc(pos);
				state.EmitLdloca(pos);
			}
		}

		internal static void CodeGenMeta(CodeGenState state, Code code)
		{
			if(code != null) code.CodeGenMeta(state);
			else state.EmitNull();
		}

		private static readonly FieldInfo empty = typeof(Code).GetField("Empty", BindingFlags.Static | BindingFlags.Public);

		internal static void CodeGenMeta(CodeGenState state, Code[] array)
		{
			if (array != null)
			{
				int length = array.Length;
				if (length == 0) state.code.Emit(OpCodes.Ldsfld, empty);
				else
				{
					state.EmitInt(length);
					state.code.Emit(OpCodes.Newarr, typeof(Code));
					for (int i = 0; i < length; i++)
					{
						state.code.Emit(OpCodes.Dup);
						state.EmitInt(i);
						array[i].CodeGenMeta(state);
						state.code.Emit(OpCodes.Stelem_Ref);
					}
				}
			}
			else state.EmitNull();
		}

		public static MType[] GetMTypes(Code[] codes)
		{
			return Array.ConvertAll<Code, MType>(codes, delegate(Code code) { return code.GetMType(); });
		}

		public static ParamType[] GetParamTypes(Code[] codes)
		{
			return Array.ConvertAll<Code, ParamType>(codes, delegate(Code code) { return code.GetParamType(); });
		}

		public object Run()
		{
			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(true);
			System.Diagnostics.StackFrame frame = trace.GetFrame(trace.FrameCount - 1);
			Module module = frame.GetMethod().Module;

			FunctionCreate fc = this as FunctionCreate;
			if (fc != null)
			{
				Function function = fc.func;
				DynamicMethodGenState dmState = new DynamicMethodGenState(module);
				TypeCheckState typeState = TypeCheckState.CreateRootState(dmState.cspObjects, function.@params);
				function.GenerateClosures(typeState);
				if (typeState.ContainsClosures()) throw new InvalidOperationException("A top-level staged function should never have closure variables.");
				object cspStore = dmState.MakeCspStore();

				CodeGenState cgState = dmState.CodeGen(null, null, typeState.GetFunctions());

				DynamicMethod dm = ((DynamicMethodCodeGenState)cgState).dm;
				if (cspStore != null) return dm.CreateDelegate(function.delegateType.GetSystemType(), cspStore);
				else return dm.CreateDelegate(function.delegateType.GetSystemType());
			}
			else
			{
				DynamicMethodGenState dmState = new DynamicMethodGenState(module);
				TypeCheckState typeState = new TypeCheckState(dmState.cspObjects, null);
				this.CodeGenClosures(typeState);
				if (typeState.ContainsClosures()) throw new InvalidOperationException("A top-level staged function should never have closure variables.");
				object cspStore = dmState.MakeCspStore();

				DynamicMethodCodeGenState cgState = dmState.DefineRun(this.GetMType().GetSystemType());
				dmState.CodeGen(cgState, this, typeState.GetFunctions());

				DynamicMethod dm = cgState.dm;
				if (cspStore != null) return dm.Invoke(null, BindingFlags.Default, null, new object[] { cspStore }, null);
				else return dm.Invoke(null, BindingFlags.Default, null, null, null);
			}

			//            AssemblyName name = new AssemblyName();
			//            name.Name = "MetaphorRun";
			//            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
			//            ModuleBuilder module = assembly.DefineDynamicModule("MetaphorRun");

			////			// this should really be an override of run
			////			FunctionCreate fc = this as FunctionCreate;
			////			if (fc != null)
			////			{
			////				Function func = fc.func;
			////				DynamicMethod method = new DynamicMethod(
			////					"Run",
			////					func.GetReturnType().GetSystemType(),
			////					ParamType.GetSystemTypes(func.GetParamTypes()),
			////					module);
			////				RunStub run = new RunStub(method, func);
			////				RootCodeGenState state = new RootRunCodeGenState(run);
			////				func.stmt.CodeGen(state);
			////				return method.CreateDelegate(func.delegateType.GetSystemDelegateType());
			////			}
			////			else
			////			{
			//            //DynamicMethod method = new DynamicMethod("Run", type.GetSystemType(), System.Type.EmptyTypes, module);
			//            MType type = GetMType();
			//            MethodBuilder method = module.DefineGlobalMethod("Run", MethodAttributes.Public | MethodAttributes.Static, type.GetSystemType(), System.Type.EmptyTypes);
			//            ModuleGenState mgState = new RunModuleGenState(assembly, module);
			//            CodeGenState cgState = new RunOrSaveCodeGenState(mgState, method);
			//            this.CodeGen(cgState);
			//            cgState.EndMethodBody();
			//            mgState.FinalizeCsp();
			//            module.CreateGlobalFunctions();
			//            return module.GetMethod("Run").Invoke(null, new object[] {});
			//			}
		}

		public object RunModule()
		{
			MType type = GetMType();

			AssemblyName name = new AssemblyName();
			name.Name = "MetaphorRun";
			AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
			ModuleBuilder module = assembly.DefineDynamicModule("MetaphorRun");
			ModuleGenState mgState = new ModuleGenState(assembly, module);

			TypeCheckState typeState = new TypeCheckState(mgState.cspStore, null);
			this.CodeGenClosures(typeState);
			if (typeState.ContainsClosures()) throw new InvalidOperationException("A top-level staged function should never have closure variables.");

			ModuleCodeGenState cgState = mgState.DefineRun(type.GetSystemType());
			mgState.CodeGen(cgState, this, typeState.GetFunctions());
			mgState.FinalizeCsp(false);

			return module.GetMethod("Run").Invoke(null, new object[] {});
		}

		public void Save(string name)
		{
			MType type = GetMType();

			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = name;
			AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
			ModuleBuilder module = assembly.DefineDynamicModule(name, name + ".exe");
			ModuleGenState mgState = new ModuleGenState(assembly, module);

			TypeCheckState typeState = new TypeCheckState(mgState.cspStore, null);
			this.CodeGenClosures(typeState);
			if (typeState.ContainsClosures()) throw new InvalidOperationException("A top-level staged function should never have closure variables.");

			MethodBuilder run;
			ModuleCodeGenState cgState = mgState.DefineRun(type.GetSystemType(), out run);
			mgState.CodeGen(cgState, this, typeState.GetFunctions());

			MethodBuilder main = module.DefineGlobalMethod("Main", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), System.Type.EmptyTypes);
			ILGenerator code = main.GetILGenerator();
			LocalBuilder local = code.DeclareLocal(typeof(Exception));
			Label @try = code.BeginExceptionBlock();
			code.Emit(OpCodes.Call, run);
			if (type.GetSystemType() != typeof(void)) code.Emit(OpCodes.Pop);
			code.BeginCatchBlock(typeof(Exception));
			code.Emit(OpCodes.Stloc, local);
			code.Emit(OpCodes.Call, typeof(Console).GetProperty("Error").GetGetMethod());
			code.Emit(OpCodes.Ldloc, local);
			code.Emit(OpCodes.Callvirt, typeof(System.IO.TextWriter).GetMethod("WriteLine", new Type[] { typeof(object) }));
			code.EndExceptionBlock();
			code.Emit(OpCodes.Ret);

			assembly.SetEntryPoint(main);
			mgState.FinalizeCsp(true);
			assembly.Save(name + ".exe");
		}

		public void Ildasm(string name)
		{
			Save(name);
			using (System.Diagnostics.Process p = System.Diagnostics.Process.Start("ildasm.exe", name + ".exe")) p.WaitForExit();
		}
	}

	public partial class CodeGroup: Code
	{
		public Code[] codes;

		public CodeGroup(Code[] codes)
		{
			this.codes = codes;
		}

		public static Code MakeCode2(Code code1, Code code2)
		{
			return MakeCode(code1, code2);
		}

		public static Code MakeCode(params Code[] codes)
		{
			List<Code> coalescedCodes = new List<Code>();
			CollectStmts(codes, coalescedCodes);
			int length = coalescedCodes.Count;
			if (length == 0) return new Empty();
			else if (length == 1) return coalescedCodes[0];
			else return new CodeGroup(coalescedCodes.ToArray());
		}

		public override bool IsAssignable()
		{
			if (codes.Length == 0) return false;
			else return codes[codes.Length - 1].IsAssignable();
		}

		public override bool IsBranch()
		{
			if (codes.Length > 0) return codes[codes.Length - 1].IsBranch();
			else return false;
		}

        public override MType GetMType()
        {
            return codes.Length > 0 ? codes[codes.Length - 1].GetMType() : PrimType.Void;
        }

		internal override void CodeGenClosures(TypeCheckState state)
		{
			Code.CodeGenClosures(state, codes);
		}

		internal override void CodeGen(CodeGenState state)
		{
			foreach(Code code in codes) code.CodeGen(state);
		}

		internal override void CodeGenMeta(CodeGenState state)
		{
			List<Code> coalescedCodes = new List<Code>();
			CollectStmts(this.codes, coalescedCodes);
			int length = coalescedCodes.Count;
			if(length == 0) state.MakeCode(typeof(Empty));
			else if(length == 1) coalescedCodes[0].CodeGenMeta(state);
			else
			{
				state.EmitInt(length);
				state.code.Emit(OpCodes.Newarr, typeof(Code));
				for(int i = 0; i < length; i++)
				{
					state.code.Emit(OpCodes.Dup);
					state.EmitInt(i);
					coalescedCodes[i].CodeGenMeta(state);
					state.code.Emit(OpCodes.Stelem_Ref);
				}
				state.MakeCode(typeof(CodeGroup));
			}
		}

		internal static void CollectStmts(IEnumerable<Code> srcList, List<Code> destList)
		{
			foreach (Code code in srcList)
			{
				CodeGroup codeGroup = code as CodeGroup;
				if (codeGroup != null) CollectStmts(codeGroup.codes, destList);
				else if (code != null && !(code is Empty)) destList.Add(code);
			}
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple=false)]
	public sealed class CodeTypeAttribute : Attribute
	{
		private readonly Type codeType;
		internal static ConstructorInfo ctor;

		static CodeTypeAttribute()
		{
			ctor = typeof(CodeTypeAttribute).GetConstructor(new Type[] { typeof(Type) });
		}

		public CodeTypeAttribute(Type codeType)
		{
			this.codeType = codeType;
		}

		internal Type GetCodeType()
		{
			return codeType;
		}
	}
}
