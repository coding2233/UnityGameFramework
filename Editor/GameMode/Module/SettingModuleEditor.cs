//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #设置模块编辑器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年12月15日 17点27分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wanderer.GameFramework
{
    [CustomModuleEditor("Settings Module",0.2f,0.8f,0.6f)]
    public class SettingModuleEditor : ModuleEditorBase
    {
        public SettingModuleEditor(string name, Color mainColor, GameMode gameMode)
    : base(name, mainColor, gameMode)
        { 
            
        }

        public override void OnDrawGUI()
        {
            GUILayout.BeginVertical("HelpBox");
           
            GUI.color = _gameMode.DebugEnable ? Color.white : Color.gray;
            // bool debugEnable = GUILayout.Toggle(GameMode.Setting.DebugEnable, "Debug Enable");
            // if (debugEnable != GameMode.Setting.DebugEnable)
            // {
            //     GameMode.Setting.DebugEnable = debugEnable;
            //     EditorUtility.SetDirty(_gameMode);
            // }
            // bool debugEnable = GUILayout.Toggle(_gameMode.DebugEnable, "Debug Enable");
            // if (debugEnable != _gameMode.DebugEnable)
            // {
            //     _gameMode.DebugEnable = debugEnable;
            //     EditorUtility.SetDirty(_gameMode);
            // }
            GUI.color = Color.white;

            GUILayout.EndVertical();
        }

        public override void OnClose()
        {
        }
    }
}