using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MessagePackLib.MessagePack
{
	// Token: 0x02000018 RID: 24
	public class MsgPack : IEnumerable
	{
		// Token: 0x0600006A RID: 106 RVA: 0x000023E6 File Offset: 0x000005E6
		private void SetName(string value)
		{
			this.name = value;
			this.lowerName = this.name.ToLower();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000469C File Offset: 0x0000289C
		private void Clear()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Clear();
			}
			this.children.Clear();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000046E4 File Offset: 0x000028E4
		private MsgPack InnerAdd()
		{
			MsgPack msgPack = new MsgPack();
			msgPack.parent = this;
			this.children.Add(msgPack);
			return msgPack;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000470C File Offset: 0x0000290C
		private int IndexOf(string name)
		{
			int num = -1;
			int num2 = -1;
			string text = name.ToLower();
			foreach (MsgPack msgPack in this.children)
			{
				num++;
				if (text.Equals(msgPack.lowerName))
				{
					num2 = num;
					break;
				}
			}
			return num2;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000478C File Offset: 0x0000298C
		public MsgPack FindObject(string name)
		{
			int num = this.IndexOf(name);
			if (num == -1)
			{
				return null;
			}
			return this.children[num];
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002400 File Offset: 0x00000600
		private MsgPack InnerAddMapChild()
		{
			if (this.valueType != MsgPackType.Map)
			{
				this.Clear();
				this.valueType = MsgPackType.Map;
			}
			return this.InnerAdd();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00002421 File Offset: 0x00000621
		private MsgPack InnerAddArrayChild()
		{
			if (this.valueType != MsgPackType.Array)
			{
				this.Clear();
				this.valueType = MsgPackType.Array;
			}
			return this.InnerAdd();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002442 File Offset: 0x00000642
		public MsgPack AddArrayChild()
		{
			return this.InnerAddArrayChild();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000047B8 File Offset: 0x000029B8
		private void WriteMap(Stream ms)
		{
			int count = this.children.Count;
			if (count <= 15)
			{
				byte b = 128 + (byte)count;
				ms.WriteByte(b);
			}
			else if (count <= 65535)
			{
				byte b = 222;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes((short)count));
				ms.Write(array, 0, array.Length);
			}
			else
			{
				byte b = 223;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes(count));
				ms.Write(array, 0, array.Length);
			}
			for (int i = 0; i < count; i++)
			{
				WriteTools.WriteString(ms, this.children[i].name);
				this.children[i].Encode2Stream(ms);
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004884 File Offset: 0x00002A84
		private void WirteArray(Stream ms)
		{
			int count = this.children.Count;
			if (count <= 15)
			{
				byte b = 144 + (byte)count;
				ms.WriteByte(b);
			}
			else if (count <= 65535)
			{
				byte b = 220;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes((short)count));
				ms.Write(array, 0, array.Length);
			}
			else
			{
				byte b = 221;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes(count));
				ms.Write(array, 0, array.Length);
			}
			for (int i = 0; i < count; i++)
			{
				this.children[i].Encode2Stream(ms);
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000244A File Offset: 0x0000064A
		public void SetAsInteger(long value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.Integer;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000245F File Offset: 0x0000065F
		public void SetAsUInt64(ulong value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.UInt64;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004938 File Offset: 0x00002B38
		public ulong GetAsUInt64()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return ulong.Parse(this.innerValue.ToString().Trim());
			case MsgPackType.Integer:
				return Convert.ToUInt64((long)this.innerValue);
			case MsgPackType.UInt64:
				return (ulong)this.innerValue;
			case MsgPackType.Float:
				return Convert.ToUInt64((double)this.innerValue);
			case MsgPackType.Single:
				return Convert.ToUInt64((float)this.innerValue);
			case MsgPackType.DateTime:
				return Convert.ToUInt64((DateTime)this.innerValue);
			}
			return 0UL;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000049E0 File Offset: 0x00002BE0
		public long GetAsInteger()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return long.Parse(this.innerValue.ToString().Trim());
			case MsgPackType.Integer:
				return (long)this.innerValue;
			case MsgPackType.UInt64:
				return Convert.ToInt64((long)this.innerValue);
			case MsgPackType.Float:
				return Convert.ToInt64((double)this.innerValue);
			case MsgPackType.Single:
				return Convert.ToInt64((float)this.innerValue);
			case MsgPackType.DateTime:
				return Convert.ToInt64((DateTime)this.innerValue);
			}
			return 0L;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004A88 File Offset: 0x00002C88
		public double GetAsFloat()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return double.Parse((string)this.innerValue);
			case MsgPackType.Integer:
				return Convert.ToDouble((long)this.innerValue);
			case MsgPackType.Float:
				return (double)this.innerValue;
			case MsgPackType.Single:
				return (double)((float)this.innerValue);
			case MsgPackType.DateTime:
				return (double)Convert.ToInt64((DateTime)this.innerValue);
			}
			return 0.0;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00002474 File Offset: 0x00000674
		public void SetAsBytes(byte[] value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.Binary;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004B1C File Offset: 0x00002D1C
		public byte[] GetAsBytes()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return BytesTools.GetUtf8Bytes(this.innerValue.ToString());
			case MsgPackType.Integer:
				return BitConverter.GetBytes((long)this.innerValue);
			case MsgPackType.Float:
				return BitConverter.GetBytes((double)this.innerValue);
			case MsgPackType.Single:
				return BitConverter.GetBytes((float)this.innerValue);
			case MsgPackType.DateTime:
				return BitConverter.GetBytes(((DateTime)this.innerValue).ToBinary());
			case MsgPackType.Binary:
				return (byte[])this.innerValue;
			}
			return new byte[0];
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00002485 File Offset: 0x00000685
		public void Add(string key, string value)
		{
			MsgPack msgPack = this.InnerAddArrayChild();
			msgPack.name = key;
			msgPack.SetAsString(value);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000249A File Offset: 0x0000069A
		public void Add(string key, int value)
		{
			MsgPack msgPack = this.InnerAddArrayChild();
			msgPack.name = key;
			msgPack.SetAsInteger((long)value);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004BCC File Offset: 0x00002DCC
		public bool LoadFileAsBytes(string fileName)
		{
			if (File.Exists(fileName))
			{
				FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				fileStream.Close();
				fileStream.Dispose();
				this.SetAsBytes(array);
				return true;
			}
			return false;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004C24 File Offset: 0x00002E24
		public bool SaveBytesToFile(string fileName)
		{
			if (this.innerValue != null)
			{
				FileStream fileStream = new FileStream(fileName, FileMode.Append);
				fileStream.Write((byte[])this.innerValue, 0, ((byte[])this.innerValue).Length);
				fileStream.Close();
				fileStream.Dispose();
				return true;
			}
			return false;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004C70 File Offset: 0x00002E70
		public MsgPack ForcePathObject(string path)
		{
			MsgPack msgPack = this;
			string[] array = path.Trim().Split(new char[] { '.', '/', '\\' });
			if (array.Length == 0)
			{
				return null;
			}
			string text;
			if (array.Length > 1)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					text = array[i];
					MsgPack msgPack2 = msgPack.FindObject(text);
					if (msgPack2 == null)
					{
						msgPack = msgPack.InnerAddMapChild();
						msgPack.SetName(text);
					}
					else
					{
						msgPack = msgPack2;
					}
				}
			}
			text = array[array.Length - 1];
			int num = msgPack.IndexOf(text);
			if (num > -1)
			{
				return msgPack.children[num];
			}
			msgPack = msgPack.InnerAddMapChild();
			msgPack.SetName(text);
			return msgPack;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000024B0 File Offset: 0x000006B0
		public void SetAsNull()
		{
			this.Clear();
			this.innerValue = null;
			this.valueType = MsgPackType.Null;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000024C6 File Offset: 0x000006C6
		public void SetAsString(string value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.String;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000024D6 File Offset: 0x000006D6
		public string GetAsString()
		{
			if (this.innerValue == null)
			{
				return "";
			}
			return this.innerValue.ToString();
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000024F4 File Offset: 0x000006F4
		public void SetAsBoolean(bool bVal)
		{
			this.valueType = MsgPackType.Boolean;
			this.innerValue = bVal;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002509 File Offset: 0x00000709
		public void SetAsSingle(float fVal)
		{
			this.valueType = MsgPackType.Single;
			this.innerValue = fVal;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000251F File Offset: 0x0000071F
		public void SetAsFloat(double fVal)
		{
			this.valueType = MsgPackType.Float;
			this.innerValue = fVal;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004D30 File Offset: 0x00002F30
		public void DecodeFromBytes(byte[] bytes)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bytes = Zip.Decompress(bytes);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Position = 0L;
				this.DecodeFromStream(memoryStream);
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004D88 File Offset: 0x00002F88
		public void DecodeFromFile(string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Open);
			this.DecodeFromStream(fileStream);
			fileStream.Dispose();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004DAC File Offset: 0x00002FAC
		public void DecodeFromStream(Stream ms)
		{
			byte b = (byte)ms.ReadByte();
			if (b <= 127)
			{
				this.SetAsInteger((long)((ulong)b));
				return;
			}
			if (b >= 128 && b <= 143)
			{
				this.Clear();
				this.valueType = MsgPackType.Map;
				int num = (int)(b - 128);
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack = this.InnerAdd();
					msgPack.SetName(ReadTools.ReadString(ms));
					msgPack.DecodeFromStream(ms);
				}
				return;
			}
			if (b >= 144 && b <= 159)
			{
				this.Clear();
				this.valueType = MsgPackType.Array;
				int num = (int)(b - 144);
				for (int i = 0; i < num; i++)
				{
					this.InnerAdd().DecodeFromStream(ms);
				}
				return;
			}
			if (b >= 160 && b <= 191)
			{
				int num = (int)(b - 160);
				this.SetAsString(ReadTools.ReadString(ms, num));
				return;
			}
			if (b >= 224 && b <= 255)
			{
				this.SetAsInteger((long)((sbyte)b));
				return;
			}
			if (b == 192)
			{
				this.SetAsNull();
				return;
			}
			if (b == 193)
			{
				throw new Exception("(never used) type $c1");
			}
			if (b == 194)
			{
				this.SetAsBoolean(false);
				return;
			}
			if (b == 195)
			{
				this.SetAsBoolean(true);
				return;
			}
			if (b == 196)
			{
				int num = ms.ReadByte();
				byte[] array = new byte[num];
				ms.Read(array, 0, num);
				this.SetAsBytes(array);
				return;
			}
			if (b == 197)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToUInt16(array, 0);
				array = new byte[num];
				ms.Read(array, 0, num);
				this.SetAsBytes(array);
				return;
			}
			if (b == 198)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				int num = BitConverter.ToInt32(array, 0);
				array = new byte[num];
				ms.Read(array, 0, num);
				this.SetAsBytes(array);
				return;
			}
			if (b == 199 || b == 200 || b == 201)
			{
				throw new Exception("(ext8,ext16,ex32) type $c7,$c8,$c9");
			}
			if (b == 202)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				this.SetAsSingle(BitConverter.ToSingle(array, 0));
				return;
			}
			if (b == 203)
			{
				byte[] array = new byte[8];
				ms.Read(array, 0, 8);
				array = BytesTools.SwapBytes(array);
				this.SetAsFloat(BitConverter.ToDouble(array, 0));
				return;
			}
			if (b == 204)
			{
				b = (byte)ms.ReadByte();
				this.SetAsInteger((long)((ulong)b));
				return;
			}
			if (b == 205)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)((ulong)BitConverter.ToUInt16(array, 0)));
				return;
			}
			if (b == 206)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)((ulong)BitConverter.ToUInt32(array, 0)));
				return;
			}
			if (b == 207)
			{
				byte[] array = new byte[8];
				ms.Read(array, 0, 8);
				array = BytesTools.SwapBytes(array);
				this.SetAsUInt64(BitConverter.ToUInt64(array, 0));
				return;
			}
			if (b == 220)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Array;
				for (int i = 0; i < num; i++)
				{
					this.InnerAdd().DecodeFromStream(ms);
				}
				return;
			}
			if (b == 221)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Array;
				for (int i = 0; i < num; i++)
				{
					this.InnerAdd().DecodeFromStream(ms);
				}
				return;
			}
			if (b == 217)
			{
				this.SetAsString(ReadTools.ReadString(b, ms));
				return;
			}
			if (b == 222)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Map;
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack2 = this.InnerAdd();
					msgPack2.SetName(ReadTools.ReadString(ms));
					msgPack2.DecodeFromStream(ms);
				}
				return;
			}
			if (b == 222)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Map;
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack3 = this.InnerAdd();
					msgPack3.SetName(ReadTools.ReadString(ms));
					msgPack3.DecodeFromStream(ms);
				}
				return;
			}
			if (b == 223)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				int num = BitConverter.ToInt32(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Map;
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack4 = this.InnerAdd();
					msgPack4.SetName(ReadTools.ReadString(ms));
					msgPack4.DecodeFromStream(ms);
				}
				return;
			}
			if (b == 218)
			{
				this.SetAsString(ReadTools.ReadString(b, ms));
				return;
			}
			if (b == 219)
			{
				this.SetAsString(ReadTools.ReadString(b, ms));
				return;
			}
			if (b == 208)
			{
				this.SetAsInteger((long)((sbyte)ms.ReadByte()));
				return;
			}
			if (b == 209)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)BitConverter.ToInt16(array, 0));
				return;
			}
			if (b == 210)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)BitConverter.ToInt32(array, 0));
				return;
			}
			if (b == 211)
			{
				byte[] array = new byte[8];
				ms.Read(array, 0, 8);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger(BitConverter.ToInt64(array, 0));
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000053D4 File Offset: 0x000035D4
		public byte[] Encode2Bytes()
		{
			byte[] array2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.Encode2Stream(memoryStream);
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				memoryStream.Read(array, 0, (int)memoryStream.Length);
				array2 = Zip.Compress(array);
			}
			return array2;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005440 File Offset: 0x00003640
		public void Encode2Stream(Stream ms)
		{
			switch (this.valueType)
			{
			case MsgPackType.Unknown:
			case MsgPackType.Null:
				WriteTools.WriteNull(ms);
				return;
			case MsgPackType.Map:
				this.WriteMap(ms);
				return;
			case MsgPackType.Array:
				this.WirteArray(ms);
				return;
			case MsgPackType.String:
				WriteTools.WriteString(ms, (string)this.innerValue);
				return;
			case MsgPackType.Integer:
				WriteTools.WriteInteger(ms, (long)this.innerValue);
				return;
			case MsgPackType.UInt64:
				WriteTools.WriteUInt64(ms, (ulong)this.innerValue);
				return;
			case MsgPackType.Boolean:
				WriteTools.WriteBoolean(ms, (bool)this.innerValue);
				return;
			case MsgPackType.Float:
				WriteTools.WriteFloat(ms, (double)this.innerValue);
				return;
			case MsgPackType.Single:
				WriteTools.WriteFloat(ms, (double)((float)this.innerValue));
				return;
			case MsgPackType.DateTime:
				WriteTools.WriteInteger(ms, this.GetAsInteger());
				return;
			case MsgPackType.Binary:
				WriteTools.WriteBinary(ms, (byte[])this.innerValue);
				return;
			default:
				WriteTools.WriteNull(ms);
				return;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00002534 File Offset: 0x00000734
		// (set) Token: 0x0600008C RID: 140 RVA: 0x0000253C File Offset: 0x0000073C
		public string AsString
		{
			get
			{
				return this.GetAsString();
			}
			set
			{
				this.SetAsString(value);
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00002545 File Offset: 0x00000745
		// (set) Token: 0x0600008E RID: 142 RVA: 0x0000254D File Offset: 0x0000074D
		public long AsInteger
		{
			get
			{
				return this.GetAsInteger();
			}
			set
			{
				this.SetAsInteger(value);
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00002556 File Offset: 0x00000756
		// (set) Token: 0x06000090 RID: 144 RVA: 0x0000255E File Offset: 0x0000075E
		public double AsFloat
		{
			get
			{
				return this.GetAsFloat();
			}
			set
			{
				this.SetAsFloat(value);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00005538 File Offset: 0x00003738
		public MsgPackArray AsArray
		{
			get
			{
				lock (this)
				{
					if (this.refAsArray == null)
					{
						this.refAsArray = new MsgPackArray(this, this.children);
					}
				}
				return this.refAsArray;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00002567 File Offset: 0x00000767
		public MsgPackType ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000256F File Offset: 0x0000076F
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new MsgPackEnum(this.children);
		}

		// Token: 0x04000033 RID: 51
		private string name;

		// Token: 0x04000034 RID: 52
		private string lowerName;

		// Token: 0x04000035 RID: 53
		private object innerValue;

		// Token: 0x04000036 RID: 54
		private MsgPackType valueType;

		// Token: 0x04000037 RID: 55
		private MsgPack parent;

		// Token: 0x04000038 RID: 56
		private List<MsgPack> children = new List<MsgPack>();

		// Token: 0x04000039 RID: 57
		private MsgPackArray refAsArray;
	}
}
