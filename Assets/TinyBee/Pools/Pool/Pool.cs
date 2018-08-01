namespace TinyBee.Pool
{
    using System.Collections.Generic;

    public abstract class Pool<T> : IPool<T>, ICountObserveAble
    {
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }
        
        protected IObjectFactory<T> mFactory;

        protected readonly Stack<T> mCacheStack = new Stack<T>();

        protected int mMaxCount = 12;

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0 ? mFactory.Create() : mCacheStack.Pop();
        }

        public abstract bool Recycle(T obj);
    }
}