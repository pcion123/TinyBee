namespace TinyBee.Debug
{
    using UnityEngine;

    public class MsgWindow : TWindow
    {
        private enum eLog
        {
            NONE,
            LOG,
            WARNING,
            ERROR,
            EXCEPTION
        }

        private eLog mLogType = eLog.LOG;
        private TLog mLog = null;
        private TLog mLogWarning = null;
        private TLog mLogError = null;
        private TLog mLogException = null;

        public MsgWindow(int id, string title, Rect rect, bool enabled) : base(id, title, rect, enabled)
        {
            mLog = new TLog();
            mLogWarning = new TLog();
            mLogError = new TLog();
            mLogException = new TLog();
        }

        public override void Dispose()
        {
            if (mLog != null)
            {
                mLog.Dispose();
                mLog = null;
            }

            if (mLogWarning != null)
            {
                mLogWarning.Dispose();
                mLogWarning = null;
            }

            if (mLogError != null)
            {
                mLogError.Dispose();
                mLogError = null;
            }

            if (mLogException != null)
            {
                mLogException.Dispose();
                mLogException = null;
            }
        }

        private TLog GetCurrentLog()
        {
            switch (mLogType)
            {
                case eLog.LOG:
                    return mLog;
                case eLog.WARNING:
                    return mLogWarning;
                case eLog.ERROR:
                    return mLogError;
                case eLog.EXCEPTION:
                    return mLogException;
            }
            return null;
        }

        protected override void DrawWindow(int id)
        {
            GUILayout.BeginHorizontal();
            try
            {
                string msg = string.Empty;
                TLog log = GetCurrentLog();
				if (log != null)
                {
					if (log.Count <= log.MaxShowLine)
                    {
						for (int i = 0; i < log.Count; i++)
                        {
							msg = i == 0 ? log.Messages[i] : msg + "\n" + log.Messages[i];
                        }
                    }
                    else
                    {
						for (int i = log.MessageLine - log.MaxShowLine; i < log.Count; i++)
                        {
							msg = i == (log.MessageLine - log.MaxShowLine) ? log.Messages[i] : msg + "\n" + log.Messages[i];
                        }
                    }
                }

				GUI.Label(new Rect(5, 15, 400, 450), msg);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

		protected override void DrawButton(int id, Rect rect)
        {
			base.DrawButton(id, rect);

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 1, 72, 36), "Log"))
            {
                mLogType = eLog.LOG;
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 2, 72, 36), "Warning"))
            {
                mLogType = eLog.WARNING;
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 3, 72, 36), "Error"))
            {
                mLogType = eLog.ERROR;
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 4, 72, 36), "Exception"))
            {
                mLogType = eLog.EXCEPTION;
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 5, 72, 36), "↑"))
            {
                AddLine();
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 6, 72, 36), "↓"))
            {
                DelLine();
            }

            if (GUI.Button(new Rect(mRect.x - 72, mRect.y + 36 * 7, 72, 36), "Clear"))
            {
                Clear();
            }
        }

        //清空訊息
        private void Clear()
        {
            switch (mLogType)
            {
                case eLog.LOG:
                    mLog.Clear();
                    break;
                case eLog.WARNING:
                    mLogWarning.Clear();
                    break;
                case eLog.ERROR:
                    mLogError.Clear();
                    break;
                case eLog.EXCEPTION:
                    mLogException.Clear();
                    break;
            }
        }

        public void Log(string vMsg)
        {
            mLog.Log(vMsg);
        }

        public void LogWarning(string vMsg)
        {
            mLog.Log(vMsg);
            mLogWarning.Log(vMsg);
        }

        public void LogError(string vMsg)
        {
            mLog.Log(vMsg);
            mLogError.Log(vMsg);
        }

        public void LogException(string vMsg)
        {
            mLog.Log(vMsg);
            mLogException.Log(vMsg);
        }

        private void AddLine()
        {
            switch (mLogType)
            {
                case eLog.LOG:
                    mLog.AddLine();
                    break;
                case eLog.WARNING:
                    mLogWarning.AddLine();
                    break;
                case eLog.ERROR:
                    mLogError.AddLine();
                    break;
                case eLog.EXCEPTION:
                    mLogException.AddLine();
                    break;
            }
        }

        private void DelLine()
        {
            switch (mLogType)
            {
                case eLog.LOG:
                    mLog.DelLine();
                    break;
                case eLog.WARNING:
                    mLogWarning.DelLine();
                    break;
                case eLog.ERROR:
                    mLogError.DelLine();
                    break;
                case eLog.EXCEPTION:
                    mLogException.DelLine();
                    break;
            }
        }
    }
}