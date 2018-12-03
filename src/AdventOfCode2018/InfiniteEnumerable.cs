using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
	internal class InfiniteEnumerable<T> : IEnumerable<T>
	{
		private readonly T[] entries;

		public InfiniteEnumerable(IEnumerable<T> seed)
		{
			if (seed == null)
			{
				throw new ArgumentNullException(nameof(seed));
			}

			entries = new T[seed.Count()];
			int i = 0;
			foreach (var item in seed)
			{
				entries[i] = item;
				i++;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new InfiniteEnumerator<T>(entries);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	internal class InfiniteEnumerator<T> : IEnumerator<T>
	{
		private T[] entries;
		private int position = -1;

		public InfiniteEnumerator(T[] entries)
		{
			this.entries = entries;
		}

		public T Current => entries[position];

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			entries = null;
		}

		public bool MoveNext()
		{
			if (position == entries.Length - 1)
			{
				Reset();
			}

			position++;
			return true;
		}

		public void Reset()
		{
			position = -1;
		}
	}
}
