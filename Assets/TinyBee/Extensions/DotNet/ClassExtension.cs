namespace TinyBee
{
	public static class ClassExtension : object
	{
		public static bool IsNull<T>(this T self) where T : class
		{
			return null == self;
		}

		public static bool IsNotNull<T>(this T self) where T : class
		{
			return null != self;
		}
	}
}