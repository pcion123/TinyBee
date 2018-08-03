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
#if UNITY_EDITOR		
        [System.Serializable]
        private class DataDictionary : SerializableDictionary<string, object> { }
		[SerializeField]
		private DataDictionary mTables = new DataDictionary();
#else
		[SerializeField]
		private Dictionary<string,object> mTables = new Dictionary<string,object>();
#endif

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
        private IEnumerator ILoad(string path, string name, TObject obj)
        {
			obj.Clear();

            //檢查檔案是否存在
#if !UNITY_ANDROID || UNITY_EDITOR
			if (File.Exists(path + name) == false)
            {
				obj.Err = string.Format("{0}{1} is not exist", path, name);
                yield break;
            }
#endif

#if UNITY_STANDALONE
			string fPath = string.Format("file://{0}{1}", path, name);
#elif UNITY_IOS || UNITY_TVOS
			string fPath = string.Format("file://{0}{1}", path, name);
#elif UNITY_ANDROID
			string fPath = string.Format("{0}{1}", path, name);
#endif

			Uri uri = new Uri(fPath);
			fPath = uri.AbsoluteUri;
			WWW bundle = new WWW(fPath);
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
				obj.Err = bundle.error;
				bundle.Dispose();
				bundle = null;
            }
            else
            {
				obj.Bundle = bundle;
            }
        }

        public IEnumerator ILoadBundle(string path, string name, TObject obj)
        {
			IEnumerator enumerator = ILoad(path, name + ".unity3d", obj);
			while ((enumerator != null) && (enumerator.MoveNext()))
				yield return enumerator.Current;
        }

		public IEnumerator ILoadTexture(string path, string name, TObject obj)
        {
			IEnumerator enumerator = ILoad(path, name + ".png", obj);
			while ((enumerator != null) && (enumerator.MoveNext()))
				yield return enumerator.Current;
        }

		public IEnumerator ILoadData(string path, string name, TObject obj)
        {
			IEnumerator enumerator = ILoad(path, name + ".dat", obj);
			while ((enumerator != null) && (enumerator.MoveNext()))
				yield return enumerator.Current;
        }

		public IEnumerator ILoadMP3(string path, string name, TObject obj)
        {
			IEnumerator enumerator = ILoad(path, name + ".mp3", obj);
			while ((enumerator != null) && (enumerator.MoveNext()))
				yield return enumerator.Current;
        }
    }
}
