//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Message响应接口# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月6日 17点06分# </time>
//-----------------------------------------------------------------------

namespace HotFix.Taurus
{
    public interface IResponse
    {
        int RpcId { get; set; }
    }
}