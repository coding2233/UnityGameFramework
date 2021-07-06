//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #消息处理的基类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年8月4日 15点06分# </time>
//-----------------------------------------------------------------------


using System.Collections;

namespace Wanderer.GameFramework
{
    public abstract class MessageHandlerBase
    {
        public abstract void Handle(object message);
    }
}
