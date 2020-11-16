// 资源版本管理
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

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
        //正在下载的文件
        private Dictionary<string, DownloadFileInfo> _downloadingFiles = new Dictionary<string, DownloadFileInfo>();
        //下载回调 报错，进度【0-1】 速度 大小
        private Action<bool, float, float, ulong> _downloadCallback;
        //总文件大小
        private ulong _totleFileSize = 0;
        //总共的下载的文件大小
        private bool _downloading = false;
        private ulong _totleDownloadSize = 0;
        private float _downloadStartTime = 0.0f;

        public ResourceVersion(string remoteUpdatePath, string localResourcePath)
        {
            //   _resource = GameFrameworkMode.GetModule<ResourceManager>();
            _webRequest = GameFrameworkMode.GetModule<WebRequestManager>();

            _remoteUpdatePath = remoteUpdatePath;
            _localResourcePath = localResourcePath;
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

            _webRequest.RequestText(versionAssetPath, (result, content) =>
            {
                if (result && !string.IsNullOrEmpty(content))
                {
                    content = content.ToEncrypt();
                    LocalVersion = JsonUtility.FromJson<AssetBundleVersionInfo>(content);
                }
                //本地可能就没有版本信息
                callback?.Invoke(null);
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
            _webRequest.RequestText(versionAssetPath, (result, content) =>
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
            _downloadCallback = callback;
            _downloadCallback?.Invoke(true,0, 0, 0);
            if (!CheckResource())
            {
                _downloadCallback?.Invoke(true,1, 0, 0);
                _downloadCallback = null;
            }
            //整理下载资源
            CollateDownloadResources();
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
            _downloadCallback = callback;
            _downloadCallback?.Invoke(true, 0, 0, 0);
            var ahif = RemoteVersion.AssetHashInfos.Find(x => x.Name.Equals(name));
            if (ahif != null)
            {
                string md5 = await CheckFileMD5(ahif.Name);
                if (!ahif.Hash.Equals(md5))
                {
                    _needDownloadFiles.Add(ahif);
                    //下载资源
                    DownloadFiles();
                }
                else
                {
                    _downloadCallback?.Invoke(true, 1, 0, 0);
                    _downloadCallback = null;
                }
            }
            else
            {
                throw new GameException($"No corresponding resource was found: {name}");
            }
            return true;
        }

        #region 事件回调
        //download 回调
        private async void OnDownloadCallback(string remoteUrl, string localPath, bool result, string message)
        {
            if (result)
            {
                var data = _downloadingFiles[localPath];
                //验证文件的完整性
                string md5 = await FileUtility.GetFileMD5(localPath);
                if (data.Asset.Hash.Equals(md5))
                {
                    int index = localPath.LastIndexOf('.');
                    string targetPath = localPath.Substring(0, index);
                    if (File.Exists(targetPath))
                    {
                        File.Delete(targetPath);
                    }
                    File.Move(localPath, targetPath);
                    data.Complete = true;
                    _downloadingFiles[localPath] = data;
                }
                bool flags = true;
				foreach (var item in _downloadingFiles)
				{
                    if (!item.Value.Complete)
                    {
                        flags = false;
                        break;
                    }
				}
                //所有的文件下载完成
                if (flags&& _needDownloadFiles.Count==_downloadingFiles.Count)
                {
                    UpdateLocalVersion();
                    _downloadCallback?.Invoke(true, 1.0f,0.0f, _totleFileSize);
                    _downloadCallback = null;
                    _downloading = false;
                }
            }
            else
            {
                _downloadCallback?.Invoke(false, 0, 0, 0);
            }
        }

        //download 下载进度
        private void OnDownloadProccess(string remoteUrl, string localPath, ulong fileSize, float progress, float seconds)
        {
            var data = _downloadingFiles[localPath];
            if (data.FileSize == 0 && fileSize > 0)
            {
                _totleFileSize += fileSize;
            }
            if (progress - data.Progress > 0.0f && fileSize > 0)
            {
                _totleDownloadSize += (ulong)(fileSize * (progress - data.Progress));
            }
            data.Progress = progress;
            data.DownloadSize =(ulong)(fileSize * progress);
            data.FileSize = fileSize;
            data.Second = seconds;
            _downloadingFiles[localPath] = data;
            //下载进度回调
            if (Time.realtimeSinceStartup - _downloadStartTime > 0.0f)
            {
                float p = _totleDownloadSize / (float)_totleFileSize;
                float speed = _totleDownloadSize/1024.0f/ (Time.realtimeSinceStartup - _downloadStartTime);
                _downloadCallback?.Invoke(true, p, speed, _totleFileSize);
            }
        }

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
        private async void CollateDownloadResources()
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
                DownloadFiles();
            else
            {
                _downloadCallback?.Invoke(true,1, 0, 0);
                _downloadCallback = null;
            }
        }

        //下载文件
        private void DownloadFiles()
        {
            _downloadingFiles.Clear();
            if (_needDownloadFiles.Count > 0)
            {
                _downloading = true;
                _totleFileSize = 0;
                _totleDownloadSize = 0;
                _downloadStartTime = Time.realtimeSinceStartup;
               
                string remoteUrl = "";
                string localPath = "";

                foreach (var item in _needDownloadFiles)
                {
                    remoteUrl = Path.Combine(_remoteUpdatePath, item.Name);
                    localPath = Path.Combine(_localResourcePath, $"{item.Name}.download");
                    _downloadingFiles.Add(localPath,new DownloadFileInfo() { Asset=item});
                    _webRequest.Download(remoteUrl, localPath, OnDownloadCallback, OnDownloadProccess);
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

        //下载文件信息
        public struct DownloadFileInfo
        {
            /// <summary>
            /// 资源信息
            /// </summary>
            public AssetHashInfo Asset;
            /// <summary>
            /// 下载大小
            /// </summary>
            public ulong DownloadSize;
            /// <summary>
            /// 文件大小
            /// </summary>
            public ulong FileSize;
            /// <summary>
            /// 下载时间
            /// </summary>
            public float Second;
            /// <summary>
            /// 下载进度
            /// </summary>
            public float Progress;
            /// <summary>
            /// 下载完整
            /// </summary>
            public bool Complete;
        }
	}
}
