namespace TinyBee.Logger
{
    using System;
    using UnityEngine;

    public class TLogger : ILogger
    {
        private static TLogger mInstance = null;

        public static TLogger Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new TLogger();

                return mInstance;
            }
        }

        public void Log(string message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
#if _DEBUG
            DebugMgr.Instance.Log(message);
#endif
        }

        public void LogWarning(string message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
#endif
#if _DEBUG
			DebugMgr.Instance.LogWarning(message);
#endif
        }

        public void LogError(string message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
#if _DEBUG
			DebugMgr.Instance.LogError(message);
#endif
        }

        public void LogException(Exception exception)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
            System.Diagnostics.StackFrame[] frames = trace.GetFrames();
            for (int i = frames.Length - 1; i >= 0; i--)
            {
                string fileName = frames[i].GetFileName();
                int line = frames[i].GetFileLineNumber();
                string message = string.Format("{0}, ErrorLine {1}", fileName, line);
#if UNITY_EDITOR
                Debug.LogWarning(message);
#endif
#if _DEBUG
				DebugMgr.Instance.LogWarning(message);
#endif
            }
        }

        public static void Log(LogType type, string message)
        {
            if (mInstance == null)
                mInstance = new TLogger();

            switch (type)
            {
                case LogType.Error:
                    mInstance.LogError(message);
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    mInstance.LogWarning(message);
                    break;
                case LogType.Log:
                    mInstance.Log(message);
                    break;
                case LogType.Exception:
                    break;
            }
        }

        public static void Log(LogType type, Exception exception)
        {
            if (mInstance == null)
                mInstance = new TLogger();

            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Warning:
                case LogType.Log:
                    break;
                case LogType.Exception:
                    mInstance.LogException(exception);
                    break;
            }
        }
    }
}