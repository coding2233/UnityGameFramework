//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #http下载帮助接口# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月12日 11点36分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public interface IWebDownloadHelper
    {
        void StartDownload(string remoteUrl, string localPath,Action<string,string,bool,string> result);
    }
}
