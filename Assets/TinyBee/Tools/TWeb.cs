namespace TinyBee
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System;
	using TLogger = TinyBee.Logger.TLogger;

	public static class TWeb : object
	{
		public static IEnumerator Post(string url, string param, Action<string> callback)
		{
			System.Uri uri = new System.Uri(url);
			using (WWW bundle = new WWW(uri.AbsoluteUri, param.ToByteArray()))
			{
				while (!bundle.isDone)
				{
					if (bundle.error != null)
					{
						break;
					}
					else
					{
						yield return bundle;
					}
				}

				if (bundle.error != null)
				{
					TLogger.Instance.Log("Post error : " + bundle.error);
				}
				else
				{
					callback.InvokeGracefully(bundle.text);
				}
			}
		}

		public static IEnumerator Post(string url, WWWForm form, Action<string> callback)
		{
			System.Uri uri = new System.Uri(url);
			using (WWW bundle = new WWW(uri.AbsoluteUri, form))
			{
				while (!bundle.isDone)
				{
					if (bundle.error != null)
					{
						break;
					}
					else
					{
						yield return bundle;
					}
				}

				if (bundle.error != null)
				{
					TLogger.Instance.Log("Post error : " + bundle.error);
				}
				else
				{
					callback.InvokeGracefully(bundle.text);
				}
			}
		}

		public static IEnumerator Get(string url, Action<string> callback)
		{
			System.Uri uri = new System.Uri(url);
			using (WWW bundle = new WWW(uri.AbsoluteUri))
			{
				while (!bundle.isDone)
				{
					if (bundle.error != null)
					{
						break;
					}
					else
					{
						yield return bundle;
					}
				}

				if (bundle.error != null)
				{
					TLogger.Instance.Log("Post error : " + bundle.error);
				}
				else
				{
					callback.InvokeGracefully(bundle.text);
				}
			}
		}
	}
}