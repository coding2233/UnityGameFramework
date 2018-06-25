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
        public static HotFixManager HotFix;

        /// <summary>
        /// 当前程序集
        /// </summary>
        public static System.Reflection.Assembly Assembly { get; private set; }

        #region 资源
        /// <summary>
        /// 资源加载方式 默认为编辑器加载
        /// </summary>
        public bool IsEditorMode = true;

        /// <summary>
        /// 资源加载路径 默认为只读路径
        /// </summary>
        public PathType DefaultPathType = PathType.ReadOnly;

        /// <summary>
        /// ab资源默认包名称
        /// </summary>
        public string AssetBundleName = "AssetBundles/AssetBundles";
        #endregion

        #endregion


        IEnumerator Start()
        {
            #region Module
            Event = GameFrameworkMode.GetModule<EventManager>();
            State = GameFrameworkMode.GetModule<GameStateManager>();
            Node = GameFrameworkMode.GetModule<NodeManager>();
            Resource = GameFrameworkMode.GetModule<ResourceManager>();
            UI = GameFrameworkMode.GetModule<UIManager>();
            HotFix = GameFrameworkMode.GetModule<HotFixManager>();
            #endregion

            #region resource
#if UNITY_EDITOR
            //设置资源的模式
            if (IsEditorMode)
                Resource.SetResourceHelper(new EditorResourceHelper());
            else
            {
                Resource.SetResourceHelper(new BundleResourceHelper());
                Resource.SetResourcePath(DefaultPathType, AssetBundleName);
            }
#else
//非编辑器模式下 -- 只支持AssetBundle资源加载
       IsEditorMode = false;
       Resource.SetResourceHelper(new BundleResourceHelper());
	   Resource.SetResourcePath(DefaultPathType, AssetBundleName);
#endif
            #endregion

            #region state
            //开启整个项目的流程
            Assembly = typeof(GameMode).Assembly;
            State.CreateContext(Assembly);
            yield return new WaitForEndOfFrame();
            State.SetStateStart();
            #endregion
        }

    }
}
