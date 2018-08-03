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
		public string Path { get; set; }
		public string Name { get; set; }

		public TAudio(AudioMgr mgr, eAudio kind, GameObject obj)
		{
			mMgr = mgr;
			mKind = kind;

			if (obj != null)
			{
				mAudio = obj.GetComponent<AudioSource>();
				if (mAudio == null)
					mAudio = obj.AddComponent<AudioSource>();
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

		private void SetClip(AudioClip clip)
		{
			mAudio.clip = clip;
		}

		private void SetPlayOnAwake(bool playOnAwake)
		{
			mAudio.playOnAwake = playOnAwake;
		}

		private void SetLoop(bool loop)
		{
			mAudio.loop = loop;
		}

		public void Play()
		{
			switch (mKind)
			{
				case eAudio.MUSIC:
					if (!mMgr.IsMusic)
						return;
					break;
				case eAudio.SOUND:
					if (!mMgr.IsSound)
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

		public override int ManagerId
		{
			get { return MgrEnumBase.Audio; }
		}

		public void OnSingletonInit() {}

		protected override void OnBeforeDestroy()
		{
			mContainer.Free();

			base.OnBeforeDestroy();
		}

		private void SetIsMusic(bool isMusic)
		{
			mIsMusic = isMusic;

			for (int i = 0; i < mContainer.Count; i++)
			{
				if (mContainer[i].Kind != eAudio.MUSIC)
					continue;

				if (isMusic)
                    mContainer[i].Play();
				else
                    mContainer[i].Stop();
			}
		}

		private void SetIsSound(bool isSound)
		{
			mIsSound = isSound;

			for (int i = 0; i < mContainer.Count; i++)
			{
				if (mContainer[i].Kind != eAudio.SOUND)
					continue;

				if (isSound)
                    mContainer[i].Play();
				else
                    mContainer[i].Stop();
			}
		}

		public TAudio Add(GameObject obj, eAudio kind)
		{
			if (obj == null)
				return null;

			TAudio audio = new TAudio(this, kind, obj);
			if (mContainer != null)
				mContainer.Add(audio);
			return audio;
		}

		public void Del(TAudio audio)
		{
			if (mContainer != null)
				mContainer.Remove(audio);
		}
	}
}
