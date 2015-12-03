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

namespace Metaphor.Collections
{
	[Serializable]
	public class Set<T>: ICollection<T>, IEnumerable<T>
	{
		private Dictionary<T,object> map;

		public Set()
		{
			this.map = new Dictionary<T,object>();
		}

		public void Add(T element)
		{
			if(!map.ContainsKey(element))
				map.Add(element, new object());
		}

		public void Clear()
		{
			map.Clear();
		}

		public bool Contains(T element)
		{
			return map.ContainsKey(element);
		}

		public bool Remove(T element)
		{
			return map.Remove(element);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public T[] ToArray()
		{
			int count = this.Count;
			T[] array = new T[count];
			this.CopyTo(array, 0);
			return array;
		}

		#region ICollection<T>
		public void CopyTo(T[] array, int arrayIndex)
		{
			map.Keys.CopyTo(array, arrayIndex);
		}

		public int Count { get { return map.Count; } }
		#endregion

		#region IEnumerable<T>
		public IEnumerator<T> GetEnumerator()
		{
			return map.Keys.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return map.Keys.GetEnumerator();
		}
		#endregion
	}

	[Serializable]
	public struct Tuple<A, B>
	{
		public A fst;
		public B snd;

		public Tuple(A fst, B snd)
		{
			this.fst = fst;
			this.snd = snd;
		}

		public A Fst
		{
			get { return fst; }
			set { fst = value; }
		}

		public B Snd
		{
			get { return snd; }
			set { snd = value; }
		}

		public override bool Equals(object obj)
		{
			if (obj is Tuple<A, B>)
			{
				Tuple<A, B> tuple = (Tuple<A, B>)obj;
				return object.Equals(fst, tuple.fst) && object.Equals(snd, tuple.snd);
			}
			else return false;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			if (fst != null) hash ^= fst.GetHashCode();
			if (snd != null) hash ^= snd.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", fst, snd);
		}

		public static bool operator ==(Tuple<A, B> x, Tuple<A, B> y)
		{
			return object.Equals(x.fst, y.fst) && object.Equals(x.snd, y.snd);
		}

		public static bool operator !=(Tuple<A, B> x, Tuple<A, B> y)
		{
			return !object.Equals(x.fst, y.fst) || !object.Equals(x.snd, y.snd);
		}

		public static Tuple<A, B>[] Zip(A[] a, B[] b)
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);

			int n = Math.Min(a.Length, b.Length);
			Tuple<A, B>[] zip = new Tuple<A, B>[n];
			for (int i = 0; i < n; i++)
				zip[i] = new Tuple<A, B>(a[i], b[i]);
			return zip;
		}

		public static Tuple<A[], B[]> Unzip(Tuple<A, B>[] zip)
		{
			Contract.Requires(zip != null);

			Tuple<A[], B[]> tuple = new Tuple<A[], B[]>();
			Unzip(zip, out tuple.fst, out tuple.snd);
			return tuple;
		}

		public static void Unzip(Tuple<A, B>[] zip, out A[] a, out B[] b)
		{
			Contract.Requires(zip != null);

			int n = zip.Length;
			a = new A[n];
			b = new B[n];
			for (int i = 0; i < n; i++)
			{
				a[i] = zip[i].fst;
				b[i] = zip[i].snd;
			}
		}

		public static A[] MapFst(Tuple<A, B>[] zip)
		{
			Contract.Requires(zip != null);

			int n = zip.Length;
			A[] a = new A[n];
			for (int i = 0; i < n; i++)
				a[i] = zip[i].fst;
			return a;
		}

		public static B[] MapSnd(Tuple<A, B>[] zip)
		{
			Contract.Requires(zip != null);

			int n = zip.Length;
			B[] b = new B[n];
			for (int i = 0; i < n; i++)
				b[i] = zip[i].snd;
			return b;
		}
	}

	[Serializable]
	public struct Tuple<A, B, C>
	{
		public A fst;
		public B snd;
		public C trd;

		public Tuple(A fst, B snd, C trd)
		{
			this.fst = fst;
			this.snd = snd;
			this.trd = trd;
		}

		public A Fst
		{
			get { return fst; }
			set { fst = value; }
		}

		public B Snd
		{
			get { return snd; }
			set { snd = value; }
		}

		public C Trd
		{
			get { return trd; }
			set { trd = value; }
		}

		public override bool Equals(object obj)
		{
			if (obj is Tuple<A, B, C>)
			{
				Tuple<A, B, C> tuple = (Tuple<A, B, C>)obj;
				return object.Equals(fst, tuple.fst) && object.Equals(snd, tuple.snd) && object.Equals(trd, tuple.trd);
			}
			else return false;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			if (fst != null) hash ^= fst.GetHashCode();
			if (snd != null) hash ^= snd.GetHashCode();
			if (trd != null) hash ^= trd.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", fst, snd, trd);
		}

		public static bool operator ==(Tuple<A, B, C> x, Tuple<A, B, C> y)
		{
			return object.Equals(x.fst, y.fst) && object.Equals(x.snd, y.snd) && object.Equals(x.trd, y.trd);
		}

		public static bool operator !=(Tuple<A, B, C> x, Tuple<A, B, C> y)
		{
			return !object.Equals(x.fst, y.fst) || !object.Equals(x.snd, y.snd) || !object.Equals(x.trd, y.trd);
		}
	}

	[Serializable]
	public struct Tuple<A, B, C, D>
	{
		public A fst;
		public B snd;
		public C trd;
		public D fth;

		public Tuple(A fst, B snd, C trd, D fth)
		{
			this.fst = fst;
			this.snd = snd;
			this.trd = trd;
			this.fth = fth;
		}

		public A Fst
		{
			get { return fst; }
			set { fst = value; }
		}

		public B Snd
		{
			get { return snd; }
			set { snd = value; }
		}

		public C Trd
		{
			get { return trd; }
			set { trd = value; }
		}

		public D Fth
		{
			get { return fth; }
			set { fth = value; }
		}

		public override bool Equals(object obj)
		{
			if (obj is Tuple<A, B, C>)
			{
				Tuple<A, B, C, D> tuple = (Tuple<A, B, C, D>)obj;
				return object.Equals(fst, tuple.fst) && object.Equals(snd, tuple.snd) && object.Equals(trd, tuple.trd) && object.Equals(fth, tuple.fth);
			}
			else return false;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			if (fst != null) hash ^= fst.GetHashCode();
			if (snd != null) hash ^= snd.GetHashCode();
			if (trd != null) hash ^= trd.GetHashCode();
			if (fth != null) hash ^= fth.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2}, {3})", fst, snd, trd, fth);
		}

		public static bool operator ==(Tuple<A, B, C, D> x, Tuple<A, B, C, D> y)
		{
			return object.Equals(x.fst, y.fst) && object.Equals(x.snd, y.snd) && object.Equals(x.trd, y.trd) && object.Equals(x.fth, y.fth);
		}

		public static bool operator !=(Tuple<A, B, C, D> x, Tuple<A, B, C, D> y)
		{
			return !object.Equals(x.fst, y.fst) || !object.Equals(x.snd, y.snd) || !object.Equals(x.trd, y.trd) || !object.Equals(x.fth, y.fth);
		}
	}

	[Serializable]
	public struct Either<A, B>
	{
		private bool which;
		private A left;
		private B right;

		public Either(A left)
		{
			this.which = false;
			this.left = left;
			this.right = default(B);
		}

		public Either(B right)
		{
			this.which = true;
			this.left = default(A);
			this.right = right;
		}

		public bool IsLeft
		{
			get { return !which; }
		}

		public A Left
		{
			get
			{
				if (!which) return left;
				else throw new InvalidOperationException();
			}
		}

		public bool IsRight
		{
			get { return which; }
		}

		public B Right
		{
			get
			{
				if (!which) throw new InvalidOperationException();
				else return right;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Either<A, B>)
			{
				Either<A, B> either = (Either<A, B>)obj;
				return (!which && !either.which && object.Equals(left, either.left)) || (which && either.which && object.Equals(right, either.right));
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			if(!which)
			{
				if (!object.ReferenceEquals(left, null)) return left.GetHashCode();
				else return 0;
			}
			else
			{
				if (!object.ReferenceEquals(right, null)) return right.GetHashCode();
				else return 0;
			}
		}

		public override string ToString()
		{
			if (!which) return string.Format("Left {0}", left);
			else return string.Format("Right {1}", right);
		}

		public static bool operator ==(Either<A, B> x, Either<A, B> y)
		{
			return (!x.which && !y.which && object.Equals(x.left, y.left)) || (x.which && y.which && object.Equals(x.right, y.right));
		}

		public static bool operator !=(Either<A, B> x, Either<A, B> y)
		{
			return (x.which || y.which || !object.Equals(x.left, y.left)) && (!x.which || !y.which || !object.Equals(x.right, y.right));
		}

		public static implicit operator Either<A, B>(A value)
		{
			return new Either<A, B>(value);
		}

		public static implicit operator Either<A, B>(B value)
		{
			return new Either<A, B>(value);
		}
	}

	[Serializable]
	public class GroupStack<T> : IEnumerable<T>
	{
		protected Stack<T> stack = new Stack<T>();
		protected Stack<int> marks = new Stack<int>();

		public GroupStack()
		{
		}

		public int Count
		{
			get { return stack.Count; }
		}

		public void Clear()
		{
			stack.Clear();
			marks.Clear();
		}

		public bool Contains(T item)
		{
			return stack.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			stack.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return stack.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return stack.GetEnumerator();
		}

		//public T Peek()
		//{
		//    return stack.Peek();
		//}

		public void Pop()
		{
			int mark = marks.Pop();
			while(stack.Count > mark) stack.Pop();
		}

		public void Push(T item)
		{
			stack.Push(item);
		}

		public void PushAndMark(T item)
		{
			marks.Push(stack.Count);
			stack.Push(item);
		}

		public void Push(IEnumerable<T> items)
		{
			foreach(T item in items)
				stack.Push(item);
		}

		public void PushAndMark(IEnumerable<T> items)
		{
			marks.Push(stack.Count);
			foreach (T item in items)
				stack.Push(item);
		}

		public void Mark()
		{
			marks.Push(stack.Count);
		}

		public T[] ToArray()
		{
			return stack.ToArray();
		}

		public void TrimExcess()
		{
			stack.TrimExcess();
			marks.TrimExcess();
		}
	}

	[Serializable]
	public class GroupDictionary<K, V> : IDictionary<K, V>
	{
		protected SortedDictionary<K, V> dictionary = new SortedDictionary<K, V>();
		protected Stack<List<K>> marks = new Stack<List<K>>();

		public GroupDictionary()
		{
		}

		public void Add(K key, V value)
		{
			dictionary.Add(key, value);
			if (marks.Count > 0) marks.Peek().Add(key);
		}

		void ICollection<KeyValuePair<K,V>>.Add(KeyValuePair<K, V> item)
		{
			((ICollection<KeyValuePair<K, V>>)dictionary).Add(item);
		}
		
		public void Clear()
		{
			dictionary.Clear();
			marks.Clear();
		}

		public bool ContainsKey(K key)
		{
			return dictionary.ContainsKey(key);
		}

		bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
		{
			return ((ICollection<KeyValuePair<K, V>>)dictionary).Contains(item);
		}

		void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<K, V>>)dictionary).CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return dictionary.Count; }
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		bool ICollection<KeyValuePair<K, V>>.IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<K, V>>)dictionary).IsReadOnly; }
		}

		public ICollection<K> Keys
		{
			get { return dictionary.Keys; }
		}

		public void Mark()
		{
			marks.Push(new List<K>());
		}

		public void Remove()
		{
			foreach(K key in marks.Pop()) dictionary.Remove(key);
		}

		public bool Remove(K key)
		{
			if (marks.Count == 0) return dictionary.Remove(key);
			else throw new InvalidOperationException("Cannot remove individual key from group dictionary.");
		}

		bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
		{
			return ((ICollection<KeyValuePair<K, V>>)dictionary).Remove(item);
		}

		public V this[K key]
		{
			get { return dictionary[key]; }
			set { dictionary[key] = value; }
		}

		public bool TryGetValue(K key, out V value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public ICollection<V> Values
		{
			get { return dictionary.Values; }
		}
	}
}
