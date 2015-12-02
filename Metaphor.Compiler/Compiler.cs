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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

using M = Metaphor;

namespace Metaphor.Compiler
{
	public class Compiler
	{
		private Assembly[] externs;

		public Compiler()
		{
			this.externs = new Assembly[] {};
		}

		public Compiler(string[] references)
		{
			int n = references.Length;
			this.externs = new Assembly[n];
			string root = Path.GetDirectoryName(typeof(void).Assembly.Location);
			for(int i = 0; i < n; i++)
			{
				string reference = references[i];
				if (File.Exists(reference)) externs[i] = Assembly.LoadFrom(reference);
				else 
				{
					reference = Path.Combine(root, reference);
					if (File.Exists(reference)) externs[i] = Assembly.LoadFrom(reference);
					else throw new FileNotFoundException("Could not load assembly reference.", references[i]);
				}
			}
		}

		public Compiler(Assembly[] externs)
		{
			this.externs = externs;
		}

		public void Compile(string inputFile, string outputFile)
		{
			Module program;
			using(StreamReader sr = File.OpenText(inputFile)) 
				program = Parse(sr);
			M.MModuleBuilder module = TypeCheck(externs, program);
			module.Save(Path.GetFileNameWithoutExtension(outputFile), outputFile);
		}

		public void Compile(string inputFile)
		{
			Module program;
			using (StreamReader sr = File.OpenText(inputFile))
				program = Parse(sr);
			M.MModuleBuilder module = TypeCheck(externs, program);
			module.Run();
		}

		public static Module Parse(StreamReader s) 
		{
			return Parse(s.ReadToEnd());
		}

		public static Module Parse(string input) 
		{
			L lexer = new L(new StringReader(input.TrimEnd('\r', '\n')));
			P parser = new P(lexer);
			return parser.compilation_unit();
		}

		public static M.MModuleBuilder TypeCheck(Assembly[] externs, Module program)
		{
			CompileState state = new CompileState(externs);
			program.CompileTypes(state);
			program.CompileMembers(state);
			return program.CompileCode(state);
		}

	}

}
