using System;
using System.IO;
using System.IO.Compression;

namespace MessagePackLib.MessagePack
{
	// Token: 0x0200001C RID: 28
	public static class Zip
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x00005958 File Offset: 0x00003B58
		public static byte[] Decompress(byte[] input)
		{
			byte[] array3;
			using (MemoryStream memoryStream = new MemoryStream(input))
			{
				byte[] array = new byte[4];
				memoryStream.Read(array, 0, 4);
				int num = BitConverter.ToInt32(array, 0);
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					byte[] array2 = new byte[num];
					gzipStream.Read(array2, 0, num);
					array3 = array2;
				}
			}
			return array3;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000059E4 File Offset: 0x00003BE4
		public static byte[] Compress(byte[] input)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] bytes = BitConverter.GetBytes(input.Length);
				memoryStream.Write(bytes, 0, 4);
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					gzipStream.Write(input, 0, input.Length);
					gzipStream.Flush();
				}
				array = memoryStream.ToArray();
			}
			return array;
		}
	}
}
