//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏的管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 12点06分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
    public class GameMode : MonoBehaviour
    {
		#region 属性
		public static EventManager Event;
        public static GameStateManager State;
        public static NodeManager Node;
        public static ResourceManager Resource;
        public static UIManager UI;
        public static WebRequestManager WebRequest;
		public static AudioManager Audio;
	    public static LocalizationManager Localization;
		public static SettingManager Setting;
        public static SystemManager System;
        public static NetworkManager Network;
		public static PoolManager Pool;

	    public static HotFixManager HotFix;

		/// <summary>
		/// 当前程序集
		/// </summary>
		public static System.Reflection.Assembly Assembly { get; private set; }

        #region 资源
        /// <summary>
        /// 资源加载方式 默认为编辑器加载
        /// </summary>
       // public bool IsEditorMode = true;
		
	    /// <summary>
	    /// 资源更新类型
	    /// </summary>
	    public ResourceUpdateType ResUpdateType = ResourceUpdateType.Editor;
	    /// <summary>
	    /// 资源本地路径
	    /// </summary>
	    public PathType LocalPathType = PathType.ReadOnly;
		/// <summary>
		/// 资源更新的路径
		/// </summary>
		public string ResUpdatePath = "";
        /// <summary>
        /// 是否开启调试器
        /// </summary>
        public bool DebugEnable = true;
        #endregion

        #endregion

        IEnumerator Start()
		{
			//默认不销毁
			DontDestroyOnLoad(gameObject);

            #region Module
            Event = GameFrameworkMode.GetModule<EventManager>();
            State = GameFrameworkMode.GetModule<GameStateManager>();
            Node = GameFrameworkMode.GetModule<NodeManager>();
            Resource = GameFrameworkMode.GetModule<ResourceManager>();
            UI = GameFrameworkMode.GetModule<UIManager>();
            WebRequest = GameFrameworkMode.GetModule<WebRequestManager>();
			Audio = GameFrameworkMode.GetModule<AudioManager>();
			Localization = GameFrameworkMode.GetModule<LocalizationManager>();
			Setting = GameFrameworkMode.GetModule<SettingManager>();
		    System= GameFrameworkMode.GetModule<SystemManager>();
		    Network= GameFrameworkMode.GetModule<NetworkManager>();
		    HotFix = GameFrameworkMode.GetModule<HotFixManager>();
			Pool = GameFrameworkMode.GetModule<PoolManager>();
			#endregion

			#region resource
			Resource.ResUpdateType = ResUpdateType;
	        Resource.ResUpdatePath = ResUpdatePath;
	        Resource.LocalPathType = LocalPathType;

			//添加对象池管理器
	        GameObject gameObjectPoolHelper = new GameObject("IGameObjectPoolHelper");
	        gameObjectPoolHelper.transform.SetParent(transform);
	        Resource.SetGameObjectPoolHelper(gameObjectPoolHelper.AddComponent<GameObjectPoolHelper>());
			#endregion

			#region auido
			//设置音频播放
			GameObject audioPlayer = new GameObject("AudioSourcePlayer");
			audioPlayer.transform.SetParent(transform);
			//添加AduioSource
			Audio.SetDefaultAudioSource(audioPlayer.AddComponent<AudioSource>(), audioPlayer.AddComponent<AudioSource>(),
				audioPlayer.AddComponent<AudioSource>());
			#endregion

			#region WebRequest
			//设置帮助类
			GameObject webRequestHelper =new GameObject("IWebRequestHelper");
	        webRequestHelper.transform.SetParent(transform);
	        GameObject webDownloadHelper =new GameObject("IWebDownloadMonoHelper");
	        webDownloadHelper.transform.SetParent(transform);
			WebRequest.SetWebRequestHelper(webRequestHelper.AddComponent<WebRquestMonoHelper>());
            WebRequest.SetWebDownloadHelper(webDownloadHelper.AddComponent<WebDownloadMonoHelper>());
            #endregion

            #region state
            //开启整个项目的流程
            Assembly = typeof(GameMode).Assembly;
            State.CreateContext(Assembly);
            yield return new WaitForEndOfFrame();
            State.SetStateStart();
            #endregion

            #region Setting
            GameObject debugHelper = new GameObject("DebugHelper");
            debugHelper.transform.SetParent(transform);
            Setting.SetDebugHelper(debugHelper.AddComponent<DebugHelper>());
            Setting.DebugEnable = DebugEnable;
            #endregion
        }
        
		private void Update()
		{
			GameFrameworkMode.Update();
		}

		private void FixedUpdate()
		{
			GameFrameworkMode.FixedUpdate();
		}

		private void OnDestroy()
		{
			GameFrameworkMode.ShutDown();
		}
	}
}
