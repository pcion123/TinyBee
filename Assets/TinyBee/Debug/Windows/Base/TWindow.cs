namespace TinyBee.Debug
{
    using System;
    using UnityEngine;

    public abstract class TWindow : IDisposable
    {
        protected int mId = 0;
        private string mTitle;
        protected bool mEnabled = false;
        protected Rect mRect;

        public int Id { get { return mId; } }
        public string Title { get { return mTitle; } }
        public bool Enabled { get { return mEnabled; } set { mEnabled = value; } }

        public TWindow(int id, string title, Rect rect, bool enabled)
        {
            mId = id;
            mTitle = title;
            mRect = rect;
            mEnabled = enabled;
        }

        public virtual void Dispose() {}

        protected abstract void DrawWindow(int id);
		protected virtual void DrawButton(int id, Rect rect)
        {
            //繪製視窗關閉按鈕
            if (GUI.Button(new Rect(mRect.x - 72, mRect.y, 72, 36), "x"))
            {
                mEnabled = false;
            }
        }

        public void Draw()
        {
            if (mEnabled)
            {
                mRect = GUILayout.Window(mId, mRect, DrawWindow, mTitle);

				DrawButton(mId, mRect);
            }
        }
    }
}