namespace TinyBee
{
	public static class TTime : object
	{
		public const long ONEMSECONDTICK = 1 * 10000;
		public const long ONESECONDTICK = 1000 * ONEMSECONDTICK;
		public const long ONEMINUTETICK = 60 * ONESECONDTICK;
		public const long ONEHOURTICK = 60 * ONEMINUTETICK;
		public const long ONEDAYTICK = 24 * ONEHOURTICK;

		public static long Current { get { return System.DateTime.Now.Ticks; } }
	}
}
