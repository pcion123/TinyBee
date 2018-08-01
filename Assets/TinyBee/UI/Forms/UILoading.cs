namespace TinyBee.UI
{
    using UnityEngine;
    using System.Collections;

    public class UILoading : UIForm
    {
        private GameObject mLoading = null;
        private Transform mContainer = null;
        private UILabel mText = null;
        private float mRadius = 10f;
        private float mSpeed = 4f;
        private float mTmpAngle = 0f;
        private Vector3 mPos = Vector3.zero;

        public override void CloseUI(params object[] param)
        {
            mLoading = null;

            base.CloseUI(param);
        }

		//建立介面
		protected override IEnumerator IBuildUIBefore(params object[] param)
		{
#if _MASK
			eUIParam p = (eUIParam)mParam;
			if ((p & eUIParam.Mask) == eUIParam.Mask)
				yield return CoroutineMgr.Instance.StartCoroutine(IBuildMask());
#else
			yield return null;
#endif
		}

		//建立介面
		protected override IEnumerator IBuildUIAfter(params object[] param)
		{
			yield return null;

			mMgr.PushUI(this, mParam);
			mMgr.SortUI();
		}

        protected override IEnumerator IBuildUI(params object[] param)
        {
            TObject vObj = new TObject();

            string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";

			AssetBundle vBundle = mMgr.GetStandardAtlas("Panel_Loading-Atlas");
            if (vBundle == null)
            {
                yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Loading-Atlas", vObj));
                if (vObj.Err != null)
                {
                    mLogger.Log(vObj.Err);
                }
                else
                {
                    GameObject vLoading = vObj.Bundle.assetBundle.mainAsset as GameObject;
                    UIAtlas vAtlas = vLoading.GetComponent<UIAtlas>();
                    mMgr.PushStandardAtlas("Panel_Loading-Atlas", vObj.Bundle.assetBundle);
                }
            }

            yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Loading", vObj));
            if (vObj.Err != null)
            {
                mLogger.Log(vObj.Err);
            }
            else
            {
                mLoading = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
                mLoading.name = "Panel_Loading";

                AddToBone(mLoading);

                mContainer = FindChild("Panel_Loading/Container").transform;
                mText = FindChild("Panel_Loading/Label").GetComponent<UILabel>();

                vObj.Bundle.assetBundle.Unload(false);
            }
            vObj.Free();

            //設置事件遮罩
            SetEventMask(mLoading);

            //設置顯示深度
            SetDepth(gameObject);

            mMgr.HideUI(UIEnumBase.Loading);
        }

		public override void Open()
		{
			Hidden = false;
		}

		public override void Close()
		{
			Hidden = true;
		}

		public override int SortUI(int vDepth)
		{
#if _MASK
			Mask = true;
#endif

			mMinDepth = vDepth + 1;
			mMaxDepth = vDepth + 1;

			SetParent(mMgr.GetParent(mParam), false);
			SetDepth(gameObject);

			return mMaxDepth;
		}

        private void Update()
        {
            if (Hidden)
                return;

            if (mContainer != null)
            {
                mTmpAngle += mSpeed * Time.deltaTime;

                mPos.x = Mathf.Cos(mTmpAngle) * mRadius;
                mPos.y = Mathf.Sin(mTmpAngle) * mRadius;
                mPos.z = mContainer.localPosition.z;

                mContainer.localPosition = mPos;
            }

            if (mText != null)
            {
                int vMod = System.DateTime.Now.Second % 3;

                if (vMod == 0)
                    mText.text = "Loading.  ";
                else if (vMod == 1)
                    mText.text = "Loading.. ";
                else if (vMod == 2)
                    mText.text = "Loading...";

                int vX = (int)mText.transform.localPosition.x;
                int vWidth = mText.width;
            }
        }
    }
}
