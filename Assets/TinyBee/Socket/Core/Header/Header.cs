namespace TinyBee.Net
{
	public class HeaderBase
	{
		public static int Size { get { return 17; } }

		public short Version { get; set; }
		public byte MainNo { get; set; }
		public byte SubNo { get; set; }
		public bool IsCompress { get; set; }
		public long SessionId { get; set; }
		public int Len { get; set; }

		public HeaderBase() {}

		public HeaderBase(short version, byte mainNo, byte subNo, bool isCompress, long sessionId, int len)
		{
			Version = version;
			MainNo = mainNo;
			SubNo = subNo;
			IsCompress = isCompress;
			SessionId = sessionId;
			Len = len;
		}
	}
}