//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using System.Reflection;
//using LitJson;
//using OdinSerializer;

//namespace Wanderer.GameFramework
//{
//    public class StandardAssetEditor : EditorWindow
//    {
//        private const string _configName = "StandardAssetEditor.json";
//        private JsonData _config;
//        private Vector2 _scrollView = Vector2.zero;

//        private Dictionary<StandardAssetBase, bool> _allStandardAssets;

//        [MenuItem("Tools/Asset Management/Standard Asset #&K")]
//        private static void OpenWindow()
//        {
//            GetWindow<StandardAssetEditor>("Standard Asset Editor");
//        }

//        private void OnEnable()
//        {
//            _config = ProjectSettingsConfig.LoadJsonData(_configName);
//            if (_config == null)
//            {
//                _config = new JsonData();
//            }

//            _allStandardAssets = new Dictionary<StandardAssetBase, bool>() { { new TextureStandardAsset(), true } };
//            foreach (var item in _allStandardAssets.Keys)
//            {
//                AssetImporter assetImporter = null;
//                if (_config.ContainsKey(item.Name))
//                {
//                    JsonData importItem = _config[item.Name];
//                    if (importItem.ContainsKey("AssetImporter"))
//                    {
//                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(importItem["AssetImporter"].ToString());
//                        assetImporter = SerializationUtility.DeserializeValue<AssetImporter>(buffer, DataFormat.Binary);
//                    }
//                }
//                else
//                {
//                    _config[item.Name] = new JsonData();
//                }
//                item.Importer = assetImporter;
//            }
//        }


//        private void OnGUI()
//        {
//            if (_allStandardAssets == null
//                || _allStandardAssets.Count <= 0)
//                return;
//            if (GUILayout.Button("Save", GUILayout.Width(100)))
//            {
//                foreach (var item in _allStandardAssets.Keys)
//                {
//                    JsonData importItem = _config[item.Name];
//                    byte[] buffer = SerializationUtility.SerializeValue(item.Importer, DataFormat.Binary);
//                    importItem["AssetImporter"] = System.Text.Encoding.UTF8.GetString(buffer);
//                }

//                ProjectSettingsConfig.SaveJsonData(_configName, _config);
//                AssetDatabase.Refresh();
//                EditorUtility.DisplayDialog("", "Save data.", "ok");
//            }
//            _scrollView = GUILayout.BeginScrollView(_scrollView);
//            foreach (var item in _allStandardAssets)
//            {
//                bool foldout = EditorGUILayout.Foldout(item.Value, item.Key.Name);
//                if (foldout != item.Value)
//                {
//                    _allStandardAssets[item.Key] = foldout;
//                    GUIUtility.ExitGUI();
//                    break;
//                }
//                if (foldout)
//                {
//                    GUILayout.BeginVertical("HelpBox");
//                    item.Key.OnDraw();
//                    GUILayout.EndVertical();
//                }
//            }
//            GUILayout.EndScrollView();
//        }
//    }

//    abstract class StandardAssetBase
//    {
//        public virtual string Name
//        {
//            get
//            {
//                return GetType().Name;
//            }
//        }

//        public virtual AssetImporter Importer { get; set; }

//        protected FieldInfo[] _filedInfos;
//        protected PropertyInfo[] _propertyInfos;


//        public abstract void OnDraw();
//    }

//    class TextureStandardAsset : StandardAssetBase
//    {
//        private TextureImporter _textureImporter;
//        public override AssetImporter Importer
//        {
//            get
//            {
//                return _textureImporter;
//            }
//            set
//            {
//                if (value == null)
//                {
//                    var template = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/UnityGameFramework/GameFramework/Editor/AssetManagement/StandardAsset/TextureImporter.bytes");
//                    _textureImporter = SerializationUtility.DeserializeValue<TextureImporter>(template.bytes, DataFormat.Binary);
//                }
//                else
//                {
//                    _textureImporter = value as TextureImporter;
//                }
//            }
//        }

//        public override void OnDraw()
//        {
//            GUILayout.Button("XXX");

//            if (_textureImporter == null)
//                return;
//            GUILayout.Button("XXX");
//            _textureImporter.textureType = (TextureImporterType)EditorGUILayout.EnumPopup(_textureImporter.textureType);
//        }
//    }
//}