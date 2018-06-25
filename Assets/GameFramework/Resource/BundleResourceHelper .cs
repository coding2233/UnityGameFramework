//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #AssetBundle资源管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月24日 17点00分# </time>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		public void SetResourcePath(PathType pathType, string rootAssetBundle = "AssetBundles/AssetBundles")
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
			LoadPlatformMainfest(rootABPath, directionPath);
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
			//if (_allObjects.ContainsKey(assetName))
			//	return (T)_allObjects[assetName];

			AssetBundle assetBundle;
			if (_allAssets.TryGetValue(assetName, out assetBundle))
			{
				//加载相关依赖
				string[] dependencies = _mainfest.GetAllDependencies(assetName);
				foreach (var item in dependencies)
				{
					AssetBundle.LoadFromFile(_readPath + "/" + item);
				}
				T asset = assetBundle.LoadAsset<T>(assetName);
				//	_allObjects.Add(assetName, asset);
				return asset;
			}
			return null;
		}

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
	    public AssetBundleRequest LoadAssetAsync<T>(string assetName) where T : Object
        {
	        assetName = assetName.ToLower();
	        AssetBundle assetBundle;
	        if (_allAssets.TryGetValue(assetName, out assetBundle))
	        {
	            //加载相关依赖
	            string[] dependencies = _mainfest.GetAllDependencies(assetName);
	            foreach (var item in dependencies)
	            {
	                AssetBundle.LoadFromFile(_readPath + "/" + item);
	            }
	            AssetBundleRequest requetAsset = assetBundle.LoadAssetAsync<T>(assetName);
	            //	_allObjects.Add(assetName, asset);
	            return requetAsset;
	        }
	        return null;
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
				//加载相关依赖
				string[] dependencies = _mainfest.GetAllDependencies(sceneName);
				foreach (var item in dependencies)
				{
					AssetBundle.LoadFromFile(_readPath + "/" + item);
				}
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
				item.Unload(true);
			_allAssets.Clear();

			_mainfest = null;
		}

		#endregion
        
		#region 内部函数
		/// <summary>
		/// 加载mainfest
		/// </summary>
		private void LoadPlatformMainfest(string rootBundleaPath, string folderPath)
		{
			//string assetBundlePath = _readPath + "/AssetBundles";
			AssetBundle mainfestAssetBundle = AssetBundle.LoadFromFile(rootBundleaPath);
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
		#endregion

	}
}
