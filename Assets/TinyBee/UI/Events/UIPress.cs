﻿namespace TinyBee.UI
{
    using System;

    public class UIPress: UIEvent
    {
        private bool mIsPress = false;
        private bool mIsShow = false;
        private long mTimer = 0L;

        public long Interval { get; set; }
        public Action<object> onClick { get; set; }
        public Action<object> onShow { get; set; }
        public Action<object> onHide { get; set; }
        public Action<object> onCheck { get; set; }

        void Update()
        {
            if (CheckShow() == true)
            {
                mIsShow = true;

                onShow.InvokeGracefully(mParameter);
            }
        }

        void OnPress(bool isPress)
        {
            mIsPress = isPress;

            if (isPress == false)
            {
                if (mIsShow == true)
                {
                    mIsShow = false;

                    onHide.InvokeGracefully(mParameter);
                }
                else
                {
                    onClick.InvokeGracefully(mParameter);
                }
            }
            else
            {
                mTimer = DateTime.Now.Ticks;
            }
        }

        //檢查是否顯示資訊
        private bool CheckShow()
        {
            if (mIsPress == false)
                return false;

            long tick1 = mTimer + Interval;
            long tick2 = DateTime.Now.Ticks;

            //檢查時間間隔
			if (tick1 > tick2)
                return false;

            return onCheck.InvokeGracefully();
        }
    }
}
