using System;
using System.Collections;
using System.Collections.Generic;

namespace MessagePackLib.MessagePack
{
	// Token: 0x02000016 RID: 22
	public class MsgPackEnum : IEnumerator
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00002317 File Offset: 0x00000517
		public MsgPackEnum(List<MsgPack> obj)
		{
			this.children = obj;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000060 RID: 96 RVA: 0x0000232D File Offset: 0x0000052D
		object IEnumerator.Current
		{
			get
			{
				return this.children[this.position];
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002340 File Offset: 0x00000540
		bool IEnumerator.MoveNext()
		{
			this.position++;
			return this.position < this.children.Count;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002363 File Offset: 0x00000563
		void IEnumerator.Reset()
		{
			this.position = -1;
		}

		// Token: 0x0400002F RID: 47
		private List<MsgPack> children;

		// Token: 0x04000030 RID: 48
		private int position = -1;
	}
}
