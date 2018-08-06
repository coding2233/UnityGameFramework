//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Message请求接口# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月6日 17点05分# </time>
//-----------------------------------------------------------------------

namespace HotFix.Taurus
{
    public interface IRequest
    {
        int RpcId { get; set; }
    }
}