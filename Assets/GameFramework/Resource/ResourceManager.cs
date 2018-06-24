//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #资源管理类 资源加载&内置对象池# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 17点11分# </time>
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

namespace GameFramework.Taurus
{
    public sealed class ResourceManager:GameFrameworkModule
    {
		#region 属性
		//资源管理器 帮助类
		private IResourceHelper _resourceHelper;

		//GameObject对象池管理器
		private IGameObjectPoolHelper _gameObjectPoolHelper;

		#endregion

		#region 构造函数
		public ResourceManager()
		{
			//添加对象池管理器
			_gameObjectPoolHelper = new GameObject("GameObject_Pool").AddComponent<GameObjectPoolHelper>();
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
		/// 设置资源的路径,默认是为只读路径:Application.streamingAssetsPath;
		/// </summary>
		/// <param name="path"></param>
		public void SetResourcePath(PathType pathType, string rootAssetBundle = "AssetBundles/AssetBundles")
		{
			if (_resourceHelper == null)
				return;

			_resourceHelper.SetResourcePath(pathType, rootAssetBundle);
		}

		/// <summary>
		/// 加载资源
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
		{
			if (_resourceHelper == null)
				return null;

			return _resourceHelper.LoadAsset<T>(assetName);
		}

		/// <summary>
		/// 异步加载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
		{
			if (_resourceHelper == null)
				return null;

			return _resourceHelper.LoadSceneAsync(sceneName, mode);
		}

		/// <summary>
		/// 卸载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public void UnloadScene(string sceneName)
		{
			UnitySceneManager.UnloadScene(sceneName);
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
		/// <param name="assetName"></param>
		/// <param name="prefabInfo"></param>
		public void AddPrefab(string assetName, PoolPrefabInfo prefabInfo)
		{
			_gameObjectPoolHelper.AddPrefab(assetName, prefabInfo);
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
		public override void OnClose()
		{
			if (_resourceHelper != null)
				_resourceHelper.Clear();

			if (_gameObjectPoolHelper != null)
				_gameObjectPoolHelper.DestroyAll();
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
	#endregion

}