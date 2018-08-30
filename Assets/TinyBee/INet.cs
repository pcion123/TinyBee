namespace TinyBee.Context
{
	public interface INet
	{
		int Version { get; }
		string Hostname { get; }
		int Port { get; }
		bool Connected { get; }
		int Ping { get; }
	}
}