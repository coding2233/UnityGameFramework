//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #资源管理类 资源加载&内置对象池# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 17点11分# </time>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

namespace GameFramework.Taurus
{
    public sealed class ResourceManager:GameFrameworkModule,IUpdate
    {
        #region 属性

        //事件触发类
        private EventManager _event;
		//资源管理器 帮助类
		private IResourceHelper _resourceHelper;
		//GameObject对象池管理器
		private IGameObjectPoolHelper _gameObjectPoolHelper;

		//资源异步加载完毕
	    private ResourceLoadAsyncSuccessEventArgs _resLoadAsyncSuccessEventArgs;
		//资源异步加载失败
	    private ResourceLoadAsyncFailureEventArgs _resLoadAsyncFailureEventArgs;

		//场景加载中事件
		private SceneLoadingEventArgs _sceneLoadingEventArgs;
        //场景加载完毕事件
        private SceneLoadedEventArgs _sceneLoadedEventArgs;
        //场景异步加载
        private Dictionary<string, AsyncOperation> _sceneAsyncOperations;

		/// <summary>
		/// 资源更新类型
		/// </summary>
	    public ResourceUpdateType ResUpdateType= ResourceUpdateType.Local;
		/// <summary>
		/// 资源本地路径 类型
		/// </summary>
		public PathType LocalPathType= PathType.ReadOnly;
        /// <summary>
        /// 资源本地路径
        /// </summary>
        public string LocalPath
        {
            get
            {
                switch (LocalPathType)
                {
                    case PathType.DataPath:
                        return Application.dataPath;
                    case PathType.ReadOnly:
                        return Application.streamingAssetsPath;
                    case PathType.ReadWrite:
                        return Application.persistentDataPath;
                    case PathType.TemporaryCache:
                        return Application.temporaryCachePath;
                }
                return "";
            }
        }
		
		/// 资源更新的路径
		/// </summary>
		public string ResUpdatePath="";
		
		#endregion

		#region 构造函数
		public ResourceManager()
		{
            //获取事件管理器
		    _event = GameFrameworkMode.GetModule<EventManager>();
			//资源异步加载的事件
			_resLoadAsyncSuccessEventArgs = new ResourceLoadAsyncSuccessEventArgs();
			_resLoadAsyncFailureEventArgs = new ResourceLoadAsyncFailureEventArgs();
			//场景事件
			_sceneLoadingEventArgs = new SceneLoadingEventArgs();
		    _sceneLoadedEventArgs = new SceneLoadedEventArgs();
		    _sceneAsyncOperations = new Dictionary<string, AsyncOperation>();
		}
		#endregion

		#region 外部接口

		/// <summary>
		/// 设置资源帮助类
		/// </summary>
		/// <param name="resourceHelper"></param>
		public void SetResourceHelper(IResourceHelper resourceHelper)
		{
			_resourceHelper = resourceHelper;
		}

        /// <summary>
        /// 在设置BundleResourceHelper 需要调用此函数加载AssetBundle的Mainfest文件
        /// </summary>
        /// <param name="mainfestName"></param>
        public void SetMainfestAssetBundle(string mainfestName, bool isEncrypt = false)
        {
            _resourceHelper?.SetResourcePath(LocalPathType, mainfestName, isEncrypt);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetBundleName,string assetName) where T : UnityEngine.Object
		{
			return _resourceHelper?.LoadAsset<T>(assetBundleName,assetName);
		}

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        public void LoadAssetAsync<T>(string assetBundleName,string assetName) where T : UnityEngine.Object
	    {
		    _resourceHelper?.LoadAssetAsync<T>(assetBundleName,assetName, LoadAssetAsyncCallback);
	    }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public AsyncOperation LoadSceneAsync(string assetBundleName,string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
		{
			if (_resourceHelper == null)
				return null;
		    AsyncOperation asyncOperation= _resourceHelper.LoadSceneAsync(assetBundleName,sceneName, mode);
		    _sceneAsyncOperations.Add(sceneName, asyncOperation);
		    return asyncOperation;

		}

		/// <summary>
		/// 卸载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public void UnloadSceneAsync(string sceneName)
		{
		    if (_resourceHelper == null)
		        return;

		     _resourceHelper.UnloadSceneAsync(sceneName);
		    return;
		}

		/// <summary>
		/// 设置对象池管理器的
		/// </summary>
		/// <param name="helper"></param>
		public void SetGameObjectPoolHelper(IGameObjectPoolHelper helper)
		{
			_gameObjectPoolHelper = helper;
		}

        /// <summary>
        /// 加载预设信息
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="prefabInfo"></param>
        public void AddPrefab(string assetBundleName, string assetName, PoolPrefabInfo prefabInfo)
		{
			_gameObjectPoolHelper.AddPrefab(assetBundleName,assetName, prefabInfo);
		}

		/// <summary>
		/// 生成物体
		/// </summary>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public GameObject Spawn(string assetName)
		{
			return _gameObjectPoolHelper.Spawn(assetName);
		}

		/// <summary>
		/// 是否包含预设
		/// </summary>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public bool HasPrefab(string assetName)
		{
			return _gameObjectPoolHelper.HasPrefab(assetName);
		}

		/// <summary>
		/// 销毁物体
		/// </summary>
		/// <param name="go"></param>
		/// <param name="isDestroy"></param>
		public void Despawn(GameObject go, bool isDestroy = false)
		{
			_gameObjectPoolHelper.Despawn(go, isDestroy);
		}

		/// <summary>
		/// 关闭所有物体
		/// </summary>
		public void DespawnAll()
		{
			_gameObjectPoolHelper.DespawnAll();
		}

		/// <summary>
		/// 销毁所有物体
		/// </summary>
		public void DestroyAll()
		{
			_gameObjectPoolHelper.DestroyAll();
		}

		/// <summary>
		/// 关闭预设的所有物体
		/// </summary>
		/// <param name="assetName"></param>
		public void DespawnPrefab(string assetName)
		{
			_gameObjectPoolHelper.DespawnPrefab(assetName);
		}

		#endregion
        
		#region 重写函数

        public void OnUpdate()
        {
            if (_sceneAsyncOperations.Count > 0)
            {
                foreach (var item in _sceneAsyncOperations)
                {
                    //触发加载完毕事件
                    if (item.Value.isDone)
                    {
                        _sceneLoadedEventArgs.SceneName = item.Key;
                        _event.Trigger(this, _sceneLoadedEventArgs);
                        _sceneAsyncOperations.Remove(item.Key);
                        break;
                    }
                    //触发正在加载事件
                    else
                    {
                        _sceneLoadingEventArgs.SceneName = item.Key;
                        _sceneLoadingEventArgs.Progress = item.Value.progress;
                        _event.Trigger(this, _sceneLoadingEventArgs);
                    }
                }
            }
        }

        public override void OnClose()
		{
			if (_resourceHelper != null)
				_resourceHelper.Clear();

			if (_gameObjectPoolHelper != null)
				_gameObjectPoolHelper.DestroyAll();
		}
		#endregion


		#region 内部函数
		//异步加载资源的回调
	    private void LoadAssetAsyncCallback(string assetName,Object asset)
	    {
		    if (_event == null)
			    return;

		    if (asset)
		    {
			    _resLoadAsyncSuccessEventArgs.AssetName = assetName;
			    _resLoadAsyncSuccessEventArgs.Asset = asset;
			    _event.Trigger(this, _resLoadAsyncSuccessEventArgs);
		    }
		    else
		    {
			    _resLoadAsyncFailureEventArgs.AssetName = assetName;
			    _event.Trigger(this, _resLoadAsyncFailureEventArgs);
			}
	    }

	    #endregion

	}

	#region 类型枚举

	/// <summary>
	/// 路径类型
	/// </summary>
	public enum PathType
	{
		/// <summary>
		/// 只读路径
		/// </summary>
		ReadOnly,
		/// <summary>
		/// 读写路径
		/// </summary>
		ReadWrite,
		/// <summary>
		/// 应用程序根目录
		/// </summary>
		DataPath,
		/// <summary>
		/// 临时缓存
		/// </summary>
		TemporaryCache,
	}

	/// <summary>
	/// 资源类型
	/// </summary>
	public enum ResourceUpdateType
	{
		/// <summary>
		/// 更新
		/// </summary>
		Update,
		/// <summary>
		/// 本地
		/// </summary>
		Local,
		/// <summary>
		/// 编辑器
		/// </summary>
		Editor
	}

	#endregion

}