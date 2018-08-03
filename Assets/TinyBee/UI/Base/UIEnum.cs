namespace TinyBee.UI.Enum
{
    using System.Collections;
    using System.Collections.Generic;
    using System;

    public class TUIEnum
    {
        public int value { get; private set; }
        public string name { get; private set; }

        public TUIEnum(int value, string name)
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
            TUIEnum tmp = obj as TUIEnum;
            if (tmp == null)
                return false;
            return tmp.value == this.value;
        }

        public static bool operator == (TUIEnum a, TUIEnum b)
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

        public static bool operator != (TUIEnum a, TUIEnum b)
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

    public class UIEnumBase
    {
        protected static readonly Dictionary<int, TUIEnum> mTables = new Dictionary<int, TUIEnum>();

        protected static TUIEnum mNone = new TUIEnum(0, "None");
        protected static TUIEnum mRoot = new TUIEnum(1, "Root");
        protected static TUIEnum mLoading = new TUIEnum(2, "Loading");
        protected static TUIEnum mConnecting = new TUIEnum(3, "Connecting");
        protected static TUIEnum mBubble = new TUIEnum(4, "Bubble");
        protected static TUIEnum mMsg = new TUIEnum(5, "Msg");
        protected static TUIEnum mController = new TUIEnum(6, "Controller");
        protected static TUIEnum mTutorial = new TUIEnum(7, "Tutorial");

        public static int None { get { return mNone.value; } }
        public static int Root { get { return mRoot.value; } }
        public static int Loading { get { return mLoading.value; } }
        public static int Connecting { get { return mConnecting.value; } }
        public static int Bubble { get { return mBubble.value; } }
        public static int Msg { get { return mMsg.value; } }
        public static int Controller { get { return mController.value; } }
        public static int Tutorial { get { return mTutorial.value; } }

        static UIEnumBase()
        {
            mTables = new Dictionary<int, TUIEnum>
            {
                { mNone.value, mNone },
                { mRoot.value, mRoot },
                { mLoading.value, mLoading },
                { mConnecting.value, mConnecting },
                { mBubble.value, mBubble },
                { mMsg.value, mMsg },
                { mController.value, mController },
                { mTutorial.value, mTutorial }
            };
        }

        public static TUIEnum[] GetAllEnum()
        {
            if (mTables == null)
                return null;

            int index = 0;
            TUIEnum[] tmps = new TUIEnum[mTables.Count];
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

        public static TUIEnum GetEnum(int key)
        {
            if (mTables == null)
                return null;

            TUIEnum value;
            if (mTables.TryGetValue(key, out value))
                return value;

            return null;
        }

        public static string GetEnumName(int key)
        {
            if (mTables == null)
                return string.Empty;

            TUIEnum value;
            if (mTables.TryGetValue(key, out value))
                return value.name;

            return string.Empty;
        }
    }
}
