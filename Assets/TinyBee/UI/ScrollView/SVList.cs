namespace TinyBee.UI
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class SVList<T> : MonoBehaviour
	{
		protected UIForm mUI = null;                     //UI介面
		protected UIScrollView mScrollView = null;       //ScrollView
		protected WrapContent mWrapContent = null;       //WrapContent
		protected UIPanel mPanel = null;                 //Panel
		protected List<T> mInfo = null;                  //資訊列表
		protected List<SVInfo<T>> mContainer = null;     //資訊容器
		private List<rTex> mTexPool = null;              //貼圖暫存池
		private GameObject mItem = null;                 //元件
		private Vector4 mBounds = Vector4.zero;          //顯示範圍
		private eDirection mDirection = eDirection.None; //方向
		private int mUnitCount = 1;                      //單元數量
		private int mItemCount = 0;                      //元件數量
		private float mItemHeight = 0f;                  //元件高度
		private float mItemWidth = 0f;                   //元件寬度
		private float mOriginalX = 0;                    //原始X座標
		private float mOriginalY = 0;                    //原始Y座標
		private float mFixX = 0f;                        //修正X座標
		private float mFixY = 0f;                        //修正Y座標
		private bool mIsDrag = false;                    //是否拖動標記
		private bool mIsRepeat = false;                  //是否循環標記

		public UIScrollView ScrollView {get{return mScrollView;}}
		public UICenterOnChild CenterOnChild {get{return mScrollView.centerOnChild;}}
		public WrapContent WrapContent {get{return mWrapContent;}}
		public UIPanel Panel {get{return mPanel;}}
		public List<T> Info {get{return mInfo;}}
		public List<SVInfo<T>> Container {get{return mContainer;}}
		public List<rTex> TexPool {get{return mTexPool;}}
		public GameObject Item {get{return mItem;} set{SetItem(value);}}
		public Vector4 Bounds {get{return mBounds;} set{SetBounds(value);}}
		public eDirection Direction {get{return mDirection;} set{SetDirection(value);}}
		public int UnitCount {get{return mUnitCount;} set{SetUnitCount(value);}}
		public int ItemCount {get{return mItemCount;} set{SetItemCount(value);}}
		public float ItemHeight {get{return mItemHeight;} set{SetItemHeight(value);}}
		public float ItemWidth {get{return mItemWidth;} set{SetItemWidth(value);}}
		public float OriginalX {get{return mOriginalX;}}
		public float OriginalY {get{return mOriginalY;}}
		public float FixX {get{return mFixX;} set{SetFixX(value);}}
		public float FixY {get{return mFixY;} set{SetFixY(value);}}
		public bool IsDrag {get{return mIsDrag;} set{SetIsDrag(value);}}
		public bool IsRepeat {get{return mIsRepeat;} set{SetIsRepeat(value);}}

		protected virtual void Awake()
		{
			//建立ScrollView
			if (mScrollView == null)
			{
				mScrollView = gameObject.AddComponent<UIScrollView>();
				mScrollView.dragEffect = UIScrollView.DragEffect.MomentumAndSpring;
				mScrollView.onDragStarted = OnDragStarted;
				mScrollView.onDragFinished = OnDragFinished;
				mScrollView.onMomentumMove = OnMomentumMove;
				mScrollView.onStoppedMoving = OnStoppedMoving;
			}

			//建立WrapContent
			if (mWrapContent == null)
			{
				GameObject container = new GameObject("Container");
				container.transform.parent = transform;
				container.transform.localPosition = Vector3.zero;
				container.transform.localEulerAngles = Vector3.zero;
				container.transform.localScale = Vector3.one;
				mWrapContent = container.AddComponent<WrapContent>();
				mWrapContent.onInitializeItem = OnUpdateItem;
				mWrapContent.isRepeat = false;
				mWrapContent.OnMoving = OnMoving;
			}

			//取得UIPanel
			if (mPanel == null)
				mPanel = gameObject.GetComponent<UIPanel>();

			//建立資訊列表
			if (mInfo == null)
				mInfo = new List<T>();

			//建立資訊容器
			if (mContainer == null)
				mContainer = new List<SVInfo<T>>();

			//建立貼圖暫存池
			if (mTexPool == null)
				mTexPool = new List<rTex>();

			mOriginalX = transform.localPosition.x;
			mOriginalY = transform.localPosition.y;
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

		//事件 更新元件
		protected virtual void OnUpdateItem(GameObject go, int wrapIndex, int realIndex) {}

		//ScrollView事件 拖動開始
		protected virtual void OnDragStarted() {}

		//ScrollView事件 拖動結束
		protected virtual void OnDragFinished() {}

		//ScrollView事件 移動
		protected virtual void OnMomentumMove() {}

		//ScrollView事件 移動停止
		protected virtual void OnStoppedMoving() {}

		//WrapContent事件 移動
		protected virtual void OnMoving() {}

		//重設
		public virtual void Reset(bool isBottom = false)
		{
			UIDragScrollView[] drag = gameObject.GetComponentsInChildren<UIDragScrollView>();

			//檢查是否可以拖動
			IsDrag = CheckCanDrag();

			//排序Item位置
			mWrapContent.SortAlphabetically();

			//重置List位置
			mScrollView.ResetPosition();

			float x = transform.localPosition.x;
			float y = transform.localPosition.y;
			float z = transform.localPosition.z;

			//檢查方向
			if (mDirection == eDirection.Horizontal)
			{
				transform.localPosition = new Vector3(mOriginalX + mFixX, y, 0);
				mPanel.baseClipRegion = mBounds;
				mPanel.clipOffset = new Vector2(-mFixX, 0);
			}
			else
			{
				transform.localPosition = new Vector3(x, mOriginalY + mFixY, 0);
				mPanel.baseClipRegion = mBounds;
				mPanel.clipOffset = new Vector2(0, -mFixY);
			}

			//
			if ((mContainer.Count * mUnitCount) <= (mItemCount * mUnitCount))
				mWrapContent.SortAlphabetically();

			if ((isBottom == true) && (Info.Count > mItemCount))
				MoveToItem(Info.Count - 1, 4, false);
		}

		//檢查數量是否可以拖動
		public virtual bool CheckCanDrag()
		{
			if ((mContainer.Count * mUnitCount) <= (mItemCount * mUnitCount))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		//檢查是否需要增加元件
		public virtual bool CheckNeedAddItem()
		{
			if (mContainer.Count > mItemCount + 2)
				return false;
			else
				return true;
		}

		//檢查是否需要加入貼圖
		private bool CheckNeedAddTex(string name)
		{
			for (int i = 0; i < mTexPool.Count; i++)
			{
				if (mTexPool[i].Name != name)
					continue;

				return false;
			}
			return true;
		}

		//取得貼圖
		public Texture GetTex(string name)
		{
			for (int i = 0; i < mTexPool.Count; i++)
			{
				if (mTexPool[i].Name != name)
					continue;

				return mTexPool[i].Tex;
			}
			return null;
		}

		//取得資訊
		public T GetInfo(int index)
		{
			//檢查物件
			if (mInfo == null)
				return default(T);

			//檢查索引
			if (index < 0)
				return default(T);

			//檢查索引
			if (index > mInfo.Count - 1)
				return default(T);

			return mInfo[index];
		}

		//設置元件
		private void SetItem(GameObject item)
		{
			mItem = item;
		}

		//設置顯示範圍
		private void SetBounds(Vector4 bounds)
		{
			mBounds = bounds;
		}

		//設置方向
		private void SetDirection(eDirection direction)
		{
			mDirection = direction;

			if (direction == eDirection.Horizontal)
			{
				if (mScrollView != null)
					mScrollView.movement = UIScrollView.Movement.Horizontal;
			}
			else
			{
				if (mScrollView != null)
					mScrollView.movement = UIScrollView.Movement.Vertical;
			}
		}

		//設置單元數量
		private void SetUnitCount(int unitCount)
		{
			mUnitCount = unitCount;
		}

		//設置元件數量
		private void SetItemCount(int itemCount)
		{
			mItemCount = itemCount;
		}

		//設置元件高度
		private void SetItemHeight(float itemHeight)
		{
			mItemHeight = itemHeight;

			if (mDirection == eDirection.Vertical)
				mWrapContent.itemSize = (int)itemHeight;
		}

		//設置元件寬度
		private void SetItemWidth(float itemWidth)
		{
			mItemWidth = itemWidth;

			if (mDirection == eDirection.Horizontal)
				mWrapContent.itemSize = (int)itemWidth;
		}

		//設置修正X座標
		private void SetFixX(float fixX)
		{
			mFixX = fixX;
		}

		//設置修正Y座標
		private void SetFixY(float fixY)
		{
			mFixY = fixY;
		}

		//設置是否拖動標記
		private void SetIsDrag(bool isDrag)
		{
			mIsDrag = isDrag;

			UIDragScrollView[] drag = gameObject.GetComponentsInChildren<UIDragScrollView>();

			if (drag == null)
				return;

			for (int i = 0; i < drag.Length; i++)
				drag[i].enabled = isDrag;
		}

		//設置是否循環標記
		private void SetIsRepeat(bool isRepeat)
		{
			mIsRepeat = isRepeat;

			if (mWrapContent != null)
				mWrapContent.isRepeat = isRepeat;
		}

		//清除元件
		public void Clear()
		{
			for (int i = 0 ; i < mContainer.Count; i++)
			{
				SVInfo<T> info = mContainer[i];
				if (info == null)
					continue;
				DestroyImmediate(info.gameObject);
			}

			mInfo.Clear();
			mContainer.Clear();

			if (mWrapContent.isRepeat == false)
			{
				mWrapContent.minIndex = 0;
				mWrapContent.maxIndex = 0;
			}

			mWrapContent.SortAlphabetically();
		}

		//加入貼圖
		public void AddTex(rTex tex)
		{
			if (CheckNeedAddTex(tex.Name) == false)
				return;

			mTexPool.Add(tex);
		}

		//加入元件
		protected void Add(SVInfo<T> info, bool isSort)
		{
			mContainer.Add(info);

			int index = mContainer.Count - 1;

			info.ID = index;
			info.Index = index;
			info.transform.name = index.ToString("D4");
			info.transform.parent = mWrapContent.transform;
			info.transform.localPosition = new Vector3(0, 0, 0);
			info.transform.localEulerAngles = new Vector3(0, 0, 0);
			info.transform.localScale = new Vector3(1, 1, 1);

			if (!mWrapContent.isRepeat)
			{
				mWrapContent.minIndex = 0;
				mWrapContent.maxIndex = index;
			}

			if (isSort)
				mWrapContent.SortAlphabetically();

			IsDrag = CheckCanDrag();
		}

		//刪除元件
		public void Del(int index, bool isSort = true)
		{
			mInfo.RemoveAt(index);

			if (mInfo.Count < mContainer.Count)
			{
				SVInfo<T> info = mContainer[index];
				mContainer.RemoveAt(index);
				DestroyImmediate(info.gameObject);

				for (int i = 0; i < mContainer.Count; i++)
				{
					mContainer[i].ID = i;
					mContainer[i].name = i.ToString("D4");
				}
			}

			if (!mWrapContent.isRepeat)
			{
				mWrapContent.minIndex = 0;
				mWrapContent.maxIndex = mInfo.Count - 1;
			}

			if (isSort)
				mWrapContent.SortAlphabetically();

			IsDrag = CheckCanDrag();
		}

		//校正移動索引值
		protected virtual int FixMoveItemIndex(int index)
		{
			//檢查索引值下限
			if (index <= 0)
				return 0;

			//檢查索引值上限
			if (index + mItemCount > mInfo.Count)
				return (mInfo.Count - mItemCount);

			return index;
		}

		protected virtual void MoveItem(int index, float speed, bool isSpring)
		{
			float x = transform.localPosition.x;
			float y = transform.localPosition.y;
			float z = transform.localPosition.z;

			//校正索引值
			index = index + 1;

			if (mDirection == eDirection.Horizontal)
			{
				x = mFixX - index * mItemWidth + mBounds.z + 10;
			}
			else
			{
				y = mFixY + index * mItemHeight - mBounds.w + 10;
			}

			if (isSpring)
			{
				SpringPanel.Begin(gameObject, new Vector3(x, y, z), speed);
			}
			else
			{
				//檢查方向
				if (mDirection == eDirection.Horizontal)
				{
					transform.localPosition = new Vector3(x, y, 0);
					mPanel.baseClipRegion = mBounds;
					mPanel.clipOffset = new Vector2(-x, 0);
					SpringPanel.Begin(gameObject, new Vector3(x, y, x), speed);
				}
				else
				{
					transform.localPosition = new Vector3(x, y, 0);
					mPanel.baseClipRegion = mBounds;
					mPanel.clipOffset = new Vector2(0, -y);
					SpringPanel.Begin(gameObject, new Vector3(x, y, x), speed);
				}
			}
		}

		//移動至指定項目位置
		public virtual void MoveToItem(int index, float speed = 1f, bool isSpring = true)
		{
			//檢查是否可以拖動
			IsDrag = CheckCanDrag();

			if (!IsDrag)
				return;

			MoveItem(index, speed, isSpring);
		}
	}
}