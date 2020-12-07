// 资源版本管理
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Wanderer.GameFramework
{
    public class ResourceVersion
    {
        private WebRequestManager _webRequest;
        //   private ResourceManager _resource;

        //平台的资源名称
        private string _assetPlatformVersionText = "AssetPlatformVersion.txt";
        //资源信息文本名称
        private string _assetVersionTxt = "AssetVersion.txt";
        //远程更新的路径
        private string _remoteUpdatePath = null;
        //本地路径
        private string _localResourcePath;
        //需要下载的文件
        private List<AssetHashInfo> _needDownloadFiles = new List<AssetHashInfo>();
        private bool _downloading = false;
        ////下载回调 报错，进度【0-1】 速度 大小
        //private Action<bool, float, float, ulong> _downloadCallback;
  

        public ResourceVersion(string remoteUpdatePath, string localResourcePath)
        {
            //   _resource = GameFrameworkMode.GetModule<ResourceManager>();
            _webRequest = GameFrameworkMode.GetModule<WebRequestManager>();

            _remoteUpdatePath = remoteUpdatePath;
            _localResourcePath = localResourcePath;
        }


        public void OnUpdate()
        {
        
        }

        /// <summary>
        /// 远程的版本信息
        /// </summary>
        /// <value></value>
        public AssetBundleVersionInfo RemoteVersion { get; private set; }

        /// <summary>
        /// 本地的版本信息
        /// </summary>
        /// <value></value>
        public AssetBundleVersionInfo LocalVersion { get; private set; }

        /// <summary>
        /// 请求本地版本信息
        /// </summary>
        /// <param name="callback"></param>
        public void RequestLocalVersion(Action<AssetBundleVersionInfo> callback)
        {
            LocalVersion = null;
            string versionAssetPath = Path.Combine(_localResourcePath, _assetVersionTxt);

            _webRequest.RequestText(versionAssetPath, null,(result, content) =>
            {
                if (result && !string.IsNullOrEmpty(content))
                {
                    content = content.ToEncrypt();
                    LocalVersion = JsonUtility.FromJson<AssetBundleVersionInfo>(content);
                }
                //本地可能就没有版本信息
                callback?.Invoke(LocalVersion);
                // if (LocalVersion == null)u
                // {
                //     throw new GameException($"Can't transition local [AssetBundleVersionInfo]!! {versionAssetPath} {content}");
                // }
            });
        }

        /// <summary>
        /// 请求本地版本信息
        /// </summary>
        /// <returns></returns>
        public Task<AssetBundleVersionInfo> RequestLocalVersion()
        {
            var resultTask = new TaskCompletionSource<AssetBundleVersionInfo>();
            RequestLocalVersion((abvi) =>
            {
                resultTask.SetResult(abvi);
            });
            return resultTask.Task;
        }

        /// <summary>
        /// 请求远程版本信息
        /// </summary>
        /// <param name="callback"></param>
        public void RequestRemoteVersion(Action<AssetBundleVersionInfo> callback)
        {
            RemoteVersion = null;
            string versionAssetPath = Path.Combine(_remoteUpdatePath, _assetVersionTxt);
            _webRequest.RequestText(versionAssetPath, null,(result, content) =>
            {
                if (result && !string.IsNullOrEmpty(content))
                {
                    content = content.ToEncrypt();
                    RemoteVersion = JsonUtility.FromJson<AssetBundleVersionInfo>(content);
                }

                if (RemoteVersion == null)
                {
                    throw new GameException($"Can't transition remote [AssetBundleVersionInfo]!! {versionAssetPath} {content}");
                }

                callback?.Invoke(RemoteVersion);
            });
        }

        /// <summary>
        /// 请求远程版本信息
        /// </summary>
        /// <returns></returns>
        public Task<AssetBundleVersionInfo> RequestRemoteVersion()
        {
            var resultTask = new TaskCompletionSource<AssetBundleVersionInfo>();
            RequestRemoteVersion((abvi) =>
            {
                resultTask.SetResult(abvi);
            });
            return resultTask.Task;
        }

        /// <summary>
        /// 检查资源
        /// </summary>
        /// <returns></returns>
        public async void CheckResource(Action<bool, AssetBundleVersionInfo, AssetBundleVersionInfo> callback)
        {
            var local = await RequestLocalVersion();
            var remote = await RequestRemoteVersion();
            bool result = false;
            if (LocalVersion == null || !LocalVersion.Equals(RemoteVersion))
                result = true;
            //检查资源
            callback?.Invoke(result, local, remote);
        }

        /// <summary>
        /// 检查资源
        /// </summary>
        /// <returns></returns>
        public bool CheckResource()
        {
            if (RemoteVersion == null)
            {
                throw new GameException("Request remote version information first!");
            }
            if (LocalVersion == null || !LocalVersion.Equals(RemoteVersion))
                return true;

            return false;
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool UpdateResource(Action<bool, float, float, ulong> callback)
        {
            if (_downloading)
                return false;
     
            if (!CheckResource())
            {
                callback?.Invoke(true,1, 0, 0);
            }
            //整理下载资源
            CollateDownloadResources(callback);
            return true;
        }

        /// <summary>
        /// 更新某一个资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> UpdateResource(string name, Action<bool, float, float, ulong> callback)
        {
            if (_downloading)
                return false;
            if (RemoteVersion == null)
            {
                throw new GameException("Request remote version information first!");
            }
          
            var ahif = RemoteVersion.AssetHashInfos.Find(x => x.Name.Equals(name));
            if (ahif != null)
            {
                string md5 = await CheckFileMD5(ahif.Name);
                if (!ahif.Hash.Equals(md5))
                {
                    _needDownloadFiles.Add(ahif);
                    //下载资源
                    DownloadFiles(callback);
                }
                else
                {
                    callback?.Invoke(true, 1, 0, 0);
                }
            }
            else
            {
                throw new GameException($"No corresponding resource was found: {name}");
            }
            return true;
        }

        #region 事件回调
      
        #endregion

        #region 内部函数
        /// <summary>
        /// 更新本地版本信息
        /// </summary>
        private void UpdateLocalVersion()
        {
            if (CheckResource())
            {
                LocalVersion = RemoteVersion;

                string versionAssetPath = Path.Combine(_localResourcePath, _assetVersionTxt);
                string content = JsonUtility.ToJson(LocalVersion).ToEncrypt();
                if (File.Exists(versionAssetPath))
                {
                    File.Delete(versionAssetPath);
                }
                File.WriteAllText(versionAssetPath, content);
            }
        }

        //整理下载资源
        private async void CollateDownloadResources(Action<bool, float, float, ulong> callback)
        {
            _needDownloadFiles.Clear();
            if (LocalVersion == null)
            {
                _needDownloadFiles.AddRange(RemoteVersion.AssetHashInfos);
            }
            else
            {
                foreach (var item in RemoteVersion.AssetHashInfos)
                {
                    if (!item.ForceUpdate)
                        continue;
                    string md5 = await CheckFileMD5(item.Name);
                    if (!item.Hash.Equals(md5))
                    {
                        _needDownloadFiles.Add(item);
                    }
                }
            }
            //下载资源
            if (_needDownloadFiles.Count > 0)
                DownloadFiles(callback);
            else
            {
                callback?.Invoke(true,1, 0, 0);
            }
        }

        //下载文件
        private async void DownloadFiles(Action<bool, float, float, ulong> callback)
        {
            if (_needDownloadFiles.Count > 0)
            {
                _downloading = true;
                //总文件大小
                ulong _totleFileSize = 0;
                //总共的下载的文件大小
                double totleDownloadSize = 0;
                //  float downloadStartTime = Time.realtimeSinceStartup;
                float progress = 0.0f;
                //文件总大小
                foreach (var item in _needDownloadFiles)
                {
                    _totleFileSize += (ulong)item.Size;
                }

                string remoteUrl = "";
                string localPath = "";

                int downloadComplete = 0;
               
                foreach (var item in _needDownloadFiles)
                {
                    remoteUrl = Path.Combine(_remoteUpdatePath, item.Name);
                    localPath = Path.Combine(_localResourcePath, $"{item.Name}.download");
                    await UniTask.NextFrame();
                    UnityWebRequest downloadWebRequest = null;
                    ulong  downloadSize = 0;
                    _webRequest.Download(remoteUrl, localPath, (localUrl, webRequest, downloadTime) => {
                        downloadWebRequest = webRequest;
                        if (webRequest.isNetworkError)
                        {
                            callback?.Invoke(false, progress, 0, _totleFileSize);
                        }
                        else
                        {
                            double downloadBytes = webRequest.downloadedBytes * webRequest.downloadProgress;
                            float speed = (float)((downloadBytes / 1024.0f) / downloadTime);
                            downloadBytes += totleDownloadSize;
                            progress = Mathf.Clamp((float)((downloadBytes / 1024.0f) / _totleFileSize),0.0f,0.99f);
                            callback?.Invoke(true, progress, speed, _totleFileSize);

                            if (downloadWebRequest.isDone)
                            {
                                downloadSize = downloadWebRequest.downloadedBytes;
                            }
                        }
                        
                    });
                    await UniTask.WaitUntil(() => downloadSize>0);
                    await UniTask.NextFrame();

                    totleDownloadSize += downloadComplete;
                    if (!downloadWebRequest.isNetworkError)
                    {
                        //验证文件的完整性
                        string md5 = await FileUtility.GetFileMD5(localPath);
                        if (item.Hash.Equals(md5))
                        {
                            int index = localPath.LastIndexOf('.');
                            string targetPath = localPath.Substring(0, index);
                            if (File.Exists(targetPath))
                            {
                                File.Delete(targetPath);
                            }
                            File.Move(localPath, targetPath);
                            downloadComplete++;
                        }
                    }
                    else
                    {
                        callback?.Invoke(false, progress, 0, _totleFileSize);
                    }
                    //清理下载器
                    downloadWebRequest.Dispose();
                    downloadWebRequest = null;
                }
                //下载完成
                if (downloadComplete == _needDownloadFiles.Count)
                {
                    UpdateLocalVersion();
                    callback?.Invoke(true, 1.0f, 0, _totleFileSize);
                }
                else
                {
                    callback?.Invoke(false, progress, 0, _totleFileSize);
                }

            }
        }

        //获取文件的md5
        private Task<string> CheckFileMD5(string fileName)
        {
            string localFilePath = Path.Combine(_localResourcePath, fileName);
            return FileUtility.GetFileMD5(localFilePath);
        }
        #endregion

	}
}
