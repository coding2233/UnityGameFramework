//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #可操作物体的状态编辑类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点50分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
	[CustomEditor(typeof(OperationAssetStatus))]
	public class OperationAssetStatusEditor : UnityEditor.Editor
	{
		private OperationAssetStatus operationAssetStatus;
		private bool _adding = false;
		private string _addValue = "";
		private string _replaceValue = "";

		private void OnEnable()
		{
			operationAssetStatus = target as OperationAssetStatus;
		}

		public override void OnInspectorGUI()
		{
			GUILayout.BeginVertical("Box");

			for (int i = 0; i < operationAssetStatus._allStates.Count; i++)
			{
				GUILayout.BeginHorizontal("HelpBox");

				GUILayout.Label((i).ToString());
				_replaceValue = GUILayout.TextField(operationAssetStatus._allStates[i], GUILayout.Width(200));
				//normal  状态 不允许改变
				if (i > 0 && !_replaceValue.Equals(operationAssetStatus._allStates[i]))
				{
					operationAssetStatus.Replace(i, _replaceValue);
					//保存数据
					EditorUtility.SetDirty(operationAssetStatus);
					break;
				}
				GUILayout.EndHorizontal();
			}

			if (_adding)
			{
				Color defaultColor = GUI.backgroundColor;
				GUI.backgroundColor = Color.green; //new Color(1.0f, 0.5468f, 0.0f, 1.0f);
				GUILayout.BeginHorizontal("HelpBox");
				GUI.backgroundColor = defaultColor;

				GUILayout.Label((operationAssetStatus._allStates.Count + 1).ToString());
				_addValue = GUILayout.TextField(_addValue);

				if (GUILayout.Button("save", GUILayout.Width(40)))
				{
					if (!string.IsNullOrEmpty(_addValue))
					{
						operationAssetStatus.Add(_addValue);
						//保存数据
						EditorUtility.SetDirty(operationAssetStatus);
					}
					_adding = false;
				}
				if (GUILayout.Button("cancel", GUILayout.Width(50)))
				{
					_adding = false;
				}

				GUILayout.EndHorizontal();


			}

			GUILayout.BeginHorizontal();

			//填满
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("+", GUILayout.Width(24)))
			{
				_adding = true;
			}
			if (GUILayout.Button("-", GUILayout.Width(24)))
			{
				if (_adding)
					_adding = false;
				else
					operationAssetStatus.Remove();
			}

			GUILayout.EndHorizontal();

			GUILayout.EndVertical();


			//保存数据
			EditorUtility.SetDirty(operationAssetStatus);

		}

	}
}
