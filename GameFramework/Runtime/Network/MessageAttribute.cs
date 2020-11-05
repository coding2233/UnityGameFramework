//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #消息发送类标记# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年8月4日 14点42分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System;

namespace Wanderer.GameFramework
{
    public class MessageAttribute : Attribute
    {
        public ushort TypeCode { get; private set; }

        public MessageAttribute(ushort typeCode)
        {
            TypeCode = typeCode;
        }
    }

}