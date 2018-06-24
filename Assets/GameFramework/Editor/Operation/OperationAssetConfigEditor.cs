//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #可操作物体的配置类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点48分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
	[CustomEditor(typeof(OperationAssetConfig))]
	public class OperationAssetConfigEditor : UnityEditor.Editor
	{

		private OperationAssetConfig _operationAsset;
		private string[] _allStatNames = new string[] { };
		private readonly string _stateAssetConfig = "Assets/Resources/OperationAssetStatus.asset";

		private void OnEnable()
		{
			_operationAsset = target as OperationAssetConfig;

			if (Mathf.Abs(_operationAsset.OperationId) < 1000)
			{
				_operationAsset.OperationId = IdGenerater.GenerateId();
			}

			//_allStatNames = Enum.GetNames(typeof(OperationAssetStatus));
		}


		public override void OnInspectorGUI()
		{
			GUILayout.BeginVertical();

			//绘制状态设置界面
			DrawStateSettingGui();

			GUILayout.EndVertical();

			base.OnInspectorGUI();

			EditorUtility.SetDirty(_operationAsset);
		}



		void DrawStateSettingGui()
		{
			OperationAssetStatus ops = AssetDatabase.LoadAssetAtPath<OperationAssetStatus>(_stateAssetConfig);
			if (ops != null)
			{
				_allStatNames = ops._allStates.ToArray();
				_operationAsset.Status = EditorGUILayout.MaskField("State", _operationAsset.Status, _allStatNames);
			}
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("State");
				if (GUILayout.Button("Click Create OperationAssetStatus"))
				{
					string path = Application.dataPath + "/Resources";
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					OperationAssetStatus operationAssetStatus = ScriptableObject.CreateInstance<OperationAssetStatus>();
					AssetDatabase.CreateAsset(operationAssetStatus, _stateAssetConfig);
				}
				GUILayout.EndHorizontal();
			}

		}




	}
}
