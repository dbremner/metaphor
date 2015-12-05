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

namespace Metaphor
{
	public class VirtualTuple
	{
		private readonly int size;
		private readonly Type type;
		private readonly ConstructorInfo ctor;
		private readonly FieldInfo[] fields;

		public VirtualTuple(Type[] types)
		{
			if (types == null || types.Length == 0) throw new ArgumentException("There must be at least one type.");
			else if (types.Length == 1)
			{
				size = 1;
				type = types[0];
			}
			else if (types.Length <= 8)
			{
				size = types.Length;
				type = typeof(VirtualTuple).Module.GetType("Metaphor.Tuples.Tuple`" + types.Length.ToString());
				type = type.MakeGenericType(types);
				ctor = type.GetConstructor(types);
				fields = new FieldInfo[size];
				for (int i = 0; i < size; i++)
					fields[i] = type.GetField("a" + i);
			}
			else throw new NotSupportedException("Maximum tuple size is 8.");
		}

		public int GetSize()
		{
			return size;
		}

		public Type GetTupleType()
		{
			return type;
		}

		public ConstructorInfo GetConstructorInfo()
		{
			if (size == 1) return null;
			else return ctor;
		}

		public FieldInfo GetFieldInfo(int index)
		{
			if (size == 1) return null;
			else return fields[index];
		}

		public object Create(object[] args)
		{
			if (size == 1 && args.Length == 1) return args[0];
			else if (size == args.Length) return type.GetConstructor(new Type[] { typeof(object[]) }).Invoke(new object[] { args });
			else throw new ArgumentException($"Expecting {size} objects; got {args.Length}.");
		}
	}
}

namespace Metaphor.Tuples
{
	public class Tuple<A0>
	{
		public A0 a0;
		
		public Tuple(A0 a0)
		{
			this.a0 = a0;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
		}
	}
	
	public class Tuple<A0,A1>
	{
		public A0 a0;
		public A1 a1;
		
		public Tuple(A0 a0, A1 a1)
		{
			this.a0 = a0;
			this.a1 = a1;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
			this.a1 = (A1)a[1];
		}
	}
	
	public class Tuple<A0,A1,A2>
	{
		public A0 a0;
		public A1 a1;
		public A2 a2;
		
		public Tuple(A0 a0, A1 a1, A2 a2)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
			this.a1 = (A1)a[1];
			this.a2 = (A2)a[2];
		}
	}
	
	public class Tuple<A0,A1,A2,A3>
	{
		public A0 a0;
		public A1 a1;
		public A2 a2;
		public A3 a3;
		
		public Tuple(A0 a0, A1 a1, A2 a2, A3 a3)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
			this.a1 = (A1)a[1];
			this.a2 = (A2)a[2];
			this.a3 = (A3)a[3];
		}
	}
	
	public class Tuple<A0,A1,A2,A3,A4>
	{
		public A0 a0;
		public A1 a1;
		public A2 a2;
		public A3 a3;
		public A4 a4;
		
		public Tuple(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
			this.a4 = a4;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
			this.a1 = (A1)a[1];
			this.a2 = (A2)a[2];
			this.a3 = (A3)a[3];
			this.a4 = (A4)a[4];
		}
	}
	
	public class Tuple<A0,A1,A2,A3,A4,A5>
	{
		public A0 a0;
		public A1 a1;
		public A2 a2;
		public A3 a3;
		public A4 a4;
		public A5 a5;
		
		public Tuple(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
			this.a4 = a4;
			this.a5 = a5;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
			this.a1 = (A1)a[1];
			this.a2 = (A2)a[2];
			this.a3 = (A3)a[3];
			this.a4 = (A4)a[4];
			this.a5 = (A5)a[5];
		}
	}
	
	public class Tuple<A0,A1,A2,A3,A4,A5,A6>
	{
		public A0 a0;
		public A1 a1;
		public A2 a2;
		public A3 a3;
		public A4 a4;
		public A5 a5;
		public A6 a6;
		
		public Tuple(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
			this.a4 = a4;
			this.a5 = a5;
			this.a6 = a6;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
			this.a1 = (A1)a[1];
			this.a2 = (A2)a[2];
			this.a3 = (A3)a[3];
			this.a4 = (A4)a[4];
			this.a5 = (A5)a[5];
			this.a6 = (A6)a[6];
		}
	}
	
	public class Tuple<A0,A1,A2,A3,A4,A5,A6,A7>
	{
		public A0 a0;
		public A1 a1;
		public A2 a2;
		public A3 a3;
		public A4 a4;
		public A5 a5;
		public A6 a6;
		public A7 a7;
		
		public Tuple(A0 a0, A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
		{
			this.a0 = a0;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
			this.a4 = a4;
			this.a5 = a5;
			this.a6 = a6;
			this.a7 = a7;
		}

		public Tuple(object[] a)
		{
			this.a0 = (A0)a[0];
			this.a1 = (A1)a[1];
			this.a2 = (A2)a[2];
			this.a3 = (A3)a[3];
			this.a4 = (A4)a[4];
			this.a5 = (A5)a[5];
			this.a6 = (A6)a[6];
			this.a7 = (A7)a[7];
		}
	}
}

