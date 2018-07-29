namespace TinyBee
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System;

	public class TObject : IDisposable
	{
		private string mErr = null;
		private WWW mBundle = null;
		private GameObject mObj = null;

		public string Err { get; set; }
		public WWW Bundle { get; set; }
		public GameObject Obj { get; set; }

		//建構子
		public TObject ()
		{
			mErr = null;
			mBundle = null;
			mObj = null;
		}

		//清除內容
		public void Clear ()
		{
			mErr = null;
			mBundle = null;
			mObj = null;
		}

		//釋放
		public void Dispose ()
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