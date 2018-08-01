namespace TinyBee
{
	using UnityEngine.Events;

	public static class UnityActionExtension : object
	{
		public static bool InvokeGracefully(this UnityAction self)
		{
			if (null != self)
			{
				self();
				return true;
			}
			return false;
		}

		public static bool InvokeGracefully<T>(this UnityAction<T> self, T t)
		{
			if (null != self)
			{
				self(t);
				return true;
			}
			return false;
		}

		public static bool InvokeGracefully<T, K>(this UnityAction<T, K> self, T t, K k)
		{
			if (null != self)
			{
				self(t, k);
				return true;
			}
			return false;
		}
	}
}