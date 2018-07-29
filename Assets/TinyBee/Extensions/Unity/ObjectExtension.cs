namespace TinyBee
{
	using UnityEngine;

	public static class ObjectExtension : object
	{
		public static T Instantiate<T>(this T self) where T : Object
		{
			return Object.Instantiate(self);
		}

		public static T Name<T>(this T self, string name) where T : Object
		{
			self.name = name;
			return self;
		}

		public static void DestroySelf<T>(this T self) where T : Object
		{
			Object.Destroy(self);
		}

		public static T DestroySelfGracefully<T>(this T self) where T : Object
		{
			if (self)
				Object.Destroy(self);
			
			return self;
		}

		public static T DestroySelfAfterDelay<T>(this T self, float delay) where T : Object
		{
			Object.Destroy(self, delay);
			return self;
		}

		public static T DestroySelfAfterDelayGracefully<T>(this T self, float delay) where T : Object
		{
			if (self)
				Object.Destroy(self, delay);
			
			return self;
		}

		public static T ApplySelfTo<T>(this T self, System.Action<T> toFunction) where T : Object
		{
			toFunction.InvokeGracefully(self);
			return self;
		}

		public static T DontDestroyOnLoad<T>(this T self) where T : Object
		{
			Object.DontDestroyOnLoad(self);
			return self;
		}

		public static T As<T>(this Object self) where T : Object
		{
			return self as T;
		}
	}
}