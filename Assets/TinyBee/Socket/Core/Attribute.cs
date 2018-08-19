namespace TinyBee.Net
{
	using System;

	public class StructureAttribute : Attribute
	{
		public int TotalSize;

		public StructureAttribute(int totalSize)
		{
			TotalSize = totalSize;
		}
	}

	public class FieldAttribute : Attribute
	{
		public int Size;
		public int Length;

		public FieldAttribute(int size, int length = 1)
		{
			Size = size;
			Length = length;
		}
	}
}