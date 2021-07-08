//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #AssetBundle资源管理类# </describe>
// <email> dutifulwanderer@gmail.com </email>
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
using Cysharp.Threading.Tasks;


namespace Wanderer.GameFramework
{
    internal sealed class BundleAssetsHelper : MonoBehaviour, IAssetsHelper
    {
        //路径
        private string _readPath;
        //资源引用
        private AssetBundleManifest _mainfest;
        //默认加密
        private bool _isEncrypt = true;
        //当前激活的Assetbundle
        private readonly Dictionary<string, AssetBundle> _liveAssetBundle = new Dictionary<string, AssetBundle>();
        //assetbundle的引用计数
        private readonly Dictionary<AssetBundle, int> _assetBundleReferenceCount = new Dictionary<AssetBundle, int>();
        //资源路径映射ab包的名称
        private readonly Dictionary<string, string> _assetsPathMapAssetbundleName = new Dictionary<string, string>();
        //所有的资源路径
        private List<string> _allAssetPaths = new List<string>();
        public List<string> AllAssetPaths
        {
            get
            {
                if (_allAssetPaths.Count == 0|| _allAssetPaths.Count!= _assetsPathMapAssetbundleName.Count)
                {
                    _allAssetPaths.Clear();
                    _allAssetPaths.AddRange(_assetsPathMapAssetbundleName.Keys);
                }
                return _allAssetPaths;
            }
        }
        /// <summary>
        /// 预加载的所有资源
        /// </summary>
        private Dictionary<string, Object> _preloadAssets = new Dictionary<string, Object>();
        //异步加载锁定的文件
        private HashSet<string> _asyncLockFiles = new HashSet<string>();

        /// <summary>
        /// 设置资源准备
        /// </summary>
        /// <param name="pathType"></param>
        /// <param name="callback"></param>
        public void SetResource(Action callback)
        {
            PathType pathType = GameFrameworkMode.GetModule<ResourceManager>().LocalPathType;
            switch (pathType)
            {
                case PathType.ReadOnly:
                    _readPath = Application.streamingAssetsPath;
#if UNITY_ADNROID && UNITY_EDITOR
                    throw new GameException($"Android does not support reading files in the StreamingAssets folder.AssetBundle is not the only resource!");
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

            //加载主包
            StartCoroutine(LoadPlatformMainfest(_readPath, callback));
        }

    

        /// <summary>
        /// 资源预加载
        /// </summary>
        /// <param name="progressCallback"></param>
        public void Preload(Action<float> progressCallback)
        {
            var version = GameFrameworkMode.GetModule<ResourceManager>().Version.LocalVersion;
            if (version == null)
            {
                throw new GameException("Version information for the local resource was not found！");
            }
            List<AssetHashInfo> preloadAsset = new List<AssetHashInfo>();
            foreach (var item in version.AssetHashInfos)
            {
                if (item.Preload)
                {
                    preloadAsset.Add(item);
                }
            }
            //协程异步加载
            StartCoroutine(ResourcePreload(preloadAsset, progressCallback));
        }
        
        /// <summary>
        /// 异步加载Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void LoadAsset<T>(string assetName, Action<T> callback) where T : Object
        {
            assetName = assetName.ToLower();
            if (_preloadAssets.TryGetValue(assetName, out Object asset))
			{
				callback?.Invoke((T)asset);
			}
			else
			{
				if (_assetsPathMapAssetbundleName.TryGetValue(assetName, out string abName))
				{
					LoadAssetBunldeAsync(abName, (ab) =>
					{
                        StartCoroutine(LoadAssetAsync<T>(assetName,ab,callback,null));
					});
					
				}
				else
				{
					throw new GameException($"Can't find asset assetbundle :{assetName}");
				}
			}
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            assetName = assetName.ToLower();
            if (_preloadAssets.TryGetValue(assetName, out Object asset))
            {
                return (T)asset;
            }

            if (_assetsPathMapAssetbundleName.TryGetValue(assetName, out string abName))
            {
                AssetBundle assetBundle = LoadAssetBundle(abName);
                T t = assetBundle.LoadAsset<T>(assetName);
                SetAssetBundleReferenceCount(assetBundle, +1);
                return t;
            }
            else
            {
                throw new GameException($"Can't find asset assetbundle :{assetName}");
            }

        }

        public T[] FindAssets<T>(List<string> tags) where T : UnityEngine.Object
        {
            HashSet<string> lowerTags = new HashSet<string>();
            foreach (var item in tags)
            {
                lowerTags.Add(item.ToLower());
            }

            List<string> matchPaths = new List<string>();
            foreach (var item in _allAssetPaths)
            {
                bool contains = true;
                foreach (var tag in lowerTags)
                {
                    contains = item.Contains(tag);
                    if (!contains)
                        break;
                }
                if (contains)
                {
                    matchPaths.Add(item);
                }
            }

            List<T> result = new List<T>();
            foreach (var item in matchPaths)
            {
                T asset = LoadAsset<T>(item);
                if (asset != null)
                {
                    result.Add(asset);
                }
            }

            return result.ToArray();
        }

        public void FindAssets<T>(List<string> tags, Action<T[]> callback) where T : UnityEngine.Object
        {
            HashSet<string> lowerTags = new HashSet<string>();
            foreach (var item in tags)
            {
                lowerTags.Add(item.ToLower());
            }

            List<string> matchPaths = new List<string>();
            foreach (var item in _allAssetPaths)
            {
                bool contains = true;
                foreach (var tag in lowerTags)
                {
                    contains = item.Contains(tag);
                    if (!contains)
                        break;
                }
                if (contains)
                {
                    matchPaths.Add(item);
                }
            }

            List<T> result = new List<T>();
            int resultCount = 0;
            foreach (var item in matchPaths)
            {
                LoadAsset<T>(item,(asset)=> {
                    resultCount++;
                    if (asset != null)
                    {
                        result.Add(asset);
                    }
                    if (resultCount >= matchPaths.Count)
                    {
                        callback?.Invoke(result.ToArray());
                    }
                });
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetName"></param>
        public void UnloadAsset(string assetName)
        {
            assetName = assetName.ToLower();
            if (_assetsPathMapAssetbundleName.TryGetValue(assetName, out string abName))
            {
                if (_liveAssetBundle.TryGetValue(abName, out AssetBundle assetBundle))
                {
                    SetAssetBundleReferenceCount(assetBundle, -1);
                }
            }
        }
       
        /// <summary>
        /// 卸载assetBundle
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="unload"></param>
        public void UnloadAssetBunlde(string assetBundleName, bool unload = false)
        {
            assetBundleName = assetBundleName.ToLower();
            if (_liveAssetBundle.TryGetValue(assetBundleName, out AssetBundle assetBundle))
            {
                if (_assetBundleReferenceCount.ContainsKey(assetBundle))
                {
                    _assetBundleReferenceCount.Remove(assetBundle);
                }
                _liveAssetBundle.Remove(assetBundleName);
                assetBundle.Unload(unload);
            }
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, Action<AsyncOperation> callback)
        {
            AsyncOperation asyncOperation = null;
            try
            {
                if (_assetsPathMapAssetbundleName.TryGetValue(sceneName, out string assetBundleName))
                {
                    LoadAssetBunldeAsync(sceneName, (ab) =>
                    {
                        asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
                        SetAssetBundleReferenceCount(ab, +1);
                        callback?.Invoke(asyncOperation);
                    });
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
            if (_assetsPathMapAssetbundleName.TryGetValue(sceneName, out string assetBundleName))
            {
                if (_liveAssetBundle.TryGetValue(sceneName, out AssetBundle assetBundle))
                {
                    SetAssetBundleReferenceCount(assetBundle, -1);
                }
            }
            return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }

		#region 内部函数
		#region 资源准备
		/// <summary>
		/// 加载mainfest -- LoadFromFile
		/// </summary>
		private IEnumerator LoadPlatformMainfest(string rootPath, Action callback)
        {
            //取根目录的assetbundle
            string rootBundlePath = Path.Combine(rootPath, "manifest");
            yield return LoadAssetBundleFromPathAsync(rootBundlePath, (mainfestAssetBundle) => {
                _mainfest = mainfestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                mainfestAssetBundle.Unload(false);
            }, null);

            bool readComplete = false;
            Task.Run(() =>
            {
                string assetsPath = Path.Combine(_readPath, "assets");
                _assetsPathMapAssetbundleName.Clear();
                byte[] buffer = File.ReadAllBytes(assetsPath);
                using (MemoryStream stream = new EncryptMemoryStream(buffer))
                {
                    stream.Read(buffer, 0, buffer.Length);
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    _assetsPathMapAssetbundleName.Clear();
                    string[] lines = content.Split('\n');
                    foreach (var item in lines)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] args = item.Split('\t');
                            if (args != null && args.Length >= 2)
                            {
                                _assetsPathMapAssetbundleName[args[0].Trim()] = args[1].Trim();
                            }
                        }
                    }
                    readComplete = true;
                }
            });
            while (!readComplete)
            {
                yield return null;
            }
            yield return null;
            //没有资源进行警告
            if (_assetsPathMapAssetbundleName.Count == 0)
            {
                Debug.LogWarning($"Assetbundle no resource!");
            }
            callback?.Invoke();
        }
      
		#endregion

		#region 同步加载
		//加载assetbundle
		private AssetBundle LoadAssetBundle(string assetBundleName)
        {
            //加载Assetbundle
            AssetBundle assetBundle;
            if (!_liveAssetBundle.TryGetValue(assetBundleName, out assetBundle))
            {
                string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                //先加载引用的assetbundle 
                LoadDependenciesAssetBundle(assetBundleName);
                //加载assetbundle
                assetBundle = LoadAssetBundleFromPath(assetBundlePath);
                //存储assetbundle
                AddAssetBundle(assetBundleName, assetBundle);
            }
            return assetBundle;
        }

        //加载引用的assetbundle --引用的assetbundle不卸载
        private void LoadDependenciesAssetBundle(string assetBundleName)
        {
            //加载相关依赖 依赖暂时不异步加载了
            string[] dependencies = _mainfest.GetAllDependencies(assetBundleName);
            foreach (var item in dependencies)
            {
                if (_liveAssetBundle.ContainsKey(item))
                    continue;

                string dependenciesBundlePath = Path.Combine(_readPath, item);
                AssetBundle assetBundle = LoadAssetBundleFromPath(dependenciesBundlePath);
                //存储资源名称
                AddAssetBundle(item, assetBundle);
            }
        }

        //同步加载AssetBundle
        private AssetBundle LoadAssetBundleFromPath(string path)
        {
            if (!File.Exists(path))
                throw new GameException("Assetbundle not found :" + path);

            using (Stream abStream = _isEncrypt ? new EncryptFileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false) : new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
            {
              //  float loadTime = Time.realtimeSinceStartup;
                var assetbundle = AssetBundle.LoadFromStream(abStream);
                //loadTime = Time.realtimeSinceStartup - loadTime;
                //Debug.Log($"LoadAssetBundleFromPath sync time spent:  {loadTime} {path}");
                return assetbundle;
            }
        }
		#endregion

        #region 异步加载
        //异步加载资源
        private IEnumerator LoadAssetAsync<T>(string assetName, AssetBundle assetBundle, Action<T> callback, Action<float> progress) where T : Object
        {
         //   float loadTime = Time.realtimeSinceStartup;
            assetName = assetName.ToLower();
            var abRequest = assetBundle.LoadAssetAsync<T>(assetName);
            while (!abRequest.isDone)
            {
                progress?.Invoke(abRequest.progress);
                yield return null;
            }
            yield return null;
            //loadTime = Time.realtimeSinceStartup - loadTime;
            //Debug.Log($"LoadAsset Async time spent: {assetName} {loadTime}");
            callback?.Invoke((T)abRequest.asset);
        }

        //异步加载AssetBundle
        private void LoadAssetBunldeAsync(string assetBundleName, Action<AssetBundle> callback)
        {
            //加载Assetbundle
            if (_liveAssetBundle.TryGetValue(assetBundleName, out AssetBundle assetBundle))
            {
                SetAssetBundleReferenceCount(assetBundle, +1);
                callback?.Invoke(assetBundle);
            }
            else
            {
                //先加载引用的assetbundle 
                StartCoroutine(LoadDependenciesAssetBundleAsync(assetBundleName, () =>
                {
                    string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                    StartCoroutine(LoadAssetBundleFromPathAsync(assetBundlePath, (ab) =>
                    {
                        AddAssetBundle(assetBundleName, ab);
                        SetAssetBundleReferenceCount(ab, +1);
                        callback?.Invoke(ab);
                    }, null));
                }));
            }
        }

        //加载引用的assetbundle --引用的assetbundle不卸载
        private IEnumerator LoadDependenciesAssetBundleAsync(string assetBundleName, Action callback)
        {
            //加载相关依赖 依赖暂时不异步加载了
            string[] dependencies = _mainfest.GetAllDependencies(assetBundleName);
            foreach (var item in dependencies)
            {
                if (_liveAssetBundle.ContainsKey(item))
                    continue;

                string dependenciesBundlePath = Path.Combine(_readPath, item);
                yield return LoadAssetBundleFromPathAsync(dependenciesBundlePath, (assetBundle) => {
                    //存储资源名称
                    AddAssetBundle(item, assetBundle);
                }, null);
            }
            yield return null;
            callback?.Invoke();
        }

        //异步加载AssetBundle
        private IEnumerator LoadAssetBundleFromPathAsync(string path, Action<AssetBundle> callback, Action<float> progress)
        {
            if (!File.Exists(path))
                throw new GameException("Assetbundle not found :" + path);

            if (_asyncLockFiles.Contains(path))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                while (!_liveAssetBundle.ContainsKey(fileName))
                {
                    yield return null;
                }
                callback.Invoke(_liveAssetBundle[fileName]);
            }
            else
            {
                _asyncLockFiles.Add(path);
                using (Stream abStream = _isEncrypt ? new EncryptFileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false) : new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
                {
                  //  float loadTime = Time.realtimeSinceStartup;
                    var ablfsa = AssetBundle.LoadFromStreamAsync(abStream);
                    while (!ablfsa.isDone)
                    {
                        progress?.Invoke(ablfsa.progress);
                        yield return null;
                    }
                    yield return null;
                    //loadTime = Time.realtimeSinceStartup - loadTime;
                    //Debug.Log($"LoadAssetBundleFromPath Async time spent: {loadTime} {path} ");
                    callback?.Invoke(ablfsa.assetBundle);
                }
                yield return null;
                _asyncLockFiles.Remove(path);
            }
        }
        #endregion

        #region 资源预加载
        private IEnumerator ResourcePreload(List<AssetHashInfo> preloadAsset, Action<float> progressCallback)
        {
         //   Application.backgroundLoadingPriority = ThreadPriority.High;
         //   float loadTime = Time.realtimeSinceStartup;
            _preloadAssets.Clear();
            float progressCount = 0;
            foreach (var item in preloadAsset)
			{
                yield return null;
                //  var ab = LoadAssetBundle(item.Name);
                AssetBundle ab = null;
                string abPath = Path.Combine(_readPath, item.Name);
                //异步获取资源
                StartCoroutine(LoadAssetBundleFromPathAsync(abPath, (loadAB) =>
                {
                    ab = loadAB;
                }, (progress) => {
                    progressCallback?.Invoke(GetPreloadProgress(0, progress, progressCount, preloadAsset.Count));
                }));
                //等待资源加载完成
                while (ab == null)
                {
                    yield return null;
                }
                AddAssetBundle(item.Name, ab);
                //异步加载资源
                var abRequest = ab.LoadAllAssetsAsync();
                while (!abRequest.isDone)
                {
                    progressCallback?.Invoke(GetPreloadProgress(0.3f, abRequest.progress, progressCount, preloadAsset.Count));
                    yield return null;
                }
                List<string> assetNames = new List<string>();
                assetNames.AddRange(ab.GetAllAssetNames());
                for (int i = 0; i < abRequest.allAssets.Length; i++)
				{
                    progressCallback?.Invoke(GetPreloadProgress(0.6f, i/(float)(abRequest.allAssets.Length-1), progressCount, preloadAsset.Count));

                    Object asset = abRequest.allAssets[i];
                    string assetName = asset.name.ToLower();
                    bool result = false;
                    string assetPath = null;
                    Task.Run(() => {
                        assetPath = assetNames.Find(x => Path.GetFileNameWithoutExtension(x).Equals(assetName));
                        result = true;
                    });
                    while (!result)
                    {
                        yield return null;
                    }
                    //string assetPath = assetNames.Find(x => Path.GetFileNameWithoutExtension(x).Equals(assetName));
                    if (string.IsNullOrEmpty(assetPath))
                    {
                        //throw new GameException($"The corresponding resource path was not found! {asset.name} {assetName}");
                        Debug.LogWarning($"There may be abnormal resources. {asset.name} {assetName}");
                    }
                    else 
                    {
                        if (!_preloadAssets.ContainsKey(assetPath))
                        {
                            _preloadAssets.Add(assetPath, asset);
                        }
                        else
                        {
                            Debug.LogError($"Preload redundant data. {assetPath}");
                        }
                    }
                }
                //UnloadAssetBunlde(item.Name);
                progressCount++;
            }
            //loadTime = Time.realtimeSinceStartup- loadTime;
            //Debug.Log($"Preload spent time: {loadTime}");
            progressCallback?.Invoke(1.0f);
        }

        //获取预加载的进度
        private float GetPreloadProgress(float startProgress,float progress,float progressIndex,int progressCount)
        {
            progress = (startProgress + 0.3f * progress + progressIndex) / progressCount;
            progress = Mathf.Clamp(progress, 0.0f, 0.995f);
            return progress;
        }
		#endregion

		#region 整理AssetBundle
		/// <summary>
		/// 添加assetBundle
		/// </summary>
		/// <param name="assetBundleName"></param>
		/// <param name="assetBundle"></param>
		private void AddAssetBundle(string assetBundleName, AssetBundle assetBundle)
        {
            if (!_liveAssetBundle.ContainsKey(assetBundleName))
            {
                //存储assetbundle
                _liveAssetBundle[assetBundleName] = assetBundle;
                //assetbundle 引用计数
                _assetBundleReferenceCount[assetBundle] = 0;
            }
        }


        /// <summary>
        /// 设置assetbundle的引用计数
        /// </summary>
        /// <param name="assetBundle"></param>
        private void SetAssetBundleReferenceCount(AssetBundle assetBundle, int interval)
        {
            if (_assetBundleReferenceCount.ContainsKey(assetBundle))
            {
                _assetBundleReferenceCount[assetBundle] += interval;
            }
        }
        #endregion
        #endregion

        #region 事件回调
        /// <summary>
        /// 清理资源
        /// </summary>
        public void Clear()
        {
            _assetsPathMapAssetbundleName.Clear();
            _assetBundleReferenceCount.Clear();
            _mainfest = null;
            foreach (var item in _liveAssetBundle.Values)
            {
                item.Unload(true);
            }
            _liveAssetBundle.Clear();
        }

        #endregion
    }
}
