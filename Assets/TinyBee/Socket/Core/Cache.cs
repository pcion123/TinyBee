namespace TinyBee.Net
{
	public class Cache<T> where T : class , new()
	{
		private T[] mCaches;
		private int mCacheCount;
		private int mCacheMax;
		private int mLeapCount;

		public Cache(int max)
		{
			mCaches = new T[max];
			mCacheMax = max;
		}

		public T NewNode()
		{
			if (mCacheCount > 0)
			{
				int index = --mCacheCount;
				T tmp = mCaches[index];
				mCaches[index] = null;
				return tmp;
			}
			else
			{
				return new T();
			}
		}

		public void DisposeNode(T node)
		{
			if (mCacheCount >= mCacheMax)
			{
				mLeapCount--;
			}
			else
			{
				mCaches[mCacheCount++] = node;
			}
		}
	}
}
