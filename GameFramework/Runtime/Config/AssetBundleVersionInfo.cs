//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Hu Tao. All rights reserved.
// </copyright>
// <describe> #版本信息# </describe>
// <email> 987947865@qq.com </email>
// <time> #2018年6月28日 11点16分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{

    //平台资源信息
    [System.Serializable]
    public class PlatformVersionInfo
    {
        public int Version;
        public List<string> Platforms = new List<string>();
    }

    //资源版本信息
    [System.Serializable]
    public class AssetBundleVersionInfo
    {
        private int _hashCode=0;
        /// <summary>
        /// 资源版本好
        /// </summary>
        public int Version = 0;
        //  public bool IsEncrypt = false;
        /// <summary>
        /// 当前的AppVersion
        /// </summary>
        public string AppVersion = "";
        /// <summary>
        /// 支持的老版本的
        /// </summary>
        public List<string> SupportOldAppVersions = new List<string>();
        /// <summary>
        /// 以前的资源路径链接
        /// </summary>
        public string OldResourceUrl = "";
        /// <summary>
        /// 强制更新
        /// </summary>
        public bool ForceUpdate = true;
        /// <summary>
        /// ManifestAssetBundle
        /// </summary>
        public string ManifestAssetBundle;
        /// <summary>
        /// 资源信息
        /// </summary>
        public List<AssetHashInfo> AssetHashInfos = new List<AssetHashInfo>();

		public override bool Equals(object obj)
		{
            AssetBundleVersionInfo other = obj as AssetBundleVersionInfo;
            if (Version != other.Version|| other.GetHashCode()!=GetHashCode())
            {
                return false;
            }
			return true;
		}

		public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                _hashCode= JsonUtility.ToJson(this).GetHashCode();
            }
            return _hashCode;
        }

    }

    //资源hash值
    [System.Serializable]
    public class AssetHashInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// md5值
        /// </summary>
        public string Hash;
        /// <summary>
        /// KB
        /// </summary>
        public int Size;
        /// <summary>
        /// 强制更新
        /// </summary>
        public bool ForceUpdate = true;

        public override bool Equals(object obj)
        {
            AssetHashInfo other = obj as AssetHashInfo;
            if (other.Name.Equals(Name) && other.Hash.Equals(Hash))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (Name + Hash).GetHashCode();
        }

    }
}