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
namespace  GameFramework.Taurus
{
	
	//平台资源信息
	[System.Serializable]
	public class PlatformVersionInfo
	{
		public int Version;
		public List<string> Platforms=new List<string>();
	}

	//资源版本信息
	[System.Serializable]
	public class AssetBundleVersionInfo
	{
		public int Version=0;
		public bool IsEncrypt=false;
        public string ManifestAssetBundle;
		public List<AssetHashInfo> AssetHashInfos=new List<AssetHashInfo>();
	}
	
	//资源hash值
	[System.Serializable]
	public class AssetHashInfo
	{
		public string Name;
		public string Hash;

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