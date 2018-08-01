namespace TinyBee
{
	using System.Collections.Generic;
	using System;
	using System.Linq;
	using UnityEngine;

	public static class IEnumerableExtension : object
	{
		public static T GetRandomItem<T>(this List<T> self)
		{
			return self[UnityEngine.Random.Range(0, self.Count - 1)];
		}

		public static int GetRandomWithPower(this List<int> powers)
		{
			var sum = 0;
			foreach (var power in powers)
			{
				sum += power;
			}

			var randomNum = UnityEngine.Random.Range(0, sum);
			var currentSum = 0;
			for (var i = 0; i < powers.Count; i++)
			{
				var nextSum = currentSum + powers[i];
				if (randomNum >= currentSum && randomNum <= nextSum)
				{
					return i;
				}
				currentSum = nextSum;
			}
			return -1;
		}

		public static T[] ForEach<T>(this T[] self, Action<T> action)
		{
			Array.ForEach(self, action);
			return self;
		}

		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action)
		{
			if (action == null) throw new ArgumentException();
			foreach (var item in self)
			{
				action(item);
			}
			return self;
		}

		public static void ForEach<T>(this List<T> self, Action<int, T> action)
		{
			for (var i = 0; i < self.Count; i++)
			{
				action(i, self[i]);
			}
		}

        public static void ForEach<K, V>(this Dictionary<K, V> self, Action<K, V> action)
        {
            var dictE = self.GetEnumerator();
            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                action(current.Key, current.Value);
            }
            dictE.Dispose();
        }

        public static List<T> ForEachReverse<T>(this List<T> self, Action<T> action)
		{
			if (action == null) throw new ArgumentException();

			for (var i = self.Count - 1; i >= 0; --i)
				action(self[i]);

			return self;
		}

		public static void CopyTo<T>(this List<T> from, List<T> to, int begin = 0, int end = -1)
		{
			if (begin < 0)
				begin = 0;

			var endIndex = Mathf.Min(from.Count, to.Count) - 1;

			if (end == -1 || end >= endIndex)
				end = endIndex;

			for (var i = begin; i < end; i++)
				to[i] = from[i];
		}

        public static void Free<T>(this List<T> self)
        {
            if (self != null)
            {
                self.Clear();
                self = null;
            }
        }

        public static void Free<T>(this Stack<T> self)
        {
            if (self != null)
            {
                self.Clear();
                self = null;
            }
        }

        public static void Free<K, V>(this Dictionary<K, V> self)
        {
            if (self != null)
            {
                self.Clear();
                self = null;
            }
        }
    }
}