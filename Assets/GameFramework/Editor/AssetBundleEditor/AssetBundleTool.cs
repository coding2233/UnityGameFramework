//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #assetbundle工具# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点33分# </time>
//-----------------------------------------------------------------------

using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GameFramework.Taurus
{
    public static class AssetBundleTool
    {
        private static readonly string[] _invalidFileFormats = new string[] { ".cs", ".js", ".shader", ".dll", ".db" };

        private static readonly string[] _invalidFolderName = new string[] { "Resources", "AssetBundleEditor", "Editor", "Gizmos", "StreamingAssets" };
        
        /// <summary>
        /// 读取资源文件夹下的所有子资源
        /// </summary>
        public static void ReadAssetsInChildren(AssetInfo asset, List<AssetInfo> validAssetList)
        {
            if (asset.AssetFileType != FileType.Folder)
            {
                return;
            }

            DirectoryInfo di = new DirectoryInfo(asset.AssetFullPath);
            FileSystemInfo[] fileinfo = di.GetFileSystemInfos();
            foreach (FileSystemInfo fi in fileinfo)
            {
                if (fi is DirectoryInfo)
                {
                    if (IsValidFolder(fi.Name))
                    {
                        AssetInfo ai = new AssetInfo(fi.FullName, fi.Name, false);
                        asset.ChildAssetInfo.Add(ai);

                        ReadAssetsInChildren(ai, validAssetList);
                    }
                }
                else
                {
                    if (fi.Extension != ".meta")
                    {
                        AssetInfo ai = new AssetInfo(fi.FullName, fi.Name, fi.Extension);
                        asset.ChildAssetInfo.Add(ai);

                        if (ai.AssetFileType == FileType.ValidFile)
                        {
                            validAssetList.Add(ai);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取AB包配置信息
        /// </summary>
        public static void ReadAssetBundleConfig(AssetBundleInfo abInfo, List<AssetInfo> validAssetList)
        {
            string[] builds = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < builds.Length; i++)
            {
                AssetBundleBuildInfo build = new AssetBundleBuildInfo(builds[i]);
                string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(builds[i]);
                for (int j = 0; j < assets.Length; j++)
                {
                    AssetInfo ai = validAssetList.GetAssetInfoByGUID(AssetDatabase.AssetPathToGUID(assets[j]));
                    if (ai != null)
                    {
                        build.AddAsset(ai);
                    }
                }
                abInfo.AssetBundles.Add(build);
            }
        }

        /// <summary>
        /// 是否存在指定的AB包名称
        /// </summary>
        public static bool IsExistName(this AssetBundleInfo abInfo, string name)
        {
            for (int i = 0; i < abInfo.AssetBundles.Count; i++)
            {
                if (abInfo.AssetBundles[i].Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 通过资源GUID获取有效的资源数据
        /// </summary>
        public static AssetInfo GetAssetInfoByGUID(this List<AssetInfo> validAssetList, string guid)
        {
            for (int i = 0; i < validAssetList.Count; i++)
            {
                if (validAssetList[i].GUID == guid)
                {
                    return validAssetList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 通过资源后缀名获取资源文件类型
        /// </summary>
        public static FileType GetFileTypeByExtension(string extension)
        {
            foreach (string format in _invalidFileFormats)
            {
                if (format == extension)
                {
                    return FileType.InValidFile;
                }
            }
            return FileType.ValidFile;
        }

        /// <summary>
        /// 是否是有效的文件夹
        /// </summary>
        public static bool IsValidFolder(string folderName)
        {
            foreach (string name in _invalidFolderName)
            {
                if (name == folderName)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 改变资源及其子资源的选中状态
        /// </summary>
        public static void ChangeCheckedInChildren(AssetInfo asset, bool isChecked)
        {
            asset.IsChecked = isChecked;
            if (asset.AssetFileType == FileType.Folder)
            {
                for (int i = 0; i < asset.ChildAssetInfo.Count; i++)
                {
                    ChangeCheckedInChildren(asset.ChildAssetInfo[i], isChecked);
                }
            }
            else if (asset.AssetFileType == FileType.InValidFile || asset.Bundled != "")
            {
                asset.IsChecked = false;
            }
        }

        /// <summary>
        /// 展开或关闭文件夹及其子文件夹
        /// </summary>
        public static void ExpandFolder(AssetInfo asset, bool expand)
        {
            if (asset.AssetFileType == FileType.Folder)
            {
                asset.IsExpanding = expand;
                for (int i = 0; i < asset.ChildAssetInfo.Count; i++)
                {
                    ExpandFolder(asset.ChildAssetInfo[i], expand);
                }
            }
        }

        /// <summary>
        /// 获取所有被选中的有效资源
        /// </summary>
        public static List<AssetInfo> GetCheckedAssets(this List<AssetInfo> validAssetList)
        {
            List<AssetInfo> currentAssets = new List<AssetInfo>();
            for (int i = 0; i < validAssetList.Count; i++)
            {
                if (validAssetList[i].IsChecked)
                {
                    currentAssets.Add(validAssetList[i]);
                }
            }
            return currentAssets;
        }

        /// <summary>
        /// 添加资源到AB包中
        /// </summary>
        public static void AddAsset(this AssetBundleBuildInfo build, AssetInfo asset)
        {
            asset.Bundled = build.Name;
            asset.IsChecked = false;
            build.Assets.Add(asset);

            AssetImporter import = AssetImporter.GetAtPath(asset.AssetPath);
            import.assetBundleName = build.Name;
        }

        /// <summary>
        /// 从AB包中删除资源
        /// </summary>
        public static void RemoveAsset(this AssetBundleBuildInfo build, AssetInfo asset)
        {
            asset.Bundled = "";
            if (build.Assets.Contains(asset))
            {
                build.Assets.Remove(asset);
            }

            AssetImporter import = AssetImporter.GetAtPath(asset.AssetPath);
            import.assetBundleName = "";
        }

        /// <summary>
        /// 清空AB包中的资源
        /// </summary>
        public static void ClearAsset(this AssetBundleBuildInfo build)
        {
            for (int i = 0; i < build.Assets.Count; i++)
            {
                build.Assets[i].Bundled = "";

                AssetImporter import = AssetImporter.GetAtPath(build.Assets[i].AssetPath);
                import.assetBundleName = "";
            }
            build.Assets.Clear();
        }

        /// <summary>
        /// 重命名AB包
        /// </summary>
        public static void RenameAssetBundle(this AssetBundleBuildInfo build, string name)
        {
            List<string> bundles = AssetDatabase.GetAllAssetBundleNames().ToList();
            if (bundles.Contains(build.Name))
            {
                AssetDatabase.RemoveAssetBundleName(build.Name, true);
            }

            build.Name = name;
            for (int i = 0; i < build.Assets.Count; i++)
            {
                build.Assets[i].Bundled = build.Name;

                AssetImporter import = AssetImporter.GetAtPath(build.Assets[i].AssetPath);
                import.assetBundleName = build.Name;
            }
        }

        /// <summary>
        /// 删除AB包
        /// </summary>
        public static void DeleteAssetBundle(this AssetBundleInfo abInfo, int index)
        {
            abInfo.AssetBundles[index].ClearAsset();
            AssetDatabase.RemoveAssetBundleName(abInfo.AssetBundles[index].Name, true);
            abInfo.AssetBundles.RemoveAt(index);
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        public static void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        /// <summary>
        /// 打包资源
        /// </summary>
        [MenuItem("Tools/Taurus/Build AssetBundles %#T")]
        public static void BuildAssetBundles()
        {
            string buildPath = EditorPrefs.GetString(Application.productName + "_BuildPath", "");
            if (!Directory.Exists(buildPath))
            {
                Debug.LogError("Please set build path！");
                return;
            }

            //打包资源
            Debug.Log("开始打包！" + System.DateTime.Now.ToString("HH:mm:ss:fff"));
            BuildAssetBundleOptions option = (BuildAssetBundleOptions)EditorPrefs.GetInt(Application.productName + "_ZipMode", 0);
            BuildTarget target = (BuildTarget)EditorPrefs.GetInt(Application.productName + "_BuildTarget", 5);
            BuildPipeline.BuildAssetBundles(buildPath, option, target);
            Debug.Log("打包完成！" + System.DateTime.Now.ToString("HH:mm:ss:fff"));
            
            //资源加密
            if ((EncryptMode)EditorPrefs.GetInt(Application.productName + "_EncryptMode", 0) == EncryptMode.AES)
            {
                string keyPath = Application.dataPath + "/Resources";
                if (!Directory.Exists(keyPath))
                {
                    Directory.CreateDirectory(keyPath);
                }
                if (!File.Exists(keyPath + "/Key.asset"))
                {
                    EnciphererKey ek = ScriptableObject.CreateInstance<EnciphererKey>();
                    ek.GeneraterKey();
                    AssetDatabase.CreateAsset(ek, "Assets/Resources/Key.asset");
                }

                EnciphererKey keyAsset = Resources.Load("Key") as EnciphererKey;
                DirectoryInfo dir = new DirectoryInfo(buildPath);
                FileSystemInfo[] infos = dir.GetFileSystemInfos();
                Debug.Log("开始加密！" + System.DateTime.Now.ToString("HH:mm:ss:fff"));
                foreach (FileSystemInfo info in infos)
                {
                    if (info is FileInfo)
                    {
                        if (info.Extension == "")
                        {
                            byte[] bs = File.ReadAllBytes(info.FullName);
                            byte[] cipbs = Encipherer.AESEncrypt(bs, keyAsset);
                            File.WriteAllBytes(info.FullName, cipbs);
                            Debug.Log("完成资源包 " + info.Name + " 的加密！" + System.DateTime.Now.ToString("HH:mm:ss:fff"));
                        }
                    }
                }
                Debug.Log("加密完成！" + System.DateTime.Now.ToString("HH:mm:ss:fff"));
            }

            //写入版本号信息
            string assetVersionPath = buildPath + "/AssetVersion.txt";
            AssetBundleVersionInfo version = new AssetBundleVersionInfo();
            version.Version = EditorPrefs.GetInt(Application.productName + "_AssetVersion", 1);
            version.IsEncrypt = (EncryptMode)EditorPrefs.GetInt(Application.productName + "_EncryptMode", 0) == EncryptMode.AES;
            version.Resources = new List<ResourcesInfo>();
            DirectoryInfo dir1 = new DirectoryInfo(buildPath);
            FileSystemInfo[] infos1 = dir1.GetFileSystemInfos();
            foreach (FileSystemInfo info in infos1)
            {
                if (info is FileInfo)
                {
                    if (info.Extension == "")
                    {
                        ResourcesInfo ri = new ResourcesInfo();
                        ri.Name = info.Name;
                        ri.Hash = info.GetHashCode().ToString();
                        version.Resources.Add(ri);
                    }
                }
            }
            string content = JsonUtility.ToJson(version);
            File.WriteAllText(assetVersionPath, content);

            //版本号迭代
            EditorPrefs.SetInt(Application.productName + "_AssetVersion", EditorPrefs.GetInt(Application.productName + "_AssetVersion", 1) + 1);

            AssetDatabase.Refresh();
        }
	}
}
