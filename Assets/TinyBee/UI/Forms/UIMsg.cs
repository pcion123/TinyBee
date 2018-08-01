namespace TinyBee.UI
{
	using UnityEngine;
	using System.Collections;
	using System;

	public class UIMsg : UIForm
	{
		private GameObject mMsg = null;

		public override void CloseUI(params object[] param)
		{
			mMsg = null;

			base.CloseUI(param);
		}

		protected override IEnumerator IBuildUI(params object[] param)
		{
			TObject vObj = new TObject();

			string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";

			AssetBundle vBundle = mMgr.GetStandardAtlas("Panel_Msg-Atlas");
			if (vBundle == null)
			{
				yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Msg-Atlas", vObj));
				if (vObj.Err != null)
				{
					mLogger.Log(vObj.Err);
				}
				else
				{
					GameObject vMsg = vObj.Bundle.assetBundle.mainAsset as GameObject;
					UIAtlas vAtlas = vMsg.GetComponent<UIAtlas>();
					mMgr.PushStandardAtlas("Panel_Msg-Atlas", vObj.Bundle.assetBundle);
				}
			}

			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Msg", vObj));
			if (vObj.Err != null)
			{
				mLogger.Log(vObj.Err);
			}
			else
			{
				mMsg = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
				mMsg.name = "Panel_Msg";

				AddToBone(mMsg);

				UISprite vBackground = FindChild("Panel_Msg/Background").GetComponent<UISprite>();
				UILabel vTitle = FindChild("Panel_Msg/Title").GetComponent<UILabel>();
				UILabel vText = FindChild("Panel_Msg/Text").GetComponent<UILabel>();
				GameObject zTitle = FindChild("Panel_Msg/Title");
				GameObject vButton_Yes = FindChild("Panel_Msg/Button_Yes");
				GameObject vButton_No = FindChild("Panel_Msg/Button_No");

				vTitle.text = (string)param[0];
				vText.text = (string)param[1];

				if ((bool)param[2] == false)
				{
					vButton_Yes.transform.localPosition = new Vector3(0f, -87.3f, 0f);
					vButton_No.transform.localPosition = new Vector3(0f, -87.3f, 0f);
					vButton_No.SetActive(false);
				}

				if (string.IsNullOrEmpty((string)param[0]))
					zTitle.SetActive(false);

				UIEvent.Register(vButton_Yes, eEventParam.Click, null, new object[] {param[3] == null ? (o) => {mMgr.CloseUI(mUIEnum);} : (Action<object>)param[3]});
				UIEvent.Register(vButton_No, eEventParam.Click, null, new object[] {param[4] == null ? (o) => {mMgr.CloseUI(mUIEnum);} : (Action<object>)param[4]});

				AdjustScale(vBackground, null, vText, zTitle, vButton_Yes, vButton_No);

				vObj.Bundle.assetBundle.Unload(false);
			}
			vObj.Free();

			//設置事件遮罩
			SetEventMask(mMsg);

			//設置顯示深度
			SetDepth(gameObject);
		}

		private void AdjustScale (UISprite vBackground, UISprite vFrame, UILabel vText, GameObject vTitle, GameObject vYes, GameObject vNo)
		{
			//Y軸Size
			float vY = NGUIMath.CalculateRelativeWidgetBounds(vText.transform).size.y;

			int vOffset = 10;

			if (vOffset <= vY)
				vOffset = (int)(vY - vOffset);
			else
				vOffset = 0;

			//設定背景大小
			if (vBackground != null)
				vBackground.height = vBackground.height + vOffset;

			//設定外框大小
			if (vFrame != null)
				vFrame.height = vFrame.height + vOffset;

			//設定標題位置
			if (vTitle != null)
				vTitle.transform.localPosition = vTitle.transform.localPosition + new Vector3(0, vOffset / 2, 0);

			//設定按鈕位置
			if (vYes != null)
				vYes.transform.localPosition = vYes.transform.localPosition - new Vector3(0, vOffset / 2, 0);

			//設定按鈕位置
			if (vNo != null)
				vNo.transform.localPosition = vNo.transform.localPosition - new Vector3(0, vOffset / 2, 0);
		}
	}
}
