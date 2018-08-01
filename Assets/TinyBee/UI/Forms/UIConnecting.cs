namespace TinyBee.UI
{
    using UnityEngine;
    using System.Collections;

    public class UIConnecting : UIForm
    {
        private GameObject mNet = null;
        private UILabel mText = null;
        private string mContent = string.Empty;

        public string Content { get { return mContent; } set { SetContent(value); } }

        private void SetContent(string vContent)
        {
            mContent = vContent;

            if (mText == null)
                return;

            mText.text = mContent;

            int vX = (int)mText.transform.localPosition.x;
            int vWidth = mText.width;
        }

        public override void CloseUI(params object[] param)
        {
            mNet = null;

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
			mMgr.HideNet();
		}

        protected override IEnumerator IBuildUI(params object[] param)
        {
            TObject vObj = new TObject();

            string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";

            yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Net", vObj));
            if (vObj.Err != null)
            {
                mLogger.Log(vObj.Err);
            }
            else
            {
                mNet = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
                mNet.name = "Panel_Net";

                AddToBone(mNet);

                mText = FindChild("Panel_Net/R/Text").GetComponent<UILabel>();

                vObj.Bundle.assetBundle.Unload(false);
            }
            vObj.Free();

            //設置事件遮罩
            SetEventMask(mNet);

            //設置顯示深度
            SetDepth(gameObject);
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

            if (mText != null)
            {
                int vMod = System.DateTime.Now.Second % 3;

                if (vMod == 0)
                    mText.text = mContent + ".  ";
                else if (vMod == 1)
                    mText.text = mContent + ".. ";
                else if (vMod == 2)
                    mText.text = mContent + "...";

                int vX = (int)mText.transform.localPosition.x;
                int vWidth = mText.width;
            }
        }
    }
}
