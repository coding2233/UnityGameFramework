﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #资源管理类 资源加载&内置对象池# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 17点11分# </time>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using System.Threading.Tasks;
using System;
using System.IO;
using UnityEngine.U2D;

namespace Wanderer.GameFramework
{
    public sealed class ResourceManager : GameFrameworkModule, IUpdate
    {
        #region 属性

        //事件触发类
        private EventManager _event;

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
        public ResourceUpdateType ResUpdateType = ResourceUpdateType.Local;
        /// <summary>
        /// 资源本地路径 类型
        /// </summary>
        public PathType LocalPathType = PathType.ReadOnly;
        /// <summary>
        /// 默认是否需要从StreamingAsset里面拷贝到可读文件夹中
        /// </summary>
        public bool DefaultInStreamingAsset = true;
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

        /// <summary>
        /// 正式服的更新路径
        /// </summary>
        public string ResOfficialUpdatePath = "";

        /// <summary>
        /// 测试服的更新路径
        /// </summary>
        public string ResTestUpdatePath = "";

        //远程更新的路径
        private string _remoteUpdatePath = null;

        /// <summary>
        /// 资源
        /// </summary>
        public IAssetsHelper Asset { get; private set; }
        /// <summary>
        /// 资源的版本信息
        /// </summary>
        /// <value></value>
        public IResourceVersion Version { get; private set; }

        #endregion

        #region 构造函数
        public ResourceManager()
        {
            ////自动监听图集请求，并异步加载图集资源
            //SpriteAtlasManager.atlasRequested += (tag, callback) =>
            //{
            //    string assetPath = Resource.Asset.AllAssetPaths.Find(x => x.EndsWith($"{tag.ToLower()}.spriteatlas"));
            //    if (!string.IsNullOrEmpty(assetPath))
            //    {
            //        Resource.Asset.LoadAsset<SpriteAtlas>(assetPath, callback);
            //    }
            //    else
            //    {
            //        throw new GameException($"The path to the SpriteAtlas file could not be found. {tag}");
            //    }
            //};

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

        //初始化
        public override void OnInit()
        {
            base.OnInit();
#if ADDRESSABLES_SUPPORT
            Asset = new AddressablesAssetsHelper();
            Version = new AddressableVersion();
#else
          var config = GameFrameworkMode.GetModule<ConfigManager>();
            //参数设置
            ResUpdateType = (ResourceUpdateType)(int)config["ResourceUpdateType"];
            ResOfficialUpdatePath = (string)config["ResOfficialUpdatePath"];
            ResTestUpdatePath = (string)config["ResTestUpdatePath"]; ;
            LocalPathType = (PathType)(int)config["PathType"];
            DefaultInStreamingAsset = (bool)config["DefaultInStreamingAsset"];
           
            //设置更新路径
#if TEST
            _remoteUpdatePath = ResTestUpdatePath;
#else
            _remoteUpdatePath = ResOfficialUpdatePath;
#endif
            _remoteUpdatePath = Path.Combine(_remoteUpdatePath, Utility.GetRuntimePlatformName());
            //资源版本
            Version = new ResourceVersion(_remoteUpdatePath,LocalPath);

            //选择更新 | 读取本地 | 编辑器
            switch (ResUpdateType)
            {
                case ResourceUpdateType.None:
                    break;
                case ResourceUpdateType.Update:
                    //BundleResourceHelper
                    GameObject bundleBehaviourHelper = new GameObject("AssetBundleBehaviourHelper");
                    Asset = bundleBehaviourHelper.AddComponent<BundleAssetsHelper>();
                    bundleBehaviourHelper.hideFlags = HideFlags.HideAndDontSave;
                    GameObject.DontDestroyOnLoad(bundleBehaviourHelper);
                    break;
                case ResourceUpdateType.Local:
                    break;
                case ResourceUpdateType.Editor:
#if UNITY_EDITOR
                    Asset = new EditorAssetsHelper();
#else
        		//如果在非编辑器模式下选择了Editor，使用本地文件
                throw new GameException("Please select the correct resource type, not ResourceUpdateType.Editor!");
#endif
                    break;
            }

			
#endif


        }

        #region 外部接口
        /// <summary>
        /// 设置资源帮助类
        /// </summary>
        /// <param name="resourceHelper"></param>
        public void SetResourceHelper(IAssetsHelper resourceHelper)
        {
            Asset?.Clear();
            Asset = resourceHelper;
        }

        /// <summary>
        /// 图集请求
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="callback"></param>
        public void OnAtlasRequested(string operation, Action<string,Action<SpriteAtlas>> callback)
        {
            if (string.IsNullOrEmpty(operation))
            {
                return;
            }
            if (operation.Equals("+"))
            {
                SpriteAtlasManager.atlasRequested += callback;
            }
            else if(operation.Equals("-"))
            {
                SpriteAtlasManager.atlasRequested -= callback;
            }
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
            _gameObjectPoolHelper.AddPrefab(assetBundleName, assetName, prefabInfo);
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
            //资源清理
            Asset?.Clear();
            if (_gameObjectPoolHelper != null)
                _gameObjectPoolHelper.DestroyAll();
        }
#endregion


#region 内部函数
        //异步加载资源的回调
        private void LoadAssetAsyncCallback(string assetName, UnityEngine.Object asset)
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
        /// 编辑器
        /// </summary>
        Editor,
        /// <summary>
        /// 更新
        /// </summary>
        Update,
        /// <summary>
        /// 本地
        /// </summary>
        Local,
        /// <summary>
        /// 没有资源加载模式
        /// </summary>
        None,
    }

#endregion

}