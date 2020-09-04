//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #AssetBundle资源管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月24日 17点00分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace Wanderer.GameFramework
{
    public sealed class BundleResourceHelper : IResourceHelper
    {
        //路径类型
        private PathType _pathType;
        //路径
        private string _readPath;
        //资源引用
        private AssetBundleManifest _mainfest;
        //默认加密
        private bool _isEncrypt=true;

        //当前激活的Assetbundle
        private readonly Dictionary<string, AssetBundle> _liveAssetBundle = new Dictionary<string, AssetBundle>();
        //assetbundle的引用计数
       private readonly Dictionary<AssetBundle,int> _assetBundleReferenceCount=new Dictionary<AssetBundle, int>();
        //资源路径映射ab包的名称
        private readonly Dictionary<string,string> _assetsPathMapAssetbundleName=new Dictionary<string, string>();

        #region  IResourceHelper

        public void SetResource(PathType pathType,Action callback)
        {
             switch (pathType)
            {
                case PathType.ReadOnly:
                    _readPath = Application.streamingAssetsPath;
                    #if UNITY_IOS && !UNITY_EDITOR
                    _readPath = $"file:///{_readPath}";
                    #endif
                    break;
                case PathType.ReadWrite:
                    _readPath = Application.persistentDataPath;
                    break;
                case PathType.DataPath:
                    _readPath = Application.dataPath;
                    break;
                case PathType.TemporaryCache:
                    _readPath = Application.temporaryCachePath;
                    break;
                default:
                    _readPath = Application.persistentDataPath;
                    break;
            }

            _pathType=pathType;

            //加载主包
            LoadPlatformMainfest(_readPath,callback);
        }

        public void LoadAsset<T>(string assetName, Action<T> callback) where T : Object
        {
            if(_assetsPathMapAssetbundleName.TryGetValue(assetName,out string abName))
            {
                if(_liveAssetBundle.TryGetValue(abName,out AssetBundle assetBundle))
                {
                     T t = assetBundle.LoadAsset<T>(assetName);
                    callback.Invoke(t);
                    SetAssetBundleReferenceCount(assetBundle,+1);
                 
                }
                else
                {
                    LoadAssetBundle(abName,(ab)=>{
                        T t = ab.LoadAsset<T>(assetName);
                        callback.Invoke(t);
                        SetAssetBundleReferenceCount(ab,+1);
                    });
                }
            }
            else
            {
                callback?.Invoke(null);
            }
        }

        public void UnloadAsset(string assetName)
        {
            if(_assetsPathMapAssetbundleName.TryGetValue(assetName,out string abName))
            {
                if(_liveAssetBundle.TryGetValue(abName,out AssetBundle assetBundle))
                {
                    SetAssetBundleReferenceCount(assetBundle,-1);
                }
            }
        }

        public void UnloadAssetBunlde(string assetBundleName, bool unload = false)
        {
            if(_liveAssetBundle.TryGetValue(assetBundleName,out AssetBundle assetBundle))
            {
                if(_assetBundleReferenceCount.ContainsKey(assetBundle))
                {
                    _assetBundleReferenceCount.Remove(assetBundle);
                }
                assetBundle.Unload(unload);
            }
        }

          /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        public async void LoadSceneAsync(string sceneName, LoadSceneMode mode,Action<AsyncOperation> callback)
        {
            AsyncOperation asyncOperation = null;
            try
            {
                if(_assetsPathMapAssetbundleName.TryGetValue(sceneName,out string assetBundleName))
                {
                    string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                    //加载依赖项
                    await LoadDependenciesAssetBundle(assetBundleName);
                    //添加
                    AssetBundle assetBundle = await LoadAssetBundleFromPath(assetBundlePath);
                    
                    asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);

                    callback?.Invoke(asyncOperation);

                    SetAssetBundleReferenceCount(assetBundle,+1);
                    //场景加载完成卸载相关的引用
                    // asyncOperation.completed += (operation02) =>
                    // {
                    //     assetBundle.Unload(false);
                    // };
                }
                else
                {
                    throw new GameException($"Can't find scene assetbundle :{sceneName}");
                }

            }
            catch (GameException ex)
            {
                Debug.LogError(ex.ToString());
            }

        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneName"></param>
        public AsyncOperation UnloadSceneAsync(string sceneName)
        {
            if(_assetsPathMapAssetbundleName.TryGetValue(sceneName,out string assetBundleName))
            {
                if(_liveAssetBundle.TryGetValue(sceneName,out AssetBundle assetBundle))
                {
                    SetAssetBundleReferenceCount(assetBundle,-1);
                }
            }
            return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }

        #endregion


        #region 事件回调
        /// <summary>
        /// 清理资源
        /// </summary>
        public void Clear()
        {
            _assetsPathMapAssetbundleName.Clear();
            _assetBundleReferenceCount.Clear();
            _mainfest=null;
            foreach (var item in _liveAssetBundle.Values)
            {
                item.Unload(true);
            }
            _liveAssetBundle.Clear();
        }

        #endregion

        #region 内部函数
        /// <summary>
        /// 加载mainfest -- LoadFromFile
        /// </summary>
        private async void LoadPlatformMainfest(string rootPath,Action callback)
        {
            try
            {
                AssetBundleVersionInfo versionInfo;
                string assetVersionPath = Path.Combine(rootPath,"AssetVersion.txt");
                using(UnityWebRequest request = new UnityWebRequest(assetVersionPath))
                {
                    request.downloadHandler=new DownloadHandlerBuffer();
                    await request.SendWebRequest();
                    if(request.isNetworkError)
                    {
                        throw new GameException($"Can't read assetbundle file : {assetVersionPath} error: {request.error}");
                    }
                    string content = request.downloadHandler.text;
                    versionInfo = JsonUtility.FromJson<AssetBundleVersionInfo>(content);
                    if(versionInfo==null)
                    {
                        throw new GameException($"Can't deserialize [AssetVersion.txt] file: {assetVersionPath} content:{content} error: {request.error}");
                    }
                }

                //取根目录的assetbundle
                string rootBundlePath = Path.Combine(rootPath,versionInfo.ManifestAssetBundle);
                AssetBundle mainfestAssetBundle = await LoadAssetBundleFromPath(rootBundlePath);
                _mainfest = mainfestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                mainfestAssetBundle.Unload(false);

                //加载所有的资源路径与ab包名称的映射
                LoadAllAssetPathForAssetbundle(Path.Combine(_readPath,"assets"),callback);
            }
            catch (GameException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 加载资源路径映射的ab包的名称
        /// </summary>
        /// <param name="assetsPath"></param>
        /// <returns></returns>
        private async void LoadAllAssetPathForAssetbundle(string assetsPath,Action callback)
        {
            using(UnityWebRequest request = new UnityWebRequest(assetsPath))
            {
                request.downloadHandler=new DownloadHandlerBuffer();
                await request.SendWebRequest();
             
                if(request.isNetworkError)
                {
                    throw new GameException($"Can't read assets path file from streamingasset: {assetsPath} error: {request.error}");
                }
                byte[] buffer = request.downloadHandler.data;
                using(MemoryStream stream =new EncryptMemoryStream(buffer))
                {
                    stream.Read(buffer,0,buffer.Length);
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    _assetsPathMapAssetbundleName.Clear();
                    string[] lines=content.Split('\n');
                    foreach (var item in lines)
                    {
                        if(!string.IsNullOrEmpty(item))
                        {
                            string[] args = item.Split('\t');
                            if(args!=null&&args.Length>=2)
                            {
                                _assetsPathMapAssetbundleName[args[0].Trim()]=args[1].Trim();
                            }
                        }
                    }
                }

                //路径准备好了
                callback?.Invoke();
               // GameFrameworkMode.GetModule<EventManager>().Trigger<ResourceAssetPathsMapReadyEventArgs>(this);
            }
        }

          //加载asstbundle
        private async void LoadAssetBundle(string assetBundleName, Action<AssetBundle> callback)
        {
             //加载Assetbundle
            AssetBundle assetBundle;
            if (!_liveAssetBundle.TryGetValue(assetBundleName,out assetBundle))
            {
                string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                //先加载引用的assetbundle 
                await LoadDependenciesAssetBundle(assetBundleName);
                //加载assetbundle
                assetBundle = await LoadAssetBundleFromPath(assetBundlePath);
                //存储assetbundle
                _liveAssetBundle[assetBundleName]=assetBundle;
                //assetbundle 引用计数
                _assetBundleReferenceCount[assetBundle]=0;
            }
            callback.Invoke(assetBundle);
        }

        //同步加载AssetBundle
        private Task<AssetBundle> LoadAssetBundleFromPath(string path)
        {
            var taskResult = new TaskCompletionSource<AssetBundle>();

            if(_pathType==PathType.ReadOnly)
            {
               LoadAssetBundleFromStreamingAssets(path,(ab)=>{
                   taskResult.SetResult(ab);
               });
            }
            else
            {
                if (!File.Exists(path))
                    throw new GameException("Assetbundle not found :" + path);
                AssetBundle assetbundle;
                if(_isEncrypt)
                {
                    using (var stream = new EncryptFileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
                    {
                        assetbundle = AssetBundle.LoadFromStream(stream);
                    }
                }
                else
                {
                    assetbundle = AssetBundle.LoadFromFile(path);
                }
                taskResult.SetResult(assetbundle);
            }   
           
            return taskResult.Task;
        }

        //加载引用的assetbundle --引用的assetbundle不卸载
        private async Task LoadDependenciesAssetBundle(string assetBundleName)
        {
            //加载相关依赖 依赖暂时不异步加载了
            string[] dependencies = _mainfest.GetAllDependencies(assetBundleName);
            foreach (var item in dependencies)
            {
                if(_liveAssetBundle.ContainsKey(item))
                    continue;

                string dependenciesBundlePath = Path.Combine(_readPath, item);
                AssetBundle assetBundle = await LoadAssetBundleFromPath(dependenciesBundlePath);
                  //存储资源名称
                _liveAssetBundle[item]= assetBundle;
            }
        }

        /// <summary>
        /// 从StreamingAsset下面读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private async void LoadAssetBundleFromStreamingAssets(string path,Action<AssetBundle> callback)
        {   
            if(_isEncrypt)
            {
                using(UnityWebRequest request = new UnityWebRequest(path))
                {
                    request.downloadHandler=new DownloadHandlerBuffer();
                    await request.SendWebRequest();
                    if(request.isNetworkError)
                    {
                        throw new GameException($"Can't read assetbundle file from streamingasset: {path} error: {request.error}");
                    }
                    AssetBundle ab;
                    byte[] buffer=request.downloadHandler.data;
                    using(MemoryStream stream =new EncryptMemoryStream(buffer))
                    {
                        ab = AssetBundle.LoadFromStream(stream);
                        callback?.Invoke(ab);
                    }
                }
            }
            else
            {
                using(UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path))
                {
                  //  request.downloadHandler=new DownloadHandlerAssetBundle();
                    await request.SendWebRequest();
                    if(request.isNetworkError)
                    {
                        throw new GameException($"Can't read assetbundle file from streamingasset: {path} error: {request.error}");
                    }
                    AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
                    callback?.Invoke(ab);
                }
            }
            
        }

        /// <summary>
        /// 设置assetbundle的引用计数
        /// </summary>
        /// <param name="assetBundle"></param>
        private void SetAssetBundleReferenceCount(AssetBundle assetBundle,int interval)
        {
            if(_assetBundleReferenceCount.ContainsKey(assetBundle))
            {
                _assetBundleReferenceCount[assetBundle]+=interval;
            }
        }
        #endregion

    }
}
