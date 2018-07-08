//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #GameMode的编辑器类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 12点23分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;

namespace GameFramework.Taurus
{
    [CustomEditor(typeof(GameMode))]
    public class GameModeEditor : Editor
    {
        private GameMode _gameMode;

        private bool _resourceModule = true;
        private bool _operationModule = true;
        private bool _stateModule = true;
        private bool _dataTableModule = true;
        private bool _nodeDataModule = true;
        private bool _stepModule = true;

        ////Color.cyan;
        private Color _defaultColor;

        //资源加载模块的颜色
        private Color _resourceColor = new Color(0.851f, 0.227f, 0.286f, 1.0f);

        //可操作模块的颜色
        private Color _operationColor = new Color(0.953f, 0.424f, 0.129f, 1.0f);

        //状态模块的颜色
        private Color _stateColor = new Color(0.141f, 0.408f, 0.635f, 1.0f);

        //配置表模块的颜色
        private Color _dataTableColor = new Color(0.989f, 0.686f, 0.090f, 1.0f);

        //数据节点模块的颜色
        private Color _nodeDataColor = new Color(0.435f, 0.376f, 0.667f, 1.0f);

        //步骤模块的颜色
        private Color _stepColor = new Color(0.439f, 0.631f, 0.624f, 1.0f);

        //所有状态
        private List<string> _listState;

        //所有可操作的物体
        private Dictionary<long, OperationAssetConfig> _operationAssets;

        //选中物体的ID
        private long _selectOperationId;

        //选中的物体
        private GameObject _selectOperationObject;

        private void OnEnable()
        {
            _gameMode = target as GameMode;

            _defaultColor = GUI.color;

            #region 获取当前状态

            _listState = new List<string>();
            Type[] types = typeof(GameMode).Assembly.GetTypes();
            foreach (var item in types)
            {
                object[] attribute = item.GetCustomAttributes(typeof(GameStateAttribute), false);
                if (attribute.Length <= 0 || item.IsAbstract)
                    continue;
                GameStateAttribute stateAttribute = (GameStateAttribute) attribute[0];
                //if (stateAttribute.StateType == VirtualStateType.Ignore)
                //    continue;
                object obj = Activator.CreateInstance(item);
                GameState gs = obj as GameState;
                if (gs != null)
                    _listState.Add("[" + stateAttribute.StateType.ToString() + "]\t" + item.FullName);
            }

            #endregion
			
            #region 获取所有可操作的物体

            _operationAssets = new Dictionary<long, OperationAssetConfig>();
            OperationAssetConfig[] assetConfig = GameObject.FindObjectsOfType<OperationAssetConfig>();
            foreach (var item in assetConfig)
            {
                if (!_operationAssets.ContainsKey(item.OperationId))
                    _operationAssets.Add(item.OperationId, item);
                else
                {
                    long oldId = item.OperationId;
                    item.OperationId = IdGenerater.GenerateId();
                    _operationAssets.Add(item.OperationId, item);
                    Debug.Log("场景中存在两个Id一样的物体!!" + oldId + "  new Id:" + item.OperationId);
                }
            }

            #endregion
        }


        public override void OnInspectorGUI()
        {
            if (_gameMode == null)
                return;
            GUILayout.BeginVertical();

            #region 资源加载模块

            GUI.color = _resourceColor;
            GUILayout.BeginVertical("Box");
            GUI.color = _defaultColor;
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            _resourceModule = EditorGUILayout.Foldout(_resourceModule, "Resource Module", true);
            GUILayout.EndHorizontal();
            if (_resourceModule)
                DrawEditorModeGUI();
            GUILayout.EndVertical();

            #endregion

            #region 可操作物体模块

            GUI.color = _operationColor;
            GUILayout.BeginVertical("Box");
            GUI.color = _defaultColor;
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            _operationModule = EditorGUILayout.Foldout(_operationModule, "Operation Module", true);
            GUILayout.EndHorizontal();
            if (_operationModule)
                DrawOperationGUI();
            GUILayout.EndVertical();

            #endregion

            #region 状态模块

            GUI.color = _stateColor;
            GUILayout.BeginVertical("Box");
            GUI.color = _defaultColor;
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            _stateModule = EditorGUILayout.Foldout(_stateModule, "State Module", true);
            GUILayout.EndHorizontal();
            if (_stateModule)
                DrawStateGUI();
            GUILayout.EndVertical();

            #endregion

            #region 配置表模块

            GUI.color = _dataTableColor;
            GUILayout.BeginVertical("Box");
            GUI.color = _defaultColor;
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            _dataTableModule = EditorGUILayout.Foldout(_dataTableModule, "DataTable Module", true);
            if (!EditorApplication.isPlaying)
                _dataTableModule = EditorApplication.isPlaying;
            GUILayout.EndHorizontal();
            if (_dataTableModule)
                DrawDataTableGUI();
            GUILayout.EndVertical();

            #endregion

            #region 数据节点模块

            GUI.color = _nodeDataColor;
            GUILayout.BeginVertical("Box");
            GUI.color = _defaultColor;
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            _nodeDataModule = EditorGUILayout.Foldout(_nodeDataModule, "NodeData Module", true);
            if (!EditorApplication.isPlaying)
                _nodeDataModule = EditorApplication.isPlaying;
            GUILayout.EndHorizontal();
            if (_nodeDataModule)
                DrawNodeDataGUI();
            GUILayout.EndVertical();

            #endregion

            #region 步骤模块

            GUI.color = _stepColor;
            GUILayout.BeginVertical("Box");
            GUI.color = _defaultColor;
            GUILayout.BeginHorizontal();
            GUILayout.Space(12);
            _stepModule = EditorGUILayout.Foldout(_stepModule, "Step Module", true);
            if (!EditorApplication.isPlaying)
                _stepModule = EditorApplication.isPlaying;
            GUILayout.EndHorizontal();
            if (_stepModule)
                DrawStepGUI();
            GUILayout.EndVertical();

            #endregion

            GUILayout.EndVertical();
        }


        //绘制 当前 资源加载的界面
        void DrawEditorModeGUI()
        {
            GUILayout.BeginVertical("HelpBox");

			//  _gameMode.IsEditorMode = EditorGUILayout.Toggle("Editor Mode", _gameMode.IsEditorMode);
			_gameMode.ResUpdateType =
				(ResourceUpdateType)EditorGUILayout.EnumPopup("Resource Update Type", _gameMode.ResUpdateType);
			if (_gameMode.ResUpdateType!=ResourceUpdateType.Editor)
            {
	    //        _gameMode.ResUpdateType =
					//(ResourceUpdateType)EditorGUILayout.EnumPopup("Resource Update Type", _gameMode.ResUpdateType);
	            if (_gameMode.ResUpdateType == ResourceUpdateType.Update)
	            {
		            _gameMode.ResUpdatePath =
			            EditorGUILayout.TextField("Resource Update Path", _gameMode.ResUpdatePath);
		            _gameMode.LocalPath =
			            (PathType)EditorGUILayout.EnumPopup("Local Path Type", PathType.ReadWrite);
				}
	            else
	            {
		            _gameMode.LocalPath =
			            (PathType)EditorGUILayout.EnumPopup("Local Path Type", _gameMode.LocalPath);
				}
				string path = "";
				switch (_gameMode.LocalPath)
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
				_gameMode.AssetBundleName =
                    EditorGUILayout.TextField("AssetBundle Name", _gameMode.AssetBundleName);
            }

            GUILayout.EndVertical();
        }

        //绘制可操作界面
        void DrawOperationGUI()
        {
            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Count");
            GUILayout.Label(_operationAssets.Count.ToString());
            GUILayout.EndHorizontal();
            //Object
            GUILayout.BeginHorizontal();
            GUILayout.Label("Find");
            long selectOperationId = EditorGUILayout.LongField(_selectOperationId);
            if (_selectOperationId != selectOperationId)
            {
                OperationAssetConfig operationAsset;
                if (_operationAssets.TryGetValue(selectOperationId, out operationAsset))
                {
                    // Selection.activeGameObject = operationAsset.gameObject;
                    _selectOperationId = selectOperationId;
                    _selectOperationObject = operationAsset.gameObject;
                }
            }

            EditorGUILayout.ObjectField(_selectOperationObject, typeof(OperationAssetConfig), true);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        //绘制状态界面
        void DrawStateGUI()
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


        //绘制配置表界面
        void DrawDataTableGUI()
        {
            //GUILayout.BeginVertical("HelpBox");

            //if (EditorApplication.isPlaying)
            //{
            //    foreach (var item in GameMode.DataTable.AllDataTables.Values)
            //    {
            //        GUILayout.Label(item.GetType().FullName);
            //    }
            //}

            //GUILayout.EndVertical();
        }

        //绘制数据节点界面
        void DrawNodeDataGUI()
        {
            if (!EditorApplication.isPlaying)
                return;

            GUILayout.BeginVertical("HelpBox");
            //int
            // if (VrCoreEntity.Node.IntNodes.Count > 0)
            {
                GUILayout.Label("Int Node", EditorStyles.boldLabel);
                foreach (var item in GameMode.Node.IntNodes)
                {
                    GUILayout.Label(item.Key + ":" + item.Value);
                }
            }
            //float
            // if (VrCoreEntity.Node.FloatNodes.Count > 0)
            {
                GUILayout.Label("Float Node", EditorStyles.boldLabel);
                foreach (var item in GameMode.Node.FloatNodes)
                {
                    GUILayout.Label(item.Key + ":" + item.Value);
                }
            }
            //bool
            // if (VrCoreEntity.Node.BoolNodes.Count > 0)
            {
                GUILayout.Label("Bool Node", EditorStyles.boldLabel);
                foreach (var item in GameMode.Node.BoolNodes)
                {
                    GUILayout.Label(item.Key + ":" + item.Value);
                }
            }
            //string
            //  if (VrCoreEntity.Node.StringNodes.Count > 0)
            {
                GUILayout.Label("String Node", EditorStyles.boldLabel);
                foreach (var item in GameMode.Node.StringNodes)
                {
                    GUILayout.Label(item.Key + ":" + item.Value);
                }
            }
            //object
            //if (VrCoreEntity.Node.ObjectNodes.Count > 0)
            {
                GUILayout.Label("Float Node", EditorStyles.boldLabel);
                foreach (var item in GameMode.Node.ObjectNodes)
                {
                    GUILayout.Label(item.Key + ":" + item.Value);
                }
            }

            GUILayout.EndVertical();
        }

        //绘制步骤界面
        void DrawStepGUI()
        {
            //if (GameMode.Step.AllStepControllers == null || GameMode.Step.AllStepControllers.Count == 0)
            //    return;

            //GUILayout.BeginVertical("HelpBox");

            //foreach (var item in GameMode.Step.AllStepControllers)
            //{
            //    GUILayout.BeginHorizontal();
            //    GUI.enabled = item.Value.IsPlaying;
            //    GUI.color = item.Value.IsPlaying ? Color.green : _defaultColor;
            //    GUILayout.Label("", GUI.skin.GetStyle("Icon.Keyframe"));
            //    GUI.color = _defaultColor;
            //    GUI.enabled = true;
            //    GUILayout.Label(item.Key);

            //    float width = Screen.width / 3.0f > 200.0f ? 200 : Screen.width / 3.0f;
            //    Rect rect = GUILayoutUtility.GetRect(width, 18);
            //    EditorGUI.ProgressBar(rect, (item.Value.CurrentStepIndex + 1.0f) / (float) item.Value.StepCount,
            //        (item.Value.CurrentStepIndex + 1.0f) + "/" + item.Value.StepCount);
            //    GUILayout.EndHorizontal();
            //}

            //GUILayout.EndVertical();
        }
    }
}
