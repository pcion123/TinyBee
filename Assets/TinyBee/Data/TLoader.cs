namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System;

	public abstract class TLoader<T> : IDisposable
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
			using (MemoryStream vStream = new MemoryStream(byteArray))
			{
				using (StreamReader vReader = new StreamReader(vStream, System.Text.UTF8Encoding.UTF8))
				{
					string vStr = vReader.ReadToEnd();
					if (vStr.Trim() == "[]")
						return;
					LoadJson(vStr);
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