namespace TinyBee
{
    public class PTStack<T>
    {
        private PTLinkedList<T> mList;

        public void Push(T data)
        {
            if (mList == null)
            {
                mList = new PTLinkedList<T>();
            }
            mList.InsertHead(data);
        }

        public T Pop()
        {
            if (mList == null)
            {
                return default(T);
            }

            T result = mList.HeadData;
            mList.RemoveHead();
            return result;
        }

        public T Top()
        {
            if (mList == null)
            {
                return default(T);
            }

            T result = mList.HeadData;
            return result;
        }

        public bool IsEmpty
        {
            get
            {
                if (mList == null)
                {
                    return true;
                }

                return mList.IsEmpty;
            }
        }
    }
}