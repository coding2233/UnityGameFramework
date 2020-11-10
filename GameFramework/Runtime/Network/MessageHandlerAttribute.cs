﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #消息处理类标记# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年8月4日 13点45分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System;

namespace Wanderer.GameFramework
{
    public class MessageHandlerAttribute : Attribute
    {
        public Type TypeMessage { get; private set; }

        public MessageHandlerAttribute(Type typeMessage)
        {
            TypeMessage = typeMessage;
        }
    }

}