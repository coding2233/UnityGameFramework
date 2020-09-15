using UnityEngine;
using UnityEditor;
using LitJson;
using System.IO;
using System;

namespace Wanderer.GameFramework
{
    [CustomModuleEditor("Config Module",0.4f,0.3f,0.9f)]
    public class ConfigModuleEditor : ModuleEditorBase
    {
        JsonData _configJsonData;
        string _addNewKey;
        JsonType _lastSelectJsonType;
        public ConfigModuleEditor(string name, Color mainColor, GameMode gameMode)
            : base(name, mainColor, gameMode)
        { 
            _addNewKey="NEWKEY";
            _lastSelectJsonType=JsonType.Int;
            UpdateJsonData();
            _isExpand=_configJsonData==null;
        }
      
        public override void OnDrawGUI()
        {
            TextAsset configAsset = (TextAsset)EditorGUILayout.ObjectField("Config Asset ",_gameMode.ConfigAsset,typeof(TextAsset),false);
            if(configAsset!=_gameMode.ConfigAsset)
            {
                _gameMode.ConfigAsset=configAsset;
                EditorUtility.SetDirty(_gameMode);
                UpdateJsonData();
            }

            if(_configJsonData==null)
            {
                GUI.color=Color.red;
                if(GUILayout.Button("Create default config file"))
                {
                    CreateDefaultConfig();
                }
                GUI.color=_defaultColor;
            }
            else
            {
                GUILayout.BeginVertical("HelpBox");
                foreach (var item in _configJsonData.Keys)
                {
                    GUILayout.BeginHorizontal();
                    string key = EditorGUILayout.TextField(item,GUILayout.Width(150));
                    JsonData value = _configJsonData[item];
                    if(!string.IsNullOrEmpty(key)&&key!=item)
                    {
                        _configJsonData[key]=value;
                        _configJsonData.Remove(item);
                        SaveConfig();
                        GUIUtility.ExitGUI();
                        break;
                    }
                    JsonData updateValue = null;
                    switch(value.GetJsonType())
                    {
                        case JsonType.Int:
                            int intValue = (int)value;
                            int newIntValue = EditorGUILayout.IntField(intValue);
                            if(intValue!=newIntValue)
                            {
                                updateValue=new JsonData(newIntValue);
                            }
                            break;
                         case JsonType.Double:
                            double doubleValue = (double)value;
                            double newDubleValue = EditorGUILayout.DoubleField(doubleValue);
                            if(doubleValue!=newDubleValue)
                            {
                                updateValue=new JsonData(newDubleValue);
                            }
                            break;
                        case JsonType.Boolean:
                            bool boolValue = (bool)value;
                            bool newboolValue = EditorGUILayout.Toggle(boolValue);
                             if(boolValue!=newboolValue)
                            {
                                updateValue=new JsonData(newboolValue);
                            }
                            break;
                         case JsonType.String:
                            string stringValue = (string)value;
                            string newStringValue = EditorGUILayout.TextField(stringValue);
                             if(stringValue!=newStringValue)
                            {
                                updateValue=new JsonData(newStringValue);
                            }
                            break;
                        default:
                            break;
                    }
                    //更新数据
                    if(updateValue!=null)
                    {
                        _configJsonData[item]=updateValue;
                        SaveConfig();
                        GUIUtility.ExitGUI();
                        break;
                    }
                    //删除数据
                    if(GUILayout.Button("x",GUILayout.Width(30)))
                    {
                        if(EditorUtility.DisplayDialog("Delete config data",$"[{item} : {value}]","OK","Cancel"))
                        {
                            _configJsonData.Remove(item);
                            SaveConfig();
                            GUIUtility.ExitGUI();
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                _addNewKey= GUILayout.TextField(_addNewKey,GUILayout.Width(150));
                _lastSelectJsonType= (JsonType)EditorGUILayout.EnumPopup(_lastSelectJsonType);
                if(GUILayout.Button("+",GUILayout.Width(30)))
                {
                    if(!string.IsNullOrEmpty(_addNewKey)&&!_configJsonData.ContainsKey(_addNewKey))
                    {
                        JsonData addNewValue=null;
                        switch (_lastSelectJsonType)
                        {
                            case JsonType.Int:
                                addNewValue=new JsonData(0);
                                break;
                            case JsonType.Boolean:
                                addNewValue=new JsonData(false);
                                break;
                            case JsonType.String:
                                addNewValue=new JsonData("");
                                break;
                            case JsonType.Double:
                                addNewValue=new JsonData(0.0f);
                                break;
                            default:
                                addNewValue=new JsonData("");
                                break;
                        }
                        if(addNewValue!=null)
                        {
                            _configJsonData[_addNewKey]=addNewValue;
                            SaveConfig();
                            GUIUtility.ExitGUI();
                        }
                    }
                }
                GUILayout.EndHorizontal();
 
                GUILayout.EndVertical();
            }
          
        }

        public override void OnClose()
        {
        }


        private void UpdateJsonData()
        {
            _configJsonData=null;
            if(_gameMode!=null&&_gameMode.ConfigAsset!=null)
            {
                _configJsonData=JsonMapper.ToObject(_gameMode.ConfigAsset.text);
            }
        }

        // private JsonData JsonDataEditor(JsonData value)
        // {
        //     JsonData updateValue = null;
        //     switch(value.GetJsonType())
        //     {
        //         case JsonType.Int:
        //             int intValue = (int)value;
        //             int newIntValue = EditorGUILayout.IntField(intValue);
        //             if(intValue!=newIntValue)
        //             {
        //                 updateValue=new JsonData(newIntValue);
        //             }
        //             break;
        //         case JsonType.Boolean:
        //             bool boolValue = (bool)value;
        //             bool newboolValue = EditorGUILayout.Toggle(boolValue);
        //             if(boolValue!=newboolValue)
        //             {
        //                 updateValue=new JsonData(newboolValue);
        //             }
        //             break;
        //         case JsonType.String:
        //             string stringValue = (string)value;
        //             string newStringValue = EditorGUILayout.TextField(stringValue);
        //             if(stringValue!=newStringValue)
        //             {
        //                 updateValue=new JsonData(newStringValue);
        //             }
        //             break;
        //         default:
        //             break;
        //     }
        //     return updateValue;
        // }

        //创建默认数据
        private void CreateDefaultConfig()
        {
            if(_gameMode==null)
                return;
            string configPath =EditorUtility.SaveFilePanelInProject("Save config file","DefaultConfig","json","Default Config");
            if(!string.IsNullOrEmpty(configPath))
            {
                _configJsonData=new JsonData();
                _configJsonData["ResourceUpdateType"]=0;
                _configJsonData["PathType"]=0;
                _configJsonData["ResOfficialUpdatePath"]="";
                _configJsonData["ResTestUpdatePath"]="";
                _configJsonData["DefaultInStreamingAsset"]=true;
                _configJsonData["DebugEnable"]=true;
                File.WriteAllText(configPath,_configJsonData.ToJson());
                AssetDatabase.Refresh();
                _gameMode.ConfigAsset=AssetDatabase.LoadAssetAtPath<TextAsset>(configPath);
                EditorUtility.SetDirty(_gameMode);
            }
        }

        //保存配置文件
        private void SaveConfig()
        {
            if(_configJsonData==null||_gameMode==null||_gameMode.ConfigAsset==null)
                return;
            string configPath = AssetDatabase.GetAssetPath(_gameMode.ConfigAsset);
            File.WriteAllText(configPath,_configJsonData.ToJson());
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(_gameMode);
        }

    }
}