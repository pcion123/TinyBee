namespace TinyBee
{
    using System.Collections;
    using System.Collections.Generic;

    [TMonoSingletonPath("[Network]/NetMgr")]
    public class NetMgr : TMgrBehaviour, ISingleton
    {
        private long mRunPingTick = 0;      //實際測試間隔Tick
        private long mSendPingTick = 0;     //送出測試Tick
        private long mRcvPingTick = 0;      //回應測試Tick

        public long RunPingTick { get { return mRunPingTick; } }
        public long SendPingTick { get { return mSendPingTick; } }
        public long RcvPingTick { get { return mRcvPingTick; } }

        public static NetMgr Instance
        {
            get { return MonoSingletonProperty<NetMgr>.Instance; }
        }

		public override int ManagerId
		{
			get { return MgrEnumBase.Net; }
		}

        public void OnSingletonInit() { }
    }
}
