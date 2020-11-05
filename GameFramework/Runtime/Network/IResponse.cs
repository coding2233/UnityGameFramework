//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Message响应接口# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年8月4日 15点57分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public interface IResponse
    {
        int RpcId { get; set; }
    }
}
