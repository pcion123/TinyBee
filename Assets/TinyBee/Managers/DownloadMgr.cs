namespace TinyBee
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Downloader;

    public struct rRes
    {
        public string Version;  //版本號
        public string FileName; //檔名
        public string Path;     //路徑
        public long FileSize;   //檔案大小
        public string MD5Code;  //MD5編碼
    }

    public struct rMD5Info
    {
        public string FlieName; //檔名
        public string MD5Code;  //MD5編碼
    }

    [TMonoSingletonPath("[Downloader]/DownloadMgr")]
    public class DownloadMgr : TMgrBehaviour, ISingleton
    {
#if UNITY_EDITOR		
		[System.Serializable]
		private class DownloaderDictionary : SerializableDictionary<string,TDownloader> { }
		[SerializeField]
		private DownloaderDictionary mTaskMap = null;
#else
		[SerializeField]
		private Dictionary<string,TDownloader> mTaskMap = null;
#endif

        public int Count { get { return mTaskMap.Count; } }
		public string HostName { get; set; }
		public string IP { get; set; }
		public string AppVersion { get; set; }

        public static DownloadMgr Instance
        {
            get { return MonoSingletonProperty<DownloadMgr>.Instance; }
        }

		public override int ManagerId
		{
			get { return MgrEnumBase.Downloader; }
		}

        public void OnSingletonInit() { }

        public override void Init(params object[] param)
        {
#if UNITY_EDITOR		
			mTaskMap = new DownloaderDictionary();
#else
			mTaskMap = new Dictionary<string,TDownloader>();
#endif
        }

		public TDownloader Get(string key)
		{
			if (!mTaskMap.ContainsKey(key))
				return null;

			TDownloader downloader;
			if (!mTaskMap.TryGetValue(key, out downloader))
				return null;

			return downloader;
		}

		public bool Push(string key, TDownloader downloader)
        {
            if (mTaskMap.ContainsKey(key))
                return false;

            mTaskMap.Add(key, downloader);
            return true;
        }

		public TDownloader Pop(string key)
        {
            if (!mTaskMap.ContainsKey(key))
                return null;

            TDownloader downloader;
            if (!mTaskMap.TryGetValue(key, out downloader))
                return null;

			mTaskMap.Remove(key);

            return downloader;
        }

		public TDownloader Download(string versionName, params object[] events)
        {
			TDownloader downloader = new TDownloaderUnity(IP, HostName, AppVersion);
			if (!Push(versionName, downloader))
			{
				return null;
			}
			downloader.Start(this, versionName, events);
			return downloader;
        }

		public TDownloader Download(string versionName, List<rRes> list, params object[] events)
		{
			TDownloader downloader = new TDownloaderUnity(IP, HostName, AppVersion);
			if (!Push(versionName, downloader))
			{
				return null;
			}
			downloader.Start(this, versionName, list, events);
			return downloader;
		}

		public IEnumerator IDownload(string versionName, params object[] events)
		{
			TDownloader downloader = new TDownloaderUnity(IP, HostName, AppVersion);
			if (!Push(versionName, downloader))
			{
				yield break;
			}
			yield return CoroutineMgr.Instance.StartCoroutine(downloader.IStart(this, versionName, events));
		}

		public IEnumerator IDownload(string versionName, List<rRes> list, params object[] events)
		{
			TDownloader downloader = new TDownloaderUnity(IP, HostName, AppVersion);
			if (!Push(versionName, downloader))
			{
				yield break;
			}
			yield return CoroutineMgr.Instance.StartCoroutine(downloader.IStart(this, versionName, list, events));
		}
    }
}
