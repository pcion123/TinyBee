namespace TinyBee
{
	using System;
	using Pool;

	public class TMsg : IPoolable, IPoolType
	{	
		public virtual int EventID { get; set; }
		public bool Processed { get; set; }
		public bool ReuseAble { get; set; }
		
		public int ManagerID
		{
			get { return EventID /*/ QMsgSpan.Count * QMsgSpan.Count*/; }
		}

		public TMsg(){}

		#region Object Pool
		public static TMsg Allocate<T>(T eventId) where T : IConvertible
		{
			TMsg msg = SafeObjectPool<TMsg>.Instance.Allocate();
			msg.EventID = eventId.ToInt32(null);
			msg.ReuseAble = true;
			return msg;
		}

		public virtual void Recycle2Cache()
		{
			SafeObjectPool<TMsg>.Instance.Recycle(this);
		}

		void IPoolable.OnRecycled()
		{
			Processed = false;
		}
		
		bool IPoolable.IsRecycled { get; set; }
		#endregion

		#region deprecated since v0.0.5
		// for proto buf;
		[Obsolete("deprecated since 0.0.5,use EventID instead")]
		public int msgId
		{
			get { return EventID; }
			set { EventID = value; }
		}
		
		[Obsolete("GetMgrID() is deprecated,please use ManagerID Property instead")]
		public int GetMgrID()
		{
			return ManagerID;
		}
		
		[Obsolete("deprecated,use allocate instead")]
		public TMsg(int eventID)
		{
			EventID = eventID;
		}
		#endregion
	}
}