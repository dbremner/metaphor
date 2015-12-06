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
	[Flags]
	public enum Modifier
	{
		None = 0x0000,
		New = 0x0001,
		Public = 0x0002,
		Protected = 0x0004,
		Internal = 0x0008,
		Private = 0x0010,
		Static = 0x0020,
		Readonly = 0x0040,
		Volatile = 0x0080,
		Virtual = 0x0100,
		Sealed = 0x0200,
		Override = 0x0400,
		Abstract = 0x0800,
		Extern = 0x1000,
		AccessMask = 0x001E,
		FieldMask = 0x00C0,
		MethodMask = 0x0F00,
		TypeMask = 0x0A00
	}

	public static class Modifiers
	{
		public static Modifier GetModifier(CompileState state, List<Modifier> mods)
		{
			Modifier result = Modifier.None;
			for (int i = 0; i < mods.Count; i++)
			{
				if ((result & mods[i]) != 0) state.ThrowTypeError(null, "Duplicate modifier '{0}'.", mods[i].ToString());
				else result |= mods[i];
			}
			return result;
		}

		public static System.Reflection.TypeAttributes GetTypeAttribures(CompileState state, Modifier mods)
		{
			System.Reflection.TypeAttributes attr = (System.Reflection.TypeAttributes)0;
			bool nested = state.GetCurrentClass() != null;
			if (nested)
			{
				switch (mods & Modifier.AccessMask)
				{
					case Modifier.Private:
						attr |= System.Reflection.TypeAttributes.NestedPrivate;
						break;
					case Modifier.Internal | Modifier.Private:
					case Modifier.Protected | Modifier.Private:
						attr |= System.Reflection.TypeAttributes.NestedFamANDAssem;
						break;
					case Modifier.Protected:
						attr |= System.Reflection.TypeAttributes.NestedFamily;
						break;
					case Modifier.Internal:
						attr |= System.Reflection.TypeAttributes.NestedAssembly;
						break;
					case Modifier.Protected | Modifier.Internal:
						attr |= System.Reflection.TypeAttributes.NestedFamORAssem;
						break;
					case Modifier.None:
					case Modifier.Public:
						attr |= System.Reflection.TypeAttributes.NestedPublic;
						break;
					default:
						throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a nested class.", ToString(mods & Modifier.AccessMask));
				}
			}
			else
			{
				switch (mods & Modifier.AccessMask)
				{
					case Modifier.Internal:
						attr |= System.Reflection.TypeAttributes.NotPublic;
						break;
					case Modifier.None:
					case Modifier.Public:
						attr |= System.Reflection.TypeAttributes.Public;
						break;
					default:
						throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a class.", ToString(mods & Modifier.AccessMask));
				}

				if ((mods & Modifier.New) != 0) throw state.ThrowTypeError(null, "The modifier 'new' is not valid for a class.");
			}

			switch (mods & Modifier.TypeMask)
			{
				case Modifier.None:
					break;
				case Modifier.Abstract:
					attr |= System.Reflection.TypeAttributes.Abstract;
					break;
				case Modifier.Sealed:
					attr |= System.Reflection.TypeAttributes.Sealed;
					break;
				default:
					throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a type.", ToString(mods & Modifier.TypeMask));
			}

			Modifier nonTypeMask = Modifier.FieldMask | Modifier.Extern | (Modifier.MethodMask & ~Modifier.TypeMask);
			if ((mods & nonTypeMask) != 0) throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a type.", ToString(mods & nonTypeMask));

			return attr;
		}

		public static System.Reflection.MethodAttributes GetMethodAttribures(CompileState state, Modifier mods)
		{
			System.Reflection.MethodAttributes attr = System.Reflection.MethodAttributes.HideBySig;
			switch(mods & Modifier.AccessMask)
			{
				case Modifier.Private:
					attr |= System.Reflection.MethodAttributes.Private;
					break;
				case Modifier.Internal | Modifier.Private:
				case Modifier.Protected | Modifier.Private:
					attr |= System.Reflection.MethodAttributes.FamANDAssem;
					break;
				case Modifier.Protected:
					attr |= System.Reflection.MethodAttributes.Family;
					break;
				case Modifier.Internal:
					attr |= System.Reflection.MethodAttributes.Assembly;
					break;
				case Modifier.Protected | Modifier.Internal:
					attr |= System.Reflection.MethodAttributes.FamORAssem;
					break;
				case Modifier.None:
				case Modifier.Public:
					attr |= System.Reflection.MethodAttributes.Public;
					break;
				default:
					throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a method.", ToString(mods & Modifier.AccessMask));
			}

			if ((mods & Modifier.Static) != 0) attr |= System.Reflection.MethodAttributes.Static;

			if((mods & Modifier.New) != 0) attr |= System.Reflection.MethodAttributes.NewSlot;

			switch (mods & Modifier.MethodMask)
			{
				case Modifier.None:
					break;
				case Modifier.Abstract:
					attr |= System.Reflection.MethodAttributes.Abstract;
					attr |= System.Reflection.MethodAttributes.Virtual;
					break;
				case Modifier.Sealed | Modifier.Override:
					attr |= System.Reflection.MethodAttributes.Final;
					attr |= System.Reflection.MethodAttributes.Virtual;
					break;
				case Modifier.Override:
				case Modifier.Virtual:
					attr |= System.Reflection.MethodAttributes.Virtual;
					break;
				default:
					throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a method.", ToString(mods & Modifier.MethodMask));
			}

			if ((mods & Modifier.Extern) != 0) state.ThrowNotImplemented(null, "extern method modifier");

			if ((mods & Modifier.FieldMask) != 0) throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a method.", ToString(mods & Modifier.FieldMask));

			return attr;
		}

		public static System.Reflection.FieldAttributes GetFieldAttribures(CompileState state, Modifier mods)
		{
			System.Reflection.FieldAttributes attr = (System.Reflection.FieldAttributes) 0;
			switch (mods & Modifier.AccessMask)
			{
				case Modifier.Private:
					attr |= System.Reflection.FieldAttributes.Private;
					break;
				case Modifier.Internal | Modifier.Private:
				case Modifier.Protected | Modifier.Private:
					attr |= System.Reflection.FieldAttributes.FamANDAssem;
					break;
				case Modifier.Protected:
					attr |= System.Reflection.FieldAttributes.Family;
					break;
				case Modifier.Internal:
					attr |= System.Reflection.FieldAttributes.Assembly;
					break;
				case Modifier.Protected | Modifier.Internal:
					attr |= System.Reflection.FieldAttributes.FamORAssem;
					break;
				case Modifier.None:
				case Modifier.Public:
					attr |= System.Reflection.FieldAttributes.Public;
					break;
				default:
					throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a field.", ToString(mods & Modifier.AccessMask));
			}

			if ((mods & Modifier.Static) != 0) attr |= System.Reflection.FieldAttributes.Static;

			switch (mods & Modifier.FieldMask)
			{
				case Modifier.None:
					break;
				case Modifier.Readonly:
					attr |= System.Reflection.FieldAttributes.InitOnly;
					break;
				case Modifier.Volatile:
					break;
				default:
					throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a field.", ToString(mods & Modifier.FieldMask));
			}

			Modifier nonTypeMask = ~(Modifier.AccessMask | Modifier.Static | Modifier.FieldMask);
			if ((mods & nonTypeMask) != 0) throw state.ThrowTypeError(null, "The modifiers '{0}' are not valid for a field.", ToString(mods & nonTypeMask));

			return attr;
		}

		public static string ToString(Modifier mods)
		{
			List<string> sb = new List<string>();
			if((mods | Modifier.New) != 0) sb.Add("new");
			if((mods | Modifier.Public) != 0) sb.Add("public");
			if((mods | Modifier.Protected) != 0) sb.Add("protected");
			if((mods | Modifier.Internal) != 0) sb.Add("internal");
			if((mods | Modifier.Private) != 0) sb.Add("private");
			if((mods | Modifier.Static) != 0) sb.Add("static");
			if((mods | Modifier.Readonly) != 0) sb.Add("readonly");
			if((mods | Modifier.Volatile) != 0) sb.Add("volatile");
			if((mods | Modifier.Virtual) != 0) sb.Add("virtual");
			if((mods | Modifier.Sealed) != 0) sb.Add("sealed");
			if((mods | Modifier.Override) != 0) sb.Add("override");
			if((mods | Modifier.Abstract) != 0) sb.Add("abstract");
			if((mods | Modifier.Extern) != 0) sb.Add("extern");
			return string.Join(" ", sb.ToArray());
		}
	}

	public abstract class Node
	{
		private IToken token;

	    protected Node(IToken token)
		{
			this.token = token;
		}

		public IToken Token
		{
			get { return token; }
			set { token = value; }
		}

	    [CanBeNull]
	    public static IToken FirstToken<T>(List<T> nodes) where T : Node
		{
			if (nodes == null || nodes.Count == 0) return null;
			else return nodes[0].Token;
		}

		public static List<T> CheckNull<T>(List<T> list)
		{
			return list != null ? list : new List<T>();
		}
	}

	public sealed class Ident : Node
	{
		private readonly string name;

		public Ident(IToken token, string name)
			: base(token)
		{
            Contract.Requires(name != null);

			this.name = name;
		}

	    [ContractInvariantMethod]
	    private void ObjectInvariant()
	    {
	        Contract.Invariant(name != null);
	    }

	    public string Name
		{
			get { return name; }
		}

		public static List<string> ToString(List<Ident> idents)
		{
			return idents.ConvertAll<string>(delegate(Ident ident) { return ident.name; });
		}
	}
}