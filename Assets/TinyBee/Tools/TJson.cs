namespace TinyBee
{
	public static class TJson : object
	{
		//Json反序列化
		public static T DeserializeObject<T> (string vJson)
		{
			return LitJson.JsonMapper.ToObject<T>(vJson);
		}

		//Json序列化
		public static string SerializeObject (object obj)
		{
			return LitJson.JsonMapper.ToJson(obj);
		}
	}
}
