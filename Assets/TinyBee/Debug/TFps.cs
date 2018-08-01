namespace TinyBee.Debug
{
    using System;
    using UnityEngine;

    [TMonoSingletonPath("[Debug]/Fps")]
    public class TFps : TMonoBehaviour, ISingleton
    {
        private float mUpdateInterval = 0.1f;
        private double mLastInterval = 0;
        private int mFrameCount = 0;
        private float mFps = 0;
        private int mFixedDelta = 0;
        //private int mFixedFrame = 0;
        private double mFixedLastInterval = 0;

        public float Fps { get { return mFps; } }

        public static TFps Instance
        {
            get { return MonoSingletonProperty<TFps>.Instance; }
        }

        public void OnSingletonInit()
        {
            //記錄遊戲開始時間
            mLastInterval = Time.realtimeSinceStartup;
            //初始化
            mFrameCount = 0;
        }

        protected override void SetupMgr()
        {

        }

        private void Update()
        {
            //增加Frame數
            mFrameCount++;

            //紀錄遊戲當前時間
            float vTimeNow = Time.realtimeSinceStartup;

            if (vTimeNow > mLastInterval + mUpdateInterval)
            {
                //計算FPS值
                mFps = (float)(mFrameCount / (vTimeNow - mLastInterval));
                //歸零
                mFrameCount = 0;
                //更新間隔時間
                mLastInterval = vTimeNow;
            }
        }

        private void FixedUpdate()
        {
            mFixedDelta++;

            float vTimeNow = Time.realtimeSinceStartup;

            if (vTimeNow > mFixedLastInterval + 1f)
            {
                //mFixedFrame = mFixedDelta;
                mFixedDelta = 0;
                mFixedLastInterval = vTimeNow;
            }
        }
    }
}