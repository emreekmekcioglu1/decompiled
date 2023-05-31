using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Client.Algorithm
{
	// Token: 0x02000011 RID: 17
	public class Aes256
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00004134 File Offset: 0x00002334
		public Aes256(string masterKey)
		{
			if (string.IsNullOrEmpty(masterKey))
			{
				throw new ArgumentException("masterKey can not be null or empty.");
			}
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(masterKey, Aes256.Salt, 50000))
			{
				this._key = rfc2898DeriveBytes.GetBytes(32);
				this._authKey = rfc2898DeriveBytes.GetBytes(64);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002256 File Offset: 0x00000456
		public string Encrypt(string input)
		{
			return Convert.ToBase64String(this.Encrypt(Encoding.UTF8.GetBytes(input)));
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000041AC File Offset: 0x000023AC
		public byte[] Encrypt(byte[] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input can not be null.");
			}
			byte[] array2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Position = 32L;
				using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
				{
					aesCryptoServiceProvider.KeySize = 256;
					aesCryptoServiceProvider.BlockSize = 128;
					aesCryptoServiceProvider.Mode = CipherMode.CBC;
					aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
					aesCryptoServiceProvider.Key = this._key;
					aesCryptoServiceProvider.GenerateIV();
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
					{
						memoryStream.Write(aesCryptoServiceProvider.IV, 0, aesCryptoServiceProvider.IV.Length);
						cryptoStream.Write(input, 0, input.Length);
						cryptoStream.FlushFinalBlock();
						using (HMACSHA256 hmacsha = new HMACSHA256(this._authKey))
						{
							byte[] array = hmacsha.ComputeHash(memoryStream.ToArray(), 32, memoryStream.ToArray().Length - 32);
							memoryStream.Position = 0L;
							memoryStream.Write(array, 0, array.Length);
						}
					}
				}
				array2 = memoryStream.ToArray();
			}
			return array2;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000226E File Offset: 0x0000046E
		public string Decrypt(string input)
		{
			return Encoding.UTF8.GetString(this.Decrypt(Convert.FromBase64String(input)));
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004304 File Offset: 0x00002504
		public byte[] Decrypt(byte[] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input can not be null.");
			}
			byte[] array6;
			using (MemoryStream memoryStream = new MemoryStream(input))
			{
				using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
				{
					aesCryptoServiceProvider.KeySize = 256;
					aesCryptoServiceProvider.BlockSize = 128;
					aesCryptoServiceProvider.Mode = CipherMode.CBC;
					aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
					aesCryptoServiceProvider.Key = this._key;
					using (HMACSHA256 hmacsha = new HMACSHA256(this._authKey))
					{
						byte[] array = hmacsha.ComputeHash(memoryStream.ToArray(), 32, memoryStream.ToArray().Length - 32);
						byte[] array2 = new byte[32];
						memoryStream.Read(array2, 0, array2.Length);
						if (!this.AreEqual(array, array2))
						{
							throw new CryptographicException("Invalid message authentication code (MAC).");
						}
					}
					byte[] array3 = new byte[16];
					memoryStream.Read(array3, 0, 16);
					aesCryptoServiceProvider.IV = array3;
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read))
					{
						byte[] array4 = new byte[memoryStream.Length - 16L + 1L];
						byte[] array5 = new byte[cryptoStream.Read(array4, 0, array4.Length)];
						Buffer.BlockCopy(array4, 0, array5, 0, array5.Length);
						array6 = array5;
					}
				}
			}
			return array6;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000044C4 File Offset: 0x000026C4
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private bool AreEqual(byte[] a1, byte[] a2)
		{
			bool flag = true;
			for (int i = 0; i < a1.Length; i++)
			{
				if (a1[i] != a2[i])
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x04000026 RID: 38
		private const int KeyLength = 32;

		// Token: 0x04000027 RID: 39
		private const int AuthKeyLength = 64;

		// Token: 0x04000028 RID: 40
		private const int IvLength = 16;

		// Token: 0x04000029 RID: 41
		private const int HmacSha256Length = 32;

		// Token: 0x0400002A RID: 42
		private readonly byte[] _key;

		// Token: 0x0400002B RID: 43
		private readonly byte[] _authKey;

		// Token: 0x0400002C RID: 44
		private static readonly byte[] Salt = new byte[]
		{
			191, 235, 30, 86, 251, 205, 151, 59, 178, 25,
			2, 36, 48, 165, 120, 67, 0, 61, 86, 68,
			210, 30, 98, 185, 212, 241, 128, 231, 230, 195,
			57, 65
		};
	}
}
