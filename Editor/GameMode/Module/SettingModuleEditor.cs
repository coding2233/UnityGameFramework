//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #设置模块编辑器# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年12月15日 17点27分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wanderer.GameFramework
{
    [CustomModuleEditor("Settings Module", 0.2f, 0.8f, 0.6f)]
    public class SettingModuleEditor : ModuleEditorBase
    {
        private BuildTargetGroup _lastBuildTargetGroup;
        private string _lastScriptingDefineSymbols;
        private int _selectType = 1;
        private int _resourcePlanType = 0;
        private HashSet<string> _defineSymbols;
        private const string TEST = "TEST";
        private const string ADDRESSABLES = "ADDRESSABLES_SUPPORT";

        public SettingModuleEditor(string name, Color mainColor, GameMode gameMode)
        : base(name, mainColor, gameMode)
        {
            //获取当前的BuildTargetGroup
            _lastBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            _lastScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_lastBuildTargetGroup);
            _defineSymbols = new HashSet<string>();
            if (!string.IsNullOrEmpty(_lastScriptingDefineSymbols))
            {
                string[] args = _lastScriptingDefineSymbols.Split(';');
                if (args != null)
                {
					foreach (var item in args)
					{
                        if (!string.IsNullOrEmpty(item))
                        {
                            _defineSymbols.Add(item);
                            if (item.Equals(TEST))
                            {
                                _selectType = 0;
                            }
                            else if (item.Equals(ADDRESSABLES))
                            {
                                _resourcePlanType = 1;
                            }
                        }
					}
                }
            }
        }

        public override void OnDrawGUI()
        {
            GUILayout.BeginVertical("HelpBox");
            //Resource plan
            int resourcePlanType = EditorGUILayout.IntPopup("Resource Plan", _resourcePlanType, new string[] { "Asset Bundle", "Addressables" }, new int[] { 0, 1 });
            if (resourcePlanType != _resourcePlanType)
            {
                if (resourcePlanType == 1)
                {
                    _defineSymbols.Add(ADDRESSABLES);
                }
                else
                {
                    _defineSymbols.Remove(ADDRESSABLES);
                }
                _resourcePlanType = resourcePlanType;
                SaveScriptingDefineSymbols();
            }
#if !ADDRESSABLES_SUPPORT
            //app server
            int selectType = EditorGUILayout.IntPopup("App Server",_selectType, new string[] { "Test", "Official" },new int[] { 0,1});
            if (selectType != _selectType)
            {
                if (selectType == 0)
                {
                    _defineSymbols.Add(TEST);
                }
                else
                {
                    _defineSymbols.Remove(TEST);
                }
                _selectType = selectType;
                SaveScriptingDefineSymbols();
            }
#endif
            //检查配置文件
            if (!NoConfigError())
            {
                GUILayout.BeginHorizontal();
                
                ////支持Address
                //bool addressablesSupport=(bool)_gameMode.ConfigJsonData["AddressablesSupport"];
                //bool newAddressablesSupport = GUILayout.Toggle(addressablesSupport, "Addressables Support");
                //if (addressablesSupport != newAddressablesSupport)
                //{
                //    _gameMode.ConfigJsonData["AddressablesSupport"] = addressablesSupport;
                //    SaveConfig();
                //}

                //启动调试器
                bool debugEnable = (bool)_gameMode.ConfigJsonData["DebugEnable"];
                bool newDebugEnable = GUILayout.Toggle(debugEnable, "Debug Enable");
                if (debugEnable != newDebugEnable)
                {
                    _gameMode.ConfigJsonData["DebugEnable"] = newDebugEnable;
                    SaveConfig();
                }
                //启动日志文件
                bool logFileEnable = (bool)_gameMode.ConfigJsonData["LogFileEnable"];
                bool newLogFileEnable = GUILayout.Toggle(logFileEnable, "LogFile Enable");
                if (logFileEnable != newLogFileEnable)
                {
                    _gameMode.ConfigJsonData["LogFileEnable"] = newLogFileEnable;
                    SaveConfig();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public override void OnClose()
        {
        }


        private void SaveScriptingDefineSymbols()
        {
            //EditorUtility.DisplayProgressBar("", "Is setting PlayerSettings ScriptingDefineSymbolsForGroup, please wait...",0.9f);
            _lastScriptingDefineSymbols = "";
            foreach (var item in _defineSymbols)
            {
                _lastScriptingDefineSymbols = $"{_lastScriptingDefineSymbols}{item};";
            }

            _lastBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(_lastBuildTargetGroup, _lastScriptingDefineSymbols);
            //EditorUtility.ClearProgressBar();
        }

    }
}