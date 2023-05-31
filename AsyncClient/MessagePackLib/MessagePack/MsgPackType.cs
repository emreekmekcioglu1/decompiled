using System;

namespace MessagePackLib.MessagePack
{
	// Token: 0x02000019 RID: 25
	public enum MsgPackType
	{
		// Token: 0x0400003B RID: 59
		Unknown,
		// Token: 0x0400003C RID: 60
		Null,
		// Token: 0x0400003D RID: 61
		Map,
		// Token: 0x0400003E RID: 62
		Array,
		// Token: 0x0400003F RID: 63
		String,
		// Token: 0x04000040 RID: 64
		Integer,
		// Token: 0x04000041 RID: 65
		UInt64,
		// Token: 0x04000042 RID: 66
		Boolean,
		// Token: 0x04000043 RID: 67
		Float,
		// Token: 0x04000044 RID: 68
		Single,
		// Token: 0x04000045 RID: 69
		DateTime,
		// Token: 0x04000046 RID: 70
		Binary
	}
}
