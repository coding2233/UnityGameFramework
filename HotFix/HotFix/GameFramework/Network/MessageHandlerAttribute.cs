//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #消息处理类标记# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月7日 00点38分# </time>
//-----------------------------------------------------------------------

using System;

namespace HotFix.Taurus
{
	public class MessageHandlerAttribute : Attribute
	{
		public string TypeMessage { get; private set; }

		public MessageHandlerAttribute(Type typeMessage)
		{
			TypeMessage = typeMessage.ToString();
		}
	}
}