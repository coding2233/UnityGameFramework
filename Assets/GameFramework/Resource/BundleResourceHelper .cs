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
		//所有资源AssetBundle引用
		private readonly Dictionary<string, AssetBundle> _allAssets = new Dictionary<string, AssetBundle>();
		//资源引用
		private AssetBundleManifest _mainfest;

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


			string rootABPath = _readPath + "/" + rootAssetBundle;
			string directionPath = _readPath;

			int index = rootAssetBundle.LastIndexOf("/");
			if (index > 0 && index < (rootAssetBundle.Length - 1))
				directionPath += "/" + rootAssetBundle.Substring(0, index);
            //加载mainfest文件
            if (!isEncrypt)
                LoadPlatformMainfest(rootABPath, directionPath);
		    else
		    {
		        EnciphererKey keyAsset = Resources.Load("Key") as EnciphererKey;
		        LoadPlatformMainfest(rootABPath, directionPath, keyAsset);
            }
		}

		/// <summary>
		/// 加载资源
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public T LoadAsset<T>(string assetName) where T : Object
		{
			assetName = assetName.ToLower();

			AssetBundle assetBundle;
			if (_allAssets.TryGetValue(assetName, out assetBundle))
			{
				////加载相关依赖
				//string[] dependencies = _mainfest.GetAllDependencies(assetName);
				//foreach (var item in dependencies)
				//{
				//	AssetBundle.LoadFromFile(_readPath + "/" + item);
				//}
				T asset = assetBundle.LoadAsset<T>(assetName);
				return asset;
			}
			return null;
		}

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
	    public void LoadAssetAsync<T>(string assetName, Action<string, UnityEngine.Object> asyncCallback) where T : Object
        {
	        assetName = assetName.ToLower();
	        AssetBundle assetBundle;
	        if (_allAssets.TryGetValue(assetName, out assetBundle))
	        {
	            ////加载相关依赖
	            //string[] dependencies = _mainfest.GetAllDependencies(assetName);
	            //foreach (var item in dependencies)
	            //{
	            //    AssetBundle.LoadFromFile(_readPath + "/" + item);
	            //}
	            AssetBundleRequest requetAsset = assetBundle.LoadAssetAsync<T>(assetName);
		        requetAsset.completed += (asyncOperation) => { asyncCallback.Invoke(assetName, requetAsset.asset); };
				return;
	        }
	        asyncCallback.Invoke(assetName, null);
			return;
        }

		/// <summary>
		/// 卸载掉资源
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="allAssets"></param>
		public void UnloadAsset(string assetName, bool allAssets)
		{
			AssetBundle assetBundle;
			if (_allAssets.TryGetValue(assetName, out assetBundle))
			{
				if (!allAssets)
					_allAssets.Remove(assetName);
				else
				{
					foreach (var item in assetBundle.GetAllAssetNames())
						if(_allAssets.ContainsKey(item))
							_allAssets.Remove(item);
				}
				//卸载资源
				assetBundle.Unload(allAssets);
			}
		}

		/// <summary>
		/// 异步加载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
		{
			AssetBundle assetBundle;
			if (_allAssets.TryGetValue(sceneName, out assetBundle))
			{
				////加载相关依赖
				//string[] dependencies = _mainfest.GetAllDependencies(sceneName);
				//foreach (var item in dependencies)
				//{
				//	AssetBundle.LoadFromFile(_readPath + "/" + item);
				//}
				return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
			}
			return null;
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

			_mainfest = null;
		}

		#endregion

		#region 内部函数
		/// <summary>
		/// 加载mainfest -- LoadFromFile
		/// </summary>
		private void LoadPlatformMainfest(string rootBundlePath, string folderPath)
		{
			   //string assetBundlePath = _readPath + "/AssetBundles";
			AssetBundle mainfestAssetBundle = AssetBundle.LoadFromFile(rootBundlePath);
			_mainfest = mainfestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//
			string[] assetBundleNames = _mainfest.GetAllAssetBundles();
			foreach (var item in assetBundleNames)
			{
				AssetBundle assetBundle = AssetBundle.LoadFromFile(folderPath + "/" + item);
				string[] assetNames = assetBundle.GetAllAssetNames();
				if (assetBundle.isStreamedSceneAssetBundle)
					assetNames = assetBundle.GetAllScenePaths();
				foreach (var name in assetNames)
				{
					if (!_allAssets.ContainsKey(name))
						_allAssets.Add(name, assetBundle);
				}
			}
			mainfestAssetBundle.Unload(false);
		}

	    private void LoadPlatformMainfest(string rootBundlePath, string folerPath,EnciphererKey keyAsset)
        {
			//从内存中加载&解密
	        byte[] datas = Encipherer.AESDecrypt( File.ReadAllBytes(rootBundlePath),keyAsset);
			AssetBundle mainfestAssetBundle = AssetBundle.LoadFromMemory(datas);
	        _mainfest = mainfestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//
	        string[] assetBundleNames = _mainfest.GetAllAssetBundles();
	        foreach (var item in assetBundleNames)
	        {
		        datas = Encipherer.AESDecrypt(File.ReadAllBytes(folerPath + "/" + item), keyAsset);
				AssetBundle assetBundle = AssetBundle.LoadFromMemory(datas);
		        string[] assetNames = assetBundle.GetAllAssetNames();
		        if (assetBundle.isStreamedSceneAssetBundle)
			        assetNames = assetBundle.GetAllScenePaths();
		        foreach (var name in assetNames)
		        {
			        if (!_allAssets.ContainsKey(name))
				        _allAssets.Add(name, assetBundle);
		        }
	        }
	        mainfestAssetBundle.Unload(false);
		}

	    #endregion

    }
}
