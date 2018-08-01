namespace TinyBee.UI
{
	using UnityEngine;
	using System.Collections;
	using System;

	public class UIController : UIForm
	{
		protected override IEnumerator IBuildUI(params object[] param)
		{
			TObject vObj = new TObject();

			string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";

			AssetBundle vBundle = mMgr.GetStandardAtlas("Panel_Controller-Atlas");
			if (vBundle == null)
			{
				yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Controller-Atlas", vObj));
				if (vObj.Err != null)
				{
					mLogger.Log(vObj.Err);
				}
				else
				{
					GameObject vController = vObj.Bundle.assetBundle.mainAsset as GameObject;
					UIAtlas vAtlas = vController.GetComponent<UIAtlas>();
					mMgr.PushStandardAtlas("Panel_Controller-Atlas", vObj.Bundle.assetBundle);
				}
			}

			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Controller", vObj));
			if (vObj.Err != null)
			{
				mLogger.Log(vObj.Err);
			}
			else
			{
				GameObject vController = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
				vController.name = "Panel_Controller";

				AddToBone(vController);

				vObj.Bundle.assetBundle.Unload(false);
			}
			vObj.Free();

			//設置顯示深度
			SetDepth(gameObject);
		}
	}
}
