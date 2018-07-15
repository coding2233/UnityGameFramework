//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #检查资源状态# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月8日 13点20分# </time>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramework.Taurus
{
    [GameState]
    public class CheckResourceState : GameState
    {
        #region 属性

        //资源信息文本名称
        private string _assetVersionTxt = "AssetVersion.txt";

        //本地版本信息
        private AssetBundleVersionInfo _localVersion;

        //远程版本信息
        private AssetBundleVersionInfo _remoteVersion;

        //资源更新完成
        private bool _resourceUpdateDone;
        //需要更新的资源
        private Dictionary<string, string> _downloadResouces;
        #endregion

        #region 重写函数

        public override void OnInit()
        {
            base.OnInit();

            _downloadResouces = new Dictionary<string, string>();
        }

        public override void OnEnter(params object[] parameters)
        {
            base.OnEnter(parameters);
            GameMode.Event.AddListener<HttpReadTextSuccessEventArgs>(OnHttpReadTextSuccess);
            GameMode.Event.AddListener<HttpReadTextFaileEventArgs>(OnHttpReadTextFaile);
            GameMode.Event.AddListener<DownloadSuccessEventArgs>(OnDownloadSuccess);
            GameMode.Event.AddListener<DownloadFaileEventArgs>(OnDownloadFaile);
            GameMode.Event.AddListener<DownloadProgressEventArgs>(OnDownloadProgress);

            _localVersion=LoadLocalVersion();
            LoadRemoteVersion();
        }


        public override void OnExit()
        {
            GameMode.Event.RemoveListener<HttpReadTextSuccessEventArgs>(OnHttpReadTextSuccess);
            GameMode.Event.RemoveListener<HttpReadTextFaileEventArgs>(OnHttpReadTextFaile);
            GameMode.Event.RemoveListener<DownloadSuccessEventArgs>(OnDownloadSuccess);
            GameMode.Event.RemoveListener<DownloadFaileEventArgs>(OnDownloadFaile);
            GameMode.Event.RemoveListener<DownloadProgressEventArgs>(OnDownloadProgress);

            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }
        
        public override void OnUpdate()
        {
            base.OnUpdate();

            //更新资源
            if (_resourceUpdateDone && _downloadResouces.Count == 0)
            {
                //更新本地资源信息文本
                UpdateAssetVersionTxt();
                //  切换到加载界面
                ChangeState<LoadResourceState>();
            }
        }

        #endregion


        #region 事件回调
        //http文本读取成功
        private void OnHttpReadTextSuccess(object sender, IEventArgs e)
        {
            HttpReadTextSuccessEventArgs ne = (HttpReadTextSuccessEventArgs)e;
            if (ne != null)
            {
                _remoteVersion = JsonUtility.FromJson<AssetBundleVersionInfo>(ne.Content);
                if (_remoteVersion == null)
                {
                    Debug.LogError("Remote Version is null");
                    return;
                }

                //如果资源版本不一样 则更新资源
                if (!CompareVersion())
                {
                    //更新资源
                    UpdateResource();
                    //下载资源
                    DownloadResource();
                }

                //资源更新完成
                _resourceUpdateDone = true;
            }
        }
        //http文件读取错误
        private void OnHttpReadTextFaile(object sender, IEventArgs e)
        {
            HttpReadTextFaileEventArgs ne = (HttpReadTextFaileEventArgs) e;
            if (ne != null)
                Debug.LogError(ne.Error);
        }
        //加载文件成功
        private void OnDownloadSuccess(object sender, IEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs) e;
            if (_downloadResouces.ContainsKey(ne.RemoteUrl))
                _downloadResouces.Remove(ne.RemoteUrl);
        }
        //下载文件失败
        private void OnDownloadFaile(object sender, IEventArgs e)
        {
            DownloadFaileEventArgs ne = (DownloadFaileEventArgs)e;
            if (ne != null)
                Debug.LogError(ne.Error);
        }
        //下载进度
        private void OnDownloadProgress(object sender, IEventArgs e)
        {
            DownloadProgressEventArgs ne = (DownloadProgressEventArgs) e;
            Debug.Log(
                $"path:{ne.LocalPath} progress:{ne.DownloadProgress} bytes:{ne.DownloadBytes} speed:{ne.DownloadSpeed}");
        }

        #endregion

        #region 内部函数

        //加载本地版本信息
        private AssetBundleVersionInfo LoadLocalVersion()
        {
            string localPath =Path.Combine(GameMode.Resource.LocalPath, _assetVersionTxt);
            if(!File.Exists(localPath))
                return null;
            
            string content= File.ReadAllText(localPath);
            return JsonUtility.FromJson<AssetBundleVersionInfo>(content);
        }

        //加载远程版本信息
        private void LoadRemoteVersion()
        {
            string remotePath =Path.Combine(GameMode.Resource.ResUpdatePath, _assetVersionTxt);
            GameMode.WebRequest.ReadHttpText(remotePath);
        }

        //比较版本
        private bool CompareVersion()
        {
            return _localVersion!=null && _remoteVersion.Version == _localVersion.Version;
        }

        //更新资源
        private void UpdateResource()
        {
            foreach (var item in _remoteVersion.Resources)
            {
                //本地有响应文件则跳过
                if (_localVersion!=null&& _localVersion.Resources!=null&&_localVersion.Resources.Contains(item))
                    continue;
                string remoteUrl = Path.Combine(GameMode.Resource.ResUpdatePath, item.Name);
                //获取本地文件的路径
                string localPath = Path.Combine(GameMode.Resource.LocalPath, item.Name);
	           
				//创建文件夹
                string localDir = Path.GetDirectoryName(localPath);
                if (!Directory.Exists(localDir))
                    Directory.CreateDirectory(localDir);

                //添加需要下载的资源
                _downloadResouces.Add(remoteUrl, localPath);
            }
          
        }

        //下载资源
        private void DownloadResource()
        {
            foreach (var item in _downloadResouces)
            {
                GameMode.WebRequest.StartDownload(item.Key, item.Value);
            }
        }

        //更新资源版本信息文本
        private void UpdateAssetVersionTxt()
        {
            if (!CompareVersion())
            {
                string localPath = Path.Combine(GameMode.Resource.LocalPath, _assetVersionTxt);
                File.WriteAllText(localPath, JsonUtility.ToJson(_remoteVersion));
            }
        }

        #endregion

    }
}
