namespace TinyBee.Net.Buffer
{
	using System.Collections;
	using System.Collections.Generic;
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

		private void WriteToEndian(byte[] value)
		{
			if ((BitConverter.IsLittleEndian && BIGENDIAN) || (!BitConverter.IsLittleEndian && !BIGENDIAN))
				Array.Reverse(value);
			Write(value);
		}

		private void ReadToEndian(byte[] value)
		{
			Read(value);
			if ((BitConverter.IsLittleEndian && BIGENDIAN) || (!BitConverter.IsLittleEndian && !BIGENDIAN))
				Array.Reverse(value);
		}

		public ByteArrayBuffer WriteByteArray(byte[] value)
		{
			if (value == null)
			{
				WriteInt(0);
			}
			else
			{
				WriteInt(value.Length);
				Write(value);
			}
			return this;
		}

		public byte[] ReadByteArray()
		{
			int len = ReadInt();
			byte[] tmp = null;
			if (len > 0)
			{
				tmp = new byte[len];
				Read(tmp);
			}
			return tmp;
		}

		public ByteArrayBuffer WriteSByte(sbyte value)
		{
			byte[] tmp = new byte[1];
			tmp[0] = (byte)value;
			WriteToEndian(tmp);
			return this;
		}

		public sbyte ReadSByte()
		{
			byte[] tmp = new byte[1];
			ReadToEndian(tmp);
			return (sbyte)tmp[0];
		}

		public ByteArrayBuffer WriteShort(short value)
		{
			byte[] tmp = BitConverter.GetBytes(value);
			WriteToEndian(tmp);
			return this;
		}

		public short ReadShort()
		{
			short value = 0;
			byte[] tmp = BitConverter.GetBytes(value);
			ReadToEndian(tmp);
			return BitConverter.ToInt16(tmp, 0);
		}

		public ByteArrayBuffer WriteInt(int value)
		{
			byte[] tmp = BitConverter.GetBytes(value);
			WriteToEndian(tmp);
			return this;
		}

		public int ReadInt()
		{
			int value = 0;
			byte[] tmp = BitConverter.GetBytes(value);
			ReadToEndian(tmp);
			return BitConverter.ToInt32(tmp, 0);            
		}

		public ByteArrayBuffer WriteLong(long value)
		{
			byte[] tmp = BitConverter.GetBytes(value);
			WriteToEndian(tmp);
			return this;
		}

		public long ReadLong()
		{
			long value = 0;
			byte[] tmp = BitConverter.GetBytes(value);
			ReadToEndian(tmp);
			return BitConverter.ToInt64(tmp, 0);  
		}

		public ByteArrayBuffer WriteFloat(float value)
		{
			byte[] tmp = BitConverter.GetBytes(value);
			WriteToEndian(tmp);
			return this;
		}

		public float ReadFloat()
		{
			float value = 0;
			byte[] tmp = BitConverter.GetBytes(value);
			ReadToEndian(tmp);
			return BitConverter.ToSingle(tmp, 0);  
		}

		public ByteArrayBuffer WriteDouble(double data)
		{
			byte[] tmp = BitConverter.GetBytes(data);
			WriteToEndian(tmp);
			return this;
		}

		public double ReadDouble()
		{
			double value = 0;
			byte[] tmp = BitConverter.GetBytes(value);
			ReadToEndian(tmp);
			return BitConverter.ToDouble(tmp, 0); 
		}

		public ByteArrayBuffer WriteChar(char value)
		{
			byte[] tmp = BitConverter.GetBytes(value);
			WriteToEndian(tmp);
			return this;
		}

		public char ReadChar()
		{
			char value = '0';
			byte[] tmp = BitConverter.GetBytes(value);
			ReadToEndian(tmp);
			return BitConverter.ToChar(tmp, 0); 
		}

		public ByteArrayBuffer WriteBool(bool value)
		{
			byte[] tmp = BitConverter.GetBytes(value);
			WriteToEndian(tmp);
			return this;
		}

		public bool ReadBool()
		{
			bool value = false;
			byte[] tmp = BitConverter.GetBytes(value);
			ReadToEndian(tmp);
			return BitConverter.ToBoolean(tmp, 0); 
		}

		public ByteArrayBuffer WriteString(string value)
		{
			if (value == null)
			{
				return WriteByteArray(null);
			}
			else
			{
				byte[] tmp = System.Text.Encoding.UTF8.GetBytes(value);
				return WriteByteArray(tmp);
			}
		}

		public string ReadString()
		{
			byte[] tmp = ReadByteArray();
			return tmp != null ? System.Text.Encoding.UTF8.GetString(tmp) : null;
		}

		public ByteArrayBuffer WriteDateTime(DateTime value)
		{
			return WriteLong(value.AddHours(-8).Ticks - TTime.TICK1970);
		}

		public DateTime ReadDateTime()
		{
			return new DateTime(ReadLong() * 10000 + TTime.TICK1970).AddHours(8);
		}

		private MemberAttribute getMember(FieldInfo field)
		{
			if (field != null)
			{
				object[] atts = field.GetCustomAttributes(false);
				foreach (MemberAttribute member in atts)
					return member;
			}
			return null;
		}

		private FieldInfo[] sortMember(FieldInfo[] fields)
		{
			List<FieldInfo> list = new List<FieldInfo>();
			foreach (FieldInfo field in fields)
			{
				MemberAttribute member = getMember(field);
				if (member != null)
				{
					if (list.Count < member.Order)
					{
						list.Add(field);
					}
					else
					{
						list.Insert(member.Order - 1, field);
					}
				}
			}
			return list.ToArray();
		}

		private ByteArrayBuffer WriteValue(object value)
		{
			Type type = value.GetType();
			if (type == Type.GetType("System.Boolean"))
				return WriteBool((bool)value);
			else if (type == Type.GetType("System.Char"))
				return WriteChar((char)value);
			else if (type == Type.GetType("System.SByte"))
				return WriteSByte((sbyte)value);
			else if (type == Type.GetType("System.Int16"))
				return WriteShort((short)value);
			else if (type == Type.GetType("System.Int32"))
				return WriteInt((int)value);
			else if (type == Type.GetType("System.Int64"))
				return WriteLong((long)value);
			else if (type == Type.GetType("System.Single"))
				return WriteFloat((float)value);
			else if (type == Type.GetType("System.Double"))
				return WriteDouble((double)value);
			else if (type == Type.GetType("System.String"))
				return WriteString((string)value);
			else if (type == Type.GetType("System.DateTime"))
				return WriteDateTime((DateTime)value);
			FieldInfo[] feilds = sortMember(value.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
			for (int i = 0; i < feilds.Length; i++)
			{
				if (feilds[i].FieldType.IsArray)
				{
					Array array = (Array)feilds[i].GetValue(value);
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							WriteValue(array.GetValue(j));
						}
					}
				}
				else if (!feilds[i].IsLiteral)
				{
					WriteValue(feilds[i].GetValue(value));
				}
			}
			return this;
		}

		private object ReadValue(Type type)
		{
			if (type == Type.GetType("System.Boolean"))
				return ReadBool();
			else if (type == Type.GetType("System.Char"))
				return ReadChar();
			else if (type == Type.GetType("System.SByte"))
				return ReadSByte();
			else if (type == Type.GetType("System.Int16"))
				return ReadShort();
			else if (type == Type.GetType("System.Int32"))
				return ReadInt();
			else if (type == Type.GetType("System.Int64"))
				return ReadLong();
			else if (type == Type.GetType("System.Single"))
				return ReadFloat();
			else if (type == Type.GetType("System.Double"))
				return ReadDouble();
			else if (type == Type.GetType("System.String"))
				return ReadString();
			else if (type == Type.GetType("System.DateTime"))
				return ReadDateTime();
			object value = Activator.CreateInstance(type);
			FieldInfo[] feilds = sortMember(value.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
			for (int i = 0; i < feilds.Length; i++)
			{
				Type fieldType = feilds[i].FieldType;
				if (feilds[i].FieldType.IsArray)
				{
					MemberAttribute member = getMember(feilds[i]);
					if (member != null)
					{
						int len = member.Length;
						Type arrayType = feilds[i].FieldType.GetElementType();
						Array array = Array.CreateInstance(arrayType, len);
						feilds[i].SetValue(value, array);
						for (int j = 0; j < array.Length; j++)
						{
							array.SetValue(ReadValue(arrayType), j);
						}
					}
				}
				else if (!feilds[i].IsLiteral)
				{
					feilds[i].SetValue(value, ReadValue(fieldType));
				}
			}
			return value;
		}

		public ByteArrayBuffer WriteStruct<T>(T value)
		{
			try
			{
				return WriteValue(value);
			}
			catch (Exception e)
			{
				mLogger.LogException(e);
			}
			return this;
		}

		public T ReadStruct<T>()
		{
			try
			{
				return (T)ReadValue(typeof(T));
			}
			catch (Exception e)
			{
				mLogger.LogException(e);
			}
			return default(T);
		}

		public void WriteBufToJson<T>(T data)
		{
			string json = TJson.SerializeObject(data);
			WriteString(json);
		}

		public void ReadBufToJson<T>(ref T value)
		{
			string json = ReadString();
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