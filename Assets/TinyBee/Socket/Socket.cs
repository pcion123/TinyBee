namespace TinyBee.Net
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using TinyBee.Net.Buffer;
	using ILogger = TinyBee.Logger.ILogger;
	using TLogger = TinyBee.Logger.TLogger;

	public class Socket : IDisposable
	{
		protected ILogger mLogger = TLogger.Instance;

		private object mSynRcv = new object();
		private object mSynSend = new object();

		private short mVersion;
		protected long mSessionId;
		private byte mEncryptionCode;
		private ushort mPackCompressSize;
		private int mHeaderSize;
		private System.Net.Sockets.Socket mSocket;
		private IPEndPoint mIPEndPoint;

		private PackagePool mPackagePool = new PackagePool();
		private RingBuffer mRcvBuffer = new RingBuffer();
		private RingBuffer mSendBuffer = new RingBuffer();
		private ByteArrayBuffer mProcessBuffer = new ByteArrayBuffer();
		private HeaderBase mProcessHeader;
		private long mTimeup = -1L;

		private NetProcess[,] mNetProcesses = new NetProcess[128,128];

		public RcvHeader OnGenRcvHeader { get; set; }
		public SendHeader OnGenSendHeader { get; set; }

		public NetEvent OnConnect { get; set; }
		public NetEvent OnDisconnect { get; set; }
		public NetEvent OnRcv { get; set; }
		public NetEvent OnSend { get; set; }
		public NetEvent OnError { get; set; }
		public NetEvent OnAnalyze { get; set; }

		public short Version { get { return mVersion; } }
		public long SessionId { get { return mSessionId; } }
		public String Hostname { get { return mIPEndPoint != null ? mIPEndPoint.Address.ToString() : string.Empty; } }
		public int Port { get { return mIPEndPoint != null ? mIPEndPoint.Port : 0; } }
		public bool Connected { get { return mSocket != null ? mSocket.Connected : false; } }

		public Socket(int version, int encryptionCode, int packCompressSize, int headerSize)
		{
			mVersion = (short)version;
			mEncryptionCode = (byte)encryptionCode;
			mPackCompressSize = (ushort)packCompressSize;
			mHeaderSize = headerSize;
		}

		public Socket(short version, byte encryptionCode, ushort packCompressSize, int headerSize)
		{
			mVersion = version;
			mEncryptionCode = encryptionCode;
			mPackCompressSize = packCompressSize;
			mHeaderSize = headerSize;
		}

		public void Dispose()
		{
			DoDisconnect();
		}

		public void Connect(string hostname, int port)
		{
			mLogger.Log(string.Format("start connect hostname={0} port={1}", hostname, port));

			DoDisconnect();

			mSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//m_tcpSsocket.ReceiveBufferSize = 10240;
			//m_tcpSsocket.ReceiveTimeout = 1000 * 30;

			mIPEndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
			IAsyncResult result = mSocket.BeginConnect(mIPEndPoint, new AsyncCallback((asyn) => { if (mSocket.Connected) DoConnect(); }), null);
			if (!result.AsyncWaitHandle.WaitOne(3000, false))
			{
				mLogger.Log("connect timeout fail");

				DoDisconnect();
			}
		}

		public void Disconnect()
		{
			DoDisconnect();
		}

		public virtual void Update()
		{
			ProtocolAnlyzer();
		}

		private void ProtocolAnlyzer()
		{
			lock (mSynRcv)
			{
				long startTick = DateTime.Now.Ticks;
				try
				{
					//長度大於協定頭大小才能進行封包解析
					while (mRcvBuffer.Length >= mHeaderSize)
					{
						byte[] temp = new byte[mHeaderSize];
						mRcvBuffer.Read(temp, 0, mHeaderSize, false);
						mProcessBuffer.Clear();
						mProcessBuffer.Write(temp);

						if (OnGenRcvHeader == null)
						{
							mProcessHeader = new HeaderBase();
							mProcessHeader.Version = mProcessBuffer.ReadShort();
							mProcessHeader.MainNo = mProcessBuffer.ReadSByte();
							mProcessHeader.SubNo = mProcessBuffer.ReadSByte();
							mProcessHeader.IsCompress = mProcessBuffer.ReadBool();
							mProcessHeader.SessionId = mProcessBuffer.ReadLong();
							mProcessHeader.Len = mProcessBuffer.ReadInt();
						}
						else
						{
							mProcessHeader = OnGenRcvHeader(mProcessBuffer);
						}
						mLogger.Log(string.Format("anlyze header receive {0} {1} {2} {3}", mProcessHeader.Version, mProcessHeader.MainNo, mProcessHeader.SubNo, mProcessHeader.Len));

						//檢查協定版本
						if (mProcessHeader.Version != mVersion)
							throw new System.Exception("Version Error");

						//協定是否接收完畢
						if (mProcessHeader.Len > mRcvBuffer.Length - mHeaderSize)
							break;

						mRcvBuffer.Read(temp, 0, mHeaderSize, true);
						byte[] command = new byte[mProcessHeader.Len];
						mRcvBuffer.Read(command, 0, command.Length, true);
						ByteArrayBuffer runningBuffer = new ByteArrayBuffer(command);

						runningBuffer.Decode(mEncryptionCode);

						//是否有進行壓縮
						if (mProcessHeader.IsCompress)
							runningBuffer.Decompress();

						//執行命令
						if (mNetProcesses[mProcessHeader.MainNo, mProcessHeader.SubNo] != null)
						{
							try
							{
								mNetProcesses[mProcessHeader.MainNo, mProcessHeader.SubNo](runningBuffer);
							}
							catch (Exception e)
							{
								mLogger.Log(string.Format("processes[{0},{1}] error -> {0}", mProcessHeader.MainNo, mProcessHeader.SubNo, e.Message));
							}

							OnAnalyze.InvokeGracefully(this, new AnalyzeEventArgs() { mainNo = mProcessHeader.MainNo, subNo = mProcessHeader.SubNo });                          
						}
						else
						{
							mLogger.Log(string.Format("processes[{0},{1}] not create function", mProcessHeader.MainNo, mProcessHeader.SubNo));
						}

						if ((DateTime.Now.Ticks - startTick) > mTimeup && mTimeup > 0)
						{
							mLogger.Log("execute anlyze receive too long");
							break;
						}
					}
				}
				catch (Exception e)
				{
					mLogger.LogException(e);
				}
			}
		}

		protected virtual void DoConnect()
		{
			mLogger.Log("connect");

			Receiving();

			OnConnect.InvokeGracefully(this, new ConnectEventArgs());
		}

		protected virtual void DoDisconnect()
		{
			if (!Connected)
				return;

			mLogger.Log("disconnect");

			try
			{
				mSocket.Shutdown(SocketShutdown.Both);
			}
			catch (Exception e)
			{
				mLogger.Log(e.ToString());
			}

			mSocket.Close();
			mRcvBuffer.Clear();
			mSendBuffer.Clear();

			OnDisconnect.InvokeGracefully(this, new DisconnectEventArgs());
		}

		private void DoReceive(ref byte[] msg, int count)
		{
			if (count > 0)
			{
				lock (mSynRcv)
				{
					mRcvBuffer.Write(msg, 0, count);
				}
				mLogger.Log(string.Format("write {0} Byte to buffer", count));
			}
			OnRcv.InvokeGracefully(this, new RcvEventArgs());
		}

		private void Receiving()
		{
			try
			{                 
				Package package = mPackagePool.NewNode();
				package.socket = mSocket;
				mSocket.BeginReceive(package.buffer, 0, package.length, SocketFlags.None, new AsyncCallback(Received), package);
			}
			catch (SocketException e)
			{
				mLogger.LogException(e);
			}
		}

		private void Received(IAsyncResult asyn)
		{
			try
			{ 
				Package package = (Package)asyn.AsyncState;

				if (!package.socket.Connected)
					return;

				SocketError error = SocketError.TypeNotFound;
				int read = package.socket.EndReceive(asyn, out error);
				if (read == 0)
				{
					//TODO:什麼也不做
				}
				else
				{
					//解析數據
					DoReceive(ref package.buffer, read);
					//封包歸還
					mPackagePool.DisposeNode(package);

					mLogger.Log(string.Format("Received() rcv {0} Bytes", read));
				}
				Receiving();
			}
			catch (Exception e)
			{
				mLogger.LogException(e);
			}
		}

		public virtual void Send(sbyte mainNo, sbyte subNo, ByteArrayBuffer msg)
		{
			if (!Connected)
				return;

			lock (mSynSend)
			{
				try
				{
					mLogger.Log(string.Format("send messgae {0} - {1}", mainNo, subNo));

					if (mPackCompressSize > 0 && mPackCompressSize < msg.Available)
						msg.Compress();

					msg.Encode(mEncryptionCode);

					ByteArrayBuffer header;
					if (OnGenSendHeader == null)
					{
						header = new ByteArrayBuffer();
						header.WriteShort(mVersion);
						header.WriteSByte(mainNo);
						header.WriteSByte(subNo);
						header.WriteBool(mPackCompressSize > 0 && mPackCompressSize < msg.Available);
						header.WriteLong(mSessionId);
						header.WriteInt(msg.Available);
					}
					else
					{
						header = OnGenSendHeader(mVersion, mainNo, subNo, mPackCompressSize > 0 && mPackCompressSize < msg.Available, mSessionId, msg.Available);
					}

					mSendBuffer.Write(header.Copy(), 0, header.Available);
					mSendBuffer.Write(msg.Copy(), 0, msg.Available);

					byte[] buffer = new byte[mSendBuffer.Length];
					mSendBuffer.Read(buffer, 0, buffer.Length);
					mSocket.Send(buffer);

					OnSend.InvokeGracefully(this, new SendEventArgs());
				}
				catch (SocketException e)
				{
					mLogger.LogException(e);
				}
			}
		}

		public void Register(int mainNo, int subNo, NetProcess process)
		{
			mNetProcesses[mainNo, subNo] = process;
		}
	}
}