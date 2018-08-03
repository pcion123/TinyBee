namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
    using System;
    using UI;
	using UI.Enum;
    using UnityEngine;

    public enum eUIParam
    {
        None = 0,
        Standard = 1,
        Main = 2,
        Top = 4,
        Blur = 8,
        Mask = 16
    }

    [TMonoSingletonPath("[UI]/UIMgr")]
    public class UIMgr : TMgrBehaviour, ISingleton
    {
		public static string[] CAMERAKEYS = new string[] {"Main", "Top", "Else"};

        private List<TUIInfo> mInfoList = null;                   //
		[SerializeField]
        private List<UIForm> mForms = null;                       //
        private List<UIForm> mStandards = null;                   //
        private TStack<UIForm> mMainStacks = null;                //
        private TStack<UIForm> mTopStacks = null;                 //
#if _BLUR
		private TStack<UIForm> mBlurStacks = null;                //
#endif
#if _MASK
		private TStack<UIForm> mMaskStacks = null;                //
#endif
        private Dictionary<string, AssetBundle> mFontMap = null;          //
        private Dictionary<string, AssetBundle> mStandardAtlasMap = null; //
        private Dictionary<string, AssetBundle> mAtlasMap = null;         //
		private Dictionary<string, Camera> mCameraMap = null;

        public int FormCount { get { return mForms.Count; } }
        public int FontCount { get { return mFontMap.Count; } }
        public int StandardAtlasCount { get { return mStandardAtlasMap.Count; } }
        public int AtlasCount { get { return mAtlasMap.Count; } }
#if _BLUR
		public Blur Blur { get; set; }
#endif
		public Material Skybox { get; private set;}

        public float Ratio { get; set; }
        public float RealHeight { get; set; }
        public float RealWidth { get; set; }
        public Transform PL { get; set; }
        public Transform PR { get; set; }

        public static UIMgr Instance
		{
			get { return MonoSingletonProperty<UIMgr>.Instance; }
		}

		public override int ManagerId
		{
			get { return MgrEnumBase.UI; }
		}

		public void OnSingletonInit() {}

		public override void Init(params object[] param)
		{
			Skybox = RenderSettings.skybox;

            mInfoList = new List<TUIInfo>()
            {
                new TUIInfo(UIEnumBase.None, "UI_None", null, eUIParam.None, 0, 0),
				new TUIInfo(UIEnumBase.Root, "UI_Root", typeof(UILoot), eUIParam.Standard, 0, 0),
                new TUIInfo(UIEnumBase.Loading, "UI_Loading", typeof(UILoading), eUIParam.Standard|eUIParam.Top|eUIParam.Mask|eUIParam.Blur, 900, 0),
                new TUIInfo(UIEnumBase.Connecting, "UI_Connecting", typeof(UIConnecting), eUIParam.Standard|eUIParam.Top|eUIParam.Mask|eUIParam.Blur, 800, 0),
                new TUIInfo(UIEnumBase.Bubble, "UI_Bubble", typeof(UIBubble), eUIParam.Standard|eUIParam.Top, 700, 0),
				new TUIInfo(UIEnumBase.Msg, "UI_Msg", typeof(UIMsg), eUIParam.Main|eUIParam.Mask|eUIParam.Blur, 0, 3),
				new TUIInfo(UIEnumBase.Controller, "UI_Controller", typeof(UIController), eUIParam.Standard|eUIParam.Top, 1000, 0),
				new TUIInfo(UIEnumBase.Tutorial, "UI_Tutorial", typeof(UITutorial), eUIParam.Standard|eUIParam.Top, 500, 1)
            };

			if (param != null)
				param.ForEach(p => mInfoList.Add((TUIInfo)p));

            mForms = new List<UIForm>();
            mStandards = new List<UIForm>();
            mMainStacks = new TStack<UIForm>();
            mTopStacks = new TStack<UIForm>();
#if _BLUR
			mBlurStacks = new TStack<UIForm>();
#endif
#if _MASK
			mMaskStacks = new TStack<UIForm>();
#endif
            mFontMap = new Dictionary<string,AssetBundle>();
            mStandardAtlasMap = new Dictionary<string,AssetBundle>();
            mAtlasMap = new Dictionary<string,AssetBundle>();
			mCameraMap = new Dictionary<string,Camera>();
        }

        protected override void OnBeforeDestroy()
        {
            mForms.Free();
            mStandards.Free();
            mMainStacks.Free();
            mTopStacks.Free();
#if _BLUR
			mBlurStacks.Free();
#endif
#if _MASK
			mMaskStacks.Free();
#endif
            mFontMap.Free();
            mStandardAtlasMap.Free();
            mAtlasMap.Free();
            mCameraMap.Free();

            base.OnBeforeDestroy();
        }

        //取得UI資訊
		private TUIInfo GetUIInfo(TUIEnum vUI)
        {
			return vUI != null ? GetUIInfo(vUI.value) : null;
        }

        //取得UI資訊
		private TUIInfo GetUIInfo(int vUI)
        {
            for (int i = 0; i < mInfoList.Count; i++)
            {
				if (mInfoList[i].ui != vUI)
                    continue;

                return mInfoList[i];
            }
            return null;
        }

        //取得UI名稱
		private string GetUIName(TUIEnum vUI)
        {
            for (int i = 0; i < mInfoList.Count; i++)
            {
				if (mInfoList[i].ui != vUI.value)
                    continue;

                return mInfoList[i].name;
            }
            return string.Empty;
        }

        //取得UI列舉
        private TUIEnum GetUIEnum(string vUIName)
        {
            for (int i = 0; i < mInfoList.Count; i++)
            {
                if (mInfoList[i].name != vUIName)
                    continue;

                return UIEnumBase.GetEnum(mInfoList[i].ui);
            }
            return null;
        }

        //取得UI
		public UIForm GetUI(TUIEnum vUI)
        {
			return vUI != null ? GetUI(vUI.value) : null;
        }

        //取得UI
        public UIForm GetUI(int vUI)
        {
            for (int i = 0; i < mForms.Count; i++)
            {
                if (mForms[i].UI != vUI)
                    continue;

                return mForms[i];
            }
            return null;
        }

        public Transform GetParent(int vParam)
        {
            return GetParent((eUIParam)vParam);
        }

		public Transform GetParent(eUIParam vParam)
        {
			if ((vParam & eUIParam.Top) == eUIParam.Top)
				return GetCamera("Top").transform;

            if ((vParam & eUIParam.Main) == eUIParam.Main)
				return GetCamera("Main").transform;

            return null;
        }

        public string GetFormString()
        {
            string str = "\n";
            mForms.ForEach(m => str += m.ToString() + "\n");
            return str;
        }

		public void PushUI(UIForm vForm, int vParam)
        {
            PushUI(vForm, (eUIParam)vParam);
        }

        private void PushUI(UIForm vForm, eUIParam vParam)
        {
            mForms.Add(vForm);

            if ((vParam & eUIParam.Standard) == eUIParam.Standard)
                mStandards.Add(vForm);

            if ((vParam & eUIParam.Main) == eUIParam.Main)
                mMainStacks.Push(vForm);

            if ((vParam & eUIParam.Top) == eUIParam.Top)
                mTopStacks.Push(vForm);
			
#if _BLUR
			if ((vParam & eUIParam.Blur) == eUIParam.Blur)
				mBlurStacks.Push(vForm);
#endif
#if _MASK
			if ((vParam & eUIParam.Mask) == eUIParam.Mask)
				mMaskStacks.Push(vForm);
#endif
        }

		public UIForm PopUI(int vUI)
        {
            UIForm vForm = null;
            for (int i = 0; i < mForms.Count; i++)
            {
                if (mForms[i].UI != vUI)
                    continue;

                vForm = mForms[i];
                break;
            }

            if (vForm == null)
                return null;

            mForms.Remove(vForm);
            mStandards.Remove(vForm);
            mMainStacks.Remove(vForm);
            mTopStacks.Remove(vForm);
#if _BLUR
			mBlurStacks.Remove(vForm);
#endif
#if _MASK
			mMaskStacks.Remove(vForm);
#endif

            return vForm;
        }

        public bool PushFont(string vKey, AssetBundle vValue)
        {
            if (mFontMap.ContainsKey(vKey))
                return false;

            mFontMap.Add(vKey, vValue);
            return true;
        }

		public AssetBundle GetFont(string vKey)
		{
			if (!mFontMap.ContainsKey(vKey))
				return null;

			AssetBundle tmp;
			if (!mFontMap.TryGetValue(vKey, out tmp))
				return null;
			
			return tmp;
		}

        public AssetBundle PopFont(string vKey)
        {
            if (!mFontMap.ContainsKey(vKey))
                return null;

            AssetBundle tmp;
            if (!mFontMap.TryGetValue(vKey, out tmp))
                return null;

            mFontMap.Remove(vKey);
            return tmp;
        }

        public bool PushStandardAtlas(string vKey, AssetBundle vValue)
        {
            if (mStandardAtlasMap.ContainsKey(vKey))
                return false;

            mStandardAtlasMap.Add(vKey, vValue);
            return true;
        }

		public AssetBundle GetStandardAtlas(string vKey)
		{
			if (!mStandardAtlasMap.ContainsKey(vKey))
				return null;

			AssetBundle tmp;
			if (!mStandardAtlasMap.TryGetValue(vKey, out tmp))
				return null;
			
			return tmp;
		}

        public AssetBundle PopStandardAtlas(string vKey)
        {
            if (!mStandardAtlasMap.ContainsKey(vKey))
                return null;

            AssetBundle tmp;
            if (!mStandardAtlasMap.TryGetValue(vKey, out tmp))
                return null;

            mStandardAtlasMap.Remove(vKey);
            return tmp;
        }

        public bool PushAtlas(string vKey, AssetBundle vValue)
        {
            if (mAtlasMap.ContainsKey(vKey))
                return false;

            mAtlasMap.Add(vKey, vValue);
            return true;
        }

		public AssetBundle GetAtlas(string vKey)
		{
			if (!mAtlasMap.ContainsKey(vKey))
				return null;

			AssetBundle tmp;
			if (!mAtlasMap.TryGetValue(vKey, out tmp))
				return null;
			
			return tmp;
		}

        public AssetBundle PopAtlas(string vKey)
        {
            if (!mAtlasMap.ContainsKey(vKey))
                return null;

            AssetBundle tmp;
            if (!mAtlasMap.TryGetValue(vKey, out tmp))
                return null;

            mAtlasMap.Remove(vKey);
            return tmp;
        }

		public bool PushCamera(string key, Camera camera)
		{
			if (mCameraMap.ContainsKey(key))
				return false;

			mCameraMap.Add(key, camera);
			return true;
		}

		public Camera GetCamera(string key)
		{
			if (!mCameraMap.ContainsKey(key))
				return null;

			Camera tmp;
			if (!mCameraMap.TryGetValue(key, out tmp))
				return null;
			
			return tmp;
		}

		public Camera PopCamera(string key)
		{
			if (!mCameraMap.ContainsKey(key))
				return null;

			Camera tmp;
			if (!mCameraMap.TryGetValue(key, out tmp))
				return null;

			mCameraMap.Remove(key);
			return tmp;
		}

		public void SortUI()
        {
            int vDepth = 0;
            for (int i = 0; i < mForms.Count; i++)
            {
                vDepth = mForms[i].SortUI(vDepth);
            }

#if _BLUR
			if (mBlurStacks.Top != null && !mBlurStacks.Top.Hidden)
				mBlurStacks.Top.Blur = true;
#endif
#if _MASK
			if (mMaskStacks.Top != null)
				mMaskStacks.Top.Mask = true;
#endif
        }

		public void OpenUI(int vUI, params object[] param)
        {
			UIForm vForm = PopUI(vUI);
            if (vForm != null)
            {
                PushUI(vForm, vForm.Param);
                SortUI();
                return;
            }

			TUIInfo vInfo = GetUIInfo(vUI);
            if (vInfo == null || vInfo.ui == UIEnumBase.None)
            {
				mLogger.Log(string.Format("OpenUI Err -> {0}", UIEnumBase.GetEnumName(vUI)));
                return;
            }

            GameObject vObject = new GameObject(vInfo.name);
            vForm = (UIForm)vObject.AddComponent(vInfo.type);
            vForm.Init(this, vInfo.ui, vInfo.name, GetParent(vInfo.param), vInfo.param, vInfo.depth, vInfo.rank);
            vForm.OpenUI(param);
        }

        public IEnumerator IOpenUI(int vUI, object vValue = null)
        {
			UIForm vForm = PopUI(vUI);
            if (vForm != null)
            {
                PushUI(vForm, vForm.Param);
                SortUI();
                yield return null;
            }

			TUIInfo vInfo = GetUIInfo(vUI);
            if (vInfo == null || vInfo.ui == UIEnumBase.None)
            {
				mLogger.Log(string.Format("OpenUI Err -> {0}", UIEnumBase.GetEnumName(vUI)));
                yield return null;
            }

            GameObject vObject = new GameObject(vInfo.name);
            vForm = (UIForm)vObject.AddComponent(vInfo.type);
            vForm.Init(this, vInfo.ui, vInfo.name, GetParent(vInfo.param), vInfo.param, vInfo.depth, vInfo.rank);
            yield return CoroutineMgr.Instance.StartCoroutine(vForm.IOpenUI(vValue));
        }

		public void CloseUI(int vUI, params object[] param)
        {
			UIForm vForm = PopUI(vUI);
            if (vForm != null)
            {
                vForm.CloseUI(param);
                Resources.UnloadUnusedAssets();
            }

			SortUI();
        }

        public void ResetALL(int vRank, int vUI = 0, params object[] param)
        {
            List<UIForm> tmp = new List<UIForm>();
            for (int i = 0; i < mForms.Count; i++)
            {
                if (mForms[i].Rank < vRank)
                    continue;

                tmp.Add(mForms[i]);
            }

            tmp.ForEach(m => { PopUI(m.UI); m.CloseUI(); });

            Resources.UnloadUnusedAssets();

            mStandards.ForEach(m => m.Close());

			SortUI();

            UIForm vForm = PopUI(vUI);
			if (vForm == null)
            {
				if (vUI != UIEnumBase.None)
                	OpenUI(vUI, param);
            }
            else
            {
                PushUI(vForm, vForm.Param);
                SortUI();
            }
        }

		public void ShowUI(int vUI)
        {
			UIForm vForm = GetUI(vUI);

            if (vForm == null)
                return;

            vForm.Hidden = false;
        }

		public void HideUI(int vUI)
        {
			UIForm vForm = GetUI(vUI);

            if (vForm == null)
                return;

            vForm.Hidden = true;
        }

		public void ShowLoading(GameObject vObj, bool vIsShow = true)
        {
			UIForm vForm = GetUI(UIEnumBase.Loading);
			if (vForm != null)
				vForm.Open();

            vObj.transform.localPosition = new Vector3(0, 10000, 0);
        }

        public void HideLoading(GameObject vObj)
        {
			UIForm vForm = GetUI(UIEnumBase.Loading);
			if (vForm != null)
				vForm.Close();

            vObj.transform.localPosition = new Vector3(0, 0, 0);
        }

        public void ShowNet(string vMsg, bool vShowPanel = true, bool vShowBtn = false)
        {
            UIForm vForm = GetUI(UIEnumBase.Connecting);
            if (vForm != null)
                vForm.Open();
        }

        public void HideNet()
        {
            UIForm vForm = GetUI(UIEnumBase.Connecting);
            if (vForm != null)
                vForm.Close();
        }

        public void ShowTutorial()
        {
            UIForm vForm = GetUI(UIEnumBase.Tutorial);
            if (vForm != null)
                vForm.Open();
        }

        public void HideTutorial()
        {
            UIForm vForm = GetUI(UIEnumBase.Tutorial);
            if (vForm != null)
                vForm.Close();
        }
    }
}
