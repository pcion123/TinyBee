namespace TinyBee.Net
{
	using TinyBee.Net.Buffer;

	public class Record
	{
		public bool Waitting { get; set; }
		public sbyte SerialId { get; set; }
		public ByteArrayBuffer Msg { get; set; }

		//建構子
		public Record()
		{
			Waitting = false;
			SerialId = 0;
			Msg = new ByteArrayBuffer();
		}

		//清除內容
		public void Clear()
		{
			Waitting = false;
			SerialId = 0;

			if (Msg != null)
				Msg.Clear();
		}
	}
}