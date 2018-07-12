//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #http文件下载 继承MonoBehaviour的实现类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月12日 11点42分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Taurus
{
    public sealed class WebDownloadMonoHelper : MonoBehaviour,IWebDownloadHelper
    {
        public void StartDownload(string remoteUrl, string localPath, Action<string, string, bool, string> result)
        {
            StartCoroutine(UnityWebStartDownload(remoteUrl,localPath,result));
        }

        IEnumerator UnityWebStartDownload(string remoteUrl, string localPath, Action<string, string, bool, string> result)
        {
            UnityWebRequest request = UnityWebRequest.Get(remoteUrl);
            request.downloadHandler = new DownloadHandlerFile(localPath);
            yield return request.SendWebRequest(); 
            if (request.isNetworkError || request.isHttpError)
                result.Invoke(remoteUrl, localPath, false,
                    "NetworkError:" + request.isNetworkError + "  HttpError:" + request.isHttpError);
            else
                result.Invoke(remoteUrl, localPath, true,
                    "File successfully downloaded and saved to " + localPath);
        }

    }
}
