namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
    using System.IO;
    using System;
    using UnityEngine;

	[TMonoSingletonPath("[Data]/DataMgr")]
	public class DataMgr : TMgrBehaviour, ISingleton
	{
        [System.Serializable]
        private class DataDictionary : SerializableDictionary<string, object> { }

        [SerializeField]
        private DataDictionary mTables = new DataDictionary();

		public static DataMgr Instance
		{
			get { return MonoSingletonProperty<DataMgr>.Instance; }
		}

		public override int ManagerId
		{
			get { return MgrEnumBase.Data; }
		}

		public void OnSingletonInit() {}

		public string GetDataName(eLanguage language, string dataName)
		{
			return string.Format("{0}_{1}", language.ToString(), dataName);
		}

		public T Get<T>(string key)
        {
			if (!mTables.ContainsKey(key))
				return default(T);
			
            object loader = null;
            if (!mTables.TryGetValue(key, out loader))
				return default(T);

            return (T)loader;
        }

        public bool Push<T>(string key, T loader)
        {
            if (mTables.ContainsKey(key))
				return false;

            mTables.Add(key, loader);
			return true;
        }

		public T Pop<T>(string key)
        {
            if (!mTables.ContainsKey(key))
				return default(T);

			object loader = null;
			if (!mTables.TryGetValue(key, out loader))
				return default(T);

            mTables.Remove(key);
			return (T)loader;
        }

        //載入資源
        private IEnumerator ILoad(string vPath, string vName, TObject vObj)
        {
            vObj.Clear();

            //檢查檔案是否存在
#if !UNITY_ANDROID || UNITY_EDITOR
            if (File.Exists(vPath + vName) == false)
            {
                vObj.Err = string.Format("{0}{1} is not exist", vPath, vName);
                yield break;
            }
#endif

#if UNITY_STANDALONE
		    string zPath = string.Format("file://{0}{1}", vPath, vName);
#elif UNITY_IOS || UNITY_TVOS
		    string zPath = string.Format("file://{0}{1}", vPath, vName);
#elif UNITY_ANDROID
            string zPath = string.Format("{0}{1}", vPath, vName);
#endif

            Uri vUri = new Uri(zPath);
            zPath = vUri.AbsoluteUri;
            WWW vBundle = new WWW(zPath);

            while (!vBundle.isDone)
            {
                if (vBundle.error != null)
                {
                    break;
                }
                else
                {
                    yield return vBundle;
                }
            }

            if (vBundle.error != null)
            {
                vObj.Err = vBundle.error;
                vBundle.Dispose();
                vBundle = null;
            }
            else
            {
                vObj.Bundle = vBundle;
            }
        }

        public IEnumerator ILoadBundle(string vPath, string vName, TObject vObj)
        {
            IEnumerator vEnumerator = ILoad(vPath, vName + ".unity3d", vObj);
            while ((vEnumerator != null) && (vEnumerator.MoveNext()))
                yield return vEnumerator.Current;
        }

        public IEnumerator ILoadTexture(string vPath, string vName, TObject vObj)
        {
            IEnumerator vEnumerator = ILoad(vPath, vName + ".png", vObj);
            while ((vEnumerator != null) && (vEnumerator.MoveNext()))
                yield return vEnumerator.Current;
        }

        public IEnumerator ILoadData(string vPath, string vName, TObject vObj)
        {
            IEnumerator vEnumerator = ILoad(vPath, vName + ".dat", vObj);
            while ((vEnumerator != null) && (vEnumerator.MoveNext()))
                yield return vEnumerator.Current;
        }

        public IEnumerator ILoadMP3(string vPath, string vName, TObject vObj)
        {
            IEnumerator vEnumerator = ILoad(vPath, vName + ".mp3", vObj);
            while ((vEnumerator != null) && (vEnumerator.MoveNext()))
                yield return vEnumerator.Current;
        }
    }
}
