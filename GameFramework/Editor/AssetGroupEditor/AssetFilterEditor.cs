using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Wanderer.GameFramework
{

    public class AssetFilterEditor : EditorWindow
    {
        private const string _configName = "AssetFilterEditor.json";
        private static JsonData _config;
        private Vector2 _scrollView = Vector2.zero;
        
        private static List<string> _listLabels = new List<string>(){"Object", "AnimationClip",
            "AudioClip","AudioMixer","ComputeShader","Font","GUISkin","Material",
            "Mesh","Model","PhysicMaterial","Prefab","Scene","Script","Shader",
            "Sprite","Texture","VideoClip","TextAsset","ScriptableObject",
            "AnimatorController","SpriteAtlas"};
        private string _newLabel = "";

        private ReorderableList _labelReorderableList;
        private bool _showAddItemWindow;

        [MenuItem("Tools/Asset Management/Asset Filter")]
        private static void OpenWindow()
        {
            GetWindow<AssetFilterEditor>(true, "Asset Filter Editor",true);
        }

        public static List<string> GetAssetFilters()
        {
            _config = ProjectSettingsConfig.LoadJsonData(_configName);
            if (_config != null && _config.Count > 0)
            {
                _listLabels.Clear();
                for (int i = 0; i < _config.Count; i++)
                {
                    string label = (string)_config[i];
                    if (!_listLabels.Contains(label))
                    {
                        _listLabels.Add(label);
                    }
                }
            }
            else
            {
                //AutomaticRefresh();

                _config = new JsonData();
                _config.SetJsonType(JsonType.Array);
            }
            return _listLabels;
        }

        private void OnEnable()
        {
            GetAssetFilters();
            SetReorderableList();
        }

        private void OnDisable()
        {
            _labelReorderableList = null;
        }

        private void SetReorderableList()
        {
            _labelReorderableList = new ReorderableList(_listLabels, typeof(string));
            _labelReorderableList.drawHeaderCallback = (rect) => { GUI.Label(rect, "Asset Filter"); };
            _labelReorderableList.drawElementCallback = (rect, index, isActive, isFocused) => {
                string label = _listLabels[index];
                GUI.Label(rect, label);
            };
            _labelReorderableList.onRemoveCallback = (list) => {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure to delete the current data?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    SaveConfig();
                }
            };
            _labelReorderableList.onAddCallback = (list) =>
            {
                _showAddItemWindow = true;
               
            };
            //_labelReorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) => { };
            //_labelReorderableList.elementHeightCallback = (index) => { return 80.0f; };
        }

        private static void AutomaticRefresh()
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                foreach (var itemType in item.GetTypes())
                {
                    if (!string.IsNullOrEmpty(itemType.Namespace))
                    {
                        if (itemType.Namespace.Contains("UnityEditor")
                            || itemType.Namespace.Contains("UnityEngine"))
                        {
                            continue;
                        }
                    }
                    if (itemType.BaseType == typeof(ScriptableObject))
                    {
                        string labelName = itemType.Name;
                        if (!_listLabels.Contains(labelName))
                            _listLabels.Add(labelName);
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (_config == null)
                return;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Automatic refresh of system type"))
            {
                AutomaticRefresh();
            }
            if (GUILayout.Button("Save Config"))
            {
                SaveConfig();
            }
            GUILayout.EndHorizontal();
            _scrollView = EditorGUILayout.BeginScrollView(_scrollView);
            //GUILayout.BeginVertical("HelpBox");
            _labelReorderableList?.DoLayoutList();

            if (_showAddItemWindow)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect.y += lastRect.height - 25;
                //lastRect.height = 60;
                
                Rect newRect = new Rect(lastRect.x, lastRect.y, lastRect.width, 70);
                EditorGUI.DrawRect(newRect,new Color(0.2196079f, 0.2196079f, 0.2196079f,1.0f));
                newRect.y += 5;
                newRect.height = 25;
                GUI.Label(newRect,"New Filter");
                newRect.x = 65;
                newRect.width -= 70;
                _newLabel = EditorGUI.TextField(newRect, _newLabel);
                newRect.width = lastRect.width*0.5f;
                newRect.y += 35;
                newRect.x = 0;
                if (GUI.Button(newRect, "Cancel"))
                {
                    _newLabel = "";
                    _showAddItemWindow = false;
                }
                newRect.x = newRect.width;
                if (GUI.Button(newRect, "Save"))
                {
                    if (!string.IsNullOrEmpty(_newLabel))
                    {
                        _newLabel = _newLabel.Trim();
                        if (!_listLabels.Contains(_newLabel))
                        {
                            _listLabels.Add(_newLabel);
                            SaveConfig();
                        }
                    }
                    _newLabel = "";
                    _showAddItemWindow = false;

                }
            }

            GUILayout.Space(100);
            //GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        private void SaveConfig()
        {
            _config.Clear();
            foreach (var item in _listLabels)
            {
                _config.Add(item);
            }
                ProjectSettingsConfig.SaveJsonData(_configName, _config);

        }

    }

}