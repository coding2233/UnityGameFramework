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
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
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

        //保存配置文件
        protected virtual void SaveConfig()
        {
            if(_gameMode==null||_gameMode.ConfigAsset==null)
                return;
            if(_gameMode.ConfigJsonData==null)
                return;
            string configPath = AssetDatabase.GetAssetPath(_gameMode.ConfigAsset);
            File.WriteAllText(configPath,_gameMode.ConfigJsonData.ToJson());
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(_gameMode);
        }

        //检测是否有对应的key
        protected virtual void CheckConfig(string key,object value)
        {
            if(!_gameMode.ConfigJsonData.ContainsKey(key))
            {
                 _gameMode.ConfigJsonData[key]=new JsonData(value);
                SaveConfig();
            }
        }

        //无配置文件错误提示
        protected virtual bool NoConfigError()
        {
            bool result = false;
            if(_gameMode==null||_gameMode.ConfigJsonData==null)
            {
                EditorGUILayout.HelpBox("No config file!", MessageType.Error); 
                result=true;
            }
            return result;
        }
    }

}