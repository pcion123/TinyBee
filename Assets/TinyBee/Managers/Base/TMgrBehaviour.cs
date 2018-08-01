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

        [UnityEngine.SerializeField]
        protected int mMgrId = MgrEnumBase.None;

		protected abstract void SetupMgrId();

		protected override void SetupMgr()
		{
			mCurMgr = this;
		}

		protected TMgrBehaviour() 
		{
            mLogger = TLogger.Instance;

			SetupMgrId();
		}

		public void RegisterEvents<T>(IEnumerable<T> eventIds, OnEvent process) where T : IConvertible
		{
			foreach (var eventId in eventIds)
			{
				RegisterEvent(eventId, process);
			}
		}

		public void RegisterEvent<T>(T msgId, OnEvent process) where T : IConvertible
		{
			mEventSystem.Register(msgId, process);
		}

		public void UnRegisterEvents(List<ushort> msgs, OnEvent process)
		{
			for (int i = 0; i < msgs.Count; i++)
			{
				UnRegistEvent(msgs[i], process);
			}
		}

		public void UnRegistEvent(int msgEvent, OnEvent process)
		{
			mEventSystem.UnRegister(msgEvent, process);
		}

		public override void SendMsg(TMsg msg)
		{
            if (msg.ManagerID == mMgrId)
			{
                Process(msg.EventID, msg);
			}
			else 
			{
				QMsgCenter.Instance.SendMsg(msg);
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
	}
}