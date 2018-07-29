namespace TinyBee.UI
{
    using UnityEngine;
    using System.Collections;
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
        protected int mUIEnum = 0;                    //UI類型
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

        public int UIEnum { get { return mUIEnum; } }
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
            return string.Format("{0}  IsHidden={1}  Min={2}  Max={3}  Offset={4}  Rank={5}", mUIName, !gameObject.activeSelf, mMinDepth, mMaxDepth, mOffsetDepth, mRank);
        }

        void OnDestroy()
        {

        }

        private bool CheckNeedBlur()
        {
            return false;
        }

        private bool CheckNeedMask()
        {
            return false;
        }

		public virtual int SortUI(int vDepth)
        {
#if _BLUR
			Blur = false;
#endif
#if _MASK
			Mask = false;
#endif

            mMinDepth = vDepth + 1;
            mMaxDepth = vDepth + 1;

			SetParent(mParent, false);
            SetDepth(gameObject);

            return mMaxDepth;
        }

        //設置圖層深度
        public void SetDepth(GameObject vObj)
        {
			Mask = true;
            UIPanel[] vPanels = vObj.GetComponentsInChildren<UIPanel>();
            if (vPanels != null)
                vPanels.ForEach(vPanel => vPanel.depth = mOffsetDepth + mMaxDepth++);
			Mask = false;
        }

        //設置事件遮罩
        public virtual void SetEventMask(GameObject vObj)
        {
            if (vObj == null)
                return;

            //取得Collider
            BoxCollider vCollider = vObj.GetComponent<BoxCollider>();

            if (vCollider == null)
                vCollider = vObj.AddComponent<BoxCollider>();

            vCollider.size = new Vector3(mMgr.RealWidth, mMgr.RealHeight, 0);
        }

        public void SetParent(Transform vParent, bool vIsIdentity = true)
        {
			mParent = vParent;

            if (vIsIdentity)
            {
                transform
                    .Parent(vParent != null ? vParent : null)
                    .LocalIdentity();
            }
            else
            {
                Vector3 vPos = transform.localPosition;
                Vector3 vRot = transform.localEulerAngles;
                Vector3 vSca = transform.localScale;

                transform.parent = vParent != null ? vParent : null;
                transform.localPosition = vPos;
                transform.localEulerAngles = vRot;
                transform.localScale = vSca;
            }
        }

        //初始化
        public void Init(UIMgr vMgr, int vUIEnum, string vUIName, Transform vParent, int vParam, int vDepth, int vRank)
        {
            mMgr = vMgr;
            mUIEnum = vUIEnum;
            mUIName = vUIName;
            mMinDepth = vDepth;
            mMaxDepth = vDepth;
            mOffsetDepth = vDepth;
            mParam = vParam;
            mRank = vRank;
#if _MASK
			mMask = null;
#endif

            SetParent(vParent);
        }

        public virtual void Open()
        {
			//TODO:子類別實作
        }

        public virtual void Close()
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
//#if _SEPARATE
//            Camera[] vCameras = transform.GetComponentsInChildren<Camera>();

//            if (vCameras != null)
//            {
//                for (int i = 0; i < vCameras.Length; i++)
//                {
//                    DelToCam(vCameras[i], 1);
//                    DelToCam(vCameras[i], 2);
//                }
//            }

//            mUIMgr.IsActive = false;
//#endif

			mParent = null;

#if _MASK
			mMask = null;
#endif

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
		}

        //建立介面
        protected abstract IEnumerator IBuildUI(params object[] param);

#if _MASK
		//建立遮罩
		protected IEnumerator IBuildMask()
		{
			TObject vObj = new TObject();
			string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";
			AssetBundle vBundle = mMgr.GetStandardAtlas("Panel_Mask-Atlas");
			if (vBundle == null)
			{
				yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Mask-Atlas", vObj));
				if (vObj.Err != null)
				{
					mLogger.Log(vObj.Err);
				}
				else
				{
					GameObject vMask = vObj.Bundle.assetBundle.mainAsset as GameObject;
					UIAtlas vAtlas = vMask.GetComponent<UIAtlas>();
					mMgr.PushStandardAtlas("Panel_Mask-Atlas", vObj.Bundle.assetBundle);
				}
			}

			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Mask", vObj));

			if (vObj.Err != null)
			{
				mLogger.Log(vObj.Err);
			}
			else
			{
				mMask = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
				mMask.name = "Panel_Mask";

				//掛載
				if (mMask != null)
					AddToBone(mMask);

				vObj.Bundle.assetBundle.Unload(false);
			}
			vObj.Free();
		}
#endif

        //搜尋子物件
		protected GameObject FindChild(string vPath)
        {
            if (vPath == null)
                return null;

            if (vPath == "")
                return gameObject;

            Transform vTarget = transform.Find(vPath);

            return vTarget == null ? null : vTarget.gameObject;
        }

        //#if _SEPARATE
        //        //加入攝影機
        //        public bool AddToCam(Camera vCamera, int vDisplay)
        //        {
        //            if (vCamera == null)
        //                return false;

        //            mMgr.AddCam(vCamera, vDisplay);

        //            return true;
        //        }

        //        //移除攝影機
        //        public bool DelToCam(Camera vCamera, int vDisplay)
        //        {
        //            if (vCamera == null)
        //                return false;

        //            mMgr.DelCam(vCamera, vDisplay);

        //            return true;
        //        }
        //#endif

        //掛載節點
		protected bool AddToBone(GameObject vObject, string vPath = null)
        {
            if (vObject == null)
                return false;

            Vector3 vPos = vObject.transform.localPosition;
            Vector3 vRot = vObject.transform.localEulerAngles;
            Vector3 vSca = vObject.transform.localScale;

            if (vPath == null)
            {
                vObject.transform.parent = transform;
            }
            else
            {
                GameObject vTarget = FindChild(vPath);

                if (vTarget == null)
                    return false;

                vObject.transform.parent = vTarget.transform;
            }

            vObject.transform.localPosition = vPos;
            vObject.transform.localEulerAngles = vRot;
            vObject.transform.localScale = vSca;

            return true;
        }

        //掛載節點
		protected bool AddToBone(GameObject vObject, Transform vParent)
        {
            if ((vObject == null) || (vParent == null))
                return false;

            Vector3 vPos = vObject.transform.localPosition;
            Vector3 vRot = vObject.transform.localEulerAngles;
            Vector3 vSca = vObject.transform.localScale;

            vObject.transform.parent = vParent;
            vObject.transform.localPosition = vPos;
            vObject.transform.localEulerAngles = vRot;
            vObject.transform.localScale = vSca;

            return true;
        }

        //移除節點
		protected bool DelToBone(GameObject vObject)
        {
            if (vObject == null)
                return false;

            vObject.transform.parent = null;
            vObject.transform.localPosition = Vector3.zero;
            vObject.transform.localEulerAngles = Vector3.zero;
            vObject.transform.localScale = Vector3.one;

            return true;
        }
    }
}