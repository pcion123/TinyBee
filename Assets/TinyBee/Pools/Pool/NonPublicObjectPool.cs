namespace TinyBee.Pool
{
    using System;

	public class NonPublicObjectPool<T> : Pool<T>, ISingleton where T : class, IPoolable
	{
		public void OnSingletonInit() {}
		
		public static NonPublicObjectPool<T> Instance
		{
			get { return SingletonProperty<NonPublicObjectPool<T>>.Instance; }
		}

		protected NonPublicObjectPool()
		{
			mFactory = new NonPublicObjectFactory<T>();
		}
		
		public void Dispose()
		{
			SingletonProperty<NonPublicObjectPool<T>>.Dispose();
		}

		public void Init(int maxCount, int initCount)
		{
			if (maxCount > 0)
			{
				initCount = Math.Min(maxCount, initCount);
			}

			if (CurCount >= initCount) return;
			
			for (var i = CurCount; i < initCount; ++i)
			{
				Recycle(mFactory.Create());
			}
		}

		public int MaxCacheCount
		{
			get { return mMaxCount; }
			set
			{
				mMaxCount = value;

				if (mCacheStack == null) return;
				if (mMaxCount <= 0) return;
				if (mMaxCount >= mCacheStack.Count) return;
				var removeCount = mMaxCount - mCacheStack.Count;
				while (removeCount > 0)
				{
					mCacheStack.Pop();
					--removeCount;
				}
			}
		}

		public override T Allocate()
		{
			var result = base.Allocate();
			result.IsRecycled = false;
			return result;
		}

		public override bool Recycle(T t)
		{
			if (t == null || t.IsRecycled)
			{
				return false;
			}

			if (mMaxCount > 0)
			{
				if (mCacheStack.Count >= mMaxCount)
				{
					t.OnRecycled();
					return false;
				}
			}

			t.IsRecycled = true;
			t.OnRecycled();
			mCacheStack.Push(t);

			return true;
		}
	}
}