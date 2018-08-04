namespace TinyBee.Data
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System;
	using Data;

	public abstract class TLoader<T> : IDisposable where T : IData
	{
		//資料庫
		protected List<T> mDataList = null;

		public int Count { get { return mDataList.Count; } }

		public void Dispose()
		{
			mDataList.Free();
		}

		//讀Json -> 開放給外界使用
		public void LoadJson(byte[] byteArray)
		{
			byteArray = TFile.XOR(byteArray);
			using (MemoryStream stream = new MemoryStream(byteArray))
			{
				using (StreamReader reader = new StreamReader(stream, System.Text.UTF8Encoding.UTF8))
				{
					string str = reader.ReadToEnd();
					if (str.Trim() == "[]")
						return;
					LoadJson(str);
				}
			}
			Analyze();
		}

		//讀Json -> 只提供給內部使用
		private void LoadJson(string json)
		{
			T[] dataArray = TJson.DeserializeObject<T[]>(json);

			if (mDataList == null)
				mDataList = new List<T>();
			else
				mDataList.Clear();

			for (int i = 0; i < dataArray.Length; i++)
				mDataList.Add(dataArray[i]);

			dataArray = null;
		}

		//依Key取得Data
		public abstract T GetDataByKey(int key);

		protected virtual void Analyze() {}

		//依Index取得Data
		public T GetDataByIndex(int index)
		{
			if (mDataList == null)
				return default(T);

			if (mDataList.Count <= index)
				return default(T);

			return mDataList[index];
		}

		public virtual void Clear()
		{
			if (mDataList != null)
			{
				mDataList.Clear();
				mDataList = null;
			}
		}
	}
}