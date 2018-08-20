namespace TinyBee.Net
{
	using System;

	public class MemberAttribute : Attribute
	{
		public int Order;
		public int Length;

		public MemberAttribute(int order, int length = 1)
		{
			Order = order;
			Length = length;
		}
	}
}