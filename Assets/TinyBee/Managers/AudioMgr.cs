namespace TinyBee
{
	using System.Collections;
	using System.Collections.Generic;
	using System;
	using UnityEngine;

	//聲音列舉
	public enum eAudio
	{
		NONE,  //無
		MUSIC, //音樂
		SOUND  //音效
	}

	public class TAudio : IDisposable
	{
		private AudioMgr mMgr = null;
		private eAudio mKind = eAudio.NONE;
		private AudioSource mAudio = null;
		private string mPath = null;
		private string mName = null;

		public eAudio Kind { get { return mKind; } }
		public AudioSource Audio { get { return mAudio; } }
		public bool playOnAwake { get { return mAudio.playOnAwake; } set { SetPlayOnAwake(value); } }
		public bool loop { get { return mAudio.loop; } set { SetLoop(value); } }
		public bool isPlaying { get { return mAudio.isPlaying; } }
		public AudioClip clip { get { return mAudio.clip; } set { SetClip(value); } }
		public string Path { get { return mPath; } set { SetPath(value); } }
		public string Name { get { return mName; } set { SetName(value); } }

		public TAudio(AudioMgr vMgr, eAudio vKind, GameObject vObject)
		{
			mMgr = vMgr;
			mKind = vKind;

			if (vObject != null)
			{
				mAudio = vObject.GetComponent<AudioSource>();

				if (mAudio == null)
					mAudio = vObject.AddComponent<AudioSource>();
			}
		}

		public void Dispose()
		{
			if (mMgr != null)
			{
				mMgr.Del(this);
				mMgr = null;
			}

			mKind = eAudio.NONE;
		}

		private void SetClip(AudioClip vClip)
		{
			mAudio.clip = vClip;
		}

		private void SetPlayOnAwake(bool vPlayOnAwake)
		{
			mAudio.playOnAwake = vPlayOnAwake;
		}

		private void SetLoop(bool vloop)
		{
			mAudio.loop = vloop;
		}

		private void SetPath(string vPath)
		{
			mPath = vPath;
		}

		private void SetName(string vName)
		{
			mName = vName;
		}

		public void Play()
		{
			switch (mKind)
			{
				case eAudio.MUSIC:
					if (mMgr.IsMusic == false)
						return;
					break;
				case eAudio.SOUND:
					if (mMgr.IsSound == false)
						return;
					break;
				default:
					return;
			}

			mAudio.Play();
		}

		public void Stop()
		{
			mAudio.Stop();
		}
	}

	[TMonoSingletonPath("[Audio]/AudioMgr")]
	public class AudioMgr : TMgrBehaviour, ISingleton
	{
		private List<TAudio> mContainer = new List<TAudio>();  //聲音物件清單
		private bool mIsMusic = true;       //背景音樂開關標記
		private bool mIsSound = true;       //遊戲音效開關標記

		public bool IsMusic { get { return mIsMusic; } set { SetIsMusic(value); } }
		public bool IsSound { get { return mIsSound; } set { SetIsSound(value); } }
		public int Count { get { return mContainer.Count; } }

		public static AudioMgr Instance
		{
			get { return MonoSingletonProperty<AudioMgr>.Instance; }
		}

		public void OnSingletonInit() {}

		protected override void SetupMgrId()
		{
            mMgrId = MgrEnumBase.Audio;
        }

		protected override void OnBeforeDestroy()
		{
			if (mContainer != null)
			{
                mContainer.Clear();
                mContainer = null;
			}
		}

		private void SetIsMusic(bool vIsMusic)
		{
			mIsMusic = vIsMusic;

			for (int i = 0; i < mContainer.Count; i++)
			{
				if (mContainer[i].Kind != eAudio.MUSIC)
					continue;

				if (vIsMusic == true)
                    mContainer[i].Play();
				else
                    mContainer[i].Stop();
			}
		}

		private void SetIsSound(bool vIsSound)
		{
			mIsSound = vIsSound;

			for (int i = 0; i < mContainer.Count; i++)
			{
				if (mContainer[i].Kind != eAudio.SOUND)
					continue;

				if (vIsSound == true)
                    mContainer[i].Play();
				else
                    mContainer[i].Stop();
			}
		}

		public TAudio Add(GameObject vObject, eAudio vKind)
		{
			if (vObject == null)
				return null;

			TAudio vAudio = new TAudio(this, vKind, vObject);

			if (mContainer != null)
                mContainer.Add(vAudio);

			return vAudio;
		}

		public void Del(TAudio vAudio)
		{
			if (mContainer != null)
                mContainer.Remove(vAudio);
		}
	}
}
