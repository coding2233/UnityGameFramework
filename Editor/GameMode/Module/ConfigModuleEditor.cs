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
        string _addNewKey;
        JsonType _lastSelectJsonType;
        public ConfigModuleEditor(string name, Color mainColor, GameMode gameMode)
            : base(name, mainColor, gameMode)
        { 
            _addNewKey="NEWKEY";
            _lastSelectJsonType=JsonType.Int;
            //UpdateJsonData();
            _isExpand=_gameMode.ConfigAsset==null;
        }
      
        public override void OnDrawGUI()
        {
            TextAsset configAsset = (TextAsset)EditorGUILayout.ObjectField("Config Asset ",_gameMode.ConfigAsset,typeof(TextAsset),false);
            if(configAsset!=_gameMode.ConfigAsset)
            {
                _gameMode.ConfigAsset=configAsset;
                EditorUtility.SetDirty(_gameMode);
              //  UpdateJsonData();
            }

            if(_gameMode.ConfigJsonData==null)
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
                foreach (var item in _gameMode.ConfigJsonData.Keys)
                {
                    GUILayout.BeginHorizontal();
                    string key = EditorGUILayout.TextField(item,GUILayout.Width(150));
                    JsonData value = _gameMode.ConfigJsonData[item];
                    if(!string.IsNullOrEmpty(key)&&key!=item)
                    {
                        _gameMode.ConfigJsonData[key]=value;
                        _gameMode.ConfigJsonData.Remove(item);
                        SaveConfig();
                        GUIUtility.ExitGUI();
                        break;
                    }
                    JsonData updateValue = JsonDataEditor(value);
                  
                    //更新数据
                    if(updateValue!=null)
                    {
                        _gameMode.ConfigJsonData[item]=updateValue;
                        SaveConfig();
                        GUIUtility.ExitGUI();
                        break;
                    }
                    //删除数据
                    if(GUILayout.Button("x",GUILayout.Width(30)))
                    {
                        if(EditorUtility.DisplayDialog("Delete config data",$"[{item} : {value}]","OK","Cancel"))
                        {
                            _gameMode.ConfigJsonData.Remove(item);
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
                    if(!string.IsNullOrEmpty(_addNewKey)&&!_gameMode.ConfigJsonData.ContainsKey(_addNewKey))
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
                            _gameMode.ConfigJsonData[_addNewKey]=addNewValue;
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



        //JsonData编辑器
        private JsonData JsonDataEditor(JsonData value)
        {
            if(value==null)
                return null;
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
            return updateValue;
        }

        //创建默认数据
        private void CreateDefaultConfig()
        {
            if(_gameMode==null)
                return;
            string configPath =EditorUtility.SaveFilePanelInProject("Save config file","DefaultConfig","json","Default Config");
            if(!string.IsNullOrEmpty(configPath))
            {
                JsonData configJsonData=new JsonData();
                configJsonData["ResourceUpdateType"]=0;
                configJsonData["PathType"]=0;
                configJsonData["ResOfficialUpdatePath"]="";
                configJsonData["ResTestUpdatePath"]="";
                configJsonData["DefaultInStreamingAsset"]=true;
                configJsonData["DebugEnable"]=true;
                File.WriteAllText(configPath,configJsonData.ToJson());
                _gameMode.ConfigJsonData=configJsonData;
                AssetDatabase.Refresh();
                _gameMode.ConfigAsset=AssetDatabase.LoadAssetAtPath<TextAsset>(configPath);
                EditorUtility.SetDirty(_gameMode);
            }
        }

     

    }
}