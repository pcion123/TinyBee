namespace TinyBee
{
	using System;
	using System.IO;
	using ICSharpCode.SharpZipLib.Zip;

	public static class TCompression : object
	{
		private static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[2000];
			int len = 0;
			while ((len = input.Read(buffer, 0, 2000)) != 0)
			{
				output.Write(buffer, 0, len);
			}
			output.Flush();
		}

		public static Stream CompressStream(Stream srcStream)
		{
			try
			{
				MemoryStream stmOutTemp = new MemoryStream();
				ZipOutputStream outZStream = new ZipOutputStream(stmOutTemp);
				CopyStream(srcStream, outZStream);
				outZStream.Close();
				return stmOutTemp;
			}
			catch (Exception e)
			{
				TinyBee.Logger.TLogger.Instance.LogException(e);
			}
			return null;
		}

		public static Stream DecompressStream(Stream srcStream)
		{
			try
			{
				MemoryStream stmOutput = new MemoryStream();
				ZipOutputStream outZStream = new ZipOutputStream(stmOutput);
				CopyStream(srcStream, outZStream);
				outZStream.Close();
				return stmOutput;
			}
			catch (Exception e)
			{
				TinyBee.Logger.TLogger.Instance.LogException(e);
			}
			return null;
		}

		public static byte[] CompressBytes(byte[] SourceByte)
		{
			try
			{
				MemoryStream stmInput = new MemoryStream(SourceByte);
				Stream stmOutPut = CompressStream(stmInput);
				byte[] bytOutPut = new byte[stmOutPut.Length];
				stmOutPut.Position = 0;
				stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
				return bytOutPut;
			}
			catch (Exception e)
			{
				TinyBee.Logger.TLogger.Instance.LogException(e);
			}
			return null;
		}

		public static byte[] DecompressBytes(byte[] SourceByte)
		{
			try
			{
				MemoryStream stmInput = new MemoryStream(SourceByte);
				Stream stmOutPut = DecompressStream(stmInput);
				byte[] bytOutPut = new byte[stmOutPut.Length];
				stmOutPut.Position = 0;
				stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
				return bytOutPut;
			}
			catch (Exception e)
			{
				TinyBee.Logger.TLogger.Instance.LogException(e);
			}
			return null;
		}
	}
}
