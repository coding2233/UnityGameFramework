﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #资源加载事件# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月8日 13点43分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public class ResourceEventArgs
    {
    }

    /// <summary>
    /// 资源异步加载成功的事件
    /// </summary>
    public class ResourceLoadAsyncSuccessEventArgs : GameEventArgs<ResourceLoadAsyncSuccessEventArgs>
    {
        /// <summary>
        /// 异步加载成功的物体名称
        /// </summary>
        public string AssetName;
        /// <summary>
        /// 异步加载的物体
        /// </summary>
        public UnityEngine.Object Asset;
    }

    /// <summary>
    /// 资源异步加载失败的事件
    /// </summary>
    public class ResourceLoadAsyncFailureEventArgs : GameEventArgs<ResourceLoadAsyncFailureEventArgs>
    {
        /// <summary>
        /// 异步加载物体名称
        /// </summary>
        public string AssetName;
    }

    /// <summary>
    /// 资源路径对应Assetbundle对应的事件
    /// </summary>
    public class ResourceAssetPathsMapReadyEventArgs : GameEventArgs<ResourceAssetPathsMapReadyEventArgs>
    {

    }
}
