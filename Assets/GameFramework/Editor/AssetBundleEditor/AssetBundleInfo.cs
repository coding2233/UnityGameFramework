//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #assetbundle信息# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点33分# </time>
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public class AssetBundleInfo
    {
        /// <summary>
        /// 当前的所有AB包
        /// </summary>
        public List<AssetBundleBuildInfo> AssetBundles
        {
            get;
            set;
        }

        public AssetBundleInfo()
        {
            AssetBundles = new List<AssetBundleBuildInfo>();
        }
    }
}
