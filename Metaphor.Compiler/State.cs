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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Metaphor.Collections;

using M = Metaphor;

namespace Metaphor.Compiler
{
	public class CompileState
	{
		public int level; 
		
		private readonly List<Assembly> externs;
		private readonly Set<string> externNamespaces;
		public M.MModuleBuilder module;

		public CompileState(Assembly[] externs)
		{
			this.externs = new List<Assembly>();
			this.externs.Add(typeof(void).Assembly);
            this.externs.Add(typeof(Code).Assembly);
			foreach(Assembly @extern in externs) this.externs.Add(@extern);
			this.externNamespaces = new Set<string>();
			LoadNamespaces();
			this.level = 0;
		}

		private void LoadNamespaces()
		{
			foreach(Assembly assembly in externs)
				foreach (System.Type type in assembly.GetExportedTypes())
					if(type.Namespace != null)
						externNamespaces.Add(type.Namespace);
		}

		#region Namespace scoping
		public bool NamespaceStartsWith(string prefix)
		{
			Contract.Requires(prefix != null);

			foreach (MTypeBuilder type in typeBuilders)
				if (type.ns != null && (type.ns == prefix || type.ns.StartsWith(prefix + ".")))
					return true;

			foreach (string ns in externNamespaces)
				if (ns == prefix || ns.StartsWith(prefix + "."))
					return true;

			return false;
		}

		public List<string> usings = new List<string>();
		public Stack<string> namespaces = new Stack<string>();

		public void PushNamespace(string @namespace)
		{
			if(namespaces.Count == 0) namespaces.Push(@namespace);
			else
				namespaces.Push($"{namespaces.Peek()}.{@namespace}");
		}

		public void PopNamespace()
		{
			namespaces.Pop();
		}

		public string GetCurrentNamespace()
		{
			return namespaces.Count > 0 ? namespaces.Peek() : null;
		}

		public string[] GetAllNamespaces()
		{
			List<string> allNamespaces = new List<string>();
			foreach(string ns in namespaces) allNamespaces.Add(ns);
			foreach(string ns in usings) allNamespaces.Add(ns);
			return allNamespaces.ToArray();
		}
		#endregion

		#region Type scoping
		private readonly Stack<MType> classes = new Stack<MType>();

		public MType GetCurrentClass()
		{
			if (classes.Count > 0)
			{
				MType type = classes.Peek();
				int myLevel = this.level;
				while (myLevel > 0) { type = LiftType.Promote(type); myLevel--; }
				return type;
			}
			//else throw new InvalidOperationException("Class stack is empty.");
			else return null;
		}

		public MType GetCurrentClassNoLifting()
		{
			if (classes.Count > 0)
			{
				MType type = classes.Peek();
				return type;
			}
			//else throw new InvalidOperationException("Class stack is empty.");
			else return null;
		}

		public void PushClass(MTypeBuilder @class)
		{
			classes.Push(@class.MakeType());
		}

		public void PopClass()
		{
			if (classes.Count > 0) classes.Pop();
			else throw new InvalidOperationException("Class stack is empty.");
		}

		private readonly GroupStack<MTypeBuilder> typeBuilders = new GroupStack<MTypeBuilder>();

		public void PushTypeBuilders(MTypeBuilder[] typeBuilders)
		{
			this.typeBuilders.PushAndMark(typeBuilders);
		}

		public void PopTypeBuilders()
		{
			typeBuilders.Pop();
		}

		public MType LookupTypeFor(string ns, string name, MType[] args)
		{
			MType mType = LookupType(ns, name, args);
			if (mType == null) return null;
			else if (mType.GetLevelKind() > level) throw ThrowTypeError(null, "to do: write error message");
			return mType;
		}

		public MType LookupType(string ns, string name, MType[] args)
		{
			int arity = args != null ? args.Length:0;

			if (ns == null && arity == 0)
			{
				//M.ExistsType.TypeVar existsVar = LookupExistsVar(name);
				//if (existsVar != null) return existsVar;

				MType typeVar = LookupTypeVar(name);
				if (typeVar != null) return typeVar;
			}

			foreach (MTypeBuilder typeBuilder in typeBuilders)
				if (typeBuilder.name == name && (ns == null || typeBuilder.ns == ns))
				{
					if (typeBuilder.@params.Length == 0 && arity == 0) return typeBuilder.MakeType(MType.Empty);
					else if (typeBuilder.@params.Length == arity)
					{
						for (int i = 0; i < typeBuilder.@params.Length; i++)
						{
							//if (!typeBuilder.@params[i].con.IsCompatibleType(args[i])) throw ThrowTypeError(null, "The type argument '{0}' at position {1} of the type '{2}' is not compatible with the declaration's type parameter constraints.", args[i], i, name);
						}
						return typeBuilder.MakeType(args);
					}
					else throw ThrowTypeError(null, "The type '{0}' expects {1} arguments but {2} arguments were supplied.", name, typeBuilder.@params.Length, arity);
				}

			System.Type type = null;
			if (ns == null)
				foreach (string @using in usings)
				{
					type = LookupSystemType(@using, name, arity);
					if (type != null) break;
				}
			if(type == null) 
				type = LookupSystemType(ns, name, arity);
			if (type != null)
			{
				if (!type.IsGenericTypeDefinition)
				{
					if (arity == 0) return SystemType.Create(type);
					else throw ThrowTypeError(null, "The type '{0}' expects {1} arguments but {2} arguments were supplied.", name, 0, arity);
				}
				else
				{
					Type[] genericParams = type.GetGenericArguments();
					if (genericParams.Length == arity)
					{
						for (int i = 0; i < genericParams.Length; i++)
						{
							if (!TypeVarCon.FromGenericParameter(genericParams[i]).IsCompatibleType(args[i])) throw ThrowTypeError(null, "The type argument '{0}' at position {1} of the type '{2}' is not compatible with the declaration's type parameter constraints.", args[i], i, name);
						}
						return GenericSystemType.Create(type, args);
					}
					else throw ThrowTypeError(null, "The type '{0}' expects {1} arguments but {2} arguments were supplied.", name, genericParams.Length, arity);
				}
			}

			if (ns == null)
			{
				if (name == "Code" || name == "CodeRef")
				{
					if (args.Length == 1) return CodeType.Create(args[0], name == "CodeRef");
					else if (args.Length == 0) return SystemType.Create(typeof(Code));
					else throw ThrowTypeError(null, "The {1} type expects 1 type argument but {0} were found.", args.Length, name);
				}
				else if (name == "CodeVoid")
				{
					if (args.Length != 0) throw ThrowTypeError(null, "The 'CodeVoid' type expects 0 type argument but {0} were found.", args.Length);
					return CodeType.Create(M.PrimType.Void, false);
				}
				else if (name == "FixedArray")
				{
					if (args.Length != 1) throw ThrowTypeError(null, "The 'FixedArray' type expects 1 type argument but {0} were found.", args.Length);
					return FixedArrayType.Create(args[0]);
				}
			}

			return null;
		}

		public System.Type LookupSystemType(string ns, string name, int arity)
		{
			string fullName = string.Format("{0}{1}{2}",
				ns != null ? $"{ns}." : string.Empty,
				name,
				arity > 0 ? $"`{arity}" : string.Empty);
			foreach (Assembly assembly in externs)
			{
				System.Type systemType = assembly.GetType(fullName);
				if (systemType != null) return systemType;
			}
			return null;
		}
		#endregion

		#region Label scoping
		private int loop = 0;

		public void PushLoop()
		{
			loop++;
		}

		public void PopLoop()
		{
			if (loop > 0) loop--;
			else throw new InvalidOperationException("loop == 0");
		}

		public bool IsInLoop()
		{
			return loop > 0;
		}
		#endregion

		#region Variable scoping
		private readonly Stack<Collections.Tuple<string, Code>> consts = new Stack<Collections.Tuple<string, Code>>();
		private readonly Stack<Collections.Tuple<VarDecl, int>> vars = new Stack<Collections.Tuple<VarDecl, int>>();
		private readonly Stack<Collections.Tuple<MType, int>> rets = new Stack<Collections.Tuple<MType, int>>();
		private readonly Stack<Collections.Tuple<int,int>> marks = new Stack<Collections.Tuple<int,int>>();

		public void PushConst(string name, Code code)
		{
			consts.Push(new Collections.Tuple<string, Code>(name, code));
		}

		public void PushLocal(VarDecl decl)
		{
			vars.Push(new Collections.Tuple<VarDecl, int>(decl, level));
		}

		public void PushMethod(ThisDecl @this, ParamDecl[] @params, MType retType)
		{
			PushBlock();
			if (@this != null) PushLocal(@this);
			foreach (ParamDecl decl in @params) PushLocal(decl);
			rets.Push(new Collections.Tuple<MType, int>(retType, level));
		}

		public void PopMethod()
		{
			rets.Pop();
			PopBlock();
		}

		public void PushBlock()
		{
			marks.Push(new Collections.Tuple<int,int>(consts.Count, vars.Count));
		}

		public void PopBlock()
		{
		    Collections.Tuple<int, int> mark = marks.Pop();
			while (consts.Count > mark.fst) consts.Pop();
			while (vars.Count > mark.snd) vars.Pop();
		}

		public Code LookupVar(string name)
		{
			foreach (Collections.Tuple<string, Code> @const in consts)
			{
				if (@const.fst == name) return @const.snd;
			}

			foreach (Collections.Tuple<VarDecl, int> var in vars)
			{
				if (var.fst.name == name)
				{
					if (var.snd > level) throw ThrowTypeError(null, "The variable '{0}' is defined at level {1} but used at level {2}.", name, var.snd, level);
					MType varType = var.fst.GetMType();
					Code myCode = var.fst.MakeVar();
					MType substType = SubstType(varType);
					if (substType != varType) myCode = new CastClass(substType, myCode);
					int myLevel = var.snd;
					while (myLevel < level) 
					{
						myCode = new M.Lift(myCode);
						myLevel++;
					}
					return myCode;
				}
			}
			return null;
		}

		public Code LookupThisVar()
		{
			return LookupVar("this");
		}

		public MType GetReturnType()
		{
			foreach (Collections.Tuple<MType, int> ret in rets)
			{
				if (ret.snd == level) return SubstType(ret.fst);
			}
			return null;
		}

		private readonly Stack<MType> expectedType = new Stack<MType>();
		public void PushExpectedType(MType type)
		{
			expectedType.Push(type);
		}

		public MType GetExpectedType()
		{
			if (expectedType.Count > 0) return SubstType(expectedType.Peek());
			else return null;
		}

		public void PopExpectedType()
		{
			if (expectedType.Count > 0) expectedType.Pop();
			else throw new InvalidOperationException("expectedType stack is empty");
		}

		#endregion

		#region Type variable scoping
		private readonly GroupStack<Collections.Tuple<TypeVarDecl, int>> typeVars = new GroupStack<Collections.Tuple<TypeVarDecl, int>>();
		private readonly Stack<Collections.Tuple<TypeVarDecl, MType>> subst = new Stack<Collections.Tuple<TypeVarDecl, MType>>();

		public void PushTypeVars(IEnumerable<TypeVarDecl> decls)
		{
			typeVars.Mark();
			foreach (TypeVarDecl decl in decls)
				typeVars.Push(new Collections.Tuple<TypeVarDecl, int>(decl, level));
		}

		public void PopTypeVar()
		{
			typeVars.Pop();
		}

		public void PushTypeVar(TypeVarDecl decl)
		{
			typeVars.PushAndMark(new Collections.Tuple<TypeVarDecl, int>(decl, level));
		}

		public MType LookupTypeVar(string name)
		{
			foreach (Collections.Tuple<TypeVarDecl, int> typeVarDecl in typeVars)
				if (typeVarDecl.fst.name == name)
				{
					if (typeVarDecl.snd > level) throw ThrowTypeError(null, "The type variable '{0}' is defined at level {1} but used at level {2}.", name, typeVarDecl.snd, level);
					//MType typeVar = TypeVar.Create(typeVarDecl.fst.levelKind, SubstTypeVarCon(typeVarDecl.fst.con), typeVarDecl.fst.name);
					MType typeVar = typeVarDecl.fst.MakeVar();
					int myLevel = typeVarDecl.snd;
					while (myLevel < level)
					{
						typeVar = new LiftType(typeVar);
						myLevel++;
					}
					return typeVar;
				}
			return null;
		}

		public MType SubstType(MType type)
		{
			foreach (Collections.Tuple<TypeVarDecl, MType> s in subst)
				type = SubstType(type, s.fst, s.snd);
			return type;
		}

		public TypeVarCon SubstTypeVarCon(TypeVarCon con)
		{
			TypeVarCon result = new TypeVarCon();
			if (con.superType != null) result.superType = SubstType(con.superType);
			if (con.interfaces != null)
			{
				result.interfaces = new List<MType>();
				foreach (MType iface in con.interfaces)
					result.interfaces.Add(SubstType(iface));
			}
			result.kind = con.kind;
			result.hasNew = con.hasNew;
			return result;
		}

		private MType SubstType(MType type, TypeVarDecl src, MType dest)
		{
			TypeVar typeVar = type as TypeVar;
			if (typeVar != null)
			{
				if (typeVar.decl == src) return dest;
				/*
				TypeVarCon substCon = new TypeVarCon();
				substCon.hasNew = typeVar.con.hasNew;
				substCon.kind = typeVar.con.kind;
				if (typeVar.con.superType != null)
					substCon.superType = SubstType(typeVar.con.superType, sym, con);
				if (typeVar.con.interfaces != null)
				{
					substCon.interfaces = new List<MType>();
					foreach (MType iface in typeVar.con.interfaces)
						substCon.interfaces.Add(SubstType(iface, sym, con));
				}
				return TypeVar.Create(typeVar.sym, typeVar.levelKind, substCon, typeVar.name);
				*/
				else return typeVar;
			}
			else
			{
				MType[] typeArgs = type.GetTypeArguments();
				if (typeArgs.Length != 0)
				{
					for (int i = 0; i < typeArgs.Length; i++)
						typeArgs[i] = SubstType(typeArgs[i], src, dest);
					return type.ReplaceTypeArguments(typeArgs);
				}
			}
			return type;
		}

		public bool PushTypeSubst(TypeVarDecl decl, MType type)
		{
			subst.Push(new Collections.Tuple<TypeVarDecl, MType>(decl, type));
			return true;
		}

		public void PopTypeSubst()
		{
			subst.Pop();
		}
		#endregion

		#region Member scoping
		protected Stack<Collections.Tuple<ForField, int>> fields = new Stack<Collections.Tuple<ForField, int>>();

		public void PushField(ForField field)
		{
			fields.Push(new Collections.Tuple<ForField, int>(field, level));
		}

		public void PopField()
		{
			fields.Pop();
		}

		public MFieldInfo LookupField(string name, bool isStatic)
		{
			foreach (Collections.Tuple<ForField, int> field in fields)
				if (field.fst.name == name && field.fst.isStatic == isStatic)
				{
					if (field.snd > level) throw ThrowTypeError(null, "The reflected field '{0}' is defined at level {1} but used at level {2}.", name, field.snd, level);
					MFieldInfo result = new RefelctedField(field.fst.sym, SubstType(field.fst.type), field.fst.isStatic, SubstType(field.fst.fieldType.MakeVar()), field.fst.name);
					int myLevel = field.snd;
					while (myLevel < level)
					{
						result = new LiftField(result);
						myLevel++;
					}
					return result;
				}
			return null;
		}

		#endregion

		public int GetLevel()
		{
			return level;
		}

		public void RaiseLevel()
		{
			level++;
		}

		public void LowerLevel()
		{
			if (level > 0) level--;
			else throw new NotImplementedException();
		}

		public void GetLocationString(StringBuilder sb)
		{
			//if (classes.Count == 0)
			//{
			//    sb.AppendLine("At top level:");
			//}
			//else
			//{
			//    M.ClassBuilder[] classArray = null; // classes.ToArray();
			//    Array.Reverse(classArray);
			//    foreach (M.ClassBuilder @class in classArray)
			//    {
			//        sb.AppendFormat("In class '{0}':", @class.name);
			//        sb.AppendLine();
			//    }

			//    Frame[] frameArray = frames.ToArray();
			//    Array.Reverse(frameArray);
			//    foreach (Frame frame in frameArray)
			//        sb.AppendLine(frame.GetLocationString());
			//}
		}

		public abstract class Frame
		{
		    protected Frame()
			{
			}

			public virtual M.MType GetReturnType()
			{
				return M.PrimType.Void;
			}

			public virtual M.ThisDecl GetThisDecl()
			{
				return null;
			}

			public virtual M.ParamDecl[] GetParamDecls()
			{
				return new M.ParamDecl[] {};
			}

			public abstract string GetLocationString();

		}

		public class MethodBaseFrame : Frame
		{
			public M.MethodBaseBuilder methodBase;

			public MethodBaseFrame(M.MethodBaseBuilder methodBase)
			{
				this.methodBase = methodBase;
			}

			public override M.MType GetReturnType()
			{
				return methodBase.retType;
			}

			public override M.ThisDecl GetThisDecl()
			{
				return methodBase.@this;
			}

			public override M.ParamDecl[] GetParamDecls()
			{
				return methodBase.@params;
			}

			public override string GetLocationString()
			{
				M.MMethodBuilder method = methodBase as M.MMethodBuilder;
				if (method != null) return $"In method '{method.name}':";

				M.MConstructorBuilder ctor = methodBase as M.MConstructorBuilder;
				if (ctor != null) return "In constructor:";

				return "In ?:";
			}

		}

		#region Error handling
		public Exception ThrowTypeError(Node node, string msg, params object[] args)
		{
			System.Text.StringBuilder sb = new StringBuilder();
			if (node != null && node.Token != null)
			{
				sb.AppendFormat("At line {0}, position {1},\n", node.Token.getLine(), node.Token.getColumn());
				//sb.AppendFormat("in token \"{0}\":\n", node.Token.getText());
			}
			sb.AppendFormat("{0}\n", string.Format(msg, args));
			return new Exception(sb.ToString());
		}

		public Exception ThrowNotImplemented(Node node, string msg, params object[] args)
		{
			System.Text.StringBuilder sb = new StringBuilder();
			if (node != null && node.Token != null)
			{
				sb.AppendFormat("At line {0}, position {1},\n", node.Token.getLine(), node.Token.getColumn());
				//sb.AppendFormat("in token \"{0}\":\n", node.Token.getText());
			}
			sb.AppendFormat("The feature '{0}' is not implemented.\n", msg);
			return new Exception(sb.ToString());
		}
		#endregion
	}

}
