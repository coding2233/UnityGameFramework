//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #assetbundle打包窗口编辑器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点32分# </time>
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace GameFramework.Taurus
{
    public class AssetBundleEditor : EditorWindow
    {
        [MenuItem("Tools/AssetBundle/AssetBundle Editor %#O")]
        private static void OpenAssetBundleWindow()
        {
            AssetBundleEditor ABEditor = GetWindow<AssetBundleEditor>("AssetBundles");
            ABEditor.Init();
            ABEditor.Show();
        }

        #region fields
        private AssetInfo _asset;
        private List<AssetInfo> _validAssets;
        private AssetBundleInfo _assetBundle;
        private int _currentAB = -1;
        private int _currentABAsset = -1;
        private bool _isRename = false;
        private string _renameValue = "";
        
        private Rect _ABViewRect;
        private Rect _ABScrollRect;
        private Vector2 _ABScroll;
        private int _ABViewHeight = 0;

        private Rect _currentABViewRect;
        private Rect _currentABScrollRect;
        private Vector2 _currentABScroll;
        private int _currentABViewHeight = 0;

        private Rect _assetViewRect;
        private Rect _assetScrollRect;
        private Vector2 _assetScroll;
        private int _assetViewHeight = 0;

        private bool _hideInvalidAsset = false;
        private bool _hideBundleAsset = false;

        private string _buildPath = "";
        private Vector2 _buildTargetScrollView = Vector2.zero;
       // private BuildTarget _buildTarget = BuildTarget.StandaloneWindows;
        private Dictionary<string, bool> _buildTargets = new Dictionary<string, bool>();
        private ZipMode _zipMode = ZipMode.LZMA;
        private EncryptMode _encryptMode = EncryptMode.AES;
        private int _assetVersion = 1;

        private bool _showSetting = false;

        private GUIStyle _box = new GUIStyle("Box");
        private GUIStyle _preButton = new GUIStyle("PreButton");
        private GUIStyle _preDropDown = new GUIStyle("PreDropDown");
        private GUIStyle _LRSelect = new GUIStyle("LODSliderRangeSelected");
        private GUIStyle _prefabLabel = new GUIStyle("PR PrefabLabel");
        private GUIStyle _miniButtonLeft = new GUIStyle("MiniButtonLeft");
        private GUIStyle _miniButtonRight = new GUIStyle("MiniButtonRight");
        private GUIStyle _oLMinus = new GUIStyle("OL Minus");
        private GUIStyle _flownode0on = new GUIStyle("flow node 0 on");
        #endregion

        private void Init()
        {
            _asset = new AssetInfo(Application.dataPath, "Assets", true);
            _validAssets = new List<AssetInfo>();
            AssetBundleTool.ReadAssetsInChildren(_asset, _validAssets);

            _assetBundle = new AssetBundleInfo();
            AssetBundleTool.ReadAssetBundleConfig(_assetBundle, _validAssets);

            _buildPath = AssetBundleTool.EditorConfigInfo.BuildPath;
            List<int> buildTargets= AssetBundleTool.EditorConfigInfo.BuildTargets;
            string[] buildTargtNames = Enum.GetNames(typeof(BuildTarget));
            for (int i = 0; i < buildTargtNames.Length; i++)
            {
                BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget),(buildTargtNames[i]));
                _buildTargets.Add(buildTargtNames[i], buildTargets.Contains((int)target));
            }
            //z = (BuildTarget) AssetBundleTool.EditorConfigInfo.BuildTarget;
            _zipMode = (ZipMode) AssetBundleTool.EditorConfigInfo.ZipMode;
            _encryptMode = (EncryptMode) AssetBundleTool.EditorConfigInfo.EncryptMode;
            _assetVersion = AssetBundleTool.EditorConfigInfo.AssetVersion;

            Resources.UnloadUnusedAssets();
        }

        private void Update()
        {
            if (EditorApplication.isCompiling)
            {
                Close();
            }
        }

        private void OnGUI()
        {
            TitleGUI();
            AssetBundlesGUI();
            CurrentAssetBundlesGUI();
            AssetsGUI();
            SettingGUI();
        }
        private void TitleGUI()
        {
            if (GUI.Button(new Rect(5, 5, 60, 15), "Create", _preButton))
            {
                AssetBundleBuildInfo build = new AssetBundleBuildInfo("ab" + System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                _assetBundle.AssetBundles.Add(build);
            }

            GUI.enabled = _currentAB == -1 ? false : true;
            if (GUI.Button(new Rect(65, 5, 60, 15), "Rename", _preButton))
            {
                _isRename = !_isRename;
            }
            if (GUI.Button(new Rect(125, 5, 60, 15), "Clear", _preButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Clear " + _assetBundle.AssetBundles[_currentAB].Name + " ？", "Yes", "No"))
                {
                    _assetBundle.AssetBundles[_currentAB].ClearAsset();
                }
            }
            if (GUI.Button(new Rect(185, 5, 60, 15), "Delete", _preButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Delete " + _assetBundle.AssetBundles[_currentAB].Name + "？This will clear all assets！", "Yes", "No"))
                {
                    _assetBundle.DeleteAssetBundle(_currentAB);
                    _currentAB = -1;
                }
            }
            if (GUI.Button(new Rect(250, 5, 100, 15), "Add Assets", _preButton))
            {
                List<AssetInfo> assets = _validAssets.GetCheckedAssets();
                for (int i = 0; i < assets.Count; i++)
                {
                    _assetBundle.AssetBundles[_currentAB].AddAsset(assets[i]);
                }
            }
            GUI.enabled = true;
            
            if (GUI.Button(new Rect(250, 25, 60, 15), "Open", _preButton))
            {
                AssetBundleTool.OpenFolder(_buildPath);
            }
            if (GUI.Button(new Rect(310, 25, 60, 15), "Browse", _preButton))
            {
                string path = EditorUtility.OpenFolderPanel("Select Path", Application.dataPath, "");
                if (path.Length != 0)
                {
                    _buildPath = path;
                    AssetBundleTool.EditorConfigInfo.BuildPath = _buildPath;
                    AssetBundleTool.SaveEditorConfigInfo();
                }
            }

            GUI.Label(new Rect(370, 25, 70, 15), "Build Path:");
            _buildPath = GUI.TextField(new Rect(440, 25, 300, 15), _buildPath);

            if (GUI.Button(new Rect(250, 45, 100, 15), "Expand All", _preButton))
            {
                AssetBundleTool.ExpandFolder(_asset, true);
            }
            if (GUI.Button(new Rect(350, 45, 100, 15), "Shrink All", _preButton))
            {
                AssetBundleTool.ExpandFolder(_asset, false);
            }

            _hideInvalidAsset = GUI.Toggle(new Rect(460, 45, 100, 15), _hideInvalidAsset, "Hide Invalid");
            _hideBundleAsset = GUI.Toggle(new Rect(560, 45, 100, 15), _hideBundleAsset, "Hide Bundled");

            _showSetting = GUI.Toggle(new Rect((int)position.width - 135, 5, 80, 15), _showSetting, "Setting", _preButton);

            if (GUI.Button(new Rect((int)position.width - 55, 5, 50, 15), "Build", _preButton))
            {
                AssetBundleTool.BuildAssetBundles();
            }
        }
        private void AssetBundlesGUI()
        {
            _ABViewRect = new Rect(5, 25, 240, (int)position.height / 2 - 20);
            _ABScrollRect = new Rect(5, 25, 240, _ABViewHeight);
            _ABScroll = GUI.BeginScrollView(_ABViewRect, _ABScroll, _ABScrollRect);
            GUI.BeginGroup(_ABScrollRect, _box);

            _ABViewHeight = 5;

            for (int i = 0; i < _assetBundle.AssetBundles.Count; i++)
            {
                string icon = _assetBundle.AssetBundles[i].Assets.Count > 0 ? "PrefabNormal Icon" : "Prefab Icon";
                if (_currentAB == i)
                {
                    GUI.Box(new Rect(0, _ABViewHeight, 240, 15), "", _LRSelect);

                    if (_isRename)
                    {
                        GUIContent content = EditorGUIUtility.IconContent(icon);
                        content.text = "";
                        GUI.Label(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel);
                        _renameValue = GUI.TextField(new Rect(40, _ABViewHeight, 140, 15), _renameValue);
                        if (GUI.Button(new Rect(180, _ABViewHeight, 30, 15), "OK", _miniButtonLeft))
                        {
                            if (_renameValue != "")
                            {
                                if (!_assetBundle.IsExistName(_renameValue))
                                {
                                    _assetBundle.AssetBundles[_currentAB].RenameAssetBundle(_renameValue);
                                    _renameValue = "";
                                    _isRename = false;
                                }
                                else
                                {
                                    Debug.LogError("Already existed name:" + _renameValue);
                                }
                            }
                        }
                        if (GUI.Button(new Rect(210, _ABViewHeight, 30, 15), "NO", _miniButtonRight))
                        {
                            _isRename = false;
                        }
                    }
                    else
                    {
                        GUIContent content = EditorGUIUtility.IconContent(icon);
                        content.text = _assetBundle.AssetBundles[i].Name;
                        GUI.Label(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel);
                    }
                }
                else
                {
                    GUIContent content = EditorGUIUtility.IconContent(icon);
                    content.text = _assetBundle.AssetBundles[i].Name;
                    if (GUI.Button(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel))
                    {
                        _currentAB = i;
                        _currentABAsset = -1;
                        _isRename = false;
                    }
                }
                _ABViewHeight += 20;
            }

            _ABViewHeight += 5;
            if (_ABViewHeight < _ABViewRect.height)
            {
                _ABViewHeight = (int)_ABViewRect.height;
            }

            GUI.EndGroup();
            GUI.EndScrollView();
        }
        private void CurrentAssetBundlesGUI()
        {
            _currentABViewRect = new Rect(5, (int)position.height / 2 + 10, 240, (int)position.height / 2 - 15);
            _currentABScrollRect = new Rect(5, (int)position.height / 2 + 10, 240, _currentABViewHeight);
            _currentABScroll = GUI.BeginScrollView(_currentABViewRect, _currentABScroll, _currentABScrollRect);
            GUI.BeginGroup(_currentABScrollRect, _box);

            _currentABViewHeight = 5;

            if (_currentAB != -1)
            {
                AssetBundleBuildInfo build = _assetBundle.AssetBundles[_currentAB];
                for (int i = 0; i < build.Assets.Count; i++)
                {
                    if (_currentABAsset == i)
                    {
                        GUI.Box(new Rect(0, _currentABViewHeight, 240, 15), "", _LRSelect);
                    }
                    GUIContent content = EditorGUIUtility.ObjectContent(null, build.Assets[i].AssetType);
                    content.text = build.Assets[i].AssetName;
                    if (GUI.Button(new Rect(5, _currentABViewHeight, 205, 15), content, _prefabLabel))
                    {
                        _currentABAsset = i;
                    }
                    if (GUI.Button(new Rect(215, _currentABViewHeight, 20, 15), "", _oLMinus))
                    {
                        build.RemoveAsset(build.Assets[i]);
                        _currentABAsset = -1;
                    }
                    _currentABViewHeight += 20;
                }
            }

            _currentABViewHeight += 5;
            if (_currentABViewHeight < _currentABViewRect.height)
            {
                _currentABViewHeight = (int)_currentABViewRect.height;
            }

            GUI.EndGroup();
            GUI.EndScrollView();
        }
        private void AssetsGUI()
        {
            _assetViewRect = new Rect(250, 65, (int)position.width - 255, (int)position.height - 70);
            _assetScrollRect = new Rect(250, 65, (int)position.width - 255, _assetViewHeight);
            _assetScroll = GUI.BeginScrollView(_assetViewRect, _assetScroll, _assetScrollRect);
            GUI.BeginGroup(_assetScrollRect, _box);

            _assetViewHeight = 5;

            AssetGUI(_asset, 0);

            _assetViewHeight += 5;
            if (_assetViewHeight < _assetViewRect.height)
            {
                _assetViewHeight = (int)_assetViewRect.height;
            }

            GUI.EndGroup();
            GUI.EndScrollView();
        }
        private void AssetGUI(AssetInfo asset, int indentation)
        {
            if (_hideInvalidAsset && asset.AssetFileType == FileType.InValidFile)
            {
                return;
            }
            if (_hideBundleAsset && asset.Bundled != "")
            {
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(indentation * 20 + 5);
            if (asset.AssetFileType == FileType.Folder)
            {
                if (GUILayout.Toggle(asset.IsChecked, "", GUILayout.Width(20)) != asset.IsChecked)
                {
                    AssetBundleTool.ChangeCheckedInChildren(asset, !asset.IsChecked);
                }

                GUIContent content = EditorGUIUtility.IconContent("Folder Icon");
                content.text = asset.AssetName;
                asset.IsExpanding = EditorGUILayout.Foldout(asset.IsExpanding, content);
            }
            else
            {
                GUI.enabled = !(asset.AssetFileType == FileType.InValidFile || asset.Bundled != "");
                if (GUILayout.Toggle(asset.IsChecked, "", GUILayout.Width(20)) != asset.IsChecked)
                {
                    AssetBundleTool.ChangeCheckedInChildren(asset, !asset.IsChecked);
                }

                GUILayout.Space(10);
                GUIContent content = EditorGUIUtility.ObjectContent(null, asset.AssetType);
                content.text = asset.AssetName;
                GUILayout.Label(content, GUILayout.Height(20));
                GUI.enabled = true;

                if (asset.Bundled != "")
                {
                    GUILayout.Label("[" + asset.Bundled + "]", _prefabLabel);
                }
            }
            _assetViewHeight += 20;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (asset.IsExpanding)
            {
                for (int i = 0; i < asset.ChildAssetInfo.Count; i++)
                {
                    AssetGUI(asset.ChildAssetInfo[i], indentation + 1);
                }
            }
        }
        
        private void SettingGUI()
        {
            if (_showSetting)
            {
                int width = 325, height = 290;

                GUI.BeginGroup(new Rect((int)position.width / 2 - width/2, (int)position.height / 2 - height/2, width, height), "", _flownode0on);
                //-----------Build Target
                GUI.Label(new Rect(5, 5, 100, 20), "Build Target:", "HeaderLabel");
                GUI.BeginGroup(new Rect(5, 25, 315, 135), _box);
               // string[] buildTargets = Enum.GetNames(typeof(BuildTarget));
                _buildTargetScrollView = GUI.BeginScrollView(new Rect(0, 2, 313, 132), _buildTargetScrollView,
                    new Rect(0, 0, 180, 20* _buildTargets.Count),false,true);
                int index = 0;
                foreach (var item in _buildTargets)
                {
                    bool value = GUI.Toggle(new Rect(5, 20 * index, 180, 20), item.Value, item.Key);
                    if (value != item.Value)
                    {
                        _buildTargets[item.Key] = value;
                        break;
                    }
                    index++;
                }
                GUI.EndScrollView();
                GUI.EndGroup();
                //--------Zip Mode
                GUI.Label(new Rect(5, 165, 60, 20), "Zip Mode:", "HeaderLabel");
                ZipMode zipMode = (ZipMode)EditorGUI.EnumPopup(new Rect(75, 165, 240, 20), _zipMode, _preDropDown);
                if (zipMode != _zipMode)
                {
                    _zipMode = zipMode;
                    AssetBundleTool.EditorConfigInfo.ZipMode = (int)_zipMode;
                    AssetBundleTool.SaveEditorConfigInfo();
                }
                //----------Encrypt
                GUI.Label(new Rect(5, 190, 60, 20), "Encrypt: ", "HeaderLabel");
                EncryptMode encryptMode = (EncryptMode)EditorGUI.EnumPopup(new Rect(75, 190, 240, 20), _encryptMode, _preDropDown);
                if (encryptMode != _encryptMode)
                {
                    _encryptMode = encryptMode;
                    AssetBundleTool.EditorConfigInfo.EncryptMode = (int)encryptMode;
                    AssetBundleTool.SaveEditorConfigInfo();
                }
               // ----------Assets Version
                GUI.Label(new Rect(5, 215, 150, 20), "Asset Version: " + _assetVersion, "HeaderLabel");
                if (GUI.Button(new Rect(160, 215, 95, 20), "Reset", _preButton))
                {
                    _assetVersion = 1;
                    AssetBundleTool.EditorConfigInfo.AssetVersion = _assetVersion;
                    AssetBundleTool.SaveEditorConfigInfo();
                }
                //sure cancel 
                if (GUI.Button(new Rect(92.5f, 255, 140, 20), "Sure", _preButton))
                {
                    //更新目标平台
                    foreach (var item in _buildTargets)
                    {
                        BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget), (item.Key));
                        int iTarget = (int)target;
                        if (item.Value)
                        {
                            if (!AssetBundleTool.EditorConfigInfo.BuildTargets.Contains(iTarget))
                                AssetBundleTool.EditorConfigInfo.BuildTargets.Add(iTarget);
                        }
                        else
                        {
                            if (AssetBundleTool.EditorConfigInfo.BuildTargets.Contains(iTarget))
                                AssetBundleTool.EditorConfigInfo.BuildTargets.Remove(iTarget);
                        }
                    }
                    //保存配置文件
                    AssetBundleTool.SaveEditorConfigInfo();

                    _showSetting = false;
                }
                //if (GUI.Button(new Rect(175, 255, 140, 20), "Cancel", _preButton))
                //{
                //    _showSetting = false;
                //}
                GUI.EndGroup();
            }
        }


        //编辑器的配置信息
        [System.Serializable]
        public class EditorConfigInfo
        {
            /// <summary>
            /// 打包路径
            /// </summary>
            public string BuildPath;
            /// <summary>
            /// 压缩方式
            /// </summary>
            public int ZipMode=0;
            /// <summary>
            /// 目标平台
            /// </summary>
            public List<int> BuildTargets=new List<int>(){5};
            /// <summary>
            /// 加密模式
            /// </summary>
            public int EncryptMode=1;
            /// <summary>
            /// 资源版本
            /// </summary>
            public int AssetVersion=1;
        }

    }

    /// <summary>
    /// 资源文件类型
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// 有效的文件资源
        /// </summary>
        ValidFile,
        /// <summary>
        /// 文件夹
        /// </summary>
        Folder,
        /// <summary>
        /// 无效的文件资源
        /// </summary>
        InValidFile
    }

    /// <summary>
    /// 压缩模式
    /// </summary>
    public enum ZipMode
    {
        /// <summary>
        /// 不压缩
        /// </summary>
        Uncompressed = 1,
        /// <summary>
        /// LZMA压缩
        /// </summary>
        LZMA = 0,
        /// <summary>
        /// LZ4压缩
        /// </summary>
        LZ4 = 256
    }

    /// <summary>
    /// 加密模式
    /// </summary>
    public enum EncryptMode
    {
        /// <summary>
        /// 不加密
        /// </summary>
        None,
        /// <summary>
        /// AES加密
        /// </summary>
        AES
    }
}
