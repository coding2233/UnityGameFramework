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
    public sealed class WebRequestManager : GameFrameworkModule,IUpdate
    {
        #region 属性
        private WebRequestBehaviourHelper _webRequest;
        //文件下载器
        public FileDownloader FileDownloader { get; private set; }
        #endregion

        public WebRequestManager()
        {
            _webRequest = new GameObject("WebRequestBehaviourHelper").AddComponent<WebRequestBehaviourHelper>();
            _webRequest.gameObject.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(_webRequest.gameObject);
            //文件下载
            FileDownloader = new FileDownloader(_webRequest);
        }
        #region 外部接口

        /// <summary>
        /// 请求网络文本的数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<string> RequestText(string url, Dictionary<string, string> header)
        {
            var taskResult = new TaskCompletionSource<string>();
            _webRequest.RequestText(url, header,(result, content) =>
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
        public void RequestText(string url, Dictionary<string, string> header, Action<bool, string> callback)
        {
            _webRequest.RequestText(url, header, callback);
        }

        /// <summary>
        /// 请求网络文本 POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="callback"></param>
        public void RequestTextPost(string url,Dictionary<string,string> header,string body, Action<bool, string> callback)
        {
            _webRequest.RequestTextPost(url, header, body, callback);
        }

        /// <summary>
        /// 请求Texture2D
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<Texture2D> RequestTexture2D(string url)
        {
            var resultTask = new TaskCompletionSource<Texture2D>();
            _webRequest.RequestTexture2D(url, (tex2d) =>
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
        public void RequestTexture2D(string url, Action<Texture2D> callback)
        {
            _webRequest.RequestTexture2D(url, callback);
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<AssetBundle> RequestAssetBundle(string url)
        {
            var resultTask = new TaskCompletionSource<AssetBundle>();
            _webRequest.RequestAssetBundle(url, (ab) =>
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
        public void RequestAssetBundle(string url, Action<AssetBundle> callback)
        {
            _webRequest.RequestAssetBundle(url, callback);
        }


        #endregion

        public void OnUpdate()
        {
            FileDownloader?.OnUpdate();
        }

        public override void OnClose()
        {
            GameObject.Destroy(_webRequest.gameObject);
            _webRequest = null;
        }


    }
}
