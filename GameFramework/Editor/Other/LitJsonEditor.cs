using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class LitJsonEditor
    {
        private string _title;
        private JsonData _rootJson;
        private bool _isExpand;
        private string _newkey;
        private JsonType _newJsonType;
        private JsonType _newArrayJsonType;
        public JsonData GetJson => _rootJson;

        public LitJsonEditor(string title, JsonData rootJson, bool defautExpand = true)
        {
            _title = title;
            _rootJson = rootJson;
            _isExpand = defautExpand;
        }

        public bool OnDraw()
        {
            if (_rootJson == null)
            {
                return false;
            }

            _isExpand = EditorGUILayout.Foldout(_isExpand, _title, true);
            if (_isExpand)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("save", GUILayout.Width(60)))
                {
                    return true;
                }
                GUILayout.EndHorizontal();

                return JsonDataObjectEditor(_rootJson);
            }
            
            return false;
        }


        //JsonData Object±à¼­Æ÷
        private bool JsonDataObjectEditor(JsonData rootJson)
        {
            GUILayout.BeginVertical("Helpbox");
            foreach (string key in rootJson.Keys)
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(key, GUILayout.Width(150));
                var jsonValue = rootJson[key];
                var updateJsonData = JsonDataEditor(jsonValue);
                if (updateJsonData != null)
                {
                    rootJson[key] = updateJsonData;
                    GUIUtility.ExitGUI();
                    return true;
                }
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    if (EditorUtility.DisplayDialog("Remove elements", $"Are you sure to remove the element with key {key}?", "ok", "cancel"))
                    {
                        rootJson.Remove(key);
                        GUIUtility.ExitGUI();
                        return true;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal("Box");

            _newkey = EditorGUILayout.TextField(_newkey, GUILayout.Width(150));
            _newJsonType = (JsonType)EditorGUILayout.EnumPopup(_newJsonType);
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                string newKey = _newkey;
                if (!string.IsNullOrEmpty(newKey) && !rootJson.ContainsKey(newKey) && _newJsonType!=JsonType.None)
                {
                    JsonData addNewValue = null;
                    switch (_newJsonType)
                    {
                        case JsonType.Object:
                            addNewValue = new JsonData();
                            break;
                        case JsonType.Array:
                            addNewValue = new JsonData();
                            break;
                        case JsonType.String:
                            addNewValue = new JsonData("");
                            break;
                        case JsonType.Int:
                            addNewValue = new JsonData(0);
                            break;
                        case JsonType.Long:
                            addNewValue = new JsonData((long)0);
                            break;
                        case JsonType.Double:
                            addNewValue = new JsonData(0.0f);
                            break;
                        case JsonType.Boolean:
                            addNewValue = new JsonData(false);
                            break;
                        default:
                            addNewValue = new JsonData();
                            break;
                    }
                    if (addNewValue != null)
                    {
                        if (rootJson.GetJsonType() == JsonType.Object)
                        {
                            addNewValue.SetJsonType(_newJsonType);
                            rootJson[newKey] = addNewValue;
                            GUIUtility.ExitGUI();
                            return true;
                        }
                        //else if (rootJson.GetJsonType() == JsonType.Array)
                        //{
                        //    rootJson.Add(addNewValue);
                        //    GUIUtility.ExitGUI();
                        //}
                        else
                        {
                            EditorUtility.DisplayDialog("Fail to add", "Object is not supported for adding data of unexpected types.", "ok");
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Fail to add", "It may have the same Key value or the value may be NULL or JsonType is None.", "ok");
                }
            }
            //var valueJsonData = JsonDataEditor(_newJson);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            return false;
        }

        //JsonData Êý¾Ý±à¼­Æ÷
        private bool JsonDataArrayEditor(JsonData rootJson)
        {
            GUILayout.BeginVertical("Helpbox");
            if (rootJson.Count > 0)
            {
                JsonType arrayJsonType = rootJson[0].GetJsonType();
                for (int i = 0; i < rootJson.Count; i++)
                {
                    GUILayout.BeginHorizontal("Box");
                    JsonDataEditor(rootJson[i]);
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal("Box");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    JsonData newItem = new JsonData();
                    newItem.SetJsonType(_newArrayJsonType);
                    rootJson.Add(newItem);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label("Set Array Type", GUILayout.Width(150));
                _newArrayJsonType = (JsonType)EditorGUILayout.EnumPopup(_newArrayJsonType);
                if (GUILayout.Button("ok", GUILayout.Width(30)))
                {
                    JsonData newItem = new JsonData();
                    newItem.SetJsonType(_newArrayJsonType);
                    rootJson.Add(newItem);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            return false;
        }

        //JsonData±à¼­Æ÷
        private JsonData JsonDataEditor(JsonData value)
        {
            if (value == null)
                return false;
            JsonData updateValue = null;
            switch (value.GetJsonType())
            {
                case JsonType.Object:
                    if (JsonDataObjectEditor(value))
                    {
                        updateValue = value;
                    }
                    break;
                case JsonType.Array:
                    if (JsonDataArrayEditor(value))
                    {
                        updateValue = value;
                    }
                    break;
                case JsonType.String:
                    string stringValue = (string)value;
                    string newStringValue = EditorGUILayout.TextField(stringValue);
                    if (stringValue != newStringValue)
                    {
                        updateValue = new JsonData(newStringValue);
                    }
                    break;
                case JsonType.Int:
                    int intValue = (int)value;
                    int newIntValue = EditorGUILayout.IntField(intValue);
                    if (intValue != newIntValue)
                    {
                        updateValue = new JsonData(newIntValue);
                    }
                    break;
                case JsonType.Long:
                    long longValue = (long)value;
                    long newLongValue = EditorGUILayout.LongField(longValue);
                    if (newLongValue != longValue)
                    {
                        updateValue = new JsonData(newLongValue);
                    }
                    break;
                case JsonType.Double:
                    double doubleValue = (double)value;
                    double newDubleValue = EditorGUILayout.DoubleField(doubleValue);
                    if (doubleValue != newDubleValue)
                    {
                        updateValue = new JsonData(newDubleValue);
                    }
                    break;
                case JsonType.Boolean:
                    bool boolValue = (bool)value;
                    bool newboolValue = EditorGUILayout.Toggle(boolValue);
                    if (boolValue != newboolValue)
                    {
                        updateValue = new JsonData(newboolValue);
                    }
                    break;

                default:
                    break;
            }
            return updateValue;
        }

    }
}
