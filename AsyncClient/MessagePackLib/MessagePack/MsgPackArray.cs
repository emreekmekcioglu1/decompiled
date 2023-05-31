using System;
using System.Collections.Generic;

namespace MessagePackLib.MessagePack
{
	// Token: 0x02000017 RID: 23
	public class MsgPackArray
	{
		// Token: 0x06000063 RID: 99 RVA: 0x0000236C File Offset: 0x0000056C
		public MsgPackArray(MsgPack msgpackObj, List<MsgPack> listObj)
		{
			this.owner = msgpackObj;
			this.children = listObj;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002382 File Offset: 0x00000582
		public MsgPack Add()
		{
			return this.owner.AddArrayChild();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000238F File Offset: 0x0000058F
		public MsgPack Add(string value)
		{
			MsgPack msgPack = this.owner.AddArrayChild();
			msgPack.AsString = value;
			return msgPack;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000023A3 File Offset: 0x000005A3
		public MsgPack Add(long value)
		{
			MsgPack msgPack = this.owner.AddArrayChild();
			msgPack.SetAsInteger(value);
			return msgPack;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000023B7 File Offset: 0x000005B7
		public MsgPack Add(double value)
		{
			MsgPack msgPack = this.owner.AddArrayChild();
			msgPack.SetAsFloat(value);
			return msgPack;
		}

		// Token: 0x1700000D RID: 13
		public MsgPack this[int index]
		{
			get
			{
				return this.children[index];
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000023D9 File Offset: 0x000005D9
		public int Length
		{
			get
			{
				return this.children.Count;
			}
		}

		// Token: 0x04000031 RID: 49
		private List<MsgPack> children;

		// Token: 0x04000032 RID: 50
		private MsgPack owner;
	}
}
