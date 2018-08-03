namespace TinyBee 
{
	using System;
	using System.Collections.Generic;
	using Pool;
    using TLogger = Logger.TLogger;

	public abstract class TMgrBehaviour : TMonoBehaviour, IManager
	{
		private readonly EventSystem mEventSystem = NonPublicObjectPool<EventSystem>.Instance.Allocate();

        public virtual void Init(params object[] param) {}

		public abstract int ManagerId { get ; }

		public override IManager Manager
		{
			get { return this; }
		}

		protected TMgrBehaviour() 
		{
            mLogger = TLogger.Instance;
		}

		public void RegisterEvent<T>(T msgId, OnEvent process) where T : IConvertible
		{
			mEventSystem.Register(msgId, process);
		}

		public void UnRegistEvent<T>(T msgEvent, OnEvent process) where T : IConvertible
		{
			mEventSystem.UnRegister(msgEvent, process);
		}

		public override void SendMsg(TMsg msg)
		{
			if (msg.ManagerID == ManagerId)
			{
				Process(msg.EventID, msg);
			}
			else 
			{
				//QMsgCenter.Instance.SendMsg(msg);
			}
		}

		public override void SendEvent<T>(T eventId)
		{
			SendMsg(TMsg.Allocate(eventId));
		}

		// 来了消息以后,通知整个消息链
		protected override void ProcessMsg(int eventId, TMsg msg)
		{
			mEventSystem.Send(msg.EventID, msg);
		}

		protected override void OnBeforeDestroy()
		{
			if (mEventSystem.IsNotNull())
				mEventSystem.OnRecycled();
		}
	}
}