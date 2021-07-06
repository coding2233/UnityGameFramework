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
        private HashSet<string> _defineSymbols;
        private const string TEST = "TEST";

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
                            if (item.Equals("TEST"))
                            {
                                _selectType = 0;
                            }
                        }
					}
                }
            }
        }

        public override void OnDrawGUI()
        {
            GUILayout.BeginVertical("HelpBox");
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
                _lastScriptingDefineSymbols = "";
				foreach (var item in _defineSymbols)
				{
                    _lastScriptingDefineSymbols = $"{_lastScriptingDefineSymbols}{item};";
                }
                
                _lastBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_lastBuildTargetGroup, _lastScriptingDefineSymbols);
                _selectType = selectType;

            }
            //检查配置文件
            if (!NoConfigError())
            {
                GUILayout.BeginHorizontal();
                
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
    }
}