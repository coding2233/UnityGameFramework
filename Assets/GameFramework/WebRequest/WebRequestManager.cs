//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网页请求管理器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月12日 11点06分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public sealed class WebRequestManager : GameFrameworkModule
    {
        #region 属性

        //事件管理类
        private EventManager _event;

        //网页请求帮助类
        private IWebRequestHelper _webRequestHelper;

        //读取http文本的成功的事件
        private HttpReadTextSuccessEventArgs _httpReadTextSuccess;

        //读取http文本失败的事件
        private HttpReadTextFaileEventArgs _httpReadTextFaile;

        //http下载帮助类
        private IWebDownloadHelper _webDownloadHelper;
        private DownloadSuccessEventArgs _downloadSuccess;
        private DownloadFaileEventArgs _downloadFaile;
        private DownloadProgressEventArgs _downloadProgress;
        #endregion

        public WebRequestManager()
        {
            _event = GameFrameworkMode.GetModule<EventManager>();
            _httpReadTextSuccess = new HttpReadTextSuccessEventArgs();
            _httpReadTextFaile = new HttpReadTextFaileEventArgs();
            _downloadSuccess = new DownloadSuccessEventArgs();
            _downloadFaile = new DownloadFaileEventArgs();
            _downloadProgress = new DownloadProgressEventArgs();
        }

        #region 外部接口

        /// <summary>
        /// 设置网页请求帮助类
        /// </summary>
        /// <param name="helper"></param>
        public void SetWebRequestHelper(IWebRequestHelper helper)
        {
            _webRequestHelper = helper;
        }
        /// <summary>
        /// 设置下载的帮助类
        /// </summary>
        /// <param name="helper"></param>
        public void SetWebDownloadHelper(IWebDownloadHelper helper)
        {
            _webDownloadHelper = helper;
        }

        /// <summary>
        /// 读取文本的信息
        /// </summary>
        /// <param name="url">http链接</param>
        public void ReadHttpText(string url)
        {
            _webRequestHelper?.ReadHttpText(url, ReadHttpTextCallback);
        }

        /// <summary>
        /// 开始下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        public void StartDownload(string remoteUrl, string localPath)
        {
            _webDownloadHelper?.StartDownload(remoteUrl, localPath, StartDownloadCallback,StartDownloadProgress);
        }

        #endregion

        public override void OnClose()
        {

        }

        #region 内部函数

        /// <summary>
        /// 读取http上的文本文件的回调函数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="result"></param>
        /// <param name="content"></param>
        private void ReadHttpTextCallback(string path, bool result, string content)
        {
            if (result)
            {
                _httpReadTextSuccess.Url = path;
                _httpReadTextSuccess.Content = content;
                _event.Trigger(this, _httpReadTextSuccess);
            }
            else
            {
                _httpReadTextFaile.Url = path;
                _httpReadTextFaile.Error = content;
                _event.Trigger(this, _httpReadTextFaile);
            }
        }

        /// <summary>
        /// 开始下载的回调
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="result"></param>
        /// <param name="content"></param>
        private void StartDownloadCallback(string remoteUrl, string localPath, bool result, string content)
        {
            if (result)
            {
                _downloadSuccess.RemoteUrl = remoteUrl;
                _downloadSuccess.LocalPath = localPath;
                _event.Trigger(this, _downloadSuccess);
            }
            else
            {
                _downloadFaile.RemoteUrl = remoteUrl;
                _downloadFaile.LocalPath = localPath;
                _downloadFaile.Error = content;
                _event.Trigger(this, _downloadFaile);
            }
        }

        /// <summary>
        /// 下载的进度
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="dataLength"></param>
        /// <param name="progess"></param>
        /// <param name="seconds"></param>
        private void StartDownloadProgress(string remoteUrl, string localPath, ulong dataLength, float progess,float seconds)
        {
            _downloadProgress.RemoteUrl = remoteUrl;
            _downloadProgress.LocalPath = localPath;
            _downloadProgress.DownloadBytes = dataLength;
            _downloadProgress.DownloadProgress = progess;
            _downloadProgress.DownloadSeconds = seconds;
            _downloadProgress.DownloadSpeed =
                dataLength == 0.0f ? dataLength : dataLength / 1024.0f  / seconds;
            _event.Trigger(this, _downloadProgress);
        }

        #endregion

    }
}
