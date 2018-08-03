namespace TinyBee.UI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public struct rTex
	{
		public string Name; //名稱
		public Texture Tex; //貼圖

		public rTex (string name, Texture tex)
		{
			Name = name;
			Tex = tex;
		}
	}

	public class SVInfo<T> : MonoBehaviour
	{
		protected UIForm mUI = null;           //UI介面
		protected SVList<T> mList = null;      //列表物件
		private UIDragScrollView mDrag = null; //拖拉物件
		private long mTimer = 0;               //計時器
		private bool mIsInfo = false;          //資訊標記
		private bool mIsPress = false;         //按壓標記
		private int mIndex = -1;               //索引
		private int mID = -1;                  //編號

		public SVList<T> List {get{return mList;} set{SetList(value);}}
		public UIDragScrollView Drag {get{return mDrag;}}
		public long Timer {get{return mTimer;} set{SetTimer(value);}}
		public bool IsInfo {get{return mIsInfo;} set{SetIsInfo(value);}}
		public bool IsPress {get{return mIsPress;} set{SetIsPress(value);}}
		public int Index {get{return mIndex;} set{SetIndex(value);}}
		public int ID {get{return mID;} set{SetID(value);}}

		protected virtual void Awake()
		{
			//掛載拖拉腳本
			mDrag = gameObject.AddComponent<UIDragScrollView>();
		}

		// Use this for initialization
		protected virtual void Start()
		{
			//在子類別實作內容
		}

		// Update is called once per frame
		protected virtual void Update()
		{
			//在子類別實作內容
		}

		//重置
		public virtual void Reset()
		{
			Index = -1;
		}

		//檢查是否顯示資訊
		protected virtual bool CheckShowInfo()
		{
			return false;
		}

		//設置列表
		private void SetList(SVList<T> list)
		{
			mList = list;
		}

		//設置計時器
		private void SetTimer(long timer)
		{
			mTimer = timer;
		}

		//設置是否顯示資訊標記
		private void SetIsInfo(bool isInfo)
		{
			mIsInfo = isInfo;
		}

		//設置是否按壓標記
		private void SetIsPress(bool isPress)
		{
			mIsPress = isPress;
		}

		//設置索引
		protected virtual void SetIndex(int index)
		{
			mIndex = index;
		}

		//設置編號
		private void SetID(int id)
		{
			mID = id;
		}

		//載入貼圖
		protected virtual IEnumerator ILoadTexture(string path, string texname, Action<Texture> callback)
		{
			Texture tex = mList.GetTex(texname);
			if (tex != null)
			{
				callback.InvokeGracefully(tex);
				yield break;
			}

			TObject obj = new TObject();
			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadTexture(path, texname, obj));
			if (obj.Bundle != null)
			{
				tex = obj.Bundle.texture as Texture;
				if (tex != null)
				{
					mList.TexPool.Add(new rTex(texname, tex));
					callback.InvokeGracefully(tex);
				}

				if (obj.Bundle.assetBundle != null)
					obj.Bundle.assetBundle.Unload(false);
			}
			else
			{
				callback.InvokeGracefully(null);
			}
			obj.Free();
		}

		//載入貼圖
		protected virtual IEnumerator ILoadTexture (string path, string texname, int index, Action<int,Texture> callback)
		{
			Texture tex = mList.GetTex(texname);
			if (tex != null)
			{
				callback.InvokeGracefully(index, tex);
				yield break;
			}

			TObject obj = new TObject();
			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadTexture(path, texname, obj));
			if (obj.Bundle != null)
			{
				tex = obj.Bundle.texture as Texture;
				if (tex != null)
				{
					mList.TexPool.Add(new rTex(texname, tex));
					callback.InvokeGracefully(index, tex);
				}

				if (obj.Bundle.assetBundle != null)
					obj.Bundle.assetBundle.Unload(false);
			}
			else
			{
				callback.InvokeGracefully(index, null);
			}
			obj.Free();
		}
	}
}