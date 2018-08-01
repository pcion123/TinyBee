namespace TinyBee
{
	using UnityEngine;

	public static class BehaviourExtension : object
	{
		public static T Enable<T>(this T self) where T : Behaviour
		{
			self.enabled = true;
			return self;
		}

		public static T Disable<T>(this T self) where T : Behaviour
		{
			self.enabled = false;
			return self;
		}
	}
}