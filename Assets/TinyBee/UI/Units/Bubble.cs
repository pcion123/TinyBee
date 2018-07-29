namespace TinyBee.UI
{
    using UnityEngine;
    using System.Collections;

    public enum eBubbleType
    {
        None,   //
        Bubble, //
        Scale   //
    }

    public class Bubble : MonoBehaviour
    {
        private UILabel mLabel = null;
        private eBubbleType mType = eBubbleType.None;
        private float mSpeed = 0.8f;
        private float mDelay = 0f;
        private Vector3 mStartPosition = new Vector3(0, 170, 0);
        private Vector3 mEndPosition = new Vector3(0f, 250f, 0f);

        public int Size
        {
            get
            {
                return (mLabel != null) ? mLabel.fontSize : 0;
            }
        }
        public int Height
        {
            get
            {
                return (mLabel != null) ? mLabel.height : 0;
            }
        }
        public int Width
        {
            get
            {
                return (mLabel != null) ? mLabel.width : 0;
            }
        }
        public string Text
        {
            get
            {
                return (mLabel != null) ? mLabel.text : string.Empty;
            }
            set
            {
                if (mLabel != null)
                    mLabel.text = value;
            }
        }
        public Vector3 StartPosition { get { return mStartPosition; } set { mStartPosition = value; } }
        public Vector3 EndPosition { get { return mEndPosition; } set { mEndPosition = value; } }
        public eBubbleType Type { get { return mType; } set { mType = value; } }
        public float Speed { get { return mSpeed; } set { mSpeed = value; } }
        public float Delay { get { return mDelay; } set { mDelay = value; } }

        void Awake()
        {
            mLabel = GetComponent<UILabel>();
        }

        public void Run()
        {
            CoroutineMgr.Instance.StartCoroutine(IRun());
        }

        private IEnumerator IRun()
        {
            transform.localPosition = mStartPosition;

            yield return new WaitForSeconds(mDelay);

            switch (mType)
            {
                case eBubbleType.Bubble:
                    TweenPosition.Begin(gameObject, mSpeed, mEndPosition);

                    TweenAlpha.Begin(gameObject, mSpeed, 0);
                    break;
                case eBubbleType.Scale:
                    TweenScale.Begin(gameObject, mSpeed, Vector3.zero);

                    TweenAlpha.Begin(gameObject, mSpeed, 0);
                    break;
            }

            GameObject.Destroy(gameObject, mSpeed + 1f);
        }

        public static void Begin(GameObject vObj, Vector3 vStartPos, Vector3 vEndPos, string vText, float vSpeed = 1f, float vDelay = 0f, eBubbleType vType = eBubbleType.Bubble, UISprite vBackground = null)
        {
            Bubble vBubble = vObj.AddComponent<Bubble>();

            if (vBubble != null)
            {
                vBubble.StartPosition = vStartPos;
                vBubble.EndPosition = vEndPos;
                vBubble.Text = vText;
                vBubble.Type = vType;
                vBubble.Speed = vSpeed;
                vBubble.Delay = vDelay;
                vBubble.Run();

                if (vBackground != null)
                {
                    string[] vStr = vText.Split('\n');

                    if (vStr.Length > 0)
                    {
                        int vMax = 0;
                        int vCount = 0;
                        for (int i = 0; i < vStr.Length; i++)
                        {
                            vCount = vStr[i].GetStringLength();

                            if (vCount > vMax)
                                vMax = vCount;
                        }

                        vBackground.height = vBubble.Height + vBubble.Size;
                        vBackground.width = vMax * vBubble.Size / 2 + vBubble.Size;
                    }
                    else
                    {
                        vBackground.height = vBubble.Height + vBubble.Size;
                        vBackground.width = vBubble.Width + vBubble.Size;
                    }
                }
            }
        }
    }
}