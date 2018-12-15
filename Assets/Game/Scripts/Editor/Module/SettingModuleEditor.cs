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

namespace GameFramework.Taurus
{
	public class SettingModuleEditor : ModuleEditorBase
	{
		public SettingModuleEditor(string name, Color mainColor, GameMode gameMode)
	: base(name, mainColor, gameMode)
		{ }

		public override void OnDrawGUI()
		{
			GUILayout.BeginVertical("HelpBox");

			GUI.color = _gameMode.DebugEnable ? Color.white : Color.gray;
			_gameMode.DebugEnable = GUILayout.Toggle(_gameMode.DebugEnable, "Debug Enable");
			GUI.color = Color.white;

			GUILayout.EndVertical();
		}

		public override void OnClose()
		{
		}
	}
}