namespace TinyBee
{
    using System.Collections;
    using System.Collections.Generic;
    using System;

    public class TMgrEnum
    {
        public int value { get; private set; }
        public string name { get; private set; }

        public TMgrEnum(int value, string name)
        {
            this.value = value;
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            TMgrEnum tmp = obj as TMgrEnum;
            if (tmp == null)
                return false;
            return tmp.value == this.value;
        }

        public static bool operator == (TMgrEnum a, TMgrEnum b)
        {
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }
            else if ((object)a == null || (object)b == null)
            {
                return false;
            }
            return a.value == b.value;
        }

        public static bool operator != (TMgrEnum a, TMgrEnum b)
        {
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }
            else if ((object)a == null || (object)b == null)
            {
                return false;
            }
            return a.value != b.value;
        }
    }

    public class MgrEnumBase
    {
        protected static readonly Dictionary<int, TMgrEnum> mTables = new Dictionary<int, TMgrEnum>();

        protected static TMgrEnum mNone = new TMgrEnum(0, "None");
        protected static TMgrEnum mDebug = new TMgrEnum(1, "Debug");
        protected static TMgrEnum mGame = new TMgrEnum(2, "Game");
        protected static TMgrEnum mCoroutine = new TMgrEnum(3, "Coroutine");
        protected static TMgrEnum mAudio = new TMgrEnum(4, "Audio");
        protected static TMgrEnum mData = new TMgrEnum(5, "Data");
        protected static TMgrEnum mNet = new TMgrEnum(6, "Net");
        protected static TMgrEnum mUI = new TMgrEnum(7, "UI");
        protected static TMgrEnum mDownloader = new TMgrEnum(8, "Downloader");

        public static int None { get { return mNone.value; } }
        public static int Debug { get { return mDebug.value; } }
        public static int Game { get { return mGame.value; } }
        public static int Coroutine { get { return mCoroutine.value; } }
        public static int Audio { get { return mAudio.value; } }
        public static int Data { get { return mData.value; } }
        public static int Net { get { return mNet.value; } }
        public static int UI { get { return mUI.value; } }
        public static int Downloader { get { return mDownloader.value; } }

        static MgrEnumBase()
        {
            mTables = new Dictionary<int, TMgrEnum>
            {
                { mNone.value, mNone },
                { mDebug.value, mDebug },
                { mGame.value, mGame },
                { mCoroutine.value, mCoroutine },
                { mAudio.value, mAudio },
                { mData.value, mData },
                { mNet.value, mNet },
                { mUI.value, mUI },
                { mDownloader.value, mDownloader }
            };
        }

        public static TMgrEnum[] GetAllEnum()
        {
            if (mTables == null)
                return null;

            int index = 0;
            TMgrEnum[] tmps = new TMgrEnum[mTables.Count];
            var tmp = mTables.GetEnumerator();
            while (tmp.MoveNext())
            {
                tmps[index++] = tmp.Current.Value;
            }
            tmp.Dispose();
            return tmps;
        }

        public static int[] GetAllEnumValue()
        {
            if (mTables == null)
                return null;

            int index = 0;
            int[] tmps = new int[mTables.Count];
            var tmp = mTables.GetEnumerator();
            while (tmp.MoveNext())
            {
                tmps[index++] = tmp.Current.Value.value;
            }
            tmp.Dispose();
            return tmps;
        }

        public static string[] GetAllEnumName()
        {
            if (mTables == null)
                return null;

            int index = 0;
            string[] tmps = new string[mTables.Count];
            var tmp = mTables.GetEnumerator();
            while (tmp.MoveNext())
            {
                tmps[index++] = tmp.Current.Value.name;
            }
            tmp.Dispose();
            return tmps;
        }

        public static TMgrEnum GetEnum(int key)
        {
            if (mTables == null)
                return null;

            TMgrEnum value;
            if (mTables.TryGetValue(key, out value))
                return value;

            return null;
        }

        public static string GetEnumName(int key)
        {
            if (mTables == null)
                return string.Empty;

            TMgrEnum value;
            if (mTables.TryGetValue(key, out value))
                return value.name;

            return string.Empty;
        }
    }
}
