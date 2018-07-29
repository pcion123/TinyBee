namespace TinyBee.Debug
{
    using UnityEngine;

    public class UIWindow : TWindow
    {
        public UIWindow(int id, string title, Rect rect, bool enabled) : base(id, title, rect, enabled) { }

        public override void Dispose() { }

        protected override void DrawWindow(int id)
        {
            GUILayout.Label("CoroutineCount: " + CoroutineMgr.Instance.Count.ToString());
            GUILayout.Label("AudioCount: " + AudioMgr.Instance.Count.ToString());
            GUILayout.Label("FormCount: " + UIMgr.Instance.FormCount.ToString());
            GUILayout.Label("FontCount: " + UIMgr.Instance.FontCount.ToString());
			GUILayout.Label("StandardAtlasCount: " + UIMgr.Instance.StandardAtlasCount.ToString());
            GUILayout.Label("AtlasCount: " + UIMgr.Instance.AtlasCount.ToString());
//#if _SEPARATE
//            GUILayout.Label("Dual: " + GameInfos.UIMgr.IsActive.ToString());
//            if (GameInfos.UIMgr.Display1 == null)
//            {
//                GUILayout.Label("Display1: null");
//            }
//            else
//            {
//                GUILayout.Label("Display1: " + GameInfos.UIMgr.Display1.ToString());
//            }
//            if (GameInfos.UIMgr.Display2 == null)
//            {
//                GUILayout.Label("Display2: null");
//            }
//            else
//            {
//                GUILayout.Label("Display2: " + GameInfos.UIMgr.Display2.ToString());
//            }
//            GUILayout.Label("CamList1: " + GameInfos.UIMgr.GetCamList1());
//            GUILayout.Label("CamList2: " + GameInfos.UIMgr.GetCamList2());
//#endif
            GUILayout.Label("Ratio: " + UIMgr.Instance.Ratio.ToString());
            GUILayout.Label("RealWidth: " + UIMgr.Instance.RealWidth.ToString());
            GUILayout.Label("RealHeight: " + UIMgr.Instance.RealHeight.ToString());
            GUILayout.Label("Forms: " + UIMgr.Instance.GetFormString());
            GUI.DragWindow();
        }

        protected override void DrawButton()
        {
            base.DrawButton();

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 1, 72, 36), "Unload"))
            {
                Resources.UnloadUnusedAssets();
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 2, 72, 36), "GC"))
            {
                System.GC.Collect();
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 3, 72, 36), "ResetALL"))
            {
                UIMgr.Instance.ResetALL(2);
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 4, 72, 36), "Bubble"))
            {
                UI.UIBubble.Msg("成就達成!", 0.8f, 0.5f);
            }

			if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 5, 72, 36), "Msg"))
			{
				UIMgr.Instance.OpenUI(UI.UIEnumBase.Msg, new object[] {"", "金幣不足", false, null, null});
			}

			if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 6, 72, 36), "Login"))
			{
				UIMgr.Instance.OpenUI(51, null);
			}

			if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 7, 72, 36), "Lobby"))
			{
				UIMgr.Instance.OpenUI(52, null);
			}
        }
    }
}