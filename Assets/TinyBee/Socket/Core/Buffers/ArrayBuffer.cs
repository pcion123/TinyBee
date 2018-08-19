namespace TinyBee.Net.Buffer
{
	using System;

	public class ArrayBuffer<T>
	{
		private const int BUFFERSIZE = 512 * 1024;

		private object mSynBuff = new object();

		protected T[] mBuffer;
		private int mCapacity = BUFFERSIZE;
		private int mSpace = 0;
		private int mAvailable = 0;

		private int mReadFlag;
		private int mWriteFlag;

		public ArrayBuffer() : this(BUFFERSIZE) {}
		public ArrayBuffer(int capacity) : this(new T[capacity]) {}
		public ArrayBuffer(T[] buf) : this(buf, 0, 0) {}
		public ArrayBuffer(T[] buf, int offset, int size)
		{
			CreateBuf(buf, offset, size);
		}

		public int Capacity
		{
			get
			{
				lock (mSynBuff)
				{
					return mCapacity;
				}
			}
		}
		public int Space
		{
			get
			{
				lock (mSynBuff)
				{
					return mSpace;
				}
			}
		}
		public int Available
		{
			get
			{
				lock (mSynBuff)
				{
					return mAvailable;
				}
			}
		}

		protected void CreateBuf(T[] buf, int offset, int size)
		{
			Clear();

			mBuffer = buf;
			mCapacity = buf.Length;
			mSpace = mCapacity - size;
			mAvailable = size;
			mReadFlag = 0;
			mWriteFlag = 0;
		}

		public void Clear()
		{
			lock (mSynBuff)
			{
				mSpace = mCapacity;
				mAvailable = 0;
				mReadFlag = 0;
				mWriteFlag = 0;
			}
		}

		private void Grow()
		{
			Grow(mCapacity * 2);
		}

		private void Grow(int size)
		{
//			lock (mSynBuff)
//			{
//				if (mCapacity >= size)
//					return;
//				
//				T[] tmp = new T[size];
//				Array.Copy(mBuffer, 0, tmp, 0, mAvailable);
//				mBuffer = tmp;
//				mCapacity = tmp.Length;
//				mSpace = mCapacity - mAvailable;
//			}

			if (mCapacity >= size)
				return;

			int blocksNum = (int)Math.Ceiling((double)(size - mCapacity) / mCapacity * 2);
			T[] buffer = new T[mCapacity + blocksNum * mCapacity * 2];
			int restoreLen = mAvailable;
			ReadInternal(buffer, 0, mAvailable);
			mBuffer = buffer;
			mCapacity = buffer.Length;
			mSpace = mCapacity - restoreLen;
			mAvailable = restoreLen;
			mReadFlag = 0;
			mWriteFlag = restoreLen;
		}

		public void Read(T[] buffer)
		{
			Read(buffer, 0, buffer.Length);
		}

		public void Read(T[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("read range check error");
			}
			if ((buffer.Length - offset) < count)
			{
				throw new ArgumentOutOfRangeException("read range check error");
			}

			lock (mSynBuff)
			{
//				if (mAvailable < size)
//					throw new Exception("read range check error");
//
//				Array.Copy(mBuffer, mAvailable, data, offset, size);
//				mSpace += size;
//				mAvailable -= size;

				int read = Math.Min(mAvailable, count);
				if (read == 0)
					return;

				ReadInternal(buffer, offset, read);
			}
		}

		private void ReadInternal(T[] buffer, int offset, int count)
		{
			if (mReadFlag < mWriteFlag)
			{
				Array.Copy(mBuffer, mReadFlag, buffer, offset, count);
			}
			else
			{
				int afterReadPosLen = mCapacity - mReadFlag;
				if (afterReadPosLen >= count)
				{
					Array.Copy(mBuffer, mReadFlag, buffer, offset, count);
				}
				else
				{
					Array.Copy(mBuffer, mReadFlag, buffer, offset, afterReadPosLen);
					int restLen = count - afterReadPosLen;
					Array.Copy(mBuffer, 0, buffer, afterReadPosLen, restLen);
				}
			}
			mSpace += count;
			mAvailable -= count;
			mReadFlag += count;
			mReadFlag %= mCapacity;
		}

		public void Write(T[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		public void Write(T[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("read range check error");
			}
			if ((buffer.Length - offset) < count)
			{
				throw new ArgumentOutOfRangeException("read range check error");
			}

			lock (mSynBuff)
			{
//				if (mSpace < size)
//					throw new Exception("write range check error");
//				
//				Array.Copy(data, offset, mBuffer, mAvailable, size);
//				mSpace -= size;
//				mAvailable += size;

				int minCapacityNeeded = mAvailable + count;

				Grow(minCapacityNeeded);

				if (minCapacityNeeded > mCapacity)
				{
					throw new Exception("cant write any byte or buffer not enough big");
				}

				WriteInternal(buffer, offset, count);
			}
		}

		private void WriteInternal(T[] buffer, int offset, int count)
		{
			if (mReadFlag > mWriteFlag)
			{
				Array.Copy(buffer, offset, mBuffer, mWriteFlag, count);
			}
			else
			{
				int afterWritePosLen = mCapacity - mWriteFlag;
				if (afterWritePosLen >= count)
				{
					Array.Copy(buffer, offset, mBuffer, mWriteFlag, count);
				}
				else
				{
					Array.Copy(buffer, offset, mBuffer, mWriteFlag, afterWritePosLen);
					int restLen = count - afterWritePosLen;
					Array.Copy(buffer, offset + afterWritePosLen, mBuffer, 0, restLen);
				}
			}
			mSpace -= count;
			mAvailable += count;
			mWriteFlag += count;
			mWriteFlag %= mCapacity;
		}

		public T[] Copy()
		{
//			lock (mSynBuff)
//			{
//				T[] tmp = new T[mAvailable];
//				Array.Copy(mBuffer, 0, tmp, 0, mAvailable);
//				return tmp;
//			}
			T[] buf = new T[mAvailable];
			lock (mSynBuff)
			{
				int restoreSpace = mSpace;
				int restoreAvailable = mAvailable;
				int restoreReadFlag = mReadFlag;
				ReadInternal(buf, 0, mAvailable);                
				mSpace = restoreSpace;
				mAvailable = restoreAvailable;
				mReadFlag = restoreReadFlag;
				return buf;
			}
		}
	}
}