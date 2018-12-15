//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #状态模块编辑器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年12月15日 17点29分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace GameFramework.Taurus
{
	public class StateModuleEditor : ModuleEditorBase
	{
		//所有状态
		private List<string> _listState;

		public StateModuleEditor(string name, Color mainColor, GameMode gameMode)
		   : base(name, mainColor, gameMode)
		{
			_listState = new List<string>();
			Type[] types = typeof(GameMode).Assembly.GetTypes();
			foreach (var item in types)
			{
				object[] attribute = item.GetCustomAttributes(typeof(GameStateAttribute), false);
				if (attribute.Length <= 0 || item.IsAbstract)
					continue;
				GameStateAttribute stateAttribute = (GameStateAttribute)attribute[0];
				//if (stateAttribute.StateType == VirtualStateType.Ignore)
				//    continue;
				object obj = Activator.CreateInstance(item);
				GameState gs = obj as GameState;
				if (gs != null)
					_listState.Add("[" + stateAttribute.StateType.ToString() + "]\t" + item.FullName);
			}
		}


		public override void OnDrawGUI()
		{
			GUILayout.BeginVertical("HelpBox");

			foreach (var item in _listState)
			{
				//正在运行
				if (EditorApplication.isPlaying)
				{
					string runName = "";
					if (GameMode.State.CurrentState != null)
						runName = GameMode.State.CurrentState.GetType().Name;
					if (item.Contains(runName))
					{
						GUILayout.BeginHorizontal();
						GUI.color = Color.green;
						GUILayout.Label("", GUI.skin.GetStyle("Icon.ExtrapolationContinue"));
						GUI.color = _defaultColor;
						GUILayout.Label(item);
						GUILayout.FlexibleSpace();
						GUILayout.Label((Profiler.GetMonoUsedSizeLong() / 1000000.0f).ToString("f3"));
						GUILayout.EndHorizontal();

						continue;
					}
				}
				//默认状态
				GUI.enabled = false;
				GUILayout.BeginHorizontal();
				GUILayout.Label("", GUI.skin.GetStyle("Icon.ExtrapolationContinue"));
				GUILayout.Label(item);
				GUILayout.EndHorizontal();
				GUI.enabled = true;
			}

			GUILayout.EndVertical();
		}


		public override void OnClose()
		{
			_listState.Clear();
		}

	}
}