﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #http下载帮助接口# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月12日 11点36分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public interface IWebDownloadHelper
    {
        void StartDownload(string remoteUrl, string localPath, Action<string, string, bool, string> result, Action<string, string, ulong, float, float> progress);
    }
}
