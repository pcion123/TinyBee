namespace TinyBee
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public enum eCountry
    {
        NONE,
        EN,
        TW,
        CN,
        JP,
        KR
    }

    public enum eLanguage
    {
        NONE,
        EN,
        TW,
        CN,
        JP,
        KR
    }

    [TMonoSingletonPath("[Game]/GameMgr")]
    public class GameMgr : TMgrBehaviour, ISingleton
    {
        private const string FTP_HEADER = "ftp://";
        private const string HTTP_HEADER = "http://";

        private eCountry mCountry = eCountry.NONE;          //國家版本標記
        private eLanguage mLanguage = eLanguage.NONE;       //語言版本標記
        private string mLanguagePath = null;                //語言版本路徑
        private string mDataPath = null;                    //
		private string mVersion = null;

        public eCountry Country { get { return mCountry; } }
        public eLanguage Language { get { return mLanguage; } }
        public string LanguagePath { get { return mLanguagePath; } }
        public string DataPath { get { return mDataPath; } }
		public string Version { get { return mVersion; }}

        public static GameMgr Instance
        {
            get { return MonoSingletonProperty<GameMgr>.Instance; }
        }

		public override int ManagerId
		{
			get { return MgrEnumBase.Game; }
		}

        public void OnSingletonInit() { }

        public override void Init(params object[] param)
        {
            mCountry = (eCountry)param[0];
            mLanguage = (eLanguage)param[1];
            mLanguagePath = GetLangPath(mLanguage);
            mDataPath = GetDataPath();
			mVersion = (string)param[2];
        }

        //檔案檢查
        public bool CheckFile()
        {
            return true;
        }

        //取得語系路徑
        private string GetLangPath(string vPath, bool vIsSlash = false)
        {
            string[] values = Enum.GetNames(typeof(eLanguage));
            foreach (string value in values)
            {
                if (vPath.Contains(value + "/") == true)
                    return vIsSlash ? value + "/" : value;
            }
            return string.Empty;
        }

        //取得語系路徑
        private string GetLangPath(eLanguage vLang, bool vIsSlash = false)
        {
            return vIsSlash ? vLang.ToString() + "/" : vLang.ToString();
        }

        //取得存取路徑
        private string GetDataPath()
        {
#if _RESOURCES && UNITY_EDITOR
			return Application.dataPath + "/" + AutoLauncher.Setting.OutputAssetsFolder + "/";
#endif

#if UNITY_TVOS && !UNITY_EDITOR
            return Application.temporaryCachePath + "/";
#else
            return Application.persistentDataPath + "/";
#endif
        }
    }
}
