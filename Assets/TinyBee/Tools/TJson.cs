namespace TinyBee
{
	public static class TJson : object
	{
		//Json反序列化
		public static T DeserializeObject<T>(string json)
		{
			return LitJson.JsonMapper.ToObject<T>(json);
		}

		//Json序列化
		public static string SerializeObject(object obj)
		{
			return LitJson.JsonMapper.ToJson(obj);
		}
	}
}
