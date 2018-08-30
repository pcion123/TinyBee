namespace TinyBee.Context
{
	public class TinyNet : INet
	{
		private static INet mInstance = null;

		public static INet Instance
		{
			get
			{
				if (mInstance == null)
					mInstance = new TinyNet();

				return mInstance;
			}
			set
			{
				mInstance = value;
			}
		}

		public int Version { get {  return mInstance != null ? mInstance.Version : 0; } }
		public string Hostname { get {  return mInstance != null ? mInstance.Hostname : null; } }
		public int Port { get {  return mInstance != null ? mInstance.Port : 0; } }
		public bool Connected { get {  return mInstance != null ? mInstance.Connected : false; } }
		public int Ping { get {  return mInstance != null ? mInstance.Ping : 0; } }
	}
}