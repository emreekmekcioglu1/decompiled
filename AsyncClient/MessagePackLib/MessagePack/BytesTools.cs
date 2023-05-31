using System;
using System.Text;

namespace MessagePackLib.MessagePack
{
	// Token: 0x02000015 RID: 21
	public class BytesTools
	{
		// Token: 0x06000054 RID: 84 RVA: 0x0000229F File Offset: 0x0000049F
		public static byte[] GetUtf8Bytes(string s)
		{
			return BytesTools.utf8Encode.GetBytes(s);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000022AC File Offset: 0x000004AC
		public static string GetString(byte[] utf8Bytes)
		{
			return BytesTools.utf8Encode.GetString(utf8Bytes);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000045C8 File Offset: 0x000027C8
		public static string BytesAsString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.Append(string.Format("{0:D3} ", b));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004614 File Offset: 0x00002814
		public static string BytesAsHexString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.Append(string.Format("{0:X2} ", b));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004660 File Offset: 0x00002860
		public static byte[] SwapBytes(byte[] v)
		{
			byte[] array = new byte[v.Length];
			int num = v.Length - 1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = v[num];
				num--;
			}
			return array;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000022B9 File Offset: 0x000004B9
		public static byte[] SwapInt64(long v)
		{
			return BytesTools.SwapBytes(BitConverter.GetBytes(v));
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000022C6 File Offset: 0x000004C6
		public static byte[] SwapInt32(int v)
		{
			byte[] array = new byte[]
			{
				0,
				0,
				0,
				(byte)v
			};
			array[2] = (byte)(v >> 8);
			array[1] = (byte)(v >> 16);
			array[0] = (byte)(v >> 24);
			return array;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000022EA File Offset: 0x000004EA
		public static byte[] SwapInt16(short v)
		{
			byte[] array = new byte[]
			{
				0,
				(byte)v
			};
			array[0] = (byte)(v >> 8);
			return array;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000022FE File Offset: 0x000004FE
		public static byte[] SwapDouble(double v)
		{
			return BytesTools.SwapBytes(BitConverter.GetBytes(v));
		}

		// Token: 0x0400002E RID: 46
		private static UTF8Encoding utf8Encode = new UTF8Encoding();
	}
}
