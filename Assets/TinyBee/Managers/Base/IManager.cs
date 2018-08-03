namespace TinyBee
{
	using System;

	public interface IManager 
	{
		void Init(params object[] param);

		void RegisterEvent<T>(T msgId, OnEvent process) where T : IConvertible;

		void UnRegistEvent<T>(T msgEvent, OnEvent process) where T : IConvertible;

		void SendEvent<T>(T eventId) where T : IConvertible;

		void SendMsg(TMsg msg);
	}
}