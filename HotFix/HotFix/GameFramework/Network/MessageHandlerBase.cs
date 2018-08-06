//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #消息处理的基类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月6日 17点05分# </time>
//-----------------------------------------------------------------------
namespace HotFix.Taurus
{
    public abstract class MessageHandlerBase
    {
        public abstract void Handle(object message);
    }
}