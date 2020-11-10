﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #场景事件加载类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月25日 16点43分# </time>
//-----------------------------------------------------------------------

namespace Wanderer.GameFramework
{
    /// <summary>
    /// 场景加载中事件
    /// </summary>
    public class SceneLoadingEventArgs : GameEventArgs<SceneLoadingEventArgs>
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName;
        /// <summary>
        /// 场景加载进度
        /// </summary>
        public float Progress;
    }

    /// <summary>
    /// 场景加载完成事件
    /// </summary>
    public class SceneLoadedEventArgs : GameEventArgs<SceneLoadedEventArgs>
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName;
    }

}
