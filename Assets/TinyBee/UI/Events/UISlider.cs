namespace TinyBee.UI
{
    using System;
    using UnityEngine;

    public class UISlider : UIEvent
    {
        private Vector2 mDelta = Vector2.zero;

        public eDirection Direction { get; set; }
        public int Amount { get; set; }
        public Action<object> onSlider { get; set; }

        void OnPress(bool isPress)
        {
            if (isPress)
                return;

            if ((Direction & eDirection.Vertical) == eDirection.Vertical)
            {
                if (mDelta.y > Amount)
                {
                    onSlider.InvokeGracefully(true);
                    mDelta = Vector2.zero;
                }
                else if (mDelta.y < -Amount)
                {
                    onSlider.InvokeGracefully(false);
                    mDelta = Vector2.zero;
                }
            }
            else if ((Direction & eDirection.Horizontal) == eDirection.Horizontal)
            {
                if (mDelta.x > Amount)
                {
                    onSlider.InvokeGracefully(true);
                    mDelta = Vector2.zero;
                }
                else if (mDelta.x < -Amount)
                {
                    onSlider.InvokeGracefully(false);
                    mDelta = Vector2.zero;
                }
            }
        }

        void OnDrag(Vector2 delta)
        {
            float x1 = Mathf.Abs(delta.x);
            float x2 = Mathf.Abs(mDelta.x);
            float y1 = Mathf.Abs(delta.y);
            float y2 = Mathf.Abs(mDelta.y);

            if ((Direction & eDirection.Vertical) == eDirection.Vertical)
            {
                if (y1 > y2)
                    mDelta = delta;
            }
            else if ((Direction & eDirection.Horizontal) == eDirection.Horizontal)
            {
                if (x1 > x2)
                    mDelta = delta;
            }
        }
    }
}
