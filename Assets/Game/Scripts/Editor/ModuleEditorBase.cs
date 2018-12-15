//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #模块编辑器的基础类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年12月15日 17点21分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
	public abstract class ModuleEditorBase
	{
		#region property
		//名称
		protected string _name;
		//主颜色
		protected Color _mainColor;
		//默认颜色
		protected Color _defaultColor;
		//主类
		protected GameMode _gameMode;
		//展开
		protected bool _isExpand;
		#endregion

		public ModuleEditorBase(string name, Color mainColor, GameMode gameMode)
		{
			_name = name;
			_mainColor = mainColor;
			_defaultColor = GUI.color;
			_gameMode = gameMode;
			_isExpand = true;
		}


		//默认绘制界面
		public virtual void OnInspectorGUI()
		{
			GUI.color = _mainColor;
			GUILayout.BeginVertical("Box");
			GUI.color = _defaultColor;
			GUILayout.BeginHorizontal();
			GUILayout.Space(12);
			_isExpand = EditorGUILayout.Foldout(_isExpand, _name, true);
			GUILayout.EndHorizontal();
			if (_isExpand)
				OnDrawGUI();
			GUILayout.EndVertical();
		}

		//绘制界面
		public abstract void OnDrawGUI();

		//关闭界面
		public abstract void OnClose();

	}

}