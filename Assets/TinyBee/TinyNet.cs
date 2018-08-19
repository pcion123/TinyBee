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

		public int Ping
		{
			get
			{
				return mInstance != null ? mInstance.Ping : 0;
			}
		}
	}
}