namespace TinyBee.Net.Buffer
{
	using System;
	using System.IO;
	using System.Threading;

	public sealed class RingBuffer : Stream
	{
		private const int INITCAPACITY = 4 * 1024;
		private const int INCREMENTSIZE = 4 * 1024;

		private object mSynObject = new object();

		private byte[] mBuffer;
		private int mCapacity;
		private int mMaxCapacity;
		private int mLength;
		private bool mExpandable;

		private int mReadFlag;
		private int mWriteFlag;

		public RingBuffer() : this(INITCAPACITY) {}
		public RingBuffer(int capacity) : this(capacity, true) {}
		public RingBuffer(int capacity, bool expandable) : this(capacity, expandable, -1) {}
		public RingBuffer(int capacity, bool expandable, int maxCapacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (expandable && (maxCapacity != -1 && maxCapacity < capacity))
			{
				throw new ArgumentOutOfRangeException("maxCapacity");
			}

			mBuffer = new byte[capacity];
			mLength = 0;
			mCapacity = capacity;
			mExpandable = expandable;
			mMaxCapacity = maxCapacity;
			mReadFlag = 0;
			mWriteFlag = 0;
		}

		public void Clear()
		{
			mLength = 0;
			mReadFlag = 0;
			mWriteFlag = 0;
		}

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return true; } }
		public override long Length
		{
			get
			{
				lock (mSynObject)
				{
					return mLength;
				}
			}
		}
		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
		public override void Flush()
		{
			throw new NotSupportedException();
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
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

			lock (mSynObject)
			{
				int read = Math.Min(mLength, count);
				if (read == 0)
					return 0;

				ReadInternal(buffer, offset, read, true);

				return read;
			}
		}

		public int Read(byte[] buffer, int offset, int count, bool posmove)
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

			lock (mSynObject)
			{
				int read = Math.Min(mLength, count);
				if (read == 0)
					return 0;

				ReadInternal(buffer, offset, read, posmove);

				return read;
			}
		}

		private void ReadInternal(byte[] buffer, int offset, int count, bool posmove)
		{
			if (mReadFlag < mWriteFlag)
			{
				Buffer.BlockCopy(mBuffer, mReadFlag, buffer, offset, count);
			}
			else
			{
				int afterReadPosLen = mCapacity - mReadFlag;
				if (afterReadPosLen >= count)
				{
					Buffer.BlockCopy(mBuffer, mReadFlag, buffer, offset, count);
				}
				else
				{
					Buffer.BlockCopy(mBuffer, mReadFlag, buffer, offset, afterReadPosLen);
					int restLen = count - afterReadPosLen;
					Buffer.BlockCopy(mBuffer, 0, buffer, afterReadPosLen, restLen);
				}
			}

			if (posmove)
			{
				mLength -= count;
				mReadFlag += count;
				mReadFlag %= mCapacity;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
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

			lock (mSynObject)
			{
				int minCapacityNeeded = mLength + count;

				Grow(minCapacityNeeded);

				if (minCapacityNeeded > mCapacity)
				{
					throw new Exception("cant write any byte or buffer not enough big");
				}

				WriteInternal(buffer, offset, count);
			}
		}

		private void WriteInternal(byte[] buffer, int offset, int count)
		{
			if (mReadFlag > mWriteFlag)
			{
				Buffer.BlockCopy(buffer, offset, mBuffer, mWriteFlag, count);
			}
			else
			{
				int afterWritePosLen = mCapacity - mWriteFlag;
				if (afterWritePosLen >= count)
				{
					Buffer.BlockCopy(buffer, offset, mBuffer, mWriteFlag, count);
				}
				else
				{
					Buffer.BlockCopy(buffer, offset, mBuffer, mWriteFlag, afterWritePosLen);
					int restLen = count - afterWritePosLen;
					Buffer.BlockCopy(buffer, offset + afterWritePosLen, mBuffer, 0, restLen);
				}
			}
			mLength += count;
			mWriteFlag += count;
			mWriteFlag %= mCapacity;
		}

		private void Grow(int size)
		{
			if (!mExpandable)
				return;
			
			if (mCapacity >= size)
				return;
			
			if (mMaxCapacity != -1 && (mMaxCapacity - mCapacity) < INCREMENTSIZE)
				return;
			
			int blocksNum = (int)Math.Ceiling((double)(size - mCapacity) / INCREMENTSIZE);
			byte[] buffer = new byte[mCapacity + blocksNum * INCREMENTSIZE];
			int restoreLen = mLength;
			ReadInternal(buffer, 0, mLength, true);
			mBuffer = buffer;
			mCapacity = buffer.Length;
			mLength = restoreLen;
			mReadFlag = 0;
			mWriteFlag = restoreLen;
		}
	}
}
