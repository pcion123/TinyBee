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
		private TUIInfo GetUIInfo(TUIEnum ui)
        {
			return ui != null ? GetUIInfo(ui.value) : null;
        }

        //取得UI資訊
		private TUIInfo GetUIInfo(int ui)
        {
            for (int i = 0; i < mInfoList.Count; i++)
            {
				if (mInfoList[i].ui != ui)
                    continue;

                return mInfoList[i];
            }
            return null;
        }

        //取得UI名稱
		private string GetUIName(TUIEnum ui)
        {
            for (int i = 0; i < mInfoList.Count; i++)
            {
				if (mInfoList[i].ui != ui.value)
                    continue;

                return mInfoList[i].name;
            }
            return string.Empty;
        }

        //取得UI列舉
        private TUIEnum GetUIEnum(string uiname)
        {
            for (int i = 0; i < mInfoList.Count; i++)
            {
				if (mInfoList[i].name != uiname)
                    continue;

                return UIEnumBase.GetEnum(mInfoList[i].ui);
            }
            return null;
        }

        //取得UI
		public UIForm GetUI(TUIEnum ui)
        {
			return ui != null ? GetUI(ui.value) : null;
        }

        //取得UI
		public UIForm GetUI(int ui)
        {
            for (int i = 0; i < mForms.Count; i++)
            {
				if (mForms[i].UI != ui)
                    continue;

                return mForms[i];
            }
            return null;
        }

        public Transform GetParent(int param)
        {
			return GetParent((eUIParam)param);
        }

		public Transform GetParent(eUIParam param)
        {
			if ((param & eUIParam.Top) == eUIParam.Top)
				return GetCamera("Top").transform;

			if ((param & eUIParam.Main) == eUIParam.Main)
				return GetCamera("Main").transform;

            return null;
        }

        public string GetFormString()
        {
            string str = "\n";
            mForms.ForEach(m => str += m.ToString() + "\n");
            return str;
        }

		public void PushUI(UIForm form, int param)
        {
			PushUI(form, (eUIParam)param);
        }

		private void PushUI(UIForm form, eUIParam param)
        {
			mForms.Add(form);

			if ((param & eUIParam.Standard) == eUIParam.Standard)
				mStandards.Add(form);

			if ((param & eUIParam.Main) == eUIParam.Main)
				mMainStacks.Push(form);

			if ((param & eUIParam.Top) == eUIParam.Top)
				mTopStacks.Push(form);
			
#if _BLUR
			if ((param & eUIParam.Blur) == eUIParam.Blur)
				mBlurStacks.Push(form);
#endif
#if _MASK
			if ((param & eUIParam.Mask) == eUIParam.Mask)
				mMaskStacks.Push(form);
#endif
        }

		public UIForm PopUI(int ui)
        {
            UIForm form = null;
            for (int i = 0; i < mForms.Count; i++)
            {
				if (mForms[i].UI != ui)
                    continue;

				form = mForms[i];
                break;
            }

			if (form == null)
                return null;

			mForms.Remove(form);
			mStandards.Remove(form);
			mMainStacks.Remove(form);
			mTopStacks.Remove(form);
#if _BLUR
			mBlurStacks.Remove(form);
#endif
#if _MASK
			mMaskStacks.Remove(form);
#endif

			return form;
        }

        public bool PushFont(string key, AssetBundle bundle)
        {
			if (mFontMap.ContainsKey(key))
                return false;

			mFontMap.Add(key, bundle);
            return true;
        }

		public AssetBundle GetFont(string key)
		{
			if (!mFontMap.ContainsKey(key))
				return null;

			AssetBundle tmp;
			if (!mFontMap.TryGetValue(key, out tmp))
				return null;
			
			return tmp;
		}

		public AssetBundle PopFont(string key)
        {
			if (!mFontMap.ContainsKey(key))
                return null;

            AssetBundle tmp;
			if (!mFontMap.TryGetValue(key, out tmp))
                return null;

			mFontMap.Remove(key);
            return tmp;
        }

		public bool PushStandardAtlas(string key, AssetBundle bundle)
        {
			if (mStandardAtlasMap.ContainsKey(key))
                return false;

			mStandardAtlasMap.Add(key, bundle);
            return true;
        }

		public AssetBundle GetStandardAtlas(string key)
		{
			if (!mStandardAtlasMap.ContainsKey(key))
				return null;

			AssetBundle tmp;
			if (!mStandardAtlasMap.TryGetValue(key, out tmp))
				return null;
			
			return tmp;
		}

		public AssetBundle PopStandardAtlas(string key)
        {
			if (!mStandardAtlasMap.ContainsKey(key))
                return null;

            AssetBundle tmp;
			if (!mStandardAtlasMap.TryGetValue(key, out tmp))
                return null;

			mStandardAtlasMap.Remove(key);
            return tmp;
        }

		public bool PushAtlas(string key, AssetBundle bundle)
        {
			if (mAtlasMap.ContainsKey(key))
                return false;

			mAtlasMap.Add(key, bundle);
            return true;
        }

		public AssetBundle GetAtlas(string key)
		{
			if (!mAtlasMap.ContainsKey(key))
				return null;

			AssetBundle tmp;
			if (!mAtlasMap.TryGetValue(key, out tmp))
				return null;
			
			return tmp;
		}

		public AssetBundle PopAtlas(string key)
        {
			if (!mAtlasMap.ContainsKey(key))
                return null;

            AssetBundle tmp;
			if (!mAtlasMap.TryGetValue(key, out tmp))
                return null;

			mAtlasMap.Remove(key);
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
            int depth = 0;
			mForms.ForEach(m => depth = m.SortUI(depth));
#if _BLUR
			if (mBlurStacks.Top != null && !mBlurStacks.Top.Hidden)
				mBlurStacks.Top.Blur = true;
#endif
#if _MASK
			if (mMaskStacks.Top != null)
				mMaskStacks.Top.Mask = true;
#endif
        }

		public void OpenUI(int ui, params object[] param)
        {
			UIForm form = PopUI(ui);
			if (form != null)
            {
				PushUI(form, form.Param);
                SortUI();
                return;
            }

			TUIInfo info = GetUIInfo(ui);
			if (info == null || info.ui == UIEnumBase.None)
            {
				mLogger.Log(string.Format("OpenUI Err -> {0}", UIEnumBase.GetEnumName(ui)));
                return;
            }

			GameObject obj = new GameObject(info.name);
			form = (UIForm)obj.AddComponent(info.type);
			form.Init(this, info.ui, info.name, GetParent(info.param), info.param, info.depth, info.rank);
			form.OpenUI(param);
        }

		public IEnumerator IOpenUI(int ui, params object[] param)
        {
			UIForm form = PopUI(ui);
			if (form != null)
            {
				PushUI(form, form.Param);
                SortUI();
                yield return null;
            }

			TUIInfo info = GetUIInfo(ui);
			if (info == null || info.ui == UIEnumBase.None)
            {
				mLogger.Log(string.Format("OpenUI Err -> {0}", UIEnumBase.GetEnumName(ui)));
                yield return null;
            }

			GameObject vObject = new GameObject(info.name);
			form = (UIForm)vObject.AddComponent(info.type);
			form.Init(this, info.ui, info.name, GetParent(info.param), info.param, info.depth, info.rank);
			yield return CoroutineMgr.Instance.StartCoroutine(form.IOpenUI(param));
        }

		public void CloseUI(int ui, params object[] param)
        {
			UIForm form = PopUI(ui);
			if (form != null)
            {
				form.CloseUI(param);
                Resources.UnloadUnusedAssets();
            }
			SortUI();
        }

        public void ResetALL(int rank, int ui = 0, params object[] param)
        {
            List<UIForm> tmp = new List<UIForm>();
            for (int i = 0; i < mForms.Count; i++)
            {
				if (mForms[i].Rank < rank)
                    continue;

                tmp.Add(mForms[i]);
            }

            tmp.ForEach(m => { PopUI(m.UI); m.CloseUI(); });

            Resources.UnloadUnusedAssets();

            mStandards.ForEach(m => m.Close());

			SortUI();

			UIForm form = PopUI(ui);
			if (form == null)
            {
				if (ui != UIEnumBase.None)
					OpenUI(ui, param);
            }
            else
            {
				PushUI(form, form.Param);
                SortUI();
            }
        }

		public void ShowUI(int ui)
        {
			UIForm form = GetUI(ui);

			if (form == null)
                return;

			form.Hidden = false;
        }

		public void HideUI(int ui)
        {
			UIForm form = GetUI(ui);

			if (form == null)
                return;

			form.Hidden = true;
        }

		public void ShowLoading(GameObject obj, bool isShow = true)
        {
			UIForm form = GetUI(UIEnumBase.Loading);
			if (form != null)
				form.Open();
			obj.transform.localPosition = new Vector3(0, 10000, 0);
        }

		public void HideLoading(GameObject obj)
        {
			UIForm form = GetUI(UIEnumBase.Loading);
			if (form != null)
				form.Close();
			obj.transform.localPosition = Vector3.zero;
        }

        public void ShowNet(string msg, bool showPanel = true, bool showButton = false)
        {
			UIForm form = GetUI(UIEnumBase.Connecting);
			if (form != null)
				form.Open();
        }

        public void HideNet()
        {
			UIForm form = GetUI(UIEnumBase.Connecting);
			if (form != null)
				form.Close();
        }

        public void ShowTutorial()
        {
			UIForm form = GetUI(UIEnumBase.Tutorial);
			if (form != null)
				form.Open();
        }

        public void HideTutorial()
        {
			UIForm form = GetUI(UIEnumBase.Tutorial);
			if (form != null)
				form.Close();
        }
    }
}
