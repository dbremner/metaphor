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
	public abstract class TypeDecl : Node
	{
		public Modifier mods;
		public string name;

		internal MTypeBuilder mTypeBuilder;

		public TypeDecl(IToken token, Modifier mods, Ident name)
			: base(token)
		{
			this.mods = mods;
			if (name == null) throw new ArgumentNullException("name");
			this.name = name.Name;
		}

		public abstract void CompileTypes(CompileState state, M.MModuleBuilder mMod);

		public abstract void CompileMembers(CompileState state);

		public abstract void CompileCode(CompileState state);
	}

	public class TypeParam : Node
	{
		public string name;
		public List<TypeParamCon> cons;

		public TypeParam(IToken token, Ident name, List<TypeParamCon> cons)
			: base(token)
		{
			if (name == null) throw new ArgumentNullException("name");
			this.name = name.Name;
			if (cons == null) throw new ArgumentNullException("cons");
			this.cons = cons;
		}

		public enum Location { Class, Method, Stmt };

		public static TypeVarDecl[] Compile(CompileState state, List<Tuple<Ident, int>> names, List<TypeParam> @params, Location loc)
		{
			List<TypeVarDecl> result = new List<TypeVarDecl>();
			foreach (Tuple<Ident, int> name in names)
			{
				foreach (TypeVarDecl typeVar2 in result)
					if (typeVar2.name == name.fst.Name)
						state.ThrowTypeError(name.fst, "Duplicate type parameter name '{0}'.", name.fst.Name);
				TypeVarDecl typeVar;
				if (name.snd == 0)
					typeVar = new GenericTypeVarDecl(name.fst.Name, loc == Location.Method);
				else if (name.snd == 1)
					switch (loc)
					{
						case Location.Class:
							typeVar = new ClassReflTypeVarDecl(name.fst.Name);
							break;
						case Location.Method:
							typeVar = new MethodReflTypeVarDecl(name.fst.Name);
							break;
						case Location.Stmt:
							typeVar = new LocalReflTypeVarDecl(name.fst.Name);
							break;
						default:
							throw new InvalidOperationException();
					}
				else throw state.ThrowNotImplemented(null, "type varibles with more than one ~");
				result.Add(typeVar);
			}
			state.PushTypeVars(result);
			foreach (TypeParam param in @params)
			{
				foreach (TypeVarDecl typeVar in result)
				{
					if (typeVar.name == param.name)
					{
						foreach (TypeParamCon con in param.cons)
						{
							con.Compile(state, typeVar.con, typeVar.name);
						}
						continue;
					}
				}
				state.ThrowTypeError(param, "The type variable '{0}' is not defined.", param.name);
			}
			state.PopTypeVar();
			return result.ToArray();
		}
	}

	public abstract class TypeParamCon : Node
	{
		public TypeParamCon(IToken token)
			: base(token)
		{
		}

		public abstract void Compile(CompileState state, TypeVarCon typeVarCon, string name);
	}

	public class TypeParamTypeCon : TypeParamCon
	{
		public Typ type;

		public TypeParamTypeCon(Typ type)
			: base(type.Token)
		{
			if (type == null) throw new ArgumentNullException("type");
			this.type = type;
		}

		public override void Compile(CompileState state, TypeVarCon typeCon, string name)
		{
			type.Compile(state);
			if (!type.mType.IsInterface())
			{
				if (typeCon.superType != null) throw state.ThrowTypeError(this, "Cannot specify two super type constraints on type parameter '{0}'.", name);
				if (typeCon.kind != TypeKind.Any) throw state.ThrowTypeError(this, "Cannot specify a super type constraint and a kind constraint on type parameter '{0}'.", name);
				typeCon.superType = type.mType;
			}
			else
			{
				if (typeCon.interfaces == null) typeCon.interfaces = new List<MType>();
				typeCon.interfaces.Add(type.mType);
			}
		}
	}

	public class TypeParamClassCon : TypeParamCon
	{
		public TypeParamClassCon(IToken token)
			: base(token)
		{
		}

		public override void Compile(CompileState state, TypeVarCon typeCon, string name)
		{
			if (typeCon.kind != TypeKind.Any) throw state.ThrowTypeError(this, "Duplicate kind constraint on type parameter '{0}'.", name);
			if (typeCon.superType != null) throw state.ThrowTypeError(this, "Cannot specify a super type constraint and a kind constraint on type parameter '{0}'.", name);
			typeCon.kind = TypeKind.Class;				
		}
	}

	public class TypeParamStructCon : TypeParamCon
	{
		public TypeParamStructCon(IToken token)
			: base(token)
		{
		}

		public override void Compile(CompileState state, TypeVarCon typeCon, string name)
		{
			if (typeCon.kind != TypeKind.Any) throw state.ThrowTypeError(this, "Duplicate kind constraint on type parameter '{0}'.", name);
			if (typeCon.superType != null) throw state.ThrowTypeError(this, "Cannot specify a super type constraint and a kind constraint on type parameter '{0}'.", name);
			if (typeCon.hasNew) throw state.ThrowTypeError(this, "Cannot specify a default constructor constraint and a struct kind constraint on type parameter '{0}'.", name);
			typeCon.kind = TypeKind.Struct;
		}
	}

	public class TypeParamCtorCon : TypeParamCon
	{
		public TypeParamCtorCon(IToken token)
			: base(token)
		{
		}
		public override void Compile(CompileState state, TypeVarCon typeCon, string name)
		{
			if (typeCon.hasNew) throw state.ThrowTypeError(this, "Duplicate default constructor constraint on type parameter '{0}'.", name);
			if (typeCon.kind == TypeKind.Struct) throw state.ThrowTypeError(this, "Cannot specify a default constructor constraint and a struct kind constraint on type parameter '{0}'.", name);
			typeCon.hasNew = true;
		}
	}

	public abstract class GenericType : TypeDecl
	{
		public List<Tuple<Ident, int>> typeParamNames;
		public List<TypeParam> typeParams;

		public GenericType(IToken token, Modifier mods, Ident name, List<Tuple<Ident, int>> typeParamNames, List<TypeParam> typeParams)
			: base(token, mods, name)
		{
			this.typeParamNames = CheckNull<Tuple<Ident, int>>(typeParamNames);
			this.typeParams = CheckNull<TypeParam>(typeParams);
		}

		protected TypeVarDecl[] CompileGenericParams(CompileState state)
		{
			// generic parameter constraints need to be appear in order of using in the source file. E.g.,
			// class A<T> {}
			// class B<T> where T : A<T> {}
			// works but,
			// class B<T> where T : A<T> {}
			// class A<T> {}
			// doesn't.
			return TypeParam.Compile(state, typeParamNames, typeParams, TypeParam.Location.Class);
		}
	}

	public class Class : GenericType
	{
		public List<Typ> baseTypes;
		public List<Member> members;

		private M.ClassBuilder mClass;

		public Class(IToken token, Modifier mods, Ident name, List<Tuple<Ident, int>> typeParamNames, List<Typ> baseTypes, List<TypeParam> typeParams, List<Member> members)
			: base(token, mods, name, typeParamNames, typeParams)
		{
			if (baseTypes != null) this.baseTypes = baseTypes;
			else
			{
				this.baseTypes = new List<Typ>();
				this.baseTypes.Add(PrimType.CreateObject(null));
			}
			this.members = CheckNull<Member>(members);
		}

		public override void CompileTypes(CompileState state, M.MModuleBuilder mMod)
		{
			TypeVarDecl[] typeParams = CompileGenericParams (state);
			mClass = new ClassBuilder(-1, name, state.GetCurrentNamespace(), Modifiers.GetTypeAttribures(state, mods), typeParams);
			mMod.AddTypeBuilder(mClass);
			mTypeBuilder = mClass;
		}

		public override void CompileMembers(CompileState state)
		{
			state.PushTypeVars(mClass.@params);
			bool oneBaseClass = true;
			foreach(Typ baseType in baseTypes)
			{
				MType type = baseType.CompileFor(state);
				if (type.IsInterface()) mClass.interfaces.Add(type);
				else if (oneBaseClass) { mClass.baseType = type; oneBaseClass = false; }
				else state.ThrowTypeError(this, "The class '{0}' has too many base classes.", name);
			}

			List<Field> staticFields = new List<Field>();
			List<Field> instanceFields = new List<Field>();
			Constructor staticCtor = null;
			List<Constructor> instanceCtors = new List<Constructor>();
			foreach (Member member in members)
			{
				Field field = member as Field;
				if (field != null && field.initExpr != null)
				{
					if ((field.mods & Modifier.Static) != 0) staticFields.Add(field);
					else instanceFields.Add(field);
				}
				Constructor ctor = member as Constructor;
				if (ctor != null)
				{
					if ((ctor.mods & Modifier.Static) != 0)
					{
						if (staticCtor == null) staticCtor = ctor;
						else throw state.ThrowTypeError(this, "Cannot define more than one static constructor in the class '{0}'.", name);
					}
					else instanceCtors.Add(ctor);
				}
				member.CompileMembers(state, mClass);
			}
			if (staticFields.Count > 0 && staticCtor == null)
			{
				List<Stmt> body = new List<Stmt>();
				body.Add(new Return(null, null));
				Constructor ctor = new Constructor(Modifier.Public | Modifier.Static, new Ident(null, name), new List<Param>(), null, body);
				staticCtor = ctor;
				members.Add(ctor);
				ctor.CompileMembers(state, mClass);
			}
			if ((mods & Modifier.Static) == 0  && instanceCtors.Count == 0)
			{
				List<Stmt> body = new List<Stmt>();
				body.Add(new Return(null, null));
				Constructor ctor = new Constructor(Modifier.Public, new Ident(null, name), new List<Param>(), null, body);
				instanceCtors.Add(ctor);
				members.Add(ctor);
				ctor.CompileMembers(state, mClass);
			}
			if(staticCtor != null)
			{
				staticCtor.fieldInits = staticFields;
			}
			foreach (Constructor ctor in instanceCtors)
			{
				ctor.fieldInits = instanceFields;
			}
			state.PopTypeVar();
		}

		public override void CompileCode(CompileState state)
		{
			state.PushTypeVars(mClass.@params);
			state.PushClass(mClass);
			foreach (Member member in members)
				member.CompileCode(state);
			state.PopClass();
			state.PopTypeVar();
		}
	}

	public class Struct : GenericType
	{
		public List<Typ> baseTypes;
		public List<Member> members;

		private M.StructBuilder mStruct;

		public Struct(IToken token, Modifier mods, Ident name, List<Tuple<Ident, int>> typeParamNames, List<Typ> baseTypes, List<TypeParam> typeParams, List<Member> members)
			: base(token, mods, name, typeParamNames, typeParams)
		{
			this.baseTypes = CheckNull<Typ>(baseTypes);
			this.members = CheckNull<Member>(members);
		}

		public override void CompileTypes(CompileState state, M.MModuleBuilder mMod)
		{
			TypeVarDecl[] typeParams = CompileGenericParams(state);
			mStruct = new StructBuilder(-1, name, state.GetCurrentNamespace(), Modifiers.GetTypeAttribures(state, mods), typeParams);
			mMod.AddTypeBuilder(mStruct);
			mTypeBuilder = mStruct;
		}

		public override void CompileMembers(CompileState state)
		{
			state.PushTypeVars(mStruct.@params);
			foreach (Typ baseType in baseTypes)
			{
				MType type = baseType.CompileFor(state);
				if (type.IsInterface()) mStruct.interfaces.Add(type);
				else state.ThrowTypeError(this, "The struct '{0}' cannot have a base class.", name);
			}

			List<Field> staticFields = new List<Field>();
			Constructor staticCtor = null;
			foreach (Member member in members)
			{
				Field field = member as Field;
				if (field != null && field.initExpr != null)
				{
					if ((field.mods & Modifier.Static) != 0) staticFields.Add(field);
					else state.ThrowTypeError(this, "The field '{0}' cannot have an initializer because it is in a struct.", field.name);
				}
				Constructor ctor = member as Constructor;
				if (ctor != null)
				{
					if ((ctor.mods & Modifier.Static) != 0)
					{
						if (staticCtor == null) staticCtor = ctor;
						else throw state.ThrowTypeError(this, "Cannot define more than one static constructor in the struct '{0}'.", name);
					}
				}
				member.CompileMembers(state, mStruct);
			}
			if (staticFields.Count > 0 && staticCtor == null)
			{
				List<Stmt> body = new List<Stmt>();
				body.Add(new Return(null, null));
				Constructor ctor = new Constructor(Modifier.Public | Modifier.Static, new Ident(null, name), new List<Param>(), null, body);
				staticCtor = ctor;
				members.Add(ctor);
				ctor.CompileMembers(state, mStruct);
			}
			if(staticCtor != null)
			{
				staticCtor.fieldInits = staticFields;
			}
			state.PopTypeVar();
		}

		public override void CompileCode(CompileState state)
		{
			state.PushTypeVars(mStruct.@params);
			state.PushClass(mStruct);
			foreach (Member member in members)
				member.CompileCode(state);
			state.PopClass();
			state.PopTypeVar();
		}
	}

	public class Delegate : GenericType
	{
		public Typ returnType;
		public List<Param> @params;

		private DelegateBuilder mDelegate;

		public Delegate(IToken token, Modifier mods, Typ returnType, Ident name, List<Tuple<Ident, int>> typeParamNames, List<Param> @params, List<TypeParam> typeParams)
			: base(token, mods, name, typeParamNames, typeParams)
		{
			if (returnType == null) throw new ArgumentNullException("returnType");
			this.returnType = returnType;
			this.@params = CheckNull<Param>(@params);
		}

		public override void CompileTypes(CompileState state, M.MModuleBuilder mMod)
		{
			TypeVarDecl[] typeParams = CompileGenericParams(state);
			mDelegate = new DelegateBuilder(-1, name, state.GetCurrentNamespace(), Modifiers.GetTypeAttribures(state, mods), typeParams);
			mMod.AddTypeBuilder(mDelegate);
			mTypeBuilder = mDelegate;
		}

		public override void CompileMembers(CompileState state)
		{
			state.PushTypeVars(mDelegate.@params);
			MType mReturnType = returnType.CompileFor(state);
			M.ParamDecl[] mParamDecls = Param.Compile(state, @params);
			mDelegate.AddDelegateMembers(mReturnType, M.ParamDecl.GetParamTypes(mParamDecls));
			state.PopTypeVar();
		}

		public override void CompileCode(CompileState state)
		{
		}
	}

	public class Param : Node
	{
		public Dir dir;
		public Typ type;
		public string name;

		private ParamDecl mParamDecl;

		public Param(Dir dir, Typ type, Ident name)
			: base(type.Token)
		{
			if (!Enum.IsDefined(typeof(Dir), dir)) throw new ArgumentOutOfRangeException("dir");
			this.dir = dir;
			if (type == null) throw new ArgumentNullException("type");
			this.type = type;
			if (name == null) throw new ArgumentNullException("name");
			this.name = name.Name;
		}

		public ParamDecl Compile(CompileState state)
		{
			type.Compile(state);
			mParamDecl = new MethodParamDecl(name, ParamType.Create(type.mType, (dir & Dir.Out) != 0));
			return mParamDecl;
		}

		public static ParamDecl[] Compile(CompileState state, List<Param> @params)
		{
			int num = @params.Count;
			ParamDecl[] mParams = new ParamDecl[num];
			for (int i = 0; i < num; i++) mParams[i] = @params[i].Compile(state);
			return mParams;
		}

		//public static ParamDecl[] Map(List<Param> @params)
		//{
		//    int num = @params.Count;
		//    ParamDecl[] mParams = new ParamDecl[num];
		//    for (int i = 0; i < num; i++) mParams[i] = @params[i].mParamDecl;
		//    return mParams;
		//}
	}
}
