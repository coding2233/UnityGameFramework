//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #资源模块编辑器# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年12月15日 17点24分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [CustomModuleEditor("Resource Module", 0.851f, 0.227f, 0.286f)]
    public class ResourceModuleEditor : ModuleEditorBase
    {
        private BuildTargetGroup _lastBuildTargetGroup;
        private string _lastScriptingDefineSymbols;
        //资源更新类型
        private const string RESOURCEUPDATETYPE = "ResourceUpdateType";
        //资源本地路径
        private const string PATHTYPE = "PathType";
        //资源更新的路径
        private const string RESOFFICIALUPDATEPATH = "ResOfficialUpdatePath";
        //测试更新的路径
        private const string RESTESTUPDATEPATH = "ResTestUpdatePath";
        //默认是否需要从StreamingAsset里面拷贝到可读文件夹中
        private const string DEFAULTINSTREAMINGASSET = "DefaultInStreamingAsset";

        public ResourceModuleEditor(string name, Color mainColor, GameMode gameMode)
            : base(name, mainColor, gameMode)
        {
            //获取当前的BuildTargetGroup
            _lastBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            _lastScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_lastBuildTargetGroup);
        }

        public override void OnDrawGUI()
        {
            //检查配置文件
            if (!NoConfigError())
            {

                GUILayout.BeginVertical("HelpBox");

                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.Label("Define", GUILayout.Width(50));
                string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_lastBuildTargetGroup);
                _lastScriptingDefineSymbols = GUILayout.TextArea(_lastScriptingDefineSymbols);
                if (GUILayout.Button("OK", GUILayout.Width(40)) && !_lastScriptingDefineSymbols.Equals(scriptingDefineSymbols))
                {
                    _lastBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(_lastBuildTargetGroup, _lastScriptingDefineSymbols);
                }
                GUILayout.EndHorizontal();

                //检查配置文件是否存在
                CheckConfig(RESOURCEUPDATETYPE, 0);
                ResourceUpdateType gameModeResourceUpdateType = (ResourceUpdateType)(int)_gameMode.ConfigJsonData[RESOURCEUPDATETYPE];
                CheckConfig(PATHTYPE, 0);
                PathType gameModePathType = (PathType)(int)_gameMode.ConfigJsonData[PATHTYPE];
                CheckConfig(RESOFFICIALUPDATEPATH, "");
                string gameModeResOfficialUpdatePath = (string)_gameMode.ConfigJsonData[RESOFFICIALUPDATEPATH];
                CheckConfig(RESTESTUPDATEPATH, "");
                string gameModeResTestUpdatePath = (string)_gameMode.ConfigJsonData[RESTESTUPDATEPATH];
                CheckConfig(DEFAULTINSTREAMINGASSET, false);
                bool gameModeDefaultInStreamingAsset = (bool)_gameMode.ConfigJsonData[DEFAULTINSTREAMINGASSET];

                ResourceUpdateType resUpdateType = (ResourceUpdateType)EditorGUILayout.EnumPopup("Resource Update Type", gameModeResourceUpdateType);
                if (resUpdateType != gameModeResourceUpdateType)
                {
                    gameModeResourceUpdateType = resUpdateType;
                    _gameMode.ConfigJsonData[RESOURCEUPDATETYPE] = (int)resUpdateType;
                    //保存数据
                    SaveConfig();
                }
                PathType localPathType = gameModePathType;
                if (gameModeResourceUpdateType != ResourceUpdateType.Editor
                && gameModeResourceUpdateType != ResourceUpdateType.None)
                {
                    if (gameModeResourceUpdateType == ResourceUpdateType.Update)
                    {
                        string officialUpdatePath = EditorGUILayout.TextField("Official Update Path", gameModeResOfficialUpdatePath);
                        if (!officialUpdatePath.Equals(gameModeResOfficialUpdatePath))
                        {
                            gameModeResOfficialUpdatePath = officialUpdatePath;
                            _gameMode.ConfigJsonData[RESOFFICIALUPDATEPATH] = officialUpdatePath;
                            //保存数据
                            SaveConfig();
                        }
                        string testUpdatePath = EditorGUILayout.TextField("Test Update Path", gameModeResTestUpdatePath);
                        if (!testUpdatePath.Equals(gameModeResTestUpdatePath))
                        {
                            gameModeResTestUpdatePath = testUpdatePath;
                            _gameMode.ConfigJsonData[RESTESTUPDATEPATH] = testUpdatePath;
                            //保存数据
                            SaveConfig();
                        }
                        localPathType =
                             (PathType)EditorGUILayout.EnumPopup("Local Path Type", PathType.ReadWrite);
                        bool value = GUILayout.Toggle(gameModeDefaultInStreamingAsset, "Default In StreamingAsset");
                        if (value != gameModeDefaultInStreamingAsset)
                        {
                            gameModeDefaultInStreamingAsset = value;
                            _gameMode.ConfigJsonData[DEFAULTINSTREAMINGASSET] = value;
                            //保存数据
                            SaveConfig();
                        }
                    }
                    else
                    {
                        localPathType =
                            (PathType)EditorGUILayout.EnumPopup("Local Path Type", gameModePathType);
                    }
                    if (gameModePathType != localPathType)
                    {
                        gameModePathType = localPathType;
                        _gameMode.ConfigJsonData[PATHTYPE] = (int)localPathType;
                        SaveConfig();
                    }
                    string path = "";
                    switch (gameModePathType)
                    {
                        case PathType.DataPath:
                            path = Application.dataPath;
                            break;
                        case PathType.ReadOnly:
                            path = Application.streamingAssetsPath;
                            break;
                        case PathType.ReadWrite:
                            path = Application.persistentDataPath;
                            break;
                        case PathType.TemporaryCache:
                            path = Application.temporaryCachePath;
                            break;
                    }

                    EditorGUILayout.LabelField("Path", path);
                }

                GUILayout.EndVertical();
            }
        }


        public override void OnClose()
        {
        }

    }
}