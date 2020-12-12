//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #游戏的管理类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月25日 12点06分# </time>
//-----------------------------------------------------------------------

using System;
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
        

        #region 资源
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

        #region 回调
        /// <summary>
        /// 游戏进入后台时执行该方法 pause为true 切换回前台时pause为false
        /// 强制暂停时,先 OnApplicationPause
        /// </summary>
        public static Action<bool> OnAppPause;
        /// <summary>
        /// 游戏失去焦点也就是进入后台时 focus为false 切换回前台时 focus为true
        /// 重新“启动”手机时,OnApplicationFocus
        /// </summary>
        public static Action<bool> OnAppFocus;
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

            Setting = GameFrameworkMode.GetModule<SettingManager>();
            Event = GameFrameworkMode.GetModule<EventManager>();
            FSM = GameFrameworkMode.GetModule<FSManager>();
            DataTable = GameFrameworkMode.GetModule<DataTableManager>();
            Node = GameFrameworkMode.GetModule<NodeManager>();
            Resource = GameFrameworkMode.GetModule<ResourceManager>();
            UI = GameFrameworkMode.GetModule<UIManager>();
            WebRequest = GameFrameworkMode.GetModule<WebRequestManager>();
            Audio = GameFrameworkMode.GetModule<AudioManager>();
            Localization = GameFrameworkMode.GetModule<LocalizationManager>();
            System = GameFrameworkMode.GetModule<SystemManager>();
            Network = GameFrameworkMode.GetModule<NetworkManager>();
            Pool = GameFrameworkMode.GetModule<PoolManager>();
            Debugger = GameFrameworkMode.GetModule<DebuggerManager>();
            #endregion

            #region resource
            //添加对象池管理器
            GameObject gameObjectPoolHelper = new GameObject("IGameObjectPoolHelper");
            gameObjectPoolHelper.transform.SetParent(transform);
            Resource.SetGameObjectPoolHelper(gameObjectPoolHelper.AddComponent<GameObjectPoolHelper>());
            #endregion
   
            #region state
            //开启整个项目的流程
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

        private void OnApplicationPause(bool pause)
        {
            OnAppPause?.Invoke(pause);
        }

        private void OnApplicationFocus(bool focus)
        {
            OnAppFocus?.Invoke(focus);
        }

    }
}
