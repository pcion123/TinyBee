namespace TinyBee.Debug
{
    using UnityEngine;
	using System;

    public class UIWindow : TWindow
    {
        public UIWindow(int id, string title, Rect rect, bool enabled) : base(id, title, rect, enabled) { }

		public static Action<int> OnDrawWindow = null;
		public static Action<int,Rect> OnDrawButton = null;

        public override void Dispose() { }

        protected override void DrawWindow(int id)
        {
			if (OnDrawWindow == null)
			{
				GUILayout.Label("CoroutineCount: " + CoroutineMgr.Instance.Count.ToString());
				GUILayout.Label("AudioCount: " + AudioMgr.Instance.Count.ToString());
				GUILayout.Label("FormCount: " + UIMgr.Instance.FormCount.ToString());
				GUILayout.Label("FontCount: " + UIMgr.Instance.FontCount.ToString());
				GUILayout.Label("StandardAtlasCount: " + UIMgr.Instance.StandardAtlasCount.ToString());
				GUILayout.Label("AtlasCount: " + UIMgr.Instance.AtlasCount.ToString());
				GUILayout.Label("Ratio: " + UIMgr.Instance.Ratio.ToString());
				GUILayout.Label("RealWidth: " + UIMgr.Instance.RealWidth.ToString());
				GUILayout.Label("RealHeight: " + UIMgr.Instance.RealHeight.ToString());
				GUILayout.Label("Forms: " + UIMgr.Instance.GetFormString());
				GUI.DragWindow();
			}
			else
			{
				OnDrawWindow.InvokeGracefully(id);
			}

        }

		protected override void DrawButton(int id, Rect rect)
        {
			base.DrawButton(id, rect);

			if (OnDrawButton == null)
			{
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
			}
			else
			{
				OnDrawButton.InvokeGracefully(new object[] {id, rect});
			}
        }
    }
}