namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
    using System;
	using UnityEngine;
    using ILogger = TinyBee.Logger.ILogger;
    using TLogger = TinyBee.Logger.TLogger;

    [System.Serializable]
    public class TCoroutine : object
	{
        private ILogger mLogger = TLogger.Instance;
        private CoroutineMgr mMgr = null;
		private bool mIsRunning = false;
		private bool mIsPaused = false;
		private bool mIsStopped = false;
		private string mName = null;
		private object mCustom = null;
		private int mTimer = 0;
		private Coroutine mCoroutine = null;
		private IEnumerator mEnumerator = null;
		private System.Action<bool> mOnFinished = null;
		private System.Action mOnTimeup = null;

		public bool IsRunning { get { return mIsRunning; } }
		public bool IsPaused { get { return mIsPaused; } }
		public bool IsStopped { get { return mIsStopped; } }
		public string Name { get { return mName; } }
		public object Custom { get { return mCustom; } }
		public Coroutine Coroutine { get { return mCoroutine; } }
		public IEnumerator Enumerator { get { return mEnumerator; } }

		public TCoroutine(CoroutineMgr mgr, object custom, IEnumerator enumerator, Action<bool> onFinished, Action onTimeup, int timer)
		{
			mMgr = mgr;
			mName = enumerator.ToString();
			mCustom = custom;
			mTimer = timer;
			mCoroutine = null;
			mEnumerator = enumerator;
            mOnFinished = onFinished;
            mOnTimeup = onTimeup;
		}

		public void Pause()
		{
			mIsPaused = true;
		}

		public void Resume()
		{
			mIsPaused = false;
		}

		public Coroutine Start()
		{
			mIsRunning = true;

			if (mMgr != null)
				mCoroutine = mMgr.Mono.StartCoroutine(Runner());

			return mCoroutine;
		}

		public void Shutdown()
		{
			mIsStopped = true;
			mIsRunning = false;
		}

		private IEnumerator Runner()
		{
			bool isTimeup = false;
			DateTime timer = DateTime.Now;

			yield return null;

			IEnumerator enumerator = mEnumerator;

			while (mIsRunning)
			{
				if (mTimer > 0)
				{
					System.TimeSpan tsc = DateTime.Now - timer;
					if (tsc.Seconds > mTimer)
					{
						isTimeup = true;
						break;
					}
				}

				if (mIsPaused)
				{
					yield return null;
				}
				else
				{
					bool isFinish = false;
					try
					{
						isFinish = enumerator.MoveNext();
					}
					catch (Exception e)
					{
						isFinish = false;
						mIsRunning = false;

                        mLogger.LogError(string.Format("{0}: {1}", mName, e.Message));
                        mLogger.LogException(e);
                    }

					if ((enumerator != null) && isFinish)
					{
						yield return enumerator.Current;
					}
					else
					{
						mIsRunning = false;
					}
				}
			}

			if (!isTimeup)
			{
				mOnFinished.InvokeGracefully(mIsStopped);
			}
			else
			{
                mOnTimeup.InvokeGracefully();
			}

			if (mMgr != null)
			{
				int index = mMgr.Container.IndexOf(this);
				if (index >= 0)
					mMgr.Container.RemoveAt(index);
			}
		}
	}

	[TMonoSingletonPath("[Coroutine]/CoroutineMgr")]
	public class CoroutineMgr : TMgrBehaviour, ISingleton
	{
		[SerializeField]
		private List<TCoroutine> mContainer = new List<TCoroutine>();

        public MonoBehaviour Mono { get { return this; } }
		public List<TCoroutine> Container { get { return mContainer; } }
		public int Count { get { return mContainer.Count; } }

		public static CoroutineMgr Instance
		{
			get { return MonoSingletonProperty<CoroutineMgr>.Instance; }
		}

		public override int ManagerId
		{
			get { return MgrEnumBase.Coroutine; }
		}

		public void OnSingletonInit() {}

		protected override void OnBeforeDestroy()
		{
			StopAllCoroutines();

            mContainer.Free();

            base.OnBeforeDestroy();
        }

		public Coroutine StartCoroutine(out TCoroutine coroutine, object custom, IEnumerator enumerator, Action<bool> onFinished = null, Action onTimeup = null, int timer = 0)
		{
            coroutine = new TCoroutine(this, custom, enumerator, onFinished, onTimeup, timer);
			if (mContainer != null)
				mContainer.Add(coroutine);
			return coroutine.Start();
		}

		public Coroutine StartCoroutine(out TCoroutine coroutine, IEnumerator enumerator, Action<bool> onFinished = null, Action onTimeup = null, int timer = 0)
		{
            coroutine = new TCoroutine(this, null, enumerator, onFinished, onTimeup, timer);
			if (mContainer != null)
				mContainer.Add(coroutine);
			return coroutine.Start();
		}

		public Coroutine StartCoroutine(object custom, IEnumerator enumerator, Action<bool> onFinished = null, Action onTimeup = null, int timer = 0)
		{
			TCoroutine coroutine = new TCoroutine(this, custom, enumerator, onFinished, onTimeup, timer);
			if (mContainer != null)
				mContainer.Add(coroutine);
			return coroutine.Start();
		}

		public Coroutine StartCoroutine(IEnumerator enumerator, Action<bool> onFinished = null, Action onTimeup = null, int timer = 0)
		{
			TCoroutine coroutine = new TCoroutine(this, null, enumerator, onFinished, onTimeup, timer);
			if (mContainer != null)
				mContainer.Add(coroutine);
			return coroutine.Start();
		}

		public Coroutine StartCoroutine(TCoroutine coroutine)
		{
			if (mContainer != null)
				mContainer.Add(coroutine);
			return coroutine.Start();
		}

		public void StopCoroutine(TCoroutine coroutine)
		{
			if (mContainer == null)
				return;
			int index = mContainer.IndexOf(coroutine);
			if (index >= 0)
				coroutine.Shutdown();
		}

		public void StopCoroutineEx(object custom)
		{
			for (int i = mContainer.Count - 1; i >= 0; i--)
			{
				if ((mContainer[i].Custom == null) || (mContainer[i].Custom != custom))
					continue;

				mContainer[i].Shutdown();
			}
		}

		public void StopAllCoroutines()
		{
			if (mContainer == null)
				return;

			for (int i = mContainer.Count - 1; i >= 0; i--)
			{
				mContainer[i].Shutdown();
			}
		}
	}
}