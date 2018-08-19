namespace TinyBee.Net
{
	using TinyBee.Net.Buffer;

	public class Record
	{
		public bool IsWaitting { get; set; }
		public byte MainNo { get; set; }
		public byte SubNo { get; set; }
		public byte SerialId { get; set; }
		public ByteArrayBuffer Msg { get; set; }

		//建構子
		public Record()
		{
			IsWaitting = false;
			MainNo = 0;
			SubNo = 0;
			SerialId = 0;
			Msg = new ByteArrayBuffer();
		}

		//清除內容
		public void Clear()
		{
			IsWaitting = false;
			MainNo = 0;
			SubNo = 0;
			SerialId = 0;
			Msg.Clear();
		}
	}
}