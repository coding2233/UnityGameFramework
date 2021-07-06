using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public interface IDownloader
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="callback">本地文件的路径，是否下载完成，下载的文件大小，下载的进度</param>
        /// <param name="errorCallback">错误回调</param>
        /// <returns></returns>
        void Download(string remoteUrl, string localPath, Action<string, bool, ulong, float> callback, Action<string, string> errorCallback);

    }
}
