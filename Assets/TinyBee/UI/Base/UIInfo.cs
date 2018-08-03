namespace TinyBee.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
	using Enum;

    public class TUIInfo
    {
        public int ui { get; private set; }
        public string name { get; private set; }
        public Type type { get; private set; }
        public int param { get; private set; }
        public int depth { get; private set; }
        public int rank { get; private set; }

        public TUIInfo(int ui, string name, Type type, eUIParam param, int depth, int rank)
        {
            this.ui = ui;
            this.name = name;
            this.type = type;
            this.param = (int)param;
            this.depth = depth;
            this.rank = rank;
        }

        public TUIInfo(int ui, string name, Type type, int param, int depth, int rank)
        {
            this.ui = ui;
            this.name = name;
            this.type = type;
            this.param = param;
            this.depth = depth;
            this.rank = rank;
        }

        public override string ToString()
        {
            return string.Format("UI={0}   Name={1}   Type={2}   Param={3}   Depth={4}   Rank={5}", UIEnumBase.GetEnumName(ui), name, type.ToString(), param, depth, rank);
        }

        public override bool Equals(object obj)
        {
            TUIInfo tmp = obj as TUIInfo;
            if (tmp == null)
                return false;
            return tmp.ui == this.ui;
        }

        public static bool operator == (TUIInfo a, TUIInfo b)
        {
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }
            else if ((object)a == null || (object)b == null)
            {
                return false;
            }
            return a.ui == b.ui;
        }

        public static bool operator != (TUIInfo a, TUIInfo b)
        {
            if (Object.ReferenceEquals(a, b))
            {
                return false;
            }
            else if ((object)a == null || (object)b == null)
            {
                return true;
            }
            return a.ui != b.ui;
        }
    }
}
