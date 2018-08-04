//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #消息处理的基类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月4日 15点06分# </time>
//-----------------------------------------------------------------------


using System.Collections;

namespace GameFramework.Taurus
{
    public abstract class MessageHandlerBase
    {
        public abstract void Handle(object message);
    }
}
