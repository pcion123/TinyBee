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

        public static void Begin(GameObject obj, Vector3 startPos, Vector3 endPos, string text, float speed = 1f, float delay = 0f, eBubbleType type = eBubbleType.Bubble, UISprite background = null)
        {
			Bubble bubble = obj.AddComponent<Bubble>();
			if (bubble != null)
            {
				bubble.StartPosition = startPos;
				bubble.EndPosition = endPos;
				bubble.Text = text;
				bubble.Type = type;
				bubble.Speed = speed;
				bubble.Delay = delay;
				bubble.Run();

				if (background != null)
                {
					string[] strs = text.Split('\n');
					if (strs.Length > 0)
                    {
                        int max = 0;
                        int count = 0;
						for (int i = 0; i < strs.Length; i++)
                        {
							count = strs[i].GetStringLength();
							if (count > max)
								max = count;
                        }
						background.height = bubble.Height + bubble.Size;
						background.width = max * bubble.Size / 2 + bubble.Size;
                    }
                    else
                    {
						background.height = bubble.Height + bubble.Size;
						background.width = bubble.Width + bubble.Size;
                    }
                }
            }
        }
    }
}