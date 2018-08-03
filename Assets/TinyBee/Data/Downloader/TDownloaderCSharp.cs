namespace TinyBee.Downloader
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System;
    using UnityEngine;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.Core;
    using ILogger = TinyBee.Logger.ILogger;
    using TLogger = TinyBee.Logger.TLogger;

    public class TDownloaderCSharp : TDownloader
    {
		public TDownloaderCSharp(string ip, string hostName, string appVersion) : base(ip, hostName, appVersion)
		{
		}

        protected override IEnumerator IDownloadBundle(string ip, string hostName, rRes data)
        {
            mLogger.Log("start download -> " + data.FileName);

            //設置下載路徑
            string dPath = GetLocation(ip, hostName) + data.Path + data.FileName;
            string sPath = GameMgr.Instance.DataPath + data.Path + data.FileName;

            //TODO:下載動作
            yield return null;

            string error = null;
            bool isDone = false;
            if ((error == null) && (isDone == true))
            {
				
            }
            else
            {
                mStatus = eStatus.Error;
                mOnError.InvokeGracefully(string.Format("下載失敗 -> {0}", error));
                mLogger.Log(string.Format("下載失敗 -> {0}", error));
            }
            mLogger.Log("end download -> " + data.FileName);
            Resources.UnloadUnusedAssets();
        }

        protected override IEnumerator IDownloadZip(string ip, string hostName, rRes data)
        {
            mLogger.Log("start download -> " + data.FileName);

            //設置下載路徑
            string dPath = GetLocation(ip, hostName) + data.Path + data.FileName;
            string sPath = GameMgr.Instance.DataPath + data.Path + data.FileName;

            //TODO:下載動作
            yield return null;

            string error = null;
            bool isDone = false;
            if ((error == null) && (isDone == true))
            {
                using (FileStream vFileStream = new FileStream(sPath, FileMode.Open, FileAccess.Read))
                {
                    using (ZipInputStream vZipInputStream = new ZipInputStream(vFileStream))
                    {
                        ZipEntry vZipEntry;

                        // 逐一取出壓縮檔內的檔案(解壓縮)
                        while ((vZipEntry = vZipInputStream.GetNextEntry()) != null)
                        {
                            string zPath = GameMgr.Instance.DataPath + data.Path + vZipEntry.Name;

                            //檢查是否存在舊檔案
                            if (File.Exists(zPath) == true)
                                File.Delete(zPath);

                            mLogger.Log(vZipEntry.Name);

                            using (FileStream fs = File.Create(zPath))
                            {
                                while (true)
                                {
                                    int size = vZipInputStream.Read(mBuffer, 0, mBuffer.Length);

                                    if (size > 0)
                                        fs.Write(mBuffer, 0, size);
                                    else
                                        break;

                                    yield return null;
                                }
                                fs.Close();
                            }
                            yield return null;
                        }
                        vZipInputStream.Close();
                    }
                    vFileStream.Close();
                }

#if !UNITY_TVOS || UNITY_EDITOR
                File.Delete(sPath);
#endif
            }
            else
            {
                mStatus = eStatus.Error;
                mOnError.InvokeGracefully(string.Format("下載失敗 -> {0}", error));
                mLogger.Log(string.Format("下載失敗 -> {0}", error));
            }
            mLogger.Log("end download -> " + data.FileName);
            Resources.UnloadUnusedAssets();
        }
    }
}