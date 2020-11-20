//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #网页请求管理器# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月12日 11点06分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public sealed class WebRequestManager : GameFrameworkModule
    {
        #region 属性

        //事件管理类
        private EventManager _event;
        #endregion

        public WebRequestManager()
        {
            _event = GameFrameworkMode.GetModule<EventManager>();
        }
        #region 外部接口

        /// <summary>
        /// 请求网络文本的数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<string> RequestText(string url)
        {
            var taskResult = new TaskCompletionSource<string>();
            RequestText(url, (result, content) =>
            {
                taskResult.SetResult(result ? content : null);
            });
            return taskResult.Task;
        }

        /// <summary>
        /// 请求网络文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async void RequestText(string url, Action<bool, string> callback)
        {
            try
            {
                UnityWebRequest request = UnityWebRequest.Get(url);
                await request.SendWebRequest();
                if (request.isNetworkError)
                {
                    callback?.Invoke(false, request.error);
                }
                else
                {
                    callback?.Invoke(true, request.downloadHandler.text);
                }
            }
            catch (System.Exception e)
            {
                callback?.Invoke(false, e.ToString());
            }
        }

        /// <summary>
        /// 请求网络文本 POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="callback"></param>
        public async void RequestTextPOST(string url,Dictionary<string,string> header,string body, Action<bool, string> callback)
        {
            try
            {
                using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
                    request.useHttpContinue = false;
                    if (header != null)
                    {
                        foreach (var item in header)
                        {
                            request.SetRequestHeader(item.Key, item.Value);
                        }
                    }
                    await request.SendWebRequest();
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
            catch (System.Exception e)
            {
                callback?.Invoke(false, e.ToString());
            }
        }

        /// <summary>
        /// 请求Texture2D
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<Texture2D> RequestTexture2D(string url)
        {
            var resultTask = new TaskCompletionSource<Texture2D>();
            RequestTexture2D(url, (result, tex2d) =>
            {
                resultTask.SetResult(tex2d);
            });
            return resultTask.Task;
        }

        /// <summary>
        /// 请求Texture2d
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async void RequestTexture2D(string url, Action<bool, Texture2D> callback)
        {
            try
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                await request.SendWebRequest();
                if (request.isNetworkError)
                {
                    callback?.Invoke(false, null);
                }
                else
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(request);
                    callback?.Invoke(true, tex);
                }
            }
            catch (System.Exception e)
            {
                callback?.Invoke(false, null);
            }
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<AssetBundle> RequestAssetBundle(string url)
        {
            var resultTask = new TaskCompletionSource<AssetBundle>();
            RequestAssetBundle(url, (result, ab) =>
            {
                resultTask.SetResult(ab);
            });
            return resultTask.Task;
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async void RequestAssetBundle(string url, Action<bool, AssetBundle> callback)
        {
            try
            {
                UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
                await request.SendWebRequest();
                if (request.isNetworkError)
                {
                    callback?.Invoke(false, null);
                }
                else
                {
                    AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
                    callback?.Invoke(true, ab);
                }
            }
            catch (System.Exception e)
            {
                callback?.Invoke(false, null);
            }

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteUrl"></param>
        /// <param name="localPath"></param>
        /// <param name="resultCallback"></param>
        /// <param name="progressCallback"></param>
        public async Task Download(string remoteUrl, string localPath, Action<string, UnityWebRequest, float> callback)
        {
            await UnityWebRequestDwonloaFile(remoteUrl, localPath, callback);
        }

        #endregion

        public override void OnClose()
        {

        }

        #region 内部函数


        //下载文件
        IEnumerator UnityWebRequestDwonloaFile(string remoteUrl, string localPath, Action<string, UnityWebRequest,float> callback)
        {
            
            //断点续传写不写呢...
            //纠结------------------

            //换一帧运行
            yield return null;
           
            UnityWebRequest request = UnityWebRequest.Get(remoteUrl);
			request.downloadHandler = new DownloadHandlerFile(localPath);
            
            request.timeout = 10;
            float lastTime = Time.realtimeSinceStartup;
            //yield return request.SendWebRequest(); 
            request.SendWebRequest();
            
            
            yield return null;
            while (!request.isDone)
            {
                float seconds = (Time.realtimeSinceStartup - lastTime);
                callback?.Invoke(localPath, request, lastTime);
                yield return null;
            }

            yield return null;

            if (request.isNetworkError || request.isHttpError)
                callback?.Invoke(localPath, request, lastTime);
            else
                callback?.Invoke(localPath, request, lastTime);
        }

        #endregion

    }
}
