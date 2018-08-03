namespace TinyBee
{
    using System.Collections;
    using System.Collections.Generic;

    public class TQueue<T>
    {
        private List<T> mItems = new List<T>();

        public T this[int index]
        {
            get
            {
                return mItems.Count > 0 ? mItems[index] : default(T);
            }
        }
        public int Count { get { return mItems.Count; } }
        public T Head
        {
            get
            {
                return mItems.Count > 0 ? mItems[0] : default(T);
            }
        }
        public T Tail
        {
            get
            {
                return mItems.Count > 0 ? mItems[mItems.Count - 1] : default(T);
            }
        }

        public bool Enqueue(T item)
        {
            try
            {
                mItems.Add(item);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public T Dequeue()
        {
            if (mItems.Count > 0)
            {
                T tmp = mItems[0];
                mItems.RemoveAt(0);
                return tmp;
            }
            else
            {
                return default(T);
            }
        }

        public void Clear()
        {
            mItems.Clear();
        }

        public bool Remove(int index)
        {
            try
            {
                mItems.RemoveAt(index);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Remove(T item)
        {
            try
            {
                mItems.Remove(item);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    public static class TQueueExtension : object
    {
        public static void Free<T>(this TQueue<T> self)
        {
            if (self != null)
            {
                self.Clear();
                self = null;
            }
        }
    }
}