using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Wanderer.GameFramework
{
    internal class WebRequestBehaviourHelper : MonoBehaviour,IDownloader
    {

        #region 接口
        /// <summary>
        /// 文本请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="callback"></param>
        internal void RequestText(string url, Dictionary<string, string> header, Action<bool, string> callback)
        {
            StartCoroutine(WebRequestRequestText(url, header, callback));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="callback"></param>
        internal void RequestTextPost(string url, Dictionary<string, string> header, string body, Action<bool, string> callback)
        {
            StartCoroutine(WebRequestRequestTextPost(url, header, body, callback));
        }

        /// <summary>
        /// 请求Texture2D
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>

        internal void RequestTexture2D(string url, Action<Texture2D> callback)
        {
            StartCoroutine(WebRequestTexture2D(url, callback));
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        internal void RequestAssetBundle(string url, Action<AssetBundle> callback)
        {
            StartCoroutine(WebRequestAssetBundle(url, callback));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="callback">本地文件的路径，是否下载完成，下载的文件大小，下载的进度</param>
        /// <param name="errorCallback">错误回调</param>
        /// <returns></returns>
        public void Download(string remoteUrl, string localPath, Action<string, bool, ulong, float> callback, Action<string,string> errorCallback)
        {
            StartCoroutine(WebRequestDownloadFile(remoteUrl,localPath, callback, errorCallback));
        }
        #endregion

        #region 内部函数
        //请求文本
        private IEnumerator WebRequestRequestText(string url, Dictionary<string, string> header, Action<bool, string> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        request.SetRequestHeader(item.Key, item.Value);
                    }
                }
                yield return request.SendWebRequest();
                if (request.isNetworkError)
                {
                    callback?.Invoke(false, request.error);
                }
                else
                {
                    callback?.Invoke(true, request.downloadHandler.text);
                }
            }
        }
        //请求文本  POST
        private IEnumerator WebRequestRequestTextPost(string url, Dictionary<string, string> header, string body, Action<bool, string> callback)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.useHttpContinue = false;
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        request.SetRequestHeader(item.Key, item.Value);
                    }
                }
                yield return request.SendWebRequest();
                if (request.isNetworkError)
                {
                    callback?.Invoke(false, request.error);
                }
                else
                {
                    callback?.Invoke(true, request.downloadHandler.text);
                }
            }
        }
        //请求Texture2D
        private IEnumerator WebRequestTexture2D(string url, Action<Texture2D> callback)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError)
                {
                    callback?.Invoke(null);
                }
                else
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(request);
                    callback?.Invoke(tex);
                }
            }
        }

        //请求AB包
        private IEnumerator WebRequestAssetBundle(string url, Action<AssetBundle> callback)
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError)
                {
                    callback?.Invoke(null);
                }
                else
                {
                    AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
                    callback?.Invoke(ab);
                }
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="callback">本地文件的路径，是否下载完成，下载的文件大小，下载的进度</param>
        /// <param name="errorCallback">错误回调</param>
        /// <returns></returns>
        private IEnumerator WebRequestDownloadFile(string remoteUrl, string localPath, Action<string, bool,ulong, float> callback,Action<string,string> errorCallback)
        {
            yield return null;
	    
	      //删除本地文件
	   if (File.Exists(localPath))
	   {
                File.Delete(localPath);
            }

            using (UnityWebRequest request = UnityWebRequest.Get(remoteUrl))
            {
                request.downloadHandler = new DownloadHandlerFile(localPath, true);

                request.SendWebRequest();

                yield return null;
                while (!request.isDone)
                {
                    if (request.downloadProgress > 0)
                    {
                        callback?.Invoke(localPath,false, request.downloadedBytes,request.downloadProgress);
                    }
                    yield return null;
                }

                yield return null;

                if (request.isNetworkError || request.isHttpError)
                    errorCallback?.Invoke(localPath,request.error);
                else
                {
                    callback?.Invoke(localPath, true, request.downloadedBytes, request.downloadProgress);
                }
            }

        }
		#endregion
	}
}
