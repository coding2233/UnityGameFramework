//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网页请求的事件# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月12日 11点25分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public class WebRequestEventArgs
    {

    }

    public class HttpReadTextSuccessEventArgs : GameEventArgs<HttpReadTextSuccessEventArgs>
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string Url;
        /// <summary>
        /// 内容
        /// </summary>
        public string Content;
    }

    public class HttpReadTextFaileEventArgs : GameEventArgs<HttpReadTextFaileEventArgs>
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string Url;
        /// <summary>
        /// 错误内容摩纳哥
        /// </summary>
        public string Error;
    }

    public class DownloadSuccessEventArgs : GameEventArgs<DownloadSuccessEventArgs>
    {
        /// <summary>
        /// 远程的链接
        /// </summary>
        public string RemoteUrl;
        /// <summary>
        /// 本地文件的路径
        /// </summary>
        public string LocalPath;
    }

    public class DownloadFaileEventArgs : GameEventArgs<DownloadFaileEventArgs>
    {
        /// <summary>
        /// 远程的链接
        /// </summary>
        public string RemoteUrl;
        /// <summary>
        /// 本地文件的路径
        /// </summary>
        public string LocalPath;
        /// <summary>
        /// 错误
        /// </summary>
        public string Error;
    }

}
