﻿namespace TinyBee.Downloader
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

    public class TDownloaderUnity : TDownloader
    {
		public TDownloaderUnity(string ip, string hostName, string appVersion) : base(ip, hostName, appVersion)
		{
		}

        protected override IEnumerator IDownloadBundle(string ip, string hostName, rRes data)
        {
            mLogger.Log("start download -> " + data.FileName);

            //設置下載路徑
            string path = GetLocation(ip, hostName) + data.Path + data.FileName;
            using (WWW bundle = new WWW(path))
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
                    //設置存檔路徑
					string sPath = TinyContext.Instance.DataPath + data.Path;
                    string sName = data.FileName;
                    TFile.Save(sPath, sName, bundle.bytes);
                }
                else
                {
                    mStatus = eStatus.Error;
                    mOnError.InvokeGracefully(string.Format("下載失敗 -> {0}", bundle.url));
                    mLogger.Log(string.Format("下載失敗 -> {0}", bundle.url));
                }
            }
            mLogger.Log("end download -> " + data.FileName);
            Resources.UnloadUnusedAssets();
        }

        protected override IEnumerator IDownloadZip(string ip, string hostName, rRes data)
        {
            mLogger.Log("start download -> " + data.FileName);

            //設置下載路徑
            string path = GetLocation(ip, hostName) + data.Path + data.FileName;

            using (WWW bundle = new WWW(path))
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
                    //設置存檔路徑
					string sPath = TinyContext.Instance.DataPath + data.Path;
                    string sName = data.FileName;
                    TFile.Save(sPath, sName, bundle.bytes);

					using (FileStream fileStream = new FileStream(sPath + sName, FileMode.Open, FileAccess.Read))
                    {
						using (ZipInputStream zipInputStream = new ZipInputStream(fileStream))
                        {
                            ZipEntry zipEntry;

                            // 逐一取出壓縮檔內的檔案(解壓縮)
							while ((zipEntry = zipInputStream.GetNextEntry()) != null)
                            {
								string zPath = TinyContext.Instance.DataPath + data.Path + zipEntry.Name;

                                //檢查是否存在舊檔案
                                if (File.Exists(zPath) == true)
                                    File.Delete(zPath);

								mLogger.Log(zipEntry.Name);

                                using (FileStream fs = File.Create(zPath))
                                {
                                    while (true)
                                    {
										int size = zipInputStream.Read(mBuffer, 0, mBuffer.Length);

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
							zipInputStream.Close();
                        }
						fileStream.Close();
                    }

#if !UNITY_TVOS || UNITY_EDITOR
					File.Delete(sPath + sName);
#endif
                }
                else
                {
                    mStatus = eStatus.Error;
                    mOnError.InvokeGracefully(string.Format("下載失敗 -> {0}", bundle.url));
                    mLogger.Log(string.Format("下載失敗 -> {0}", bundle.url));
                }
            }
            mLogger.Log("end download -> " + data.FileName);
            Resources.UnloadUnusedAssets();
        }
    }
}