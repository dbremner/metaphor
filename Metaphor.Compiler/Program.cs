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
using System.IO;
using System.Reflection;
using System.Text;

namespace Metaphor.Compiler
{
	enum Action { None, Save, Run }
	class Program
	{
		static string inputFile;
		static string outputFile;
		static readonly List<string> references = new List<string>();
		static Action action = Action.None;

		static void Main(string[] args)
		{
			try
			{
				foreach (string arg in args) ProcessArg(arg);
				if (inputFile == null) throw new Exception("Must specify input file.");
				if (outputFile == null) outputFile = Path.ChangeExtension(Path.GetFileName(inputFile), ".exe");
				Metaphor.Compiler.Compiler compiler = new Metaphor.Compiler.Compiler(references.ToArray());
				if (action == Action.Run) compiler.Compile(inputFile);
				else
				{
					compiler.Compile(inputFile, outputFile);
					if (action == Action.Save)
					{
						//Assembly asm = AppDomain.CurrentDomain.Load(Path.GetFileNameWithoutExtension(outputFile));
						//int argc = asm.EntryPoint.GetParameters().Length;
						//if (argc == 0) asm.EntryPoint.Invoke(null, null);
						//else if (argc == 1) asm.EntryPoint.Invoke(null, new object[] { new string[] { } });
						//else throw new Exception("Cannot recognise signature of entrypoint.");
						AppDomain.CurrentDomain.ExecuteAssembly(outputFile);
					}
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.ToString());
			}
		}

		static void ProcessArg(string arg)
		{
			if (arg != null && arg != string.Empty)
			{
				if (arg.StartsWith("/"))
				{
					if (arg.StartsWith("/out:")) outputFile = arg.Substring(5);
					else if (arg.StartsWith("/r:")) references.Add(arg.Substring(3));
					else if (arg == "/run") action = Action.Run;
					else if (arg == "/save") action = Action.Save;
					else throw new Exception($"Unknown argument: {arg}");
				}
				else if (inputFile == null) inputFile = arg;
				else throw new Exception("Can only specify one input file.");
			}
		}
	}
}
