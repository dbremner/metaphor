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
using JetBrains.Annotations;
using Metaphor.Collections;
using M = Metaphor;
using IToken = antlr.IToken;

namespace Metaphor.Compiler
{
	public class Module : Node
	{
		public List<string> usings;
		public Namespace defaultNamespace;
		public M.MModuleBuilder mModule;

        public Module(List<Ident> usings, Namespace defaultNamespace)
			: base(null)
		{
			Contract.Requires(defaultNamespace != null);
			this.usings = usings != null ? Ident.ToString(usings) : new List<string>();
            this.defaultNamespace = defaultNamespace;
		}

        public void CompileTypes(CompileState state)
        {
            foreach (string @using in usings) state.usings.Add(@using);
            mModule = state.module = new MModuleBuilder();
			defaultNamespace.CompileTypes(state, mModule);
        }

		public void CompileMembers(CompileState state)
		{
			defaultNamespace.CompileMembers(state);
		}

		public MModuleBuilder CompileCode(CompileState state)
		{
			defaultNamespace.CompileCode(state);
			return mModule;
		}
	}

	public class Namespace : Node
	{
	    [CanBeNull] public string name;
        public List<Namespace> namespaces;
        public List<TypeDecl> typeDecls;

		private M.MTypeBuilder[] typeBuilders;

        public Namespace(IToken token, Ident name, List<Namespace> namespaces, List<TypeDecl> typeDecls)
			: base(token)
		{
            this.name = name != null ? name.Name : null;
			this.namespaces = CheckNull(namespaces);
			this.typeDecls = CheckNull(typeDecls);
		}

        public void CompileTypes(CompileState state, MModuleBuilder mMod)
        {
            if (name != null) state.PushNamespace(name);
			int index = 0;
			typeBuilders = new MTypeBuilder[typeDecls.Count];
            foreach (TypeDecl typeDecl in typeDecls)
            {
				typeDecl.CompileTypes(state, mMod);
				typeBuilders[index++] = typeDecl.mTypeBuilder;
            }
            foreach (Namespace @namespace in namespaces)
            {
				@namespace.CompileTypes(state, mMod);
            }
            if (name != null) state.namespaces.Pop();
        }

		public void CompileMembers(CompileState state)
		{
			if (name != null) state.PushNamespace(name);
			state.PushTypeBuilders(typeBuilders);
			foreach (TypeDecl typeDecl in typeDecls)
			{
				typeDecl.CompileMembers(state);
			}
			foreach (Namespace @namespace in namespaces)
			{
				@namespace.CompileMembers(state);
			}
			state.PopTypeBuilders();
			if (name != null) state.namespaces.Pop();
		}

		public void CompileCode(CompileState state)
		{
			if (name != null) state.PushNamespace(name);
			state.PushTypeBuilders(typeBuilders);
			foreach (TypeDecl typeDecl in typeDecls)
			{
				typeDecl.CompileCode(state);
			}
			foreach (Namespace @namespace in namespaces)
			{
				@namespace.CompileCode(state);
			}
			state.PopTypeBuilders();
			if (name != null) state.namespaces.Pop();
		}
	}
}