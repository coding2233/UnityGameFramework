//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #assetbundle打包信息# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点32分# </time>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public class AssetBundleBuildInfo
    {
        /// <summary>
        /// AB包的名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// AB包中的所有资源
        /// </summary>
        public List<AssetInfo> Assets
        {
            get;
            set;
        }

        public AssetBundleBuildInfo(string name)
        {
            Name = name;
            Assets = new List<AssetInfo>();
        }
    }
}
