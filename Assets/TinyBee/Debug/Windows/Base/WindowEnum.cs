namespace TinyBee.Debug
{
    using System.Collections;
    using System.Collections.Generic;
    using System;

    public class TWindowEnum
    {
        public int value { get; private set; }
        public string name { get; private set; }

        public TWindowEnum(int value, string name)
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
            TWindowEnum tmp = obj as TWindowEnum;
            if (tmp == null)
                return false;
            return tmp.value == this.value;
        }

        public static bool operator == (TWindowEnum a, TWindowEnum b)
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

        public static bool operator != (TWindowEnum a, TWindowEnum b)
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

    public class WindowEnumBase
    {
        protected static readonly Dictionary<int, TWindowEnum> mTables = new Dictionary<int, TWindowEnum>();

        public static TWindowEnum mNone = new TWindowEnum(0, "None");
        public static TWindowEnum mMsg = new TWindowEnum(1, "Msg");
        public static TWindowEnum mDevice = new TWindowEnum(2, "Device");
        public static TWindowEnum mSystem = new TWindowEnum(3, "System");
        public static TWindowEnum mNetwork = new TWindowEnum(4, "Network");
        public static TWindowEnum mUI = new TWindowEnum(5, "UI");

        public static int None { get { return mNone.value; } }
        public static int Msg { get { return mMsg.value; } }
        public static int Device { get { return mDevice.value; } }
        public static int System { get { return mSystem.value; } }
        public static int Network { get { return mNetwork.value; } }
        public static int UI { get { return mUI.value; } }

        static WindowEnumBase()
        {
            mTables = new Dictionary<int, TWindowEnum>
            {
                { mNone.value, mNone },
                { mMsg.value, mMsg },
                { mDevice.value, mDevice },
                { mSystem.value, mSystem },
                { mNetwork.value, mNetwork },
                { mUI.value, mUI }
            };
        }

        public static TWindowEnum[] GetAllEnum()
        {
            if (mTables == null)
                return null;

            int index = 0;
            TWindowEnum[] tmps = new TWindowEnum[mTables.Count];
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

        public static TWindowEnum GetEnum(int key)
        {
            if (mTables == null)
                return null;

            TWindowEnum value;
            if (mTables.TryGetValue(key, out value))
                return value;

            return null;
        }

        public static string GetEnumName(int key)
        {
            if (mTables == null)
                return string.Empty;

            TWindowEnum value;
            if (mTables.TryGetValue(key, out value))
                return value.name;

            return string.Empty;
        }
    }
}