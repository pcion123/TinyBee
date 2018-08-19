namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using TinyBee.Debug;
	using TinyBee.Context;

	[TMonoSingletonPath("[Debug]/DebugMgr")]
	public class DebugMgr : TMgrBehaviour, ISingleton
    {
		private bool mIsShow = false;
		private GUIStyle mStyle;
		private Dictionary<int, TWindow> mWindowTable = null;

		public static DebugMgr Instance
		{
			get { return MonoSingletonProperty<DebugMgr>.Instance; }
		}

		public override int ManagerId
		{
			get { return MgrEnumBase.Debug; }
		}

		public void OnSingletonInit() {}

        public override void Init(params object[] param)
		{
            mStyle = new GUIStyle();
            mStyle.alignment = TextAnchor.MiddleCenter;

            mWindowTable = new Dictionary<int, TWindow>
			{
				{WindowEnumBase.Msg, new MsgWindow(WindowEnumBase.Msg, "Window_MSG", new Rect(200, 50, 400, 450), false)},
                {WindowEnumBase.Device, new DeviceWindow(WindowEnumBase.Device, "Window_DEVICE", new Rect(200, 50, 400, 450), false)},
                {WindowEnumBase.UI, new UIWindow(WindowEnumBase.UI, "Window_UI", new Rect(200, 50, 400, 450), false)}
            };

			if (param != null)
				param.ForEach(p => mWindowTable.Add(((TWindow)p).Id, (TWindow)p));
		}

		protected override void OnBeforeDestroy()
		{
			if (mWindowTable != null)
			{
                mWindowTable.ForEach(pair => pair.Value.Dispose());
                mWindowTable.Clear();
				mWindowTable = null;
			}

			base.OnBeforeDestroy();
		}

		public void OnGUI()
		{
			//計算縮放倍率
			float rate = (float)Screen.width / (float)(UIMgr.Instance.RealWidth == 0f ? 960f : UIMgr.Instance.RealWidth);

			//縮放倍率不可小於1
			if (rate < 1f)
				rate = 1f;

			//設置新的縮放比
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one * rate);

			//設定控制元件 依照 Table排列
			GUILayout.BeginVertical();
			if (mWindowTable != null)
			{
				if (GUILayout.Button("Debug Menu"))
					mIsShow = !mIsShow;

				if (mIsShow == true)
				{
                    //顯示FPS值
                    GUILayout.Box(TFps.Instance.Fps.ToString("F2"), GetGUIStyle(mStyle, 1, TFps.Instance.Fps));

                    //顯示Ping值
					GUILayout.Box(NetMgr.Instance.Ping.ToString() + " ms", GetGUIStyle(mStyle, 2, TinyNet.Instance.Ping));

					//繪製視窗選單按鈕
					foreach (KeyValuePair<int, TWindow> window in mWindowTable)
						window.Value.Enabled = GUILayout.Toggle(window.Value.Enabled, window.Value.Title);
				}

                //繪製視窗
                foreach (KeyValuePair<int, TWindow> window in mWindowTable)
                    window.Value.Draw();

            }
			GUILayout.EndVertical();
		}

        private TWindow GetWindow(int id)
        {
            TWindow window;
			if (!mWindowTable.TryGetValue(id, out window))
				return null;

			return window;
        }

        //取得GUIStyle
        private GUIStyle GetGUIStyle(GUIStyle style, int kind, long value)
        {
			return GetGUIStyle(style, kind, (int)value);
        }

        //取得GUIStyle
		private GUIStyle GetGUIStyle(GUIStyle style, int kind, float value)
        {
			return GetGUIStyle(style, kind, (int)value);
        }

        //取得GUIStyle
		private GUIStyle GetGUIStyle(GUIStyle style, int kind, int value)
        {
			switch (kind)
            {
                case 1:
					if (value >= 50)
						style.normal.textColor = Color.yellow;
					else if (value > 25 && value < 50)
						style.normal.textColor = Color.white;
					else if (value <= 25)
						style.normal.textColor = Color.red;
					return style;
                case 2:
					if (value <= 200)
						style.normal.textColor = Color.green;
					else if (value > 200 && value <= 500)
						style.normal.textColor = Color.white;
					else if (value > 500 && value < 1000)
						style.normal.textColor = Color.yellow;
					else if (value >= 1000)
						style.normal.textColor = Color.red;
					return style;
                default:
					return style;
            }
        }

        public void AddWindow(int key, TWindow window)
        {
            if (mWindowTable.ContainsKey(key))
                return;

            mWindowTable.Add(key, window);
        }

        public void Log(string msg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
				window.Log(msg);
        }

		public void LogWarning(string msg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
				window.LogWarning(msg);
        }

		public void LogError(string msg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
				window.LogError(msg);
        }

		public void LogException(string msg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
				window.LogException(msg);
        }
    }
}