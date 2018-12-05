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

[Serializable]
public class AssetPlatformVersionInfo
{
    public int Version;
    public List<string> Platforms;
}

[Serializable]
public class AssetBundleVersionInfo
{
    public int Version;
    public bool IsEncrypt;
    public string ManifestAssetBundle;
    public List<ResourcesInfo> Resources;
}

[Serializable]
public class ResourcesInfo
{
    public string Name;
    public string MD5;

    public override bool Equals(object obj)
    {
        ResourcesInfo other = obj as ResourcesInfo;
        if (other.Name.Equals(Name) && other.MD5.Equals(MD5))
            return true;

        return false;
    }

    public override int GetHashCode()
    {
        return (Name + MD5).GetHashCode();
    }

}
