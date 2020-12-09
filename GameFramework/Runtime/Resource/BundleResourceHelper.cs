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
    internal sealed class BundleResourceHelper : MonoBehaviour, IResourceHelper
    {
        //路径类型
        private PathType _pathType;
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
        //异步资源加载
        private Dictionary<AssetBundleRequest, Action<Object>> _assetAsyncRequest = new Dictionary<AssetBundleRequest, Action<Object>>();
        private Dictionary<string, Object> _preloadAssets = new Dictionary<string, Object>();
        private void Update()
        {
            //异步资源检测
			if (_assetAsyncRequest.Count > 0)
			{
				foreach (var item in _assetAsyncRequest)
				{
					if (item.Key.isDone)
					{
						item.Value.Invoke(item.Key.asset);
						_assetAsyncRequest.Remove(item.Key);
						break;
					}
				}
			}
		}

        public void SetResource(PathType pathType, Action callback)
        {
            switch (pathType)
            {
                case PathType.ReadOnly:
                    _readPath = Application.streamingAssetsPath;
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

            _pathType = pathType;

            //加载主包
            LoadPlatformMainfest(_readPath, callback);
        }

        /// <summary>
        /// 异步加载Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        public void LoadAsset<T>(string assetName, Action<T> callback) where T : Object
        {
            float loadTime = Time.realtimeSinceStartup;
            LoadAssetAsync(assetName, (asset) =>
            {
                loadTime = Time.realtimeSinceStartup - loadTime;
                Debug.Log($"LoadAsset Async time spent: {assetName} {loadTime}");
                callback?.Invoke((T)asset);
            });
        }

        //异步加载资源
        private void LoadAssetAsync(string assetName, Action<Object> callback)
        {
            assetName = assetName.ToLower();

            if (_preloadAssets.TryGetValue(assetName,out Object asset))
            {
                callback?.Invoke(asset);
                return;
            }

            if (_assetsPathMapAssetbundleName.TryGetValue(assetName, out string abName))
            {
                AssetBundle assetBundle;
                if (!_liveAssetBundle.TryGetValue(abName, out assetBundle))
                {
                    assetBundle=LoadAssetBundle(abName);
                }
                var assetBundleRequest = assetBundle.LoadAssetAsync(assetName);
                _assetAsyncRequest.Add(assetBundleRequest, callback);
            }
            else
            {
                throw new GameException($"Can't find asset assetbundle :{assetName}");
                // callback?.Invoke(null);
            }
        }

		public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            if (_preloadAssets.TryGetValue(assetName, out Object asset))
            {
                return (T)asset;
            }

            float loadTime = Time.realtimeSinceStartup;
            assetName = assetName.ToLower();
#if UNITY_ANDROID
            if (_pathType == PathType.ReadOnly)
            {
                throw new GameException("The Android path does not support synchronous loading under read-only paths!");
            }
#endif
            if (_assetsPathMapAssetbundleName.TryGetValue(assetName, out string abName))
            {
                AssetBundle assetBundle;
                if (!_liveAssetBundle.TryGetValue(abName, out assetBundle))
                {
                    assetBundle = LoadAssetBundle(abName);
                }
                T t = assetBundle.LoadAsset<T>(assetName);
                SetAssetBundleReferenceCount(assetBundle, +1);
                loadTime = Time.realtimeSinceStartup - loadTime;
                Debug.Log($"LoadAssetSync time spent: {assetName} {loadTime}");
                return t;
            }
            else
            {
                throw new GameException($"Can't find asset assetbundle :{assetName}");
            }

        }
      
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
       
        public void UnloadAssetBunlde(string assetBundleName, bool unload = false)
        {
            assetBundleName = assetBundleName.ToLower();
            if (_liveAssetBundle.TryGetValue(assetBundleName, out AssetBundle assetBundle))
            {
                if (_assetBundleReferenceCount.ContainsKey(assetBundle))
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
        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, Action<AsyncOperation> callback)
        {
            AsyncOperation asyncOperation = null;
            try
            {
                if (_assetsPathMapAssetbundleName.TryGetValue(sceneName, out string assetBundleName))
                {
                    string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                    //加载依赖项
                    LoadDependenciesAssetBundle(assetBundleName);
                    //添加
                    AssetBundle assetBundle = LoadAssetBundleFromPathSync(assetBundlePath);

                    asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);

                    callback?.Invoke(asyncOperation);

                    SetAssetBundleReferenceCount(assetBundle, +1);
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
            if (_assetsPathMapAssetbundleName.TryGetValue(sceneName, out string assetBundleName))
            {
                if (_liveAssetBundle.TryGetValue(sceneName, out AssetBundle assetBundle))
                {
                    SetAssetBundleReferenceCount(assetBundle, -1);
                }
            }
            return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }

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

#region 内部函数
        /// <summary>
        /// 加载mainfest -- LoadFromFile
        /// </summary>
        private void LoadPlatformMainfest(string rootPath, Action callback)
        {
            try
            {
                //取根目录的assetbundle
                string rootBundlePath = Path.Combine(rootPath, "manifest");
                AssetBundle mainfestAssetBundle = LoadAssetBundleFromPathSync(rootBundlePath);
                _mainfest = mainfestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                mainfestAssetBundle.Unload(false);

                //加载所有的资源路径与ab包名称的映射
                LoadAllAssetPathForAssetbundle(Path.Combine(_readPath, "assets"), callback);
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
        private void LoadAllAssetPathForAssetbundle(string assetsPath, Action callback)
        {
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
			}
			
			if (_assetsPathMapAssetbundleName.Count == 0)
            {
                Debug.LogWarning($"Assetbundle no resource!");
            }

            //路径准备好了
            callback?.Invoke();
        }

        //加载asstbundle
        private void LoadAssetBundle(string assetBundleName, Action<AssetBundle> callback)
        {
            //加载Assetbundle
            AssetBundle assetBundle;
            if (!_liveAssetBundle.TryGetValue(assetBundleName, out assetBundle))
            {
                string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                //先加载引用的assetbundle 
                LoadDependenciesAssetBundle(assetBundleName);
                //加载assetbundle
                assetBundle = LoadAssetBundleFromPathSync(assetBundlePath);
                //存储assetbundle
                _liveAssetBundle[assetBundleName] = assetBundle;
                //assetbundle 引用计数
                _assetBundleReferenceCount[assetBundle] = 0;
            }
            callback.Invoke(assetBundle);
        }

        //加载assetbundle
        private AssetBundle LoadAssetBundle(string assetBundleName)
        {
            //加载Assetbundle
            AssetBundle assetBundle;
            if (!_liveAssetBundle.TryGetValue(assetBundleName, out assetBundle))
            {
                string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                //先加载引用的assetbundle 
                LoadDependenciesAssetBundleSync(assetBundleName);
                //加载assetbundle
                assetBundle = LoadAssetBundleFromPathSync(assetBundlePath);
                //存储assetbundle
                _liveAssetBundle[assetBundleName] = assetBundle;
                //assetbundle 引用计数
                _assetBundleReferenceCount[assetBundle] = 0;
            }

            return assetBundle;
        }

        //同步加载AssetBundle
        private AssetBundle LoadAssetBundleFromPathSync(string path)
        {
            if (!File.Exists(path))
                throw new GameException("Assetbundle not found :" + path);
            float loadTime = Time.realtimeSinceStartup;

            AssetBundle assetbundle;
            if (_isEncrypt)
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
            loadTime = Time.realtimeSinceStartup - loadTime;
            Debug.Log($"LoadAssetBundleFromPathSync time spent: {path} {loadTime}");
            return assetbundle;
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
                AssetBundle assetBundle = LoadAssetBundleFromPathSync(dependenciesBundlePath);
                //存储资源名称
                _liveAssetBundle[item] = assetBundle;
            }
        }

        //加载引用的assetbundle --引用的assetbundle不卸载
        private void LoadDependenciesAssetBundleSync(string assetBundleName)
        {
            //加载相关依赖 依赖暂时不异步加载了
            string[] dependencies = _mainfest.GetAllDependencies(assetBundleName);
            foreach (var item in dependencies)
            {
                if (_liveAssetBundle.ContainsKey(item))
                    continue;

                string dependenciesBundlePath = Path.Combine(_readPath, item);
                AssetBundle assetBundle = LoadAssetBundleFromPathSync(dependenciesBundlePath);
                //存储资源名称
                _liveAssetBundle[item] = assetBundle;
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

        //资源预加载
        private IEnumerator ResourcePreload(List<AssetHashInfo> preloadAsset, Action<float> progressCallback)
        {
         //   Application.backgroundLoadingPriority = ThreadPriority.High;
            float loadTime = Time.realtimeSinceStartup;
            _preloadAssets.Clear();
            float progressCount = 0;
            foreach (var item in preloadAsset)
			{
                yield return null;
                //  var ab = LoadAssetBundle(item.Name);
                AssetBundle ab = null;
                string abPath = Path.Combine(_readPath, item.Name);
                using (var stream = new EncryptFileStream(abPath, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
                {
                    var ablfsa = AssetBundle.LoadFromStreamAsync(stream);
                    while (!ablfsa.isDone)
                    {
                        progressCallback?.Invoke(GetPreloadProgress(0, ablfsa.progress, progressCount, preloadAsset.Count));
                        yield return null;
                    }
                    yield return null;
                    ab = ablfsa.assetBundle;
                }
                //存储assetbundle
                _liveAssetBundle[item.Name] = ab;
                //assetbundle 引用计数
                _assetBundleReferenceCount[ab] = 0;

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
                        throw new GameException($"The corresponding resource path was not found! {asset.name} {assetName}");
                    }
                    if (!_preloadAssets.ContainsKey(assetPath))
                    {
                        _preloadAssets.Add(assetPath, asset);
                    }
                    else
                    {
                        Debug.LogError($"Preload redundant data. {assetPath}");
                    }
                }
                //UnloadAssetBunlde(item.Name);
                progressCount++;
            }
            loadTime = Time.realtimeSinceStartup- loadTime;
            Debug.Log($"Preload spent time: {loadTime}");
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

    }
}
