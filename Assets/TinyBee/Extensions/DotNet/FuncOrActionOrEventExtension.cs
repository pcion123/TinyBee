namespace TinyBee
{
	using System;

	public static class FuncOrActionOrEventExtension : object
	{
		public static T InvokeGracefully<T>(this Func<T> self)
		{
			return null != self ? self() : default(T);
		}

		public static bool InvokeGracefully(this Action self)
		{
			if (null != self)
			{
				self();
				return true;
			}
			return false;
		}

		public static bool InvokeGracefully<T>(this Action<T> self, T t)
		{
			if (null != self)
			{
				self(t);
				return true;
			}
			return false;
		}

		public static bool InvokeGracefully<T, K>(this Action<T, K> self, T t, K k)
		{
			if (null != self)
			{
				self(t, k);
				return true;
			}
			return false;
		}

		public static bool InvokeGracefully(this Delegate self, params object[] args)
		{
			if (null != self)
			{
				self.DynamicInvoke(args);
				return true;
			}
			return false;
		}
	}
}