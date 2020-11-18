using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;

namespace Wanderer.GameFramework
{

    public class AssetBundleBuildEditor : EditorWindow
    {
        //资源信息文本名称
        private const string _assetVersionTxt = "AssetVersion.txt";
        //所有的文件对应的文件名称
        private const string _assetsTxt = "assets";
        private const string _configPath = "ProjectSettings/AssetBundleEditorConifg.json";
        private static AssetBundleConifgInfo _config;
        private static List<string> _buildTargets;
        private static string _outPutPath = "";

        private Vector2 _scrollViewPos;
        private Vector2 _scrollViewPosOtherRs;
        private static string _rootPath;

        private static readonly Dictionary<BuildTarget, bool> _allTargets = new Dictionary<BuildTarget, bool>();

        //压缩内容
        string[] _compressionOptionsContent = new string[] { "No Compression", "Standard Compression (LZMA)", "Chunk Based Compression (LZ4)" };

        [MenuItem("Tools/Asset Bundle/AssetBundle Build Options %#O")]
        public static void AssetBundilesOptions()
        {
            _rootPath = Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));

            LoadConfig();

            //BuildTarget[] allTargets = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));
            ////_allTargets
            //foreach (var item in allTargets)
            //{
            //    int index = (int)item;
            //    if (_config.BuildTargets.Contains(index))
            //        _allTargets[item] = true;
            //    else
            //        _allTargets[item] = false;
            //}

            GetWindowWithRect<AssetBundleBuildEditor>(new Rect(200, 300, 500, 400), true, "Options");
        }

        //[MenuItem("Tools/Asset Bundle/Build AssetBundles %#T")]
        //public static void BuildAssetBundles()
        //{
        //    LoadConfig();

        //    BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        //    //打包编辑器的激活平台
        //    BuildTarget(target);

        //    //保存平台信息
        //    SavePlatformVersion(new List<BuildTarget>() { target });

        //    if(_config.Copy2StreamingAssets)
        //    {
        //        //复制资源
        //        CopyResource(target);
        //        AssetDatabase.Refresh();
        //    }
        //}

        //[MenuItem("Tools/Asset Bundle/Build AssetBundles Targets %#Y")]
        //public static void BuildAssetBundlesAllTargets()
        //{
        //    LoadConfig();

        //    List<BuildTarget> targets = new List<BuildTarget>();
        //    for (int i = 0; i < _config.BuildTargets.Count; i++)
        //    {
        //        BuildTarget target = (BuildTarget)_config.BuildTargets[i];
        //        BuildTarget(target);
        //        targets.Add(target);
        //    }

        //    //保存平台信息
        //    SavePlatformVersion(targets);
        //    if (_config.Copy2StreamingAssets&&targets.Contains(EditorUserBuildSettings.activeBuildTarget))
        //    {
        //        //复制资源
        //        CopyResource(EditorUserBuildSettings.activeBuildTarget);
        //        AssetDatabase.Refresh();
        //    }
        //}


        /// <summary>
        /// 打包AssetBundle
        /// </summary>
        public static string BuildAssetBundles(BuildTarget target)
        {
            LoadConfig();
            //打包编辑器的激活平台
            string buildPath = BuildTarget(target);
            //保存平台信息
          //  SavePlatformVersion(new List<BuildTarget>() { target });
            if (_config.Copy2StreamingAssets)
            {
                //复制资源
                CopyResource(target);
                AssetDatabase.Refresh();
            }
            return buildPath;
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events.
        /// This function can be called multiple times per frame (one call per event).
        /// </summary>
        void OnGUI()
        {
            if (_config == null)
            {
                return;
            }

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Version:");
            GUILayout.Label(_config.Version.ToString());
            if (GUILayout.Button("RESET", GUILayout.Width(60)))
            {
                _config.Version = 0;
            }
            GUILayout.EndHorizontal();

            //压缩格式
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Compression:");
            _config.CompressOptions = EditorGUILayout.Popup(_config.CompressOptions, _compressionOptionsContent);
            GUILayout.EndHorizontal();

            ////Encrypt------------------------------------
            // GUILayout.BeginHorizontal("Box");
            // GUILayout.FlexibleSpace();
            // _config.IsEncrypt = GUILayout.Toggle(_config.IsEncrypt, "Encrypt");
            // GUILayout.EndHorizontal();

            //BUILD PATH
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Build Path:");
            GUILayout.TextArea(string.IsNullOrEmpty(_config.BuildPath) ? _rootPath : _config.BuildPath);

            if (GUILayout.Button("BROWSE", GUILayout.Width(80)))
            {
                string path = (string.IsNullOrEmpty(_config.BuildPath) || !Directory.Exists(_config.BuildPath)) ? _rootPath : _config.BuildPath;

                path = EditorUtility.OpenFolderPanel("Build Path", path, "");
                if (!string.IsNullOrEmpty(path))
                {
                    path = Path.GetFullPath(path);
                    if (path.Contains(_rootPath))
                    {
                        path = path.Replace(_rootPath, "").Replace("\\", "/");
                        if (path.IndexOf("/") == 0)
                        {
                            path = path.Substring(1, path.Length - 1);
                        }
                    }
                    _config.BuildPath = path;
                }
                return;
            }
            GUILayout.EndHorizontal();

            //copy------------------------------------
            GUILayout.BeginHorizontal("Box");
            _config.UseAssetBundleEditor = GUILayout.Toggle(_config.UseAssetBundleEditor, "Use AssetBundleEditor");
            GUILayout.FlexibleSpace();
            _config.Copy2StreamingAssets = GUILayout.Toggle(_config.Copy2StreamingAssets, "Copy to StreamingAssets");
            GUILayout.EndHorizontal();

            //build target----------------------------------------------------------------------------
            BuildTarget buildTarget = (BuildTarget)_config.BuildTarget;
            BuildTarget newBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target:", buildTarget);
            if (newBuildTarget != buildTarget)
            {
                _config.BuildTarget = (int)newBuildTarget;
            }
            //GUILayout.BeginVertical("Box");
            //GUILayout.Label("Build Targets:");
            //_scrollViewPos = GUILayout.BeginScrollView(_scrollViewPos, "Box");
            //foreach (var item in _allTargets)
            //{
            //    bool value = GUILayout.Toggle(item.Value, item.Key.ToString());
            //    if (value != item.Value)
            //    {
            //        _allTargets[item.Key] = value;
            //        break;
            //    }
            //}
            //GUILayout.EndScrollView();
            //GUILayout.EndVertical();

            //other reseources----------------------------------------------------------------------------
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Other Resources:");
            _scrollViewPosOtherRs = GUILayout.BeginScrollView(_scrollViewPosOtherRs, "Box");
            _config.OtherResources = EditorGUILayout.TextArea(_config.OtherResources, GUILayout.Height(200));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();


            //确认更改--------------------------------------------------------------------------------
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(60)))
            {
                //保存配置文件
                SaveConfig();
                //关闭窗口
                Close();
            }
            if (GUILayout.Button("Build", GUILayout.Width(60)))
            {
                //保存配置文件
                SaveConfig();
                //资源打包
                buildTarget = (BuildTarget)_config.BuildTarget;
                BuildAssetBundles(buildTarget);
                //BuildTarget();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        //加载配置信息
        private static void LoadConfig()
        {
            if (!File.Exists(_configPath))
            {
                File.WriteAllText(_configPath, JsonUtility.ToJson(new AssetBundleConifgInfo()));
            }

            _config = JsonUtility.FromJson<AssetBundleConifgInfo>(File.ReadAllText(_configPath));
        }

        //保存配置信息
        private static void SaveConfig()
        {
            //_config.BuildTargets = new List<int>();
            //foreach (var item in _allTargets)
            //{
            //    if (item.Value)
            //    {
            //        _config.BuildTargets.Add((int)item.Key);
            //    }
            //}
            string json = JsonUtility.ToJson(_config);
            File.WriteAllText(_configPath, json);
        }

        //资源打包
        private static string BuildTarget(BuildTarget target)
        {
            try
            {
                //打包路径
                string targetName = $"{Application.version}_{_config.Version}";
                string buildPath = Path.Combine(_config.BuildPath, BuildTargetToString(target), targetName); ;
                if (!Directory.Exists(buildPath))
                    Directory.CreateDirectory(buildPath);
                //设置打包的相关选项
                BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
                //设置压缩 默认LZMA
                if (_config.CompressOptions == 0)
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                //LZ4
                else if (_config.CompressOptions == 2)
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                //打包  Build
                if (_config.UseAssetBundleEditor)
                {
                    BuildPipeline.BuildAssetBundles(buildPath, AssetBundleEditor.GetAssetBundleBuild(), options, target);
                }
                else
                {
                    BuildPipeline.BuildAssetBundles(buildPath, options, target);
                }
                //保存资源版本信息
                SaveAssetVersion(buildPath, targetName, target);
                Debug.Log($"资源打包成功: {buildPath}");
                //    //更新资源版本号 -- 保存配置文件
                _config.Version++;
                SaveConfig();
                return buildPath;
            }
            catch (System.Exception e)
            {
                throw new GameException($"Build assetbundle error [{target.ToString()}] :{e.ToString()}");
            }
			
        }

        //保存资源版本信息
        private static void SaveAssetVersion(string buildPath, string targetName, BuildTarget target)
        {
            //string targetName = target.ToString().ToLower();
            string targetBundlePath = Path.Combine(buildPath, targetName);
            if (!File.Exists(targetBundlePath))
                return;
            //删除manifest文件
            string targetManifestPath =$"{targetBundlePath}.manifest";
            if (File.Exists(targetManifestPath))
                File.Delete(targetManifestPath);
            //移动manifest文件
            targetName = "manifest";
            string tempTargetBundlePath = Path.Combine(buildPath, targetName);
            if (File.Exists(tempTargetBundlePath))
                File.Delete(tempTargetBundlePath);
            File.Move(targetBundlePath, tempTargetBundlePath) ;
            targetBundlePath = tempTargetBundlePath;

            //整理AssetBundleVersionInfo
            AssetBundleVersionInfo assetVersionInfo = new AssetBundleVersionInfo();
            assetVersionInfo.Version = _config.Version;
            assetVersionInfo.ManifestAssetBundle = targetName;
            assetVersionInfo.AssetHashInfos = new List<AssetHashInfo>();

            AssetBundle targetBundle = AssetBundle.LoadFromFile(targetBundlePath);
            AssetBundleManifest manifest = targetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //整理全部的资源并生成配置文件
            string assetContent = GetAssetsFromAssetBundle(buildPath,manifest);
            string assetsTxtPath=Path.Combine(buildPath,_assetsTxt);
            File.WriteAllText(Path.Combine(buildPath,"assets"),assetContent);
            //assetbundle
            List<string> assetNames = new List<string>();
            assetNames.Add(_assetsTxt);
            assetNames.Add(targetName);
            assetNames.AddRange(manifest.GetAllAssetBundles());
            
            for (int i = 0; i < assetNames.Count; i++)
            {
                AssetHashInfo assetHashInfo = new AssetHashInfo();
                assetHashInfo.Name = assetNames[i];
                assetHashInfo.ForceUpdate = _config.UseAssetBundleEditor ? AssetBundleEditor.GetForceUpdate(assetHashInfo.Name) : true;
                string filePath = Path.Combine(buildPath, assetNames[i]);
                byte[] data = FileUtility.GetBytes(filePath);
                 //加密
                if (_config.IsEncrypt)
                {
					using (var stream = new EncryptFileStream(filePath, FileMode.Create))
					{
						stream.Write(data, 0, data.Length);
					}
				}
                assetHashInfo.Size = data.Length > 1024 ? (int)(data.Length / 1024.0f) : 1;
                assetHashInfo.Hash = FileUtility.GetFileMD5(data);
                assetVersionInfo.AssetHashInfos.Add(assetHashInfo);
                //删除manifest文件
                string manifestPath = Path.Combine(buildPath, assetNames[i] + ".manifest");
              //  manifestPath=Path.GetFullPath(manifestPath);
                if (File.Exists(manifestPath))
                {
                    File.Delete(manifestPath);
                }
            }

            //添加其他资源的信息
            List<AssetHashInfo> otherResInfo = LoadOtherResource(buildPath);
            assetVersionInfo.AssetHashInfos.AddRange(otherResInfo);

            string json = JsonUtility.ToJson(assetVersionInfo);
            //VersionAsset 加密
            json = json.ToEncrypt();
            string buildAssetVersionPath = Path.Combine(buildPath, _assetVersionTxt);
            File.WriteAllText(buildAssetVersionPath, json);
            targetBundle.Unload(true);
        }

        //保存平台版本信息
        //private static void SavePlatformVersion(List<BuildTarget> targets)
        //{
        //    if (targets == null || targets.Count == 0)
        //        return;

        //    PlatformVersionInfo platformInfo = new PlatformVersionInfo();
        //    platformInfo.Version = _config.Version;
        //    platformInfo.Platforms = new List<string>();
        //    foreach (var item in targets)
        //    {
        //        platformInfo.Platforms.Add(item.ToString().ToLower());
        //    }
        //    string json = JsonUtility.ToJson(platformInfo);
        //    //保存平台信息
        //    File.WriteAllText(Path.Combine(_config.BuildPath, _assetVersionTxt), json);
        //    //更新资源版本号 -- 保存配置文件
        //    _config.Version++;
        //    SaveConfig();
        //    //打开文件夹
        //    // EditorUtility.OpenWithDefaultApp(_config.BuildPath);
        //}

        //添加本地资源
        private static List<AssetHashInfo> LoadOtherResource(string buildPath)
        {
            List<AssetHashInfo> assetHashInfos = new List<AssetHashInfo>();
            if (_config == null || string.IsNullOrEmpty(_config.OtherResources))
                return assetHashInfos;

            string[] otherRes = _config.OtherResources.Split('\n');
            if (otherRes != null && otherRes.Length > 0)
            {
                string rootPath = Application.dataPath;
                foreach (var item in otherRes)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;

                    string srcPath = Path.Combine(rootPath, item);
                    if (!File.Exists(srcPath))
                    {
                        Debug.LogWarning($"找不到文件:{srcPath}");
                        continue;
                    }

                    //获取文件的md5值
                    byte[] data = FileUtility.GetBytes(srcPath);
                    string hash = FileUtility.GetFileMD5(data);
                    string fileName = Path.GetFileName(srcPath);
                    int size = data.Length > 1024 ? (int)(data.Length / 1024.0f) : 1;
                    //复制文件
                    File.Copy(srcPath, Path.Combine(buildPath, fileName), true);
                    //添加信息
                    assetHashInfos.Add(new AssetHashInfo() { Name = fileName, Hash = hash, Size = size });
                }
            }

            return assetHashInfos;
        }

        //获取所有的资源
        private static string GetAssetsFromAssetBundle(string buildPath,AssetBundleManifest manifest)
        {
            HashSet<string> assets=new HashSet<string>();
            StringBuilder stringBuilder=new StringBuilder();
            string[] assetBundles = manifest.GetAllAssetBundles();
            for (int i = 0; i < assetBundles.Length; i++)
            {
                 string filePath = Path.Combine(buildPath, assetBundles[i]);
                AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
                 //存储资源名称
                string[] assetNames = assetBundle.GetAllAssetNames();
                if (assetBundle.isStreamedSceneAssetBundle)
                    assetNames = assetBundle.GetAllScenePaths();
                foreach (var item in assetNames)
                {
                     if(!assets.Contains(item))
                     {
                         stringBuilder.AppendLine($"{item}\t{assetBundles[i]}");
                         assets.Add(item);
                     }
                }
                assetBundle.Unload(true);
            }
            return stringBuilder.ToString();
        }

        //复制资源
        private static void CopyResource(BuildTarget target)
        {
            //打包路径
            string buildPath = Path.GetFullPath(Path.Combine(_config.BuildPath, target.ToString().ToLower()));
            //获取源文件夹下的所有文件  不考虑子文件夹
            string[] files = Directory.GetFiles(buildPath);
            string targetPath = Path.GetFullPath(Application.streamingAssetsPath);
            if(!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i].Replace(buildPath, targetPath);
                File.Copy(files[i], path, true);
            }
        }


        /// <summary>
        /// BuildTarget转字符串
        /// </summary>
        /// <param name="target"></param>
        private static string BuildTargetToString(BuildTarget target)
        {
            string targetName = target.ToString();
            if (targetName.Contains("Windows"))
            {
                targetName = "Windows";
            }
            else if (targetName.Contains("OSX"))
            {
                targetName = "OSX";
            }
            else if (targetName.Contains("Linux"))
            {
                targetName = "Linux";
            }
            return targetName.ToLower();
        }

        //ab包的配置文件信息
        [System.Serializable]
        public class AssetBundleConifgInfo
        {
            public int Version = 0;
            public string BuildPath = "";
            public int CompressOptions = 1;
            public bool UseAssetBundleEditor = true;
            public bool Copy2StreamingAssets = false;
            public int BuildTarget = 13;
         //   public List<int> BuildTargets = new List<int>();
            public string OtherResources = "";
            public bool IsEncrypt = true;
        }
    }
}