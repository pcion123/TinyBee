namespace TinyBee.Net
{
	using System;
	using TinyBee.Net.Buffer;

	public delegate HeaderBase RcvHeader (ByteArrayBuffer buffer);
	public delegate ByteArrayBuffer SendHeader (short version, sbyte mainNo, sbyte subNo, bool isCompress, long sessionId, int len);
	public delegate void NetProcess (ByteArrayBuffer msg);
	public delegate void NetEvent (object sender, EventArgs e);

	public class ConnectEventArgs : EventArgs {}
	public class DisconnectEventArgs : EventArgs {}
	public class RcvEventArgs : EventArgs {}
	public class SendEventArgs : EventArgs {}
	public class ErrorEventArgs : EventArgs {}
	public class AnalyzeEventArgs : EventArgs
	{
		public sbyte mainNo;
		public sbyte subNo;
	}
}
