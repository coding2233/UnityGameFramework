//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #assetbundle工具# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点33分# </time>
//-----------------------------------------------------------------------

using System;
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

        //编辑器配置文件信息
        private static AssetBundleEditor.EditorConfigInfo _editorConfigInfo;
        //编辑器配置文件的路径
        private static string _editorConfigPath;

        public static AssetBundleEditor.EditorConfigInfo EditorConfigInfo
        {
            get
            {
                if (_editorConfigInfo == null)
                {
                    #region 获取编辑器配置文件的信息
                    _editorConfigPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/", StringComparison.Ordinal)), "ProjectSettings");
                    if (!Directory.Exists(_editorConfigPath))
                        Directory.CreateDirectory(_editorConfigPath);
                    _editorConfigPath = Path.Combine(_editorConfigPath, "AssetBundleEditorConifg.json");
                    if (!File.Exists(_editorConfigPath))
                        _editorConfigInfo = new AssetBundleEditor.EditorConfigInfo();
                    else
                    {
                        string content = File.ReadAllText(_editorConfigPath);
                        _editorConfigInfo = string.IsNullOrEmpty(content) ? new AssetBundleEditor.EditorConfigInfo() : JsonUtility.FromJson<AssetBundleEditor.EditorConfigInfo>(content);
                    }
                    #endregion
                }

                return _editorConfigInfo;
            }
        }

        //保存编辑器配置信息
        public static void SaveEditorConfigInfo()
        {
            #region 设置编辑器配置文件的信息
            //写入文本文件
            if (EditorConfigInfo != null&& !string.IsNullOrEmpty(_editorConfigPath))
                File.WriteAllText(_editorConfigPath, JsonUtility.ToJson(EditorConfigInfo));
            #endregion
        }


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
        [MenuItem("Tools/AssetBundle/Build AssetBundles %#T")]
        public static void BuildAssetBundles()
        {
            string buildPath = EditorConfigInfo.BuildPath;
            if (!Directory.Exists(buildPath))
            {
                Debug.LogError("Please set build path！");
                return;
            }

            //平台资源的信息
            string platformVersionPath = Path.Combine(buildPath, "AssetPlatformVersion.txt");
            AssetPlatformVersionInfo assetPlatformVersionInfo = new AssetPlatformVersionInfo();
            assetPlatformVersionInfo.Platforms = new List<string>();
            //资源加密
            EnciphererKey keyAsset = null;
            if ((EncryptMode) EditorConfigInfo.EncryptMode == EncryptMode.AES)
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
                    AssetDatabase.Refresh();
                }

                keyAsset = Resources.Load("Key") as EnciphererKey;
            }
           
            //根据各个平台打包
            foreach (var item in EditorConfigInfo.BuildTargets)
            {
                //打包
                BuildTarget target = (BuildTarget) item;
                string targetName = target.ToString().ToLower();
                //添加平台的信息
                assetPlatformVersionInfo.Platforms.Add(targetName);
                string targetBuildPath = Path.Combine(buildPath, targetName);
                if (!Directory.Exists(targetBuildPath))
                    Directory.CreateDirectory(targetBuildPath);

                Debug.Log("开始打包--"+ targetName + System.DateTime.Now.ToString("  HH:mm:ss:fff"));
                BuildAssetBundleOptions option = (BuildAssetBundleOptions)EditorConfigInfo.ZipMode;
                BuildPipeline.BuildAssetBundles(targetBuildPath, option, target);
                Debug.Log("打包完成--"+ targetName + System.DateTime.Now.ToString("  HH:mm:ss:fff"));

                //写入版本号信息
                string assetVersionPath = targetBuildPath + "/AssetVersion.txt";
                AssetBundleVersionInfo version = new AssetBundleVersionInfo();
                version.ManifestAssetBundle = targetName;
                version.Version = EditorConfigInfo.AssetVersion;
                version.IsEncrypt = (EncryptMode)EditorConfigInfo.EncryptMode == EncryptMode.AES;
                version.Resources = new List<ResourcesInfo>();
                
                List<FileInfo> allFiles = GetAllFiles(targetBuildPath);
                for (int i = 0; i < allFiles.Count; i++)
                {
                    FileInfo fileInfo = allFiles[i];
                    if (fileInfo.Extension == ".manifest")
                        File.Delete(fileInfo.FullName);
                    else if (fileInfo.Name == "AssetVersion.txt")
                        continue;
                    //加密
                    else
                    {
                        byte[] bs = File.ReadAllBytes(fileInfo.FullName);
                        if (keyAsset != null)
                        {
                            byte[] cipbs = Encipherer.AESEncrypt(bs, keyAsset);
                            File.WriteAllBytes(fileInfo.FullName, cipbs);
                            //加密后md5的值应该刷新
                            bs = cipbs;
							//保存keyAsset
							EditorUtility.SetDirty(keyAsset);
                        }
                        ResourcesInfo resourcesInfo = new ResourcesInfo();
                        string fullPath = Path.GetFullPath(targetBuildPath);
                        resourcesInfo.Name = fileInfo.FullName.Replace(fullPath+ "\\", "");
                        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                        //计算字节数组的哈希值  
                        byte[] toData = md5.ComputeHash(bs);
                        string fileMD5 = "";
                        for (int j = 0; j < toData.Length; j++)
                            fileMD5 += toData[j].ToString("x2");
                        resourcesInfo.MD5 = fileMD5;
                        version.Resources.Add(resourcesInfo);
                    }
                }

                //保存AssetVersion文件
                string content = JsonUtility.ToJson(version);
                File.WriteAllText(assetVersionPath, content);

            }

            //保存平台信息文件
            string platformContent = JsonUtility.ToJson(assetPlatformVersionInfo);
            File.WriteAllText(platformVersionPath, platformContent);

            //版本号迭代
            EditorConfigInfo.AssetVersion += 1;
            SaveEditorConfigInfo();

            AssetDatabase.Refresh();

            //打开打包文件夹
            EditorUtility.OpenWithDefaultApp(buildPath);
        }

        /// <summary>
        /// 获取所有的文件
        /// </summary>
        /// <param name="dirtector"></param>
        /// <returns></returns>
        public static List<FileInfo> GetAllFiles(string folder)
        {
            List<FileInfo> allfiles = new List<FileInfo>();
            DirectoryInfo theFolder = new DirectoryInfo(folder);
            FileInfo[] fileInfos = theFolder.GetFiles();
            foreach (var item in fileInfos)
                allfiles.Add(item);
            DirectoryInfo[] directoryInfos = theFolder.GetDirectories();
            foreach (var item in directoryInfos)
                allfiles.AddRange(GetAllFiles(item.FullName));
            return allfiles;
        }

    }
}
