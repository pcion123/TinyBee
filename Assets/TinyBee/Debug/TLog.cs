namespace TinyBee.Debug
{
    using System.Collections;
    using System.Collections.Generic;

    public class TLog : object
    {
        private List<string> mMessages = null;
        private int mMessageLine = 0;
        private int mMaxSaveLine = 50;
        private int mMaxShowLine = 28;

        public List<string> Messages { get { return mMessages; } }
        public int Count { get { return mMessages.Count; } }
        public int MessageLine { get { return mMessageLine; } }
        public int MaxSaveLine { get { return mMaxSaveLine; } }
        public int MaxShowLine { get { return mMaxShowLine; } }

        public TLog()
        {
            mMessages = new List<string>();
        }

        public void Dispose()
        {
            if (mMessages != null)
            {
                mMessages.Clear();
                mMessages = null;
            }
        }

        //清空訊息
        public void Clear()
        {
            mMessages.Clear();
            mMessageLine = 0;
        }

        //加入訊息
        public void Log(string vMsg)
        {
            if (mMessages == null)
                mMessages = new List<string>();

            if (mMessages.Count > mMaxSaveLine)
                Clear();

            mMessageLine++;

            mMessages.Add(mMessageLine.ToString("D2") + " " + vMsg);
        }

        public void AddLine()
        {
            if (mMessageLine < mMessages.Count)
                mMessageLine++;
        }

        public void DelLine()
        {
            if (mMessageLine > mMaxShowLine)
                mMessageLine--;
        }
    }
}