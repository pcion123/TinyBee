namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
	using System;
	using System.Security.Cryptography;

	public static class StringExtension : object
	{
		public static bool IsNullOrEmpty(this string self)
		{
			return string.IsNullOrEmpty(self);
		}

		public static bool IsTrimNullAndEmpty(this string self)
		{
			return string.IsNullOrEmpty(self.Trim());
		}

        public static int GetStringLength(this string self)
        {
            if (self == null)
                return 0;

            int len = 0;
            char[] chars = self.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                int len0 = System.Text.Encoding.UTF8.GetBytes(chars[i].ToString()).Length;

                if (len0 > 2)
                    len0 = 2;

                len = len + len0;
            }
            return len;
        }

        public static int ToInt(this string self, int defaultValue = 0)
		{
			int tmp = defaultValue;
			return int.TryParse(self, out tmp) ? tmp : defaultValue;
		}

		public static uint ToUint(this string self, uint defaultValue = 0)
		{
			uint tmp = defaultValue;
			return uint.TryParse(self, out tmp) ? tmp : defaultValue;
		}

		public static long ToLong(this string self, long defaultValue = 0)
		{
			long tmp = defaultValue;
			return long.TryParse(self, out tmp) ? tmp : defaultValue;
		}

		public static float ToFloat(this string self, float defaultValue = 0f)
		{
			float tmp = defaultValue;
			return float.TryParse(self, out tmp) ? tmp : defaultValue;
		}

		public static double ToDouble(this string self, double defaultValue = 0f)
		{
			double tmp = defaultValue;
			return double.TryParse(self, out tmp) ? tmp : defaultValue;
		}

		public static DateTime ToDateTime(this string self, DateTime defaultValue = default(DateTime))
		{
			DateTime tmp = defaultValue;
			return DateTime.TryParse(self, out tmp) ? tmp : defaultValue;
		}

		//string to Byte[]
		public static byte[] ToByteArray (this string self)
		{
			try
			{
				if (string.IsNullOrEmpty(self))
					return null;
				return System.Text.UTF8Encoding.UTF8.GetBytes(self);
			}
			catch
			{
				return null;
			}
		}

		//string to MD5
		public static string ToMD5 (this string self)
		{
			try {
				if (string.IsNullOrEmpty(self))
					return string.Empty;

				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
				byte[] t = md5.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(self));
				System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
				for (int i = 0; i < t.Length; i++)
					sb.Append(t[i].ToString("x").PadLeft(2, '0'));
				
				return sb.ToString();
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}