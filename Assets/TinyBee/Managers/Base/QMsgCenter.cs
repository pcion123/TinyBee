namespace TinyBee
{
	using UnityEngine;

	[TMonoSingletonPath("[Event]/QMsgCenter")]
	public partial class QMsgCenter : MonoBehaviour, ISingleton
	{
		public static QMsgCenter Instance
		{
			get { return MonoSingletonProperty<QMsgCenter>.Instance; }
		}

		public void OnSingletonInit()
		{

		}

		public void Dispose()
		{
			MonoSingletonProperty<QMsgCenter>.Dispose();
		}

		void Awake()
		{
			DontDestroyOnLoad(this);
		}

		public void SendMsg(TMsg tmpMsg)
		{
			// Framework Msg
			switch (tmpMsg.ManagerID)
			{
				case QMgrID.UI:
					//QUIManager.Instance.SendMsg(tmpMsg);
					return;
				case QMgrID.Audio:
					//AudioManager.Instance.SendMsg(tmpMsg);
					return;
			}

			// ForwardMsg(tmpMsg);
		}
	}
}