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
        //更新中
        private bool _updating = false;


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

            if (!File.Exists(versionAssetPath))
            {
                callback?.Invoke(LocalVersion);
                return;
            }
            versionAssetPath = $"file:///{versionAssetPath}";
            _webRequest.RequestText(versionAssetPath, null,(result, content) =>
            {
                if (result && !string.IsNullOrEmpty(content))
                {
                    content = content.ToEncrypt();
                    LocalVersion = JsonUtility.FromJson<AssetBundleVersionInfo>(content);
                }
                //本地可能就没有版本信息
                callback?.Invoke(LocalVersion);
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
        /// <param name="callback">下载回调[进度(0-1)，大小(KB),速度(KB/S),剩余时间(s)]</param>
        /// <param name="downloadComplete">下载完成</param>
        /// <param name="errorCallback">下载错误</param>
        /// <param name="name">是否是更新单个资源,需要更新则传单个文件的名称</param>
        /// <returns></returns>
        public bool UpdateResource(Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback,string name=null)
        {
            if (_updating)
                return false;
     
            if (!CheckResource())
            {
                return false;
            }
            _updating = true;
            //整理下载资源
             CollateDownloadResources((needDownloadFiles)=> {
                 DownloadFiles(needDownloadFiles, callback, downloadComplete, errorCallback);
             }, name);
            return true;
        }

        /// <summary>
        /// 检查某一个资源是否需要更新
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback">[本地是否存在文件，是否需要更新]</param>
        public async void CheckResource(string name, Action<bool,bool> callback)
        {
            
#if UNITY_EDITOR
            if (GameFrameworkMode.GetModule<ResourceManager>().ResUpdateType == ResourceUpdateType.Editor)
            {
                callback?.Invoke(true,false);
                return;
            }
#endif
            name = name.ToLower();
            var ahif = RemoteVersion.AssetHashInfos.Find(x => x.Name.Equals(name));
            if (ahif != null)
            {
                string md5 = await CheckFileMD5(ahif.Name);
                callback?.Invoke(!string.IsNullOrEmpty(md5),!ahif.Hash.Equals(md5));
            }
            else
            {
                callback?.Invoke(false, true);
                //throw new GameException($"There is no corresponding resource on the resource server: {name}");
            }
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

        /// <summary>
        /// 整理下载资源
        /// </summary>
        /// <param name="name">资源名称</param>
        private async void CollateDownloadResources(Action<List<AssetHashInfo>> needDownloadCallback, string name = null)
        {
            List<AssetHashInfo> needDownloadFiles = new List<AssetHashInfo>();
            if (string.IsNullOrEmpty(name))
            {
                //遍历需要下载的文件
                foreach (var item in RemoteVersion.AssetHashInfos)
                {
                    if (!item.ForceUpdate)
                        continue;
                    string md5 = await CheckFileMD5(item.Name);
                    if (!item.Hash.Equals(md5))
                    {
                        needDownloadFiles.Add(item);
                    }
                }
            }
            else
            {
                name = name.ToLower();
                var ahif = RemoteVersion.AssetHashInfos.Find(x => x.Name.Equals(name));
                if (ahif != null)
                {
                    string md5 = await CheckFileMD5(ahif.Name);
                    if (!ahif.Hash.Equals(md5))
                    {
                        needDownloadFiles.Add(ahif);
                    }
                }
            }
            //需要下载的文件大小
            needDownloadCallback?.Invoke(needDownloadFiles);
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="callback">[进度(0-1)，大小(KB),速度(KB/S),剩余时间(s)]</param>
        /// <param name="errorCallback">[文件路径，错误]</param>
        private void DownloadFiles(List<AssetHashInfo> needDownloadFiles,Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback)
        {
            //下载资源
            if (needDownloadFiles.Count > 0)
            {
                int downloadFileCount = 0;
                double totleFileSize = 0;
                Dictionary<string, AssetHashInfo> downloadFiles = new Dictionary<string, AssetHashInfo>();
                foreach (var item in needDownloadFiles)
                {
                    string remoteUrl = Path.Combine(_remoteUpdatePath, item.Name);
                    string localPath = Path.Combine(_localResourcePath, $"{item.Name}.download");
                    _webRequest.FileDownloader.AddDownloadFile(remoteUrl, localPath);
                    //整理文件大小
                    totleFileSize += item.Size;
                    downloadFiles.Add(localPath, item);
                }
                //下载文件
                _webRequest.FileDownloader.StartDownload((localPath, size, time, speed) =>
                {
                    float progress = Mathf.Clamp01((float)(size / totleFileSize));
                    float remainingTime = (float)((totleFileSize - size) / speed);
                    callback?.Invoke(progress, totleFileSize, speed, remainingTime);
                }, async (localPath) =>
                {
                    //验证文件的完整性
                    string md5 = await FileUtility.GetFileMD5(localPath);
                    var assetHashInfo = downloadFiles[localPath];
                    if (assetHashInfo.Hash.Equals(md5))
                    {
                        int index = localPath.LastIndexOf('.');
                        string targetPath = localPath.Substring(0, index);
                        if (File.Exists(targetPath))
                        {
                            File.Delete(targetPath);
                        }
                        File.Move(localPath, targetPath);
                        downloadFiles.Remove(localPath);
                        downloadFileCount++;
                        //下载完成
                        if (downloadFiles.Count == 0)
                        {
                            //更新本地资源
                            if (downloadFileCount == needDownloadFiles.Count)
                            {
                                UpdateLocalVersion();
                            }
                            needDownloadFiles = null;
                            _updating = false;
                            downloadComplete?.Invoke();
                        }
                    }
                    else
                    {
                        File.Delete(localPath);
                        throw new GameException($"File integrity verification failed. {localPath}");
                    }
                }, (localPath, error) =>
                {
                    errorCallback?.Invoke(localPath, error);
                });
            }
            else
            {
                //下载完成
                downloadComplete?.Invoke();
                _updating = false;
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
