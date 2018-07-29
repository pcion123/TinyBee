namespace TinyBee.UI
{
    using UnityEngine;
    using System.Collections;

    public class UIBubble : UIForm
    {
        private UILabel mLabel = null;

        public override void CloseUI(params object[] param)
        {
            mLabel = null;

            base.CloseUI(param);
        }

        protected override IEnumerator IBuildUI(params object[] param)
        {
            TObject vObj = new TObject();

            string vPath = Application.streamingAssetsPath + "/" + GameMgr.Instance.LanguagePath + "/Common/UIs/Panels/";

            yield return CoroutineMgr.Instance.StartCoroutine(DataMgr.Instance.ILoadBundle(vPath, "Panel_Bubble", vObj));
            if (vObj.Err != null)
            {
                mLogger.Log(vObj.Err);
            }
            else
            {
                GameObject vBubble = GameObject.Instantiate(vObj.Bundle.assetBundle.mainAsset) as GameObject;
                vBubble.name = "Panel_Bubble";

                AddToBone(vBubble);

                mLabel = FindChild("Panel_Bubble/Label").GetComponent<UILabel>();

                vObj.Bundle.assetBundle.Unload(false);
            }
            vObj.Free();

            //設置顯示深度
            SetDepth(gameObject);

            mMgr.HideUI(UIEnumBase.Connecting);
        }

        public void Msgs(Vector3 vStartPos, Vector3 vEndPos, string vMsg, float vSpeed, float vDelay)
        {
            GameObject vBubble = GameObject.Instantiate(mLabel.gameObject) as GameObject;
            AddToBone(vBubble, "Panel_Bubble");
            Bubble.Begin(vBubble, vStartPos, vEndPos, vMsg, vSpeed, vDelay);
        }

        public void Msgs(string vMsg, float vSpeed, float vDelay)
        {
            GameObject vBubble = GameObject.Instantiate(mLabel.gameObject) as GameObject;
            AddToBone(vBubble, "Panel_Bubble");
            Bubble.Begin(vBubble, new Vector3(0f, 170f, 0f), new Vector3(0f, 250f, 0f), vMsg, vSpeed, vDelay);
        }

        public static void Msg(Vector3 vStartPos, Vector3 vEndPos, string vMsg, float vSpeed, float vDelay)
        {
            UIBubble vBubble = (UIBubble)UIMgr.Instance.GetUI(UIEnumBase.Bubble);
            if (vBubble != null)
                vBubble.Msgs(vStartPos, vEndPos, vMsg, vSpeed, vDelay);
        }

        public static void Msg(string vMsg, float vSpeed, float vDelay)
        {
            UIBubble vBubble = (UIBubble)UIMgr.Instance.GetUI(UIEnumBase.Bubble);
            if (vBubble != null)
                vBubble.Msgs(vMsg, vSpeed, vDelay);
        }
    }
}
