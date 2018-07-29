namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using Debug;

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

		public void OnSingletonInit() {}

        public override void Init(params object[] param)
		{
            mStyle = new GUIStyle();
            mStyle.alignment = TextAnchor.MiddleCenter;

            mWindowTable = new Dictionary<int, TWindow>
			{
				//觸發事件接在這
				{WindowEnumBase.Msg, new MsgWindow(WindowEnumBase.Msg, "Window_MSG", new Rect(200, 50, 400, 450), false)},
                {WindowEnumBase.Device, new DeviceWindow(WindowEnumBase.Device, "Window_DEVICE", new Rect(200, 50, 400, 450), false)},
                {WindowEnumBase.UI, new UIWindow(WindowEnumBase.UI, "Window_UI", new Rect(200, 50, 400, 450), false)}
            };
		}

		protected override void SetupMgrId()
		{
			mMgrId = MgrEnumBase.Debug;
		}

		protected override void OnBeforeDestroy()
		{
			if (mWindowTable != null)
			{
                mWindowTable.ForEach(pair => pair.Value.Dispose());
                mWindowTable.Clear();
				mWindowTable = null;
			}
		}

		public void OnGUI()
		{
			//計算縮放倍率
			float vRate = (float)Screen.width / (float)(UIMgr.Instance.RealWidth == 0f ? 960f : UIMgr.Instance.RealWidth);

			//縮放倍率不可小於1
			if (vRate < 1f)
				vRate = 1f;

			//設置新的縮放比
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one * vRate);

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
                    GUILayout.Box(NetMgr.Instance.RunPingTick.ToString() + " ms", GetGUIStyle(mStyle, 2, NetMgr.Instance.RunPingTick));

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
            if (mWindowTable.TryGetValue(id, out window))
            {
                return window;
            }
            else
            {
                return null;
            }
        }

        //取得GUIStyle
        private GUIStyle GetGUIStyle(GUIStyle vStyle, int vKind, long vValue)
        {
            return GetGUIStyle(vStyle, vKind, (int)vValue);
        }

        //取得GUIStyle
        private GUIStyle GetGUIStyle(GUIStyle vStyle, int vKind, float vValue)
        {
            return GetGUIStyle(vStyle, vKind, (int)vValue);
        }

        //取得GUIStyle
        private GUIStyle GetGUIStyle(GUIStyle vStyle, int vKind, int vValue)
        {
            switch (vKind)
            {
                case 1:
                    if (vValue >= 50)
                        vStyle.normal.textColor = Color.yellow;
                    else if (vValue > 25 && vValue < 50)
                        vStyle.normal.textColor = Color.white;
                    else if (vValue <= 25)
                        vStyle.normal.textColor = Color.red;
                    return vStyle;
                case 2:
                    if (vValue <= 200)
                        vStyle.normal.textColor = Color.green;
                    else if (vValue > 200 && vValue <= 500)
                        vStyle.normal.textColor = Color.white;
                    else if (vValue > 500 && vValue < 1000)
                        vStyle.normal.textColor = Color.yellow;
                    else if (vValue >= 1000)
                        vStyle.normal.textColor = Color.red;
                    return vStyle;
                default:
                    return vStyle;
            }
        }

        public void AddWindow(int key, TWindow window)
        {
            if (mWindowTable.ContainsKey(key))
                return;

            mWindowTable.Add(key, window);
        }

        public void Log(string vMsg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
                window.Log(vMsg);
        }

        public void LogWarning(string vMsg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
                window.LogWarning(vMsg);
        }

        public void LogError(string vMsg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
                window.LogError(vMsg);
        }

        public void LogException(string vMsg)
        {
            MsgWindow window = GetWindow(WindowEnumBase.Msg) as MsgWindow;
            if (window != null)
                window.LogException(vMsg);
        }
    }
}