namespace TinyBee.Net.Buffer
{
	using System;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using ILogger = TinyBee.Logger.ILogger;
	using TLogger = TinyBee.Logger.TLogger;

	public sealed class ByteArrayBuffer : ArrayBuffer<Byte>
	{
		private ILogger mLogger = TLogger.Instance;

		private const bool BIGENDIAN = true;
		private const int BUFFERSIZE = 10 * 1024;

		public ByteArrayBuffer() : base(BUFFERSIZE) {}
		public ByteArrayBuffer(byte[] buf) : base(buf, 0, buf.Length) {}

		private void WriteToEndian(byte[] data)
		{
			if ((BitConverter.IsLittleEndian && BIGENDIAN) || (!BitConverter.IsLittleEndian && !BIGENDIAN))
				Array.Reverse(data);
			Write(data);
		}

		private void ReadToEndian(byte[] data)
		{
			Read(data);
			if ((BitConverter.IsLittleEndian && BIGENDIAN) || (!BitConverter.IsLittleEndian && !BIGENDIAN))
				Array.Reverse(data);
		}

		public void WriteByteArray(byte[] data)
		{
			WriteInt(data.Length);
			WriteToEndian(data);
		}

		public byte[] ReadByteArray()
		{
			int len = ReadInt();
			byte[] tmp = new byte[len];
			ReadToEndian(tmp);
			return tmp;
		}

		public void WriteByte(byte data)
		{
			byte[] tmp = new byte[1];
			tmp[0] = data;
			WriteToEndian(tmp);
		}

		public byte ReadByte()
		{
			byte[] tmp = new byte[1];
			ReadToEndian(tmp);
			return tmp[0];
		}

		public void WriteSByte(sbyte data)
		{
			byte[] tmp = new byte[1];
			tmp[0] = (byte)data;
			WriteToEndian(tmp);
		}

		public sbyte ReadSByte()
		{
			byte[] tmp = new byte[1];
			ReadToEndian(tmp);
			return (sbyte)tmp[0];
		}

		public void WriteShort(short data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public short ReadShort()
		{
			short data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToInt16(tmp, 0);
		}

		public void WriteUShort(ushort data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public ushort ReadUShort()
		{
			ushort data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToUInt16(tmp, 0);
		}

		public void WriteInt(int data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public int ReadInt()
		{
			int data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToInt32(tmp, 0);            
		}

		public void WriteUInt(uint data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public uint ReadUInt()
		{
			uint data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToUInt32(tmp, 0);  
		}

		public void WriteLong(long data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public long ReadLong()
		{
			long data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToInt64(tmp, 0);  
		}

		public void WriteULong(ulong data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public ulong ReadULong()
		{
			ulong data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToUInt64(tmp, 0);  
		}

		public void WriteFloat(float data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public float ReadFloat()
		{
			float data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToSingle(tmp, 0);  
		}

		public void WriteDouble(double data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public double ReadDouble()
		{
			double data = 0;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToDouble(tmp, 0); 
		}

		public void WriteChar(char data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public char ReadChar()
		{
			char data = '0';
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToChar(tmp, 0); 
		}

		public void WriteBool(bool data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
		}

		public bool ReadBool()
		{
			bool data = false;
			byte[] tmp = BitConverter.GetBytes(data);
			ReadToEndian(tmp);
			return BitConverter.ToBoolean(tmp, 0); 
		}

		public void WriteStringByByte(string data)
		{
			byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data);
			WriteByte((byte)tmp.Length);
			WriteToEndian(tmp);
		}

		public string ReadStringByByte()
		{
			byte len = ReadByte();
			byte[] tmp = new byte[len];
			ReadToEndian(tmp);
			return System.Text.Encoding.UTF8.GetString(tmp);
		}

		public void WriteStringByUShort(string data)
		{
			byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data);
			WriteUShort((ushort)tmp.Length);
			WriteToEndian(tmp);
		}

		public string ReadStringByUShort()
		{
			ushort len = ReadUShort();
			byte[] tmp = new byte[len];
			ReadToEndian(tmp);
			return System.Text.Encoding.UTF8.GetString(tmp);
		}

		public void WriteStringByInt(string data)
		{
			byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data);
			WriteInt(tmp.Length);
			WriteToEndian(tmp);
		}

		public string ReadStringByInt()
		{
			int len = ReadInt();
			byte[] tmp = new byte[len];
			ReadToEndian(tmp);
			return System.Text.Encoding.UTF8.GetString(tmp);
		}

		public void WriteDateTime(DateTime data)
		{
			//要知道這行為什麼是這個數值要先知道幾件事
			//Delphi中TDateTime初始時間為1899/12/30 00:00:00
			//C#中DateTime的初始值為0001/01/01 00:00:00
			//DateTime是初始日期(Ticks=0) + offset而成
			const double DAYS_BETWEEN_00010101_AND_18991230 = 693593;
			//24 * 60 * 60 * 1000 * 1000 * 10
			const double TIME_UNIT = 864000000000;
			//C# DateTime 為 delphi中所存的日期 + C#與Delphi日期格式初始值的差異
			double doubleTime = data.Ticks / TIME_UNIT - DAYS_BETWEEN_00010101_AND_18991230;
			WriteDouble(doubleTime);
		}

		public DateTime ReadDateTime()
		{
			double doubleTime = ReadDouble();
			//要知道這行為什麼是這個數值要先知道幾件事
			//Delphi中TDateTime初始時間為1899/12/30 00:00:00
			//C#中DateTime的初始值為0001/01/01 00:00:00
			//DateTime是初始日期(Ticks=0) + offset而成
			const double DAYS_BETWEEN_00010101_AND_18991230 = 693593;
			//24 * 60 * 60 * 1000 * 1000 * 10
			const double TIME_UNIT = 864000000000;
			//C# DateTime 為 delphi中所存的日期 + C#與Delphi日期格式初始值的差異
			return new DateTime((long)((doubleTime + DAYS_BETWEEN_00010101_AND_18991230) * TIME_UNIT));
		}

		private void WriteValue(object value)
		{
			System.IConvertible convertible = value as System.IConvertible;
			if (convertible != null)
			{
				switch (convertible.GetTypeCode())
				{
					case System.TypeCode.Boolean:
						WriteBool((bool)value);
						return;
					case System.TypeCode.Char:
						WriteChar((char)value);
						return;
					case System.TypeCode.SByte:
						WriteSByte((sbyte)value);
						return;
					case System.TypeCode.Byte:
						WriteByte((byte)value);
						return;
					case System.TypeCode.Int16:
						WriteShort((short)value);
						return;
					case System.TypeCode.UInt16:
						WriteUShort((ushort)value);
						return;
					case System.TypeCode.Int32:
						WriteInt((int)value);
						return;
					case System.TypeCode.UInt32:
						WriteUInt((uint)value);
						return;
					case System.TypeCode.Int64:
						WriteLong((long)value);
						return;
					case System.TypeCode.UInt64:
						WriteULong((ulong)value);
						return;
					case System.TypeCode.Single:
						WriteFloat((float)value);
						return;
					case System.TypeCode.Double:
						WriteDouble((double)value);
						return;
					case System.TypeCode.DateTime:
						WriteDouble(((DateTime)value).ToOADate());
						return;
					case System.TypeCode.String:
						return;
				}
			}

			FieldInfo[] f = value.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
			System.Array.Sort(f, (FieldInfo a, FieldInfo b) => { return a.MetadataToken - b.MetadataToken; });
			for (int i = 0; i < f.Length; i++)
			{
				if (f[i].FieldType.IsArray)
				{
					Array array = (Array)f[i].GetValue(value);
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							WriteValue(array.GetValue(j));
						}
					}
				}
				else if (!f[i].IsLiteral)
				{
					WriteValue(f[i].GetValue(value));
				}
			}
		}

		private void ReadValue(ref object value)
		{
			System.IConvertible convertible = value as System.IConvertible;
			if (convertible != null)
			{
				switch (convertible.GetTypeCode())
				{
					case System.TypeCode.Boolean:
						value = ReadBool();
						return;
					case System.TypeCode.Char:
						value = ReadChar();
						return;
					case System.TypeCode.SByte:
						value = ReadSByte();
						return;
					case System.TypeCode.Byte:
						value = ReadByte();
						return;
					case System.TypeCode.Int16:
						value = ReadShort();
						return;
					case System.TypeCode.UInt16:
						value = ReadUShort();
						return;
					case System.TypeCode.Int32:
						value = ReadInt();
						return;
					case System.TypeCode.UInt32:
						value = ReadUInt();
						return;
					case System.TypeCode.Int64:
						value = ReadLong();
						return;
					case System.TypeCode.UInt64:
						value = ReadULong();
						return;
					case System.TypeCode.Single:
						value = ReadFloat();
						return;
					case System.TypeCode.Double:
						value = ReadDouble();
						return;
					case System.TypeCode.DateTime:
						value = DateTime.FromOADate(ReadDouble());
						return;
					case System.TypeCode.String:
						return;
				}
			}

			FieldInfo[] f = value.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
			System.Array.Sort(f, (FieldInfo a, FieldInfo b) => { return a.MetadataToken - b.MetadataToken; });
			for (int i = 0; i < f.Length; i++)
			{
				if (f[i].FieldType.IsArray == true)
				{
					Array array = (Array)f[i].GetValue(value);
					if (array == null)
					{
						foreach (FieldAttribute attf in f[i].GetCustomAttributes(false))
						{
							int size = attf.Length;
							array = System.Array.CreateInstance(f[i].FieldType.GetElementType(), size);
							f[i].SetValue(value, array);
							break;
						}
					}

					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							object temp = array.GetValue(j);
							ReadValue(ref temp);
							array.SetValue(temp, j);
						}
					}
				}
				else if (!f[i].IsLiteral)
				{
					object temp = f[i].GetValue(value);
					if (temp == null)
						temp = Activator.CreateInstance(temp.GetType());

					ReadValue(ref temp);

					f[i].SetValue(value, temp);
				}
			}
		}

		public void WriteStruct<T>(T value)
		{
			try
			{
				WriteValue(value);
			}
			catch (Exception e)
			{
				mLogger.LogException(e);
			}
		}

		public void ReadStruct<T>(ref T value)
		{
			try
			{
				object temp = value;
				ReadValue(ref temp);
				value = (T)temp;
			}
			catch (Exception e)
			{
				mLogger.LogException(e);
			}
		}

		public void WriteBufToJson<T>(T data)
		{
			string json = TJson.SerializeObject(data);
			WriteStringByInt(json);
		}

		public void ReadBufToJSON<T>(ref T value)
		{
			string json = ReadStringByInt();
			try
			{
				value = TJson.DeserializeObject<T>(json);
			}
			catch (Exception e)
			{
				mLogger.LogException(e);
			}
		}

		public void Compress()
		{
			byte[] data = Copy();
			byte[] compressData = TCompression.CompressBytes(data);
			CreateBuf(compressData, 0, compressData.Length);
		}

		public void Decompress()
		{
			byte[] data = ReadByteArray();
			byte[] deccompressData = TCompression.DecompressBytes(data);
			if (deccompressData.Length > Capacity)
			{
				CreateBuf(deccompressData, 0, deccompressData.Length);
			}
			else
			{
				Write(deccompressData, 0, deccompressData.Length);
			}
		}

		public void Encode(byte code)
		{
			if (code == 0)
				return;

			for (int i = 0 ; i < Available ; i++)
				mBuffer[i] ^= code;
		}

		public void Decode(byte code)
		{
			if (code == 0)
				return;

			for (int i = 0; i < Available; i++)
				mBuffer[i] ^= code;
		}
	}
}