namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
	using System;
	using UnityEngine;
	using ILogger = TinyBee.Logger.ILogger;
	using TLogger = TinyBee.Logger.TLogger;

	public class TSchedule
	{
		private ILogger mLogger = TLogger.Instance;
		private Action mTask;
		private int mInterval;
		private int mDelay;
		private int mTimes;
		private int mCount;
		private long mTimer;
		private bool mIsRunning;
		private bool mIsPause;
		private TCoroutine mRunner;

		public int Count { get { return mCount; } }
		public bool IsRunning { get { return mIsRunning; } }
		public bool IsPause { get { return mIsPause; } }

		public void Start(Action task, int interval)
		{
			Start(task, interval, -1);
		}

		public void Start(Action task, int interval, int times)
		{
			Start(task, interval, times, true);
		}

		public void Start(Action task, int interval, bool coroutinable)
		{
			Start(task, interval, -1, coroutinable);
		}

		public void Start(Action task, int interval, int times, bool coroutinable)
		{
			Start(task, interval, 0, times, coroutinable);
		}

		public void Start(Action task, int interval, int delay, int times, bool coroutinable)
		{
			mIsRunning = true;
			mIsPause = false;
			mTask = task;
			mInterval = interval;
			mDelay = delay;
			mTimes = times;
			mCount = 0;
			mTimer = TTime.Current + ((delay + interval) * TTime.ONESECONDTICK);

			if (coroutinable)
			{
				if (mRunner != null)
					mRunner.Shutdown();

				CoroutineMgr.Instance.StartCoroutine(out mRunner, Running());
			}
		}

		public void Shutdown()
		{
			mIsRunning = false;
			mIsPause = false;

			if (mRunner != null)
				mRunner.Shutdown();
		}

		public void Pause()
		{
			mIsPause = true;
		}

		public void Resume()
		{
			mIsPause = false;
		}

		public void Update()
		{
			if (!mIsRunning || mIsPause)
				return;

			long timer = TTime.Current;
			if (mTimer < timer)
			{
				mTask.InvokeGracefully();
				mCount++;
				mTimer += mInterval * TTime.ONESECONDTICK;
			}
			if (mTimes > 0 && mTimes == mCount)
				mIsRunning = false;
		}

		private IEnumerator Running()
		{
			yield return new WaitForSeconds(mDelay);
			while (mIsRunning)
			{
				if (!mIsPause)
				{
					mTask.InvokeGracefully();
					mCount++;
				}
				else
				{
					//TODO:什麼也不做
				}
				if (mTimes > 0 && mTimes == mCount)
					break;

				yield return new WaitForSeconds(mInterval);
			}
			mIsRunning = false;
		}
	}
}