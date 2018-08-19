namespace TinyBee.Net
{
	public class Package
	{
		public System.Net.Sockets.Socket socket;
		public byte[] buffer = new byte[1024];
		public int length { get { return buffer != null ? buffer.Length : 0; } }
	}

	public class PackagePool : Cache<Package>
	{
		public PackagePool() : base(1024) {}
	}
}
