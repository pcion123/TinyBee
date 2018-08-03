namespace TinyBee.UI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public struct rTex
	{
		public string Name; //名稱
		public Texture Tex; //貼圖

		public rTex (string vName, Texture vTex)
		{
			Name = vName;
			Tex = vTex;
		}
	}

	public class SVInfo<T> : MonoBehaviour
	{
		protected UIForm mUI = null;       //UI介面
		protected SVList<T> mList = null; //列表物件
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

		protected virtual void Awake ()
		{
			//掛載拖拉腳本
			mDrag = gameObject.AddComponent<UIDragScrollView>();
		}

		// Use this for initialization
		protected virtual void Start ()
		{
			//在子類別實作內容
		}

		// Update is called once per frame
		protected virtual void Update ()
		{
			//在子類別實作內容
		}

		//重置
		public virtual void Reset ()
		{
			Index = -1;
		}

		//檢查是否顯示資訊
		protected virtual bool CheckShowInfo ()
		{
			return false;
		}

		//設置列表
		private void SetList (SVList<T> vList)
		{
			mList = vList;
		}

		//設置計時器
		private void SetTimer (long vTimer)
		{
			mTimer = vTimer;
		}

		//設置是否顯示資訊標記
		private void SetIsInfo (bool vIsInfo)
		{
			mIsInfo = vIsInfo;
		}

		//設置是否按壓標記
		private void SetIsPress (bool vIsPress)
		{
			mIsPress = vIsPress;
		}

		//設置索引
		protected virtual void SetIndex (int vIndex)
		{
			mIndex = vIndex;
		}

		//設置編號
		private void SetID (int vID)
		{
			mID = vID;
		}

		//載入貼圖
		protected virtual IEnumerator ILoadTexture (string vPath, string vTexName, Action<Texture> vCallback)
		{
			TObject vObject = new TObject();

			Texture vTex = mList.GetTex(vTexName);

			if (vTex != null)
			{
				if (vCallback != null)
					vCallback(vTex);

				vObject.Free();
				yield break;
			}

			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadTexture(vPath, vTexName, vObject));

			if (vObject.Bundle != null)
			{
				vTex = vObject.Bundle.texture as Texture;

				if (vTex != null)
				{
					mList.TexPool.Add(new rTex(vTexName, vTex));

					vCallback.InvokeGracefully(vTex);
				}

				if (vObject.Bundle.assetBundle != null)
					vObject.Bundle.assetBundle.Unload(false);
			}
			else
			{
				vCallback.InvokeGracefully(null);
			}

			vObject.Free();
		}

		//載入貼圖
		protected virtual IEnumerator ILoadTexture (string vPath, string vTexName, int vIndex, Action<int,Texture> vCallback)
		{
			TObject vObject = new TObject();

			Texture vTex = mList.GetTex(vTexName);

			if (vTex != null)
			{
				vCallback.InvokeGracefully(vIndex, vTex);
				vObject.Free();
				yield break;
			}

			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadTexture(vPath, vTexName, vObject));

			if (vObject.Bundle != null)
			{
				vTex = vObject.Bundle.texture as Texture;

				if (vTex != null)
				{
					mList.TexPool.Add(new rTex(vTexName, vTex));
					vCallback.InvokeGracefully(vIndex, vTex);
				}

				if (vObject.Bundle.assetBundle != null)
					vObject.Bundle.assetBundle.Unload(false);
			}
			else
			{
				vCallback.InvokeGracefully(vIndex, null);
			}

			vObject.Free();
		}
	}
}