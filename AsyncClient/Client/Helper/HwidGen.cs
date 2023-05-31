using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Client.Helper
{
	// Token: 0x02000007 RID: 7
	public static class HwidGen
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00003698 File Offset: 0x00001898
		public static string HWID()
		{
			string text;
			try
			{
				text = HwidGen.GetHash(string.Concat(new object[]
				{
					Environment.ProcessorCount,
					Environment.UserName,
					Environment.MachineName,
					Environment.OSVersion,
					new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize
				}));
			}
			catch
			{
				text = "Err HWID";
			}
			return text;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000371C File Offset: 0x0000191C
		public static string GetHash(string strToHash)
		{
			HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
			byte[] array = Encoding.ASCII.GetBytes(strToHash);
			array = hashAlgorithm.ComputeHash(array);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString().Substring(0, 20).ToUpper();
		}
	}
}
