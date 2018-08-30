namespace TinyBee.UI
{
    using UnityEngine;
    using System.Collections;
	using TinyContext = TinyBee.Context.TinyContext;
    using ILogger = Logger.ILogger;
    using TLogger = Logger.TLogger;

    //攝影機列舉
    public enum eCamera
    {
        None, //無
        Main, //主攝影機
        Top   //副攝影機
    }

    //方向列舉
    public enum eDirection
    {
        None = 0,       //無
        Horizontal = 1, //水平
        Vertical = 2    //垂直
    }

    //水平方向列舉
    public enum eHorizontal
    {
        NONE,   //無
        LFFT_R, //左 -> 右
        LEFT,   //左
        MIDDLE, //中
        RIGHT,  //右
        RIGHT_L //右 -> 左
    }

    //垂直方向列舉
    public enum eVertical
    {
        NONE,   //無
        UP_D,   //上 -> 下
        UP,     //上
        MIDDLE, //中
        DOWN,   //下
        DOWN_U  //下 -> 上
    }

    public abstract class UIForm : MonoBehaviour
    {
        protected ILogger mLogger = TLogger.Instance; //
        protected UIMgr mMgr = null;                  //UI管理
        protected int mUI = 0;                        //UI類型
        private string mUIName = null;                //UI名稱
		protected Transform mParent = null;           //
		protected int mMinDepth = 0;                  //最低深度
		protected int mMaxDepth = 0;                  //最高深度
        private int mOffsetDepth = 0;                 //
		protected int mParam = 0;                     //
        private int mRank = 0;                        //UI位階
        protected bool mIsLoading = false;            //
#if _MASK
		private GameObject mMask = null;              //
#endif
		protected TAudio mBGAudio = null;

		public int UI { get { return mUI; } }
        public string UIName { get { return mUIName; } }
        public int Param { get { return mParam; } }
        public int Rank { get { return mRank; } }
        public bool IsLoading { get { return mIsLoading; } }
        public bool Hidden { get { return !gameObject.activeSelf; } set { gameObject.SetActive(!value); } }
#if _BLUR
		public bool Blur
		{
			get
			{
				eUIParam p = (eUIParam)mParam;
				bool isTop = (p & eUIParam.Top) == eUIParam.Top;
				bool isBlur = (p & eUIParam.Blur) == eUIParam.Blur;
				return (isTop ? transform.parent == mMgr.GetParent(eUIParam.Top) : transform.parent == mMgr.GetParent(eUIParam.None)) && isBlur;
			}
			set
			{
				eUIParam p = (eUIParam)mParam;
				bool isTop = (p & eUIParam.Top) == eUIParam.Top;
				bool isBlur = (p & eUIParam.Blur) == eUIParam.Blur;
				if (!isTop)
				{
					Vector3 vPos = transform.localPosition;
					Vector3 vRot = transform.localEulerAngles;
					Vector3 vSca = transform.localScale;

					transform.parent = mMgr.GetParent(eUIParam.Top);
					transform.localPosition = vPos;
					transform.localEulerAngles = vRot;
					transform.localScale = vSca;
				}
				mMgr.Blur.enabled = value;
			}
		}
#endif
#if _MASK
		public bool Mask
		{
			get
			{
				return mMask != null && mMask.activeSelf;
			}
			set
			{
				if (mMask != null)
					mMask.SetActive(value);
			}
		}
#endif

        public override string ToString()
        {
            return string.Format("{0}  Hidden={1}  Min={2}  Max={3}  Offset={4}", mUIName, !gameObject.activeSelf, mMinDepth, mMaxDepth, mOffsetDepth);
        }

		public virtual int SortUI(int depth)
        {
#if _BLUR
			Blur = false;
#endif
#if _MASK
			Mask = false;
#endif

			mMinDepth = depth + 1;
			mMaxDepth = depth + 1;

			SetParent(mParent, false);
            SetDepth(gameObject);

            return mMaxDepth;
        }

        //設置圖層深度
		public virtual void SetDepth(GameObject obj)
        {
			Mask = true;
			UIPanel[] panels = obj.GetComponentsInChildren<UIPanel>();
			if (panels != null)
				panels.ForEach(p => p.depth = mOffsetDepth + mMaxDepth++);
			Mask = false;
        }

        //設置事件遮罩
		public virtual void SetEventMask(GameObject obj)
        {
			if (obj == null)
                return;
			
			BoxCollider collider = obj.GetComponent<BoxCollider>();
			if (collider == null)
				collider = obj.AddComponent<BoxCollider>();
			collider.size = new Vector3(mMgr.RealWidth, mMgr.RealHeight, 0);
        }

        public void SetParent(Transform parent, bool isIdentity = true)
        {
			mParent = parent;

			if (isIdentity)
            {
                transform
					.Parent(parent != null ? parent : null)
                    .LocalIdentity();
            }
            else
            {
                Vector3 pos = transform.localPosition;
                Vector3 rot = transform.localEulerAngles;
                Vector3 sca = transform.localScale;

				transform.parent = parent != null ? parent : null;
				transform.localPosition = pos;
				transform.localEulerAngles = rot;
				transform.localScale = sca;
            }
        }

        //初始化
        public void Init(UIMgr mgr, int ui, string uiname, Transform parent, int param, int depth, int rank)
        {
			mMgr = mgr;
			mUI = ui;
			mUIName = uiname;
			mMinDepth = depth;
			mMaxDepth = depth;
			mOffsetDepth = depth;
			mParam = param;
			mRank = rank;
#if _MASK
			mMask = null;
#endif
			mBGAudio = null;

			SetParent(parent);
        }

		public virtual void Open(params object[] param)
        {
			//TODO:子類別實作
        }

		public virtual void Close(params object[] param)
        {
			//TODO:子類別實作
        }

        //開啟介面
        public void OpenUI(params object[] param)
        {
			CoroutineMgr.Instance.StartCoroutine(IBuild(param));
        }

        //開啟介面
        public IEnumerator IOpenUI(params object[] param)
        {
			yield return CoroutineMgr.Instance.StartCoroutine(IBuild(param));
        }

        //關閉介面
        public virtual void CloseUI(params object[] param)
        {
			mParent = null;
#if _MASK
			mMask = null;
#endif
			if (mBGAudio != null)
			{
				mBGAudio.Dispose();
				mBGAudio = null;
			}

            this.DestroyGameObjGracefully();
        }

		//建立介面
		private IEnumerator IBuild(params object[] param)
		{
			yield return CoroutineMgr.Instance.StartCoroutine(IBuildUIBefore(param));
			yield return CoroutineMgr.Instance.StartCoroutine(IBuildUI(param));
			yield return CoroutineMgr.Instance.StartCoroutine(IBuildUIAfter(param));
		}

		//建立介面
		protected virtual IEnumerator IBuildUIBefore(params object[] param)
		{
			mIsLoading = true;
			mMgr.ShowLoading(gameObject);
#if _MASK
			eUIParam p = (eUIParam)mParam;
			if ((p & eUIParam.Mask) == eUIParam.Mask)
				yield return CoroutineMgr.Instance.StartCoroutine(IBuildMask());
#else
			yield return null;
#endif
		}

		//建立介面
		protected virtual IEnumerator IBuildUIAfter(params object[] param)
		{
			yield return null;

			mMgr.HideLoading(gameObject);
			mMgr.PushUI(this, mParam);
			mMgr.SortUI();
			mIsLoading = false;
		}

        //建立介面
        protected abstract IEnumerator IBuildUI(params object[] param);

#if _MASK
		//建立遮罩
		protected IEnumerator IBuildMask()
		{
			TObject obj = new TObject();
			string path = Application.streamingAssetsPath + "/" + TinyContext.Instance.LanguagePath + "/Main/Common/UIs/Panels/";
			AssetBundle bundle = mMgr.GetStandardAtlas("Panel_Mask-Atlas");
			if (bundle == null)
			{
				yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(path, "Panel_Mask-Atlas", obj));
				if (obj.Err != null)
				{
					mLogger.Log(obj.Err);
				}
				else
				{
					GameObject mask = obj.Bundle.assetBundle.mainAsset as GameObject;
					UIAtlas atlas = mask.GetComponent<UIAtlas>();
					mMgr.PushStandardAtlas("Panel_Mask-Atlas", obj.Bundle.assetBundle);
				}
			}

			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(path, "Panel_Mask", obj));
			if (obj.Err != null)
			{
				mLogger.Log(obj.Err);
			}
			else
			{
				mMask = GameObject.Instantiate(obj.Bundle.assetBundle.mainAsset) as GameObject;
				mMask.name = "Panel_Mask";

				//掛載
				if (mMask != null)
					AddToBone(mMask);

				obj.Bundle.assetBundle.Unload(false);
			}
			obj.Free();
		}
#endif

		protected IEnumerator IBuildMusic(string path, string fileName)
		{
			TObject obj = new TObject();
			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadMP3(path, fileName, obj));
			if (obj.Err != null)
			{
				mLogger.Log(obj.Err);
			}
			else
			{
				mBGAudio = AudioMgr.Instance.Add(gameObject, eAudio.MUSIC);
				mBGAudio.clip = obj.Bundle.GetAudioClip(false, false, AudioType.MPEG);
				mBGAudio.playOnAwake = false;
				mBGAudio.loop = true;
				mBGAudio.Play();
			}
			obj.Free();
		}

        //搜尋子物件
		protected GameObject FindChild(string path)
        {
			if (path == null)
                return null;

			if (path == "")
                return gameObject;

			Transform target = transform.Find(path);

			return target == null ? null : target.gameObject;
        }

        //掛載節點
		protected bool AddToBone(GameObject obj, string path = null)
        {
			if (obj == null)
                return false;

			Vector3 pos = obj.transform.localPosition;
			Vector3 rot = obj.transform.localEulerAngles;
			Vector3 sca = obj.transform.localScale;

			if (path == null)
            {
				obj.transform.parent = transform;
            }
            else
            {
				GameObject target = FindChild(path);
				if (target == null)
                    return false;
				obj.transform.parent = target.transform;
            }

			obj.transform.localPosition = pos;
			obj.transform.localEulerAngles = rot;
			obj.transform.localScale = sca;
            return true;
        }

        //掛載節點
		protected bool AddToBone(GameObject obj, Transform parent)
        {
			if ((obj == null) || (parent == null))
                return false;

			Vector3 pos = obj.transform.localPosition;
			Vector3 rot = obj.transform.localEulerAngles;
			Vector3 sca = obj.transform.localScale;

			obj.transform.parent = parent;
			obj.transform.localPosition = pos;
			obj.transform.localEulerAngles = rot;
			obj.transform.localScale = sca;
            return true;
        }

        //移除節點
		protected bool DelToBone(GameObject obj)
        {
			if (obj == null)
                return false;

			obj.transform.parent = null;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localEulerAngles = Vector3.zero;
			obj.transform.localScale = Vector3.one;
            return true;
        }
    }
}