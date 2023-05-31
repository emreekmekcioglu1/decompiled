using System;
using System.IO;

namespace MessagePackLib.MessagePack
{
	// Token: 0x0200001B RID: 27
	internal class WriteTools
	{
		// Token: 0x06000099 RID: 153 RVA: 0x0000259E File Offset: 0x0000079E
		public static void WriteNull(Stream ms)
		{
			ms.WriteByte(192);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000567C File Offset: 0x0000387C
		public static void WriteString(Stream ms, string strVal)
		{
			byte[] utf8Bytes = BytesTools.GetUtf8Bytes(strVal);
			int num = utf8Bytes.Length;
			if (num <= 31)
			{
				byte b = 160 + (byte)num;
				ms.WriteByte(b);
			}
			else if (num <= 255)
			{
				byte b = 217;
				ms.WriteByte(b);
				b = (byte)num;
				ms.WriteByte(b);
			}
			else if (num <= 65535)
			{
				byte b = 218;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes((short)num));
				ms.Write(array, 0, array.Length);
			}
			else
			{
				byte b = 219;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes(num));
				ms.Write(array, 0, array.Length);
			}
			ms.Write(utf8Bytes, 0, utf8Bytes.Length);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005744 File Offset: 0x00003944
		public static void WriteBinary(Stream ms, byte[] rawBytes)
		{
			int num = rawBytes.Length;
			if (num <= 255)
			{
				byte b = 196;
				ms.WriteByte(b);
				b = (byte)num;
				ms.WriteByte(b);
			}
			else if (num <= 65535)
			{
				byte b = 197;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes((short)num));
				ms.Write(array, 0, array.Length);
			}
			else
			{
				byte b = 198;
				ms.WriteByte(b);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes(num));
				ms.Write(array, 0, array.Length);
			}
			ms.Write(rawBytes, 0, rawBytes.Length);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000025AB File Offset: 0x000007AB
		public static void WriteFloat(Stream ms, double fVal)
		{
			ms.WriteByte(203);
			ms.Write(BytesTools.SwapDouble(fVal), 0, 8);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000025C6 File Offset: 0x000007C6
		public static void WriteSingle(Stream ms, float fVal)
		{
			ms.WriteByte(202);
			ms.Write(BytesTools.SwapBytes(BitConverter.GetBytes(fVal)), 0, 4);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000025E6 File Offset: 0x000007E6
		public static void WriteBoolean(Stream ms, bool bVal)
		{
			if (bVal)
			{
				ms.WriteByte(195);
				return;
			}
			ms.WriteByte(194);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000057E4 File Offset: 0x000039E4
		public static void WriteUInt64(Stream ms, ulong iVal)
		{
			ms.WriteByte(207);
			byte[] bytes = BitConverter.GetBytes(iVal);
			ms.Write(BytesTools.SwapBytes(bytes), 0, 8);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005814 File Offset: 0x00003A14
		public static void WriteInteger(Stream ms, long iVal)
		{
			if (iVal >= 0L)
			{
				if (iVal <= 127L)
				{
					ms.WriteByte((byte)iVal);
					return;
				}
				if (iVal <= 255L)
				{
					ms.WriteByte(204);
					ms.WriteByte((byte)iVal);
					return;
				}
				if (iVal <= 65535L)
				{
					ms.WriteByte(205);
					ms.Write(BytesTools.SwapInt16((short)iVal), 0, 2);
					return;
				}
				if (iVal <= (long)((ulong)(-1)))
				{
					ms.WriteByte(206);
					ms.Write(BytesTools.SwapInt32((int)iVal), 0, 4);
					return;
				}
				ms.WriteByte(211);
				ms.Write(BytesTools.SwapInt64(iVal), 0, 8);
				return;
			}
			else
			{
				if (iVal <= -2147483648L)
				{
					ms.WriteByte(211);
					ms.Write(BytesTools.SwapInt64(iVal), 0, 8);
					return;
				}
				if (iVal <= -32768L)
				{
					ms.WriteByte(210);
					ms.Write(BytesTools.SwapInt32((int)iVal), 0, 4);
					return;
				}
				if (iVal <= -128L)
				{
					ms.WriteByte(209);
					ms.Write(BytesTools.SwapInt16((short)iVal), 0, 2);
					return;
				}
				if (iVal <= -32L)
				{
					ms.WriteByte(208);
					ms.WriteByte((byte)iVal);
					return;
				}
				ms.WriteByte((byte)iVal);
				return;
			}
		}
	}
}
