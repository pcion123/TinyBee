namespace TinyBee.Pool
{
    using System;

    public interface IPoolType
    {
        void Recycle2Cache();
    }

    public interface IPoolable
    {
        void OnRecycled();
        bool IsRecycled { get; set; }
    }

    public interface ICountObserveAble
    {
        int CurCount { get; }
    }

    public class SafeObjectPool<T> : Pool<T>, ISingleton where T : IPoolable, new()
    {
        void ISingleton.OnSingletonInit() {}

        protected SafeObjectPool()
        {
            mFactory = new DefaultObjectFactory<T>();
        }

        public static SafeObjectPool<T> Instance
        {
            get { return SingletonProperty<SafeObjectPool<T>>.Instance; }
        }

        public void Dispose()
        {
            SingletonProperty<SafeObjectPool<T>>.Dispose();
        }

        public void Init(int maxCount, int initCount)
        {
            MaxCacheCount = maxCount;
            
            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount < initCount)
            {
                for (int i = CurCount; i < initCount; ++i)
                {
                    Recycle(new T());
                }
            }
        }

        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;

                if (mCacheStack != null)
                {
                    if (mMaxCount > 0)
                    {
                        if (mMaxCount < mCacheStack.Count)
                        {
                            int removeCount = mMaxCount - mCacheStack.Count;
                            while (removeCount > 0)
                            {
                                mCacheStack.Pop();
                                --removeCount;
                            }
                        }
                    }
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