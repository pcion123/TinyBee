namespace TinyBee
{
    using System.Collections;
    using System.Collections.Generic;

    public class TStack<T>
    {
        private List<T> mItems = new List<T>();

        public T this [int index]
        {
            get
            {
                return mItems.Count > 0 ? mItems[index] : default(T);
            }
        }
        public int Count { get { return mItems.Count; } }
        public T Top
        {
            get
            {
                return mItems.Count > 0 ? mItems[mItems.Count - 1] : default(T);
            }
        }

        public bool Push(T item)
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

        public T Pop()
        {
            if (mItems.Count > 0)
            {
                T tmp = mItems[mItems.Count - 1];
                mItems.RemoveAt(mItems.Count - 1);
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

    public static class TStackExtension : object
    {
        public static void Free<T>(this TStack<T> self)
        {
            if (self != null)
            {
                self.Clear();
                self = null;
            }
        }
    }
}