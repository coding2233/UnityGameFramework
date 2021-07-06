//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #GameMode的编辑器类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月25日 12点23分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using System.Reflection;
using System.Linq;

namespace Wanderer.GameFramework
{
    [CustomEditor(typeof(GameMode))]
    public class GameModeEditor : Editor
    {
        private GameMode _gameMode;

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

        //调试模块的颜色
        private Color _debugColor = new Color(1f, 0.100f, 0.888f, 1.0f);

        //所有的模块
        private List<ModuleEditorBase> _listModuleEditors;

        private void OnEnable()
        {
            _listModuleEditors = new List<ModuleEditorBase>();

            _gameMode = target as GameMode;

            _defaultColor = GUI.color;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //从unity编译管线获取到当前所有的程序集的信息
            UnityEditor.Compilation.Assembly[] unityAssemblys = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
            for (int i = 0; i < unityAssemblys.Length; i++)
            {
                string assemblyName = unityAssemblys[i].name;
                Assembly assembly = assemblies.Where(x => x.GetName().Name.Equals(assemblyName)).ElementAt(0);
                foreach (var item in assembly.GetTypes())
                {
                    CustomModuleEditor attar = item.GetCustomAttribute<CustomModuleEditor>();
                    if (attar != null && item.BaseType == typeof(ModuleEditorBase))
                    {
                        ModuleEditorBase module = Activator.CreateInstance(item, attar.Name, attar.Color, _gameMode) as ModuleEditorBase;
                        _listModuleEditors.Add(module);
                    }
                }
            }
        }

        private void OnDisable()
        {
            if (_listModuleEditors == null)
                return;

            for (int i = 0; i < _listModuleEditors.Count; i++)
            {
                _listModuleEditors[i].OnClose();
            }
            _listModuleEditors.Clear();
        }

        public override void OnInspectorGUI()
        {
            if (_gameMode == null || _listModuleEditors == null)
                return;

            GUILayout.BeginVertical();

            for (int i = 0; i < _listModuleEditors.Count; i++)
            {
                _listModuleEditors[i].OnInspectorGUI();
            }

            GUILayout.EndVertical();

            //EditorUtility.SetDirty(_gameMode);
        }


    }
}
