//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #AssetBundle资源管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月24日 17点00分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameFramework.Taurus
{
	public sealed class BundleResourceHelper : IResourceHelper
	{
		//路径
		private string _readPath;
	    //资源引用
	    private AssetBundleManifest _mainfest;
        //解密密钥
	    private EnciphererKey _enciphererkeyAsset;
        //所有资源AssetBundle引用
        private readonly Dictionary<string, AssetBundle> _allAssets = new Dictionary<string, AssetBundle>();
        //所有AssetBundle包含的资源
	    private readonly Dictionary<string, KeyValuePair<AssetBundle, string[]>> _allAssetBundles =
	        new Dictionary<string, KeyValuePair<AssetBundle, string[]>>();
        
		/// <summary>
		/// 设置资源的路径,默认是为只读路径:Application.streamingAssetsPath;
		/// </summary>
		/// <param name="path"></param>
		public void SetResourcePath(PathType pathType, string rootAssetBundle = "AssetBundles/AssetBundles",bool isEncrypt=false)
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
					_readPath = Application.streamingAssetsPath;
					break;
			}


			string rootAbPath = Path.Combine(_readPath,rootAssetBundle);
		    _readPath = Path.GetDirectoryName(rootAbPath);
            //加载mainfest文件
		    if (isEncrypt)
		    {
		        _enciphererkeyAsset = Resources.Load("Key") as EnciphererKey;
		        if (_enciphererkeyAsset == null)
		            throw new GamekException("Resource EnciphererKey not Found");
		    }

		    LoadPlatformMainfest(rootAbPath);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetBundleName, string assetName) where T : Object
		{
            //转小写
			assetName = assetName.ToLower();

            //加载Assetbundle
			AssetBundle assetBundle;
			if (!_allAssets.TryGetValue(assetName, out assetBundle))
			{
			    string assetBundlePath = Path.Combine(_readPath, assetBundleName);
			    if (!File.Exists(assetBundlePath))
			        throw new GamekException("AssetBundle is Null");
                //加载assetbundle
			    assetBundle = LoadAssetBundle(assetBundlePath);
                //存储资源名称
                string[] assetNames = assetBundle.GetAllAssetNames();
                if (assetBundle.isStreamedSceneAssetBundle)
                    assetNames = assetBundle.GetAllScenePaths();
                foreach (var name in assetNames)
                {
                    if (!_allAssets.ContainsKey(name))
                        _allAssets.Add(name, assetBundle);
                }
                //存储assetbundle
			    _allAssetBundles[assetName] = new KeyValuePair<AssetBundle, string[]>(assetBundle, assetNames);
			}

            //加载依赖项
		    LoadDependenciesAssetBundel(assetBundleName);
            //加载资源
            T asset = assetBundle.LoadAsset<T>(assetName);
            
            return asset;
		}

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
	    public void LoadAssetAsync<T>(string assetBundleName,string assetName, Action<string, UnityEngine.Object> asyncCallback) where T : Object
        {
	        assetName = assetName.ToLower();

            AssetBundle assetBundle;

            if (!_allAssets.TryGetValue(assetName, out assetBundle))
            {
                string assetBundlePath = Path.Combine(_readPath, assetBundleName);
                try
                {
                    //异步加载assetbundle
                    AssetBundleCreateRequest createRequest = LoadAssetBundleAsync(assetBundlePath);
                    createRequest.completed += (operation) =>
                    {
                        assetBundle = createRequest.assetBundle;

                        //存储资源名称
                        string[] assetNames = assetBundle.GetAllAssetNames();
                        if (assetBundle.isStreamedSceneAssetBundle)
                            assetNames = assetBundle.GetAllScenePaths();
                        foreach (var name in assetNames)
                        {
                            if (!_allAssets.ContainsKey(name))
                                _allAssets.Add(name, assetBundle);
                        }

                        //存储assetbundle
                        _allAssetBundles[assetName] = new KeyValuePair<AssetBundle, string[]>(assetBundle, assetNames);

                        //加载依赖项
                        LoadDependenciesAssetBundel(assetBundleName);

                        //assetbundle异步加载资源
                        AssetBundleRequest requetAsset = assetBundle.LoadAssetAsync<T>(assetName);
                        requetAsset.completed += (asyncOperation) =>
                        {
                            asyncCallback.Invoke(assetName, requetAsset.asset);
                        };

                    };
                }
                catch (GamekException ex)
                {
                    asyncCallback.Invoke(assetName, null);
                    Debug.LogError(ex.ToString());
                }
            }
            else
            {
                //加载依赖项
                LoadDependenciesAssetBundel(assetBundleName);

                //assetbundle异步加载资源
                AssetBundleRequest requetAsset = assetBundle.LoadAssetAsync<T>(assetName);
                requetAsset.completed += (asyncOperation) =>
                {
                    asyncCallback.Invoke(assetName, requetAsset.asset);
                };
            }
        }

		/// <summary>
		/// 卸载掉资源
		/// </summary>
		/// <param name="assetBundleName"></param>
		/// <param name="unload"></param>
		public void UnloadAsset(string assetBundleName, bool unload=false)
		{
            KeyValuePair<AssetBundle, string[]> assetBundles;
			if (_allAssetBundles.TryGetValue(assetBundleName, out assetBundles))
			{
			    if (!unload)
			        assetBundles.Key.Unload(false);
                else
			    {
			        foreach (var item in assetBundles.Value)
			        {
			            if (_allAssets.ContainsKey(item))
			                _allAssets.Remove(item);
			        }
			        assetBundles.Key.Unload(true);
			    }

			}
		}

		/// <summary>
		/// 异步加载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public AsyncOperation LoadSceneAsync(string assetBundleName, string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
		{
		    AsyncOperation asyncOperation = null;
		    try
		    {
		        string assetBundlePath = Path.Combine(_readPath, assetBundleName);
		        AssetBundleCreateRequest createRequest = LoadAssetBundleAsync(assetBundlePath);
		        createRequest.completed += (operation) =>
		        {
                    AssetBundle assetBundle = createRequest.assetBundle;
		            //加载依赖项
		            LoadDependenciesAssetBundel(assetBundleName);

                    asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
                    //场景加载完成卸载相关的引用
		            asyncOperation.completed += (operation02) =>
		            {
                        assetBundle.Unload(false);
		            };
		        };
                
            }
		    catch (GamekException ex)
		    {
		        Debug.LogError(ex.ToString());
		    }

		    return asyncOperation;
		}

		/// <summary>
		/// 卸载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public AsyncOperation UnloadSceneAsync(string sceneName)
		{
			return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
		}


		#region 事件回调
		/// <summary>
		/// 清理资源
		/// </summary>
		public void Clear()
		{
			foreach (var item in _allAssets.Values)
				if(item!=null)
					item.Unload(true);
			_allAssets.Clear();
		    _allAssetBundles.Clear();
            _mainfest = null;
		}

		#endregion

		#region 内部函数
		/// <summary>
		/// 加载mainfest -- LoadFromFile
		/// </summary>
		private void LoadPlatformMainfest(string rootBundlePath)
		{
		    try
		    {
		        AssetBundle mainfestAssetBundle = LoadAssetBundle(rootBundlePath);
		        _mainfest = mainfestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		        mainfestAssetBundle.Unload(false);
		    }
		    catch (GamekException ex)
		    {
		        Debug.LogError(ex.ToString());
		    }
		}


        //同步加载AssetBundle
	    private AssetBundle LoadAssetBundle(string path)
	    {
	        if (!File.Exists(path))
	            throw new Exception("assetbundle not found :" + path);

	        AssetBundle mainfestAssetBundle;
	        if (_enciphererkeyAsset != null)
	        {
	            byte[] datas = Encipherer.AESDecrypt(File.ReadAllBytes(path), _enciphererkeyAsset);
	            mainfestAssetBundle = AssetBundle.LoadFromMemory(datas);
	        }
	        else
	            mainfestAssetBundle = AssetBundle.LoadFromFile(path);
	       

            return mainfestAssetBundle;
	    }

        //异步加载AssetBundle
	    private AssetBundleCreateRequest LoadAssetBundleAsync(string path)
	    {
	        if (!File.Exists(path))
	            throw new Exception("assetbundle not found :" + path);
	        AssetBundleCreateRequest assetBundleCreateRequest;
	        if (_enciphererkeyAsset != null)
	        {
	            byte[] datas = Encipherer.AESDecrypt(File.ReadAllBytes(path), _enciphererkeyAsset);
	            assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(datas);
	        }
	        else
	            assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);

	        return assetBundleCreateRequest;
	    }

        //加载引用的assetbundle --引用的assetbundle不卸载
	    private void LoadDependenciesAssetBundel(string assetBundleName)
	    {
	        //加载相关依赖 依赖暂时不异步加载了
	        string[] dependencies = _mainfest.GetAllDependencies(assetBundleName);
	        foreach (var item in dependencies)
	        {
	            if (_allAssetBundles.ContainsKey(item))
	                continue;

	            string dependenciesBundlePath = Path.Combine(_readPath, item);
	            AssetBundle assetBundle= LoadAssetBundle(dependenciesBundlePath);

	            //存储资源名称
	            string[] assetNames = assetBundle.GetAllAssetNames();
	            if (assetBundle.isStreamedSceneAssetBundle)
	                assetNames = assetBundle.GetAllScenePaths();
	            foreach (var name in assetNames)
	            {
	                if (!_allAssets.ContainsKey(name))
	                    _allAssets.Add(name, assetBundle);
	            }
	            //存储assetbundle
	            _allAssetBundles[item] = new KeyValuePair<AssetBundle, string[]>(assetBundle, assetNames);
            }
        }

	    #endregion

    }
}
