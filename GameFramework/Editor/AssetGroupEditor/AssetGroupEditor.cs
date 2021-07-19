using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class AssetGroupEditor:EditorWindow
    {
        private const string _configName = "AssetGroupEditor.json";
        private JsonData _config;
        private JsonData _newData;
        private JsonData _selectFoldersJsonData;
        private Vector2 _scrollView = Vector2.zero;
		private string[] _assetFilter;


		[MenuItem("Tools/Assets Management/Asset Group #&G")]
        private static void OpenWindow()
        {
            GetWindow<AssetGroupEditor>("Asset Group Editor");
        }

        private void OnEnable()
        {
            _config = ProjectSettingsConfig.LoadJsonData(_configName);
            if (_config == null)
            {
                _config = new JsonData();
                _config.SetJsonType(JsonType.Array);
            }
            _newData = new JsonData();

			_assetFilter = AssetFilterEditor.GetAssetFilters().ToArray();
		}

        private void OnDisable()
        {
            _config = null;
            _newData = null;
        }
		List<int> linexs = new List<int>();

        private void OnGUI()
        {
			linexs.Clear();

			//÷˜¥∞ø⁄
			GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                ProjectSettingsConfig.SaveJsonData(_configName, _config);
                EditorUtility.DisplayDialog("Save Config", "Data saved successfully!", "OK");
            }
            GUILayout.EndHorizontal();

            _scrollView = GUILayout.BeginScrollView(_scrollView);
         
            GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));//GUILayout.Width(100)
            GUILayout.Label("Group Name", GUILayout.Width(150));
			Rect latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x+ latRect.width));

            GUILayout.Label("Annotation", GUILayout.Width(100));
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.Label("Variant", GUILayout.Width(100));
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.Label("Filter", GUILayout.Width(100));
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.Label("Search In Folders", GUILayout.Width(300));
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.Label("Split", GUILayout.Width(100));
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.Label("Split Count", GUILayout.Width(100));
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.Label("Force Update", GUILayout.Width(100));
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.Label("Preload");
			latRect = GUILayoutUtility.GetLastRect();
			linexs.Add((int)(latRect.x + latRect.width));

			GUILayout.EndHorizontal();

            int configCount = _config.Count;
            if (configCount > 0)
            {
                for (int i = 0; i < configCount; i++)
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));
                    DrawJsonData(_config[i]);
                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        _config.Remove(_config[i]);
                        GUIUtility.ExitGUI();
                    }
                    GUILayout.EndHorizontal();
                }
            }

			foreach (var item in linexs)
			{
				Rect lineRect = new Rect(item, 0, 1, Screen.height);
				EditorGUI.DrawRect(lineRect, new Color(0.2196079f, 0.2196079f, 0.2196079f, 1.0f)*1.5f);
			}

			GUILayout.EndScrollView();


			GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));
			DrawJsonData(_newData);
			if (GUILayout.Button("+", GUILayout.Width(30)))
			{
				bool addResult = false;
				if (_newData.Keys.Count > 0)
				{
					JsonData selectFoldersJsonData = _newData["SearchInFolders"];
					if (selectFoldersJsonData != null && selectFoldersJsonData.Count > 0)
					{
						_config.Add(_newData);
						_newData = new JsonData();
						addResult = true;
					}
				}

				if (!addResult)
				{
					EditorUtility.DisplayDialog("Tips", "The search folder cannot be empty!", "OK");
				}

			}
			GUILayout.EndHorizontal();

      

		}

		//ªÊ÷∆jsondata
		private void DrawJsonData(JsonData jsonData)
		{
			string key = "AssetBundleName";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = "Game";
			}
			string content = jsonData[key].ToString();
			string newContent = EditorGUILayout.TextField(content, GUILayout.Width(150));
			if (!content.Equals(newContent))
			{
				jsonData[key] = newContent;
			}

			key = "Annotation";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = "";
			}
			content = jsonData[key].ToString();
			newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
			if (!content.Equals(newContent))
			{
				jsonData[key] = newContent;
			}

			key = "Variant";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = "";
			}
			content = jsonData[key].ToString();
			newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
			if (!content.Equals(newContent))
			{
				jsonData[key] = newContent;
			}

			key = "Filter";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = 1024;
			}
			int filter = (int)jsonData[key];
			int newFilter = EditorGUILayout.MaskField(filter, _assetFilter, GUILayout.Width(100));
			if (filter != newFilter)
			{
				jsonData[key] = newFilter;
			}

			key = "SearchInFolders";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = new JsonData();
				jsonData[key].SetJsonType(JsonType.Array);
			}
			JsonData selectFoldersJsonData = jsonData[key];
			content = selectFoldersJsonData.ToJson();
			newContent = EditorGUILayout.TextField(content, GUILayout.Width(300));
			if (!content.Equals(newContent))
			{
				var jd = JsonMapper.ToObject(newContent);
				if (jd != null)
				{
					jsonData[key] = jd;
				}
			}
			key = "Split";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = false;
			}
			bool isSplit = (bool)jsonData[key];
			bool newIsSplit = EditorGUILayout.Toggle(isSplit, GUILayout.Width(100));
			if (isSplit != newIsSplit)
			{
				jsonData[key] = newIsSplit;
			}

			key = "SplitCount";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = 1;
			}
			int splitCount = (int)jsonData[key];
			int newSplitCount = EditorGUILayout.IntField(splitCount, GUILayout.Width(100));
			if (splitCount != newSplitCount)
			{
				jsonData[key] = newSplitCount;
			}

			key = "ForceUpdate";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = true;
			}
			bool forceUpdate = (bool)jsonData[key];
			bool newForceUpdate = EditorGUILayout.Toggle(forceUpdate, GUILayout.Width(110));
			if (forceUpdate != newForceUpdate)
			{
				jsonData[key] = newForceUpdate;
			}

			key = "Preload";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = false;
			}
			bool preloadUpdate = (bool)jsonData[key];
			bool newPreloadUpdate = EditorGUILayout.Toggle(preloadUpdate, GUILayout.Width(70));
			if (preloadUpdate != newPreloadUpdate)
			{
				jsonData[key] = newPreloadUpdate;
			}
		}


	}

}