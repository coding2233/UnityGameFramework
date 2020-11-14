//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #游戏的管理类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月25日 12点06分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public partial class GameMode : MonoBehaviour
    {
        #region 属性
        public static EventManager Event;
        //  public static GameStateManager GameState;
        public static FSManager FSM;
        public static DataTableManager DataTable;
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
        public static DebuggerManager Debugger;
        public static ConfigManager Config;

        public static GameMode Self;

        /// <summary>
        /// 当前程序集
        /// </summary>
        public static System.Reflection.Assembly Assembly { get; private set; }

        #region 资源
        /// <summary>
        /// 是否开启调试器
        /// </summary>
        //public bool DebugEnable = true;

        /// <summary>
        /// 配置文件
        /// </summary>
        public TextAsset ConfigAsset;

        private JsonData _configJsonData;
        public JsonData ConfigJsonData
        {
            get
            {
                if (ConfigAsset == null || string.IsNullOrEmpty(ConfigAsset.text))
                {
                    _configJsonData = null;
                }
                else
                {
                    if (_configJsonData == null)
                    {
                        _configJsonData = JsonMapper.ToObject(ConfigAsset.text);
                    }
                }
                return _configJsonData;
            }
            set
            {
                _configJsonData = value;
            }

        }
        #endregion

        #endregion

        IEnumerator Start()
        {
            GameMode.Self = this;
            //默认不销毁
            DontDestroyOnLoad(gameObject);

            #region Module
            //config 要特殊处理
            Config = GameFrameworkMode.GetModule<ConfigManager>();
            Config.SetData(ConfigAsset.text);

            Event = GameFrameworkMode.GetModule<EventManager>();
            // GameState = GameFrameworkMode.GetModule<GameStateManager>();
            FSM = GameFrameworkMode.GetModule<FSManager>();
            DataTable = GameFrameworkMode.GetModule<DataTableManager>();
            Node = GameFrameworkMode.GetModule<NodeManager>();
            Resource = GameFrameworkMode.GetModule<ResourceManager>();
            UI = GameFrameworkMode.GetModule<UIManager>();
            WebRequest = GameFrameworkMode.GetModule<WebRequestManager>();
            Audio = GameFrameworkMode.GetModule<AudioManager>();
            Localization = GameFrameworkMode.GetModule<LocalizationManager>();
            Setting = GameFrameworkMode.GetModule<SettingManager>();
            System = GameFrameworkMode.GetModule<SystemManager>();
            Network = GameFrameworkMode.GetModule<NetworkManager>();
            Pool = GameFrameworkMode.GetModule<PoolManager>();
            Debugger = GameFrameworkMode.GetModule<DebuggerManager>();
            #endregion
            //设置是否启动
            Debugger.SetDebuggerEnable((bool)ConfigJsonData["DebugEnable"]);

            #region resource
            //参数设置
            Resource.ResUpdateType = (ResourceUpdateType)(int)ConfigJsonData["ResourceUpdateType"];
            Resource.ResOfficialUpdatePath = (string)ConfigJsonData["ResOfficialUpdatePath"];
            Resource.ResTestUpdatePath = (string)ConfigJsonData["ResTestUpdatePath"]; ;
            Resource.LocalPathType = (PathType)(int)ConfigJsonData["PathType"];
            Resource.DefaultInStreamingAsset = (bool)ConfigJsonData["DefaultInStreamingAsset"];

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
            // Audio.SetDefaultAudioSource(audioPlayer.AddComponent<AudioSource>(), audioPlayer.AddComponent<AudioSource>(),
            //     audioPlayer.AddComponent<AudioSource>());
            #endregion

            #region WebRequest
            //设置帮助类
            GameObject webDownloadHelper = new GameObject("IWebDownloadMonoHelper");
            webDownloadHelper.transform.SetParent(transform);
            WebRequest.SetWebDownloadHelper(webDownloadHelper.AddComponent<WebDownloadMonoHelper>());
            #endregion

            #region state
            //开启整个项目的流程
            Assembly = typeof(GameMode).Assembly;
            FSM.AddFSM<GameStateContext>();
          //  yield return new WaitForEndOfFrame();
            GameFrameworkMode.Init();
            FSM.GetFSM<GameStateContext>().OnBegin();
            #endregion

            yield return new WaitForEndOfFrame();

        }

        private void Update()
        {
            GameFrameworkMode.Update();
        }

        private void FixedUpdate()
        {
            GameFrameworkMode.FixedUpdate();
        }

        private void OnGUI()
        {
            GameFrameworkMode.ImGui();
        }

        private void OnDestroy()
        {
            GameFrameworkMode.ShutDown();
        }

    }
}
