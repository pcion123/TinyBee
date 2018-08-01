namespace TinyBee.UI
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class UILoot : UIForm
    {
        private GameObject mRoot = null;

        public float PixelSize
        {
            get
            {
                return mRoot == null ? 0 : mRoot.transform.localScale.x;
            }
        }

        public override void CloseUI(params object[] param)
        {
            mRoot = null;

            base.CloseUI(param);
        }

		//建立介面
		protected override IEnumerator IBuildUIBefore(params object[] param)
		{
			yield return null;
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
            mIsLoading = true;

            TObject vObj = new TObject();

            AssetBundle vBundle1 = mMgr.PopFont("Word_System01");
			if (vBundle1 == null)
            {
                string xPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/Words/";
                yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(xPath, "Word_System01", vObj));
                if (vObj.Err != null)
                {
                    mLogger.Log(vObj.Err);
                }
                else
                {
                    UIFont vFont = (vObj.Bundle.assetBundle.mainAsset as GameObject).GetComponent<UIFont>();
                    mMgr.PushFont("Word_System01", vObj.Bundle.assetBundle);
                }
            }

			AssetBundle vBundle2 = mMgr.GetStandardAtlas("Panel_Mask-Atlas");
			if (vBundle2 == null)
			{
				string xPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";
				yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(xPath, "Panel_Mask-Atlas", vObj));
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

            string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Items/";

            yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Item_Root", vObj));

            if (vObj.Err != null)
            {
                mLogger.Log(vObj.Err);
            }
            else
            {
                mRoot = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
                mRoot.name = "root";

                AddToBone(mRoot);

                UIRoot vUIRoot = mRoot.GetComponent<UIRoot>();

                CalculateScreenSize(vUIRoot, (eDirection)param[0]);

                //設定MainCamera
                mMgr.MainCamera = FindChild("root/MainCamera").GetComponent<Camera>();
                mMgr.MainCamera.depth = 5;

                //設定TopCamera
                mMgr.TopCamera = FindChild("root/TopCamera").GetComponent<Camera>();
                mMgr.TopCamera.depth = 10;
                mMgr.TopCamera.transform.localPosition = new Vector3(-100000, 0, 0);

                //設定左上定位點參數
                mMgr.PL = new GameObject("PL").transform;
                mMgr.PL.parent = mMgr.MainCamera.transform;
                mMgr.PL.localScale = Vector3.one;
                UIAnchor vL = mMgr.PL.gameObject.AddComponent<UIAnchor>();
                vL.uiCamera = mMgr.MainCamera;
                vL.side = UIAnchor.Side.TopLeft;

                //設定右下定位點參數
                mMgr.PR = new GameObject("PR").transform;
                mMgr.PR.parent = mMgr.MainCamera.transform;
                mMgr.PR.localScale = Vector3.one;
                UIAnchor vR = mMgr.PR.gameObject.AddComponent<UIAnchor>();
                vR.uiCamera = mMgr.MainCamera;
                vR.side = UIAnchor.Side.BottomRight;

#if UNITY_EDITOR
                vL.runOnlyOnce = false;
                vR.runOnlyOnce = false;
#endif

                //設置TopCamera參數
                UIViewport vUIViewport = mMgr.TopCamera.gameObject.AddComponent<UIViewport>();
                vUIViewport.sourceCamera = mMgr.MainCamera;
                vUIViewport.bottomRight = mMgr.PR;
                vUIViewport.topLeft = mMgr.PL;
            }
            vObj.Bundle.assetBundle.Unload(false);
            vObj.Free();

#if _BLUR
			if (UIMgr.Instance.BlurCamera == null)
			{
				GameObject vCam = new GameObject("BlurCamera");
				vCam.transform.parent = mRoot.transform;
				vCam.transform.localPosition = Vector3.zero;
				vCam.transform.localEulerAngles = Vector3.zero;
				vCam.transform.localScale = Vector3.one;

				UIMgr.Instance.BlurCamera = vCam.AddComponent<Camera>();
				UIMgr.Instance.BlurCamera.clearFlags = CameraClearFlags.Depth;
				UIMgr.Instance.BlurCamera.orthographic = true;
				UIMgr.Instance.BlurCamera.depth = 9;
				UIMgr.Instance.BlurCamera.nearClipPlane = 1;
				UIMgr.Instance.BlurCamera.farClipPlane = 2;

				UIViewport vUIViewport = vCam.AddComponent<UIViewport>();
				vUIViewport.sourceCamera = UIMgr.Instance.MainCamera;
				vUIViewport.bottomRight = UIMgr.Instance.PR;
				vUIViewport.topLeft = UIMgr.Instance.PL;

				UIMgr.Instance.Blur = vCam.AddComponent<Blur>();
				UIMgr.Instance.Blur.iterations = 2;
			}
#endif

            //阻止機器進入休眠
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            //鎖定畫面更新頻率
            Application.targetFrameRate = 30;

#if UNITY_EDITOR
            QualitySettings.vSyncCount = 2;
#endif

            //開啟UI_Loading -> 建立UI_Loading
            mMgr.OpenUI(UIEnumBase.Loading);

            //開啟UI_Connecting -> 建立UI_Connecting
            mMgr.OpenUI(UIEnumBase.Connecting);

            //開啟UI_Bubble -> 建立UI_Bubble
            mMgr.OpenUI(UIEnumBase.Bubble);

            mIsLoading = false;
        }

		public override int SortUI(int vDepth)
		{
			mMinDepth = vDepth + 1;
			mMaxDepth = vDepth + 1;

			SetParent(mParent, false);

			return mMaxDepth;
		}

        private void CalculateScreenSize(UIRoot vUIRoot, eDirection vDirection)
        {
            Vector2 vScreen = NGUITools.screenSize;
            float vAspect = vScreen.x / vScreen.y;

            vUIRoot.scalingStyle = UIRoot.Scaling.Constrained;
            vUIRoot.fitHeight = vAspect < 1.5f ? false : true;
            vUIRoot.fitWidth = vAspect < 1.5f ? true : false;
            vUIRoot.manualHeight = vDirection == eDirection.Vertical ? 960 : 640;
            vUIRoot.manualWidth = vDirection == eDirection.Vertical ? 640 : 960;

            float vRatio = (float)vUIRoot.activeHeight / (float)Screen.height;
            mMgr.Ratio = vRatio;
            mMgr.RealWidth = Mathf.Ceil(Screen.width * vRatio);
            mMgr.RealHeight = Mathf.Ceil(Screen.height * vRatio);
        }
    }
}
