//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #Message请求接口# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年8月4日 15点57分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public interface IRequest
    {
        int RpcId { get; set; }
    }
}
