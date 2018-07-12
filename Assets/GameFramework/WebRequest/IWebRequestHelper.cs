//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网页请求帮助类接口# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月12日 11点16分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public interface IWebRequestHelper
    {
        /// <summary>
        /// 读取http上的文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="result"></param>
        void ReadHttpText(string url, Action<string, bool,string> result);
    }
}
