namespace TinyBee
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System;

	public class TObject : IDisposable
	{
		public string Err { get; set; }
		public WWW Bundle { get; set; }

		//建構子
		public TObject()
		{
			Err = null;
			Bundle = null;
		}

		//清除內容
		public void Clear()
		{
			Err = null;
			Bundle = null;
		}

		//釋放
		public void Dispose()
		{
			Clear();
		}
    }

    public static class TObjectExtension : object
    {
        public static void Free(this TObject self)
        {
            if (self != null)
            {
                self.Dispose();
                self = null;
            }
        }
    }
}