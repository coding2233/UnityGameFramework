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

        //本地版本信息
        private AssetBundleVersionInfo _localVersion;

        //远程版本信息
        private AssetBundleVersionInfo _remoteVersion;

        #endregion

        #region 重写函数

        public override void OnEnter(params object[] parameters)
        {
            base.OnEnter(parameters);
            GameMode.Event.AddListener<HttpReadTextSuccessEventArgs>(OnHttpReadTextSuccess);
            GameMode.Event.AddListener<HttpReadTextFaileEventArgs>(OnHttpReadTextFaile);
            GameMode.Event.AddListener<DownloadSuccessEventArgs>(OnDownloadSuccess);
            GameMode.Event.AddListener<DownloadFaileEventArgs>(OnDownloadFaile);

            _localVersion=LoadLocalVersion();
            LoadRemoteVersion();
        }


        public override void OnExit()
        {
            GameMode.Event.RemoveListener<HttpReadTextSuccessEventArgs>(OnHttpReadTextSuccess);
            GameMode.Event.RemoveListener<HttpReadTextFaileEventArgs>(OnHttpReadTextFaile);
            GameMode.Event.RemoveListener<DownloadSuccessEventArgs>(OnDownloadSuccess);
            GameMode.Event.RemoveListener<DownloadFaileEventArgs>(OnDownloadFaile);
            base.OnExit();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //更新完资源
            //...
            //切换到加载界面
           // ChangeState<LoadResourceState>();
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
                //如果资源版本不一样 则更新资源
                if (!CompareVersion())
                    UpdateResource();
            }
        }
        //http文件读取错误
        private void OnHttpReadTextFaile(object sender, IEventArgs e)
        {
        }
        //加载文件成功
        private void OnDownloadSuccess(object sender, IEventArgs e)
        {
        }
        //下载文件失败
        private void OnDownloadFaile(object sender, IEventArgs e)
        {

        }

        #endregion

        #region 内部函数

        //加载本地版本信息
        private AssetBundleVersionInfo LoadLocalVersion()
        {
            string localPath = GameMode.Resource.LocalPath + "/AssetVersion.txt";
            if(!File.Exists(localPath))
                return null;
            
            string content= File.ReadAllText(localPath);
            return JsonUtility.FromJson<AssetBundleVersionInfo>(content);
        }

        //加载远程版本信息
        private void LoadRemoteVersion()
        {
            string remotePath = GameMode.Resource.ResUpdatePath + "/AssetVersion.txt";
            GameMode.WebRequest.ReadHttpText(remotePath);
        }

        //比较版本
        private bool CompareVersion()
        {
            return _remoteVersion.Version == _localVersion.Version;
        }

        //更新资源
        private void UpdateResource()
        {
            foreach (var item in _remoteVersion.Resources)
            {
                //本地有响应文件则跳过
                if (_localVersion.Resources.Contains(item))
                    continue;
                string remoteUrl = Path.Combine(GameMode.Resource.ResUpdatePath, item.Name);
                //获取本地文件的路径
                string localPath = Path.Combine(GameMode.Resource.LocalPath, item.Name);
                //创建文件夹
                int index = localPath.LastIndexOf("/", StringComparison.Ordinal);
                string localDir = localPath.Substring(0, index);
                if (!Directory.Exists(localDir))
                    Directory.CreateDirectory(localDir);
                GameMode.WebRequest.StartDownload(remoteUrl, localPath);
            }
        }

        #endregion

    }
}
