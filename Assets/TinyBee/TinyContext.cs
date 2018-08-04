namespace TinyBee.Context
{
	public class TinyContext : IContext
	{
		private static IContext mInstance = null;

		public static IContext Instance
		{
			get
			{
				if (mInstance == null)
					mInstance = new TinyContext();
				
				return mInstance;
			}
			set
			{
				mInstance = value;
			}
		}

		public string LanguagePath
		{
			get
			{
				return mInstance != null ? mInstance.LanguagePath : null;
			}
		}

		public string DataPath
		{
			get
			{
				return mInstance != null ? mInstance.DataPath : null;
			}
		}

		public string ZipPath
		{
			get
			{
				return mInstance != null ? mInstance.ZipPath : null;
			}
		}

		public string Version
		{
			get
			{
				return mInstance != null ? mInstance.Version : null;
			}
		}
	}
}