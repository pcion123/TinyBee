namespace TinyBee.UI
{
	using UnityEngine;
	using System.Collections;
	using System;

	public class UITutorial : UIForm
	{
		protected override IEnumerator IBuildUI(params object[] param)
		{
			TObject vObj = new TObject();

			string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";

			AssetBundle vBundle = mMgr.GetStandardAtlas("Panel_Tutorial-Atlas");
			if (vBundle == null)
			{
				yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Tutorial-Atlas", vObj));
				if (vObj.Err != null)
				{
					mLogger.Log(vObj.Err);
				}
				else
				{
					GameObject vTutorial = vObj.Bundle.assetBundle.mainAsset as GameObject;
					UIAtlas vAtlas = vTutorial.GetComponent<UIAtlas>();
					mMgr.PushStandardAtlas("Panel_Tutorial-Atlas", vObj.Bundle.assetBundle);
				}
			}

			yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Tutorial", vObj));
			if (vObj.Err != null)
			{
				mLogger.Log(vObj.Err);
			}
			else
			{
				GameObject vTutorial = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
				vTutorial.name = "Panel_Tutorial";

				AddToBone(vTutorial);

				vObj.Bundle.assetBundle.Unload(false);
			}
			vObj.Free();

			//設置顯示深度
			SetDepth(gameObject);
		}
	}
}
