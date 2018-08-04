namespace TinyBee.Downloader
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System;
    using UnityEngine;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.Core;
	using TinyContext = TinyBee.Context.TinyContext;
    using ILogger = TinyBee.Logger.ILogger;
    using TLogger = TinyBee.Logger.TLogger;

    public abstract class TDownloader
    {
        //HTTP狀態列舉
        protected enum eStatus
        {
            None,        //無
            Init,        //初始化
            Updating,    //下載列表
            Downloading, //下載檔案
            Pause,       //暫停
            Shutdown,    //中止
            Finish,      //完成
            Error        //錯誤   
        }

        private const string HTTP_HEADER = "http://";

        protected ILogger mLogger = TLogger.Instance;
		private DownloadMgr mMgr = null;
		private string mIp = null;
		private string mHostName = null;
		private string mAppVersion = null;
		private string mVersionName = null;
		private Dictionary<string,string> mRecordMap = null;
		private List<rRes> mDownloadList = null;
		private int mDownloadCount = 0;
		private long mTotalSize = 0L;
		private long mNowSize = 0L;
		private eStatus mPreStatus = eStatus.None;
        protected eStatus mStatus = eStatus.None;
        protected byte[] mBuffer = new byte[1024 * 1024];
		private TCoroutine mTask = null;
		protected Action<object> mOnFinish = null;
        protected Action<object[]> mOnUpdate = null;
        protected Action<object> mOnError = null;

		public TDownloader(string ip, string hostName, string appVersion)
		{
			mIp = ip;
			mHostName = hostName;
			mAppVersion = appVersion;
			mRecordMap = new Dictionary<string, string>();
		}

		//載入資源列表
		private void LoadVersion (string versionName)
		{
#if UNITY_TVOS && !UNITY_EDITOR
			string path = Application.temporaryCachePath + "/" + TinyContext.Instance.LanguagePath + "/" + "Versions/";
#else
			string path = Application.persistentDataPath + "/" + TinyContext.Instance.LanguagePath + "/" + "Versions/";
#endif
			byte[] datas = TFile.Load(path, versionName);

			if (datas == null)
				return;
			
			string json = System.Text.UTF8Encoding.UTF8.GetString(datas);
			rMD5Info[] info = TJson.DeserializeObject<rMD5Info[]>(json);
			if (mRecordMap != null)
			{
				mRecordMap.Clear();
			}
			else
			{
				mRecordMap = new Dictionary<string, string>();
			}
			for (int i = 0; i < info.Length; i++)
				mRecordMap.Add(info[i].FlieName, info[i].MD5Code);

			return;
		}

		//存入資源列表
		private void SaveVersion(string versionName)
		{
#if UNITY_TVOS && !UNITY_EDITOR
			string path = Application.temporaryCachePath + "/" + TinyContext.Instance.LanguagePath + "/" + "Versions/";
#else
			string path = Application.persistentDataPath + "/" + TinyContext.Instance.LanguagePath + "/" + "Versions/";
#endif
			if (mRecordMap.Count > 0)
			{
				rMD5Info[] info = new rMD5Info[mRecordMap.Count];
				int count = 0;
				foreach (KeyValuePair<string, string> pair in mRecordMap)
				{
					info[count].FlieName = pair.Key;
					info[count].MD5Code = pair.Value;
					count++;
				}
				string json = TJson.SerializeObject(info);
				TFile.Save(path, versionName, System.Text.UTF8Encoding.UTF8.GetBytes(json));
			}
		}

		private void Record (rRes data)
		{
			//檢查紀錄檔是否存在
			if (mRecordMap == null)
			{
				mRecordMap = new Dictionary<string, string>();
				mRecordMap.Add(data.Path + data.FileName, data.MD5Code);
			}
			else
			{
				//檢查BundleKey是否存在
				if (mRecordMap.ContainsKey(data.Path + data.FileName) == true)
				{
					//檢查MD5碼
					if (String.Compare(mRecordMap[data.Path + data.FileName], data.MD5Code, false) != 0)
						mRecordMap[data.Path + data.FileName] = data.MD5Code;
				}
				else
				{
					mRecordMap.Add(data.Path + data.FileName, data.MD5Code);
				}
			}
			mDownloadCount++;
			mNowSize += data.FileSize;
		}

		public void Start(DownloadMgr mgr, string versionName, params object[] events)
        {
			Start(mgr, versionName, null, events);
        }

		public void Start(DownloadMgr mgr, string versionName, List<rRes> list, params object[] events)
        {
			mMgr = mgr;
			mVersionName = versionName;
            mDownloadList = list;
            mDownloadCount = 0;

            if (events != null)
            {
                mOnFinish = (Action<object>)events[0];
                mOnUpdate = (Action<object[]>)events[1];
                mOnError = (Action<object>)events[2];
            }

			if (mTask != null)
				mTask.Shutdown();

			LoadVersion(versionName);
			
			CoroutineMgr.Instance.StartCoroutine(mTask, IRun(versionName));
        }

		public IEnumerator IStart(DownloadMgr mgr, string versionName, params object[] events)
		{
			yield return CoroutineMgr.Instance.StartCoroutine(IStart(mgr, versionName, null, events));
		}

		public IEnumerator IStart(DownloadMgr mgr, string versionName, List<rRes> list, params object[] events)
		{
			mMgr = mgr;
			mVersionName = versionName;
			mDownloadList = list;
			mDownloadCount = 0;

			if (events != null)
			{
				mOnFinish = (Action<object>)events[0];
				mOnUpdate = (Action<object[]>)events[1];
				mOnError = (Action<object>)events[2];
			}

			if (mTask != null)
				mTask.Shutdown();

			LoadVersion(versionName);

			yield return CoroutineMgr.Instance.StartCoroutine(mTask, IRun(versionName));
		}

		public IEnumerator IRun(string versionName)
        {
            mStatus = eStatus.Init;
			if (mDownloadList == null)
			{
				yield return CoroutineMgr.Instance.StartCoroutine(IGetDownloadList(mIp, mHostName, versionName, mAppVersion));
			}

			yield return null;

			if (mStatus != eStatus.Error)
			{
				mNowSize = 0L;
				for (int i = 0; i < mDownloadList.Count; i++)
				{
					mStatus = eStatus.Downloading;
					IEnumerator enumerator = GetDownloader(mDownloadList[i]);
					while (enumerator != null)
					{
						try
						{
							if (!enumerator.MoveNext())
								break;
						}
						catch (Exception e)
						{
							mStatus = eStatus.Error;
							mOnError.InvokeGracefully(e.Message);
							mLogger.LogException(e);
							break;
						}
						yield return enumerator.Current;
					}
					if (mStatus != eStatus.Error)
					{
						Record(mDownloadList[i]);
						mOnUpdate.InvokeGracefully(new object[] {mVersionName, mDownloadList.Count, mDownloadCount, mTotalSize, mNowSize, mDownloadList[i]});
						yield return null;
					}
					else
					{
						break;
					}
				}
				if (mStatus != eStatus.Error)
				{
					mStatus = eStatus.Finish;
					mOnFinish.InvokeGracefully("finish");
				}
			}

			SaveVersion(versionName);

			mMgr.Pop(versionName);
        }

        public void Pause()
        {
			mPreStatus = mStatus;
            mStatus = eStatus.Pause;
            if (mTask != null)
                mTask.Pause();
        }

		public void Resume()
		{
			mStatus = mPreStatus;
			if (mTask != null)
				mTask.Resume();
		}

        public void Shutdown()
        {
            mStatus = eStatus.Shutdown;
            if (mTask != null)
				mTask.Shutdown();
			mMgr.Pop(mVersionName);
        }

		private bool CheckNeedUpdate(rRes data)
		{
			string path = TinyContext.Instance.DataPath + data.Path + data.FileName;

#if UNITY_TVOS && !UNITY_EDITOR
			//檢查檔案是否存在
			if (!File.Exists(path))
				return true;
#else
			//檢查是否為壓縮檔
			if (!data.FileName.EndsWith(".zip"))
			{
#if !UNITY_ANDROID || UNITY_EDITOR
				//檢查檔案是否存在
				if (!File.Exists(path))
					return true;
#endif
			}
#endif

			//檢查紀錄檔是否存在
			if (mRecordMap == null)
			{
				mRecordMap = new Dictionary<string, string>();
				return true;
			}
			else
			{
				//檢查BundleKey是否存在
				if (!mRecordMap.ContainsKey(data.Path + data.FileName))
					return true;

				//檢查BundleKey值是否相符
				if (String.Compare(mRecordMap[data.Path + data.FileName], data.MD5Code, false) != 0)
					return true;
			}

			return false;
		}

        //取得HTTP路徑
        protected string GetLocation(string ip, string hostName)
        {
#if UNITY_STANDALONE
			return HTTP_HEADER + ip + "/" + hostName + "/Windows/";
#elif UNITY_IOS
			return HTTP_HEADER + ip + "/" + hostName + "/Ios/";
#elif UNITY_TVOS
			return HTTP_HEADER + ip + "/" + hostName + "/Tvos/";
#elif UNITY_ANDROID
			return HTTP_HEADER + ip + "/" + hostName + "/Android/";
#endif
        }

        protected IEnumerator IGetDownloadList(string ip, string hostName, string versionName, string version)
        {
			string path = GetLocation(ip, hostName) + TinyContext.Instance.LanguagePath + "/" + "Versions/";
            string name = versionName;

            mStatus = eStatus.Updating;

            yield return null;

            using (WWW bundle = new WWW(path + name))
            {
                yield return bundle;

                //檢查下載錯誤訊息
                if (bundle.error != null)
                {
                    mStatus = eStatus.Error;
                    mOnError.InvokeGracefully(bundle.error);
                    mLogger.Log(bundle.error);
                    yield break;
                }

                //檢查是否下載完成
                if (bundle.isDone == true)
                {
                    byte[] xor = TFile.XOR(bundle.bytes);
					string json = System.Text.UTF8Encoding.UTF8.GetString(xor);
					rRes[] datas = TJson.DeserializeObject<rRes[]>(json);

					if (datas[0].Version != version)
                    {
                        mStatus = eStatus.Error;
                        mOnError.InvokeGracefully("版本不符");
                        mLogger.Log("版本不符");
                        yield break;
                    }

					if (mDownloadList == null)
					{
						mDownloadList = new List<rRes>();
					}
					else
					{
						mDownloadList.Clear();
					}

					mTotalSize = 0;;
					for (int i = 0; i < datas.Length; i++)
                    {
						if (!CheckNeedUpdate(datas [i]))
							continue;

						mDownloadList.Add(datas[i]);
						mTotalSize += datas[i].FileSize;
                    }
                }
                else
                {
                    mStatus = eStatus.Error;
                    mOnError.InvokeGracefully(string.Format("更新失敗 -> {0}", bundle.url));
                    mLogger.Log(string.Format("更新失敗 -> {0}", bundle.url));
                    yield break;
                }
            }
        }

		private IEnumerator GetDownloader(rRes data)
		{
			if (data.FileName.EndsWith(".zip"))
				return IDownloadZip(mIp, mHostName, data);

			return IDownloadBundle(mIp, mHostName, data);
		}

        protected abstract IEnumerator IDownloadBundle(string ip, string hostName, rRes data);
        protected abstract IEnumerator IDownloadZip(string ip, string hostName, rRes data);
    }
}