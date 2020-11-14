using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class AssetBundleEditor : EditorWindow
	{
		private const string _configName = "AssetBundleEditor.json";
		private JsonData _config;
		private JsonData _newData;
		private JsonData _selectFoldersJsonData;

		private Vector2 _scrollView = Vector2.zero;

		public static string[] AssetFilter = new string[] { "AnimationClip",
			"AudioClip",
			"AudioMixer",
			"ComputeShader",
			"Font",
			"GUISkin",
			"Material",
			"Mesh",
			"Model",
			"PhysicMaterial",
			"Prefab",
			"Scene",
			"Script",
			"Shader",
			"Sprite",
			"Texture",
			"VideoClip","TextAsset","ScriptableObject","AnimatorController"};

		//窗口的位置
		private Rect _foldersWindowRect = Rect.zero;
		private Vector2 _foldersWindowScrollView = Vector2.zero;
		private string _newFolderName = "Assets/";


		[MenuItem("Tools/Asset Bundle/Asset Bundle Editor")]
		private static void MainWindow()
		{
			GetWindowWithRect<AssetBundleEditor>(new Rect(100, 100, 900, 600), false, "Asset Bundle Editor");
		}


		/// <summary>
		/// 获取所有的AssetBundleBuild
		/// </summary>
		/// <returns></returns>
		public static AssetBundleBuild[] GetAssetBundleBuild()
		{
			JsonData config = ProjectSettingsConfig.LoadJsonData(_configName);
			if (config == null || config.Count == 0)
			{
				if (EditorUtility.DisplayDialog("Waring!", "No data!", "OK"))
				{
					MainWindow();
				}

			}
			else
			{
				List<AssetBundleBuild> abbs = new List<AssetBundleBuild>();
				for (int i = 0; i < config.Count; i++)
				{
					JsonData item = config[i];
					abbs.AddRange(JsonToABB(item));
				}
				return abbs.ToArray();
			}
			return null;
		}

		/// <summary>
		/// JsonData转AssetBundleBuild
		/// </summary>
		/// <param name="jsonData"></param>
		/// <returns></returns>
		private static List<AssetBundleBuild> JsonToABB(JsonData jsonData)
		{
			List<AssetBundleBuild> listABB = new List<AssetBundleBuild>();
			//AssetBundleBuild abb = new AssetBundleBuild();
			string abName = (string)jsonData["AssetBundleName"];
			string abVariant = (string)jsonData["Variant"];
			//Filter
			int jsonFilter = (int)jsonData["Filter"];
			string filter = "";
			StringBuilder filterBuilder = new StringBuilder();
			if (jsonFilter == -1)
			{
				foreach (var item in AssetFilter)
				{
					filterBuilder.Append($"t:{item} ");
				}
			}
			else
			{
				for (int i = 0; i < AssetFilter.Length; i++)
				{
					int byteIndex = 1 << i;
					if ((jsonFilter & byteIndex) == byteIndex)
					{
						filterBuilder.Append($"t:{AssetFilter[i]} ");
					}
				}
			}
			filter = filterBuilder.ToString().Trim();
			//SearchInFolders
			JsonData sifJsonData = jsonData["SearchInFolders"];
			string[] searchInFolders = new string[sifJsonData.Count];
			for (int i = 0; i < sifJsonData.Count; i++)
			{
				searchInFolders[i] = (string)sifJsonData[i];
			}
			//获取到所有的资源
			string[] assets = AssetDatabase.FindAssets(filter, searchInFolders);
			//Split
			bool split = (bool)jsonData["Split"];
			int splitCount=(int)jsonData["SplitCount"];

			if (split && assets.Length > 0 && splitCount > 0 && assets.Length >= splitCount)
			{
				int splitNum = assets.Length / splitCount + 1;
				int index = 0;
				for (int i = 0; i < splitCount; i++)
				{
					string partAbName = $"{abName}_part{i}";
					List<string> assetNames = new List<string>();
					int tempNum = index + splitNum;
					tempNum = Mathf.Min(tempNum, assets.Length);

					for (int j = index; j < tempNum; j++)
					{
						string guid = assets[j];
						assetNames.Add(AssetDatabase.GUIDToAssetPath(guid));
					}
					index = tempNum;

					AssetBundleBuild abb = new AssetBundleBuild();
					abb.assetBundleName = partAbName;
					abb.assetBundleVariant = abVariant;
					abb.assetNames = assetNames.ToArray();
					listABB.Add(abb);
				}
			}
			else
			{
				List<string> assetNames = new List<string>();
				for (int j = 0; j < assets.Length; j++)
				{
					string guid = assets[j];
					assetNames.Add(AssetDatabase.GUIDToAssetPath(guid));
				}

				AssetBundleBuild abb = new AssetBundleBuild();
				abb.assetBundleName = abName;
				abb.assetBundleVariant = abVariant;
				abb.assetNames = assetNames.ToArray();
				listABB.Add(abb);
			}

			return listABB;
			//abb.assetNames = (string)jsonData["Variant"];
			//return abb;
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

		}

		private void OnDisable()
		{
			_config = null;
			_newData = null;
			_selectFoldersJsonData = null;
		}

		private void OnGUI()
		{
			//拦截事件
			if (_foldersWindowRect != Rect.zero)
			{
				if (Event.current.type == EventType.MouseDown)
				{
					if (!_foldersWindowRect.Contains(Event.current.mousePosition))
					{
						Event.current.Use();
					}
				}
			}

			//主窗口
			_scrollView = GUILayout.BeginScrollView(_scrollView);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Apply", GUILayout.Width(100)))
			{
				ProjectSettingsConfig.SaveJsonData(_configName, _config);
				EditorUtility.DisplayDialog("Save Config", "Data saved successfully!", "OK");
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("box",GUILayout.Width(Screen.width));//GUILayout.Width(100)
			GUILayout.Label("AssetBundleName", GUILayout.Width(150));
			GUILayout.Label("Variant", GUILayout.Width(100));
			GUILayout.Label("Filter", GUILayout.Width(100));
			GUILayout.Label("SearchInFolders",GUILayout.Width(300));
			GUILayout.Label("Split",GUILayout.Width(100));
			GUILayout.Label("SplitCount", GUILayout.Width(100));
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
			GUILayout.EndScrollView();

			//绘制文件夹的窗口
			if (_foldersWindowRect != Rect.zero)
			{
				GUILayout.BeginArea(_foldersWindowRect, "Folders Window", "window");
				_foldersWindowScrollView=GUILayout.BeginScrollView(_foldersWindowScrollView);
				int folderCount = _selectFoldersJsonData.Count;
				if (folderCount > 0)
				{
					for (int i = 0; i < folderCount; i++)
					{
						GUILayout.BeginHorizontal();
						string folderName = (string)_selectFoldersJsonData[i];
						string newFolderName = EditorGUILayout.TextField(folderName);
						if (!string.IsNullOrEmpty(newFolderName) && !newFolderName.Equals(folderName))
						{
							_selectFoldersJsonData[i]=newFolderName;
						}
						if (GUILayout.Button("-", GUILayout.Width(30)))
						{
							_selectFoldersJsonData.Remove(_selectFoldersJsonData[i]);
							GUIUtility.ExitGUI();
						}
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.BeginHorizontal();
				_newFolderName = EditorGUILayout.TextField(_newFolderName);
				if (GUILayout.Button("+", GUILayout.Width(30)))
				{
					if (!string.IsNullOrEmpty(_newFolderName))
					{
						_selectFoldersJsonData.Add(_newFolderName);
						//_selectFoldersJsonData[folderCount.ToString()] = _newFolderName;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.EndScrollView();
				if (GUILayout.Button("X"))
				{
					_foldersWindowRect = Rect.zero;
				}
				GUILayout.EndArea();
			}
		}

		//绘制jsondata
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

			key = "Variant";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = "";
			}
			content =jsonData[key].ToString();
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
			int newFilter = EditorGUILayout.MaskField(filter, AssetFilter, GUILayout.Width(100));
			if (filter != newFilter)
			{
				jsonData[key] = newFilter;
			}

			key = "SearchInFolders";
			_selectFoldersJsonData = jsonData.Keys.Contains(key) ? jsonData[key] : null;
			string buttonContent = "No data";
			if (_selectFoldersJsonData != null && _selectFoldersJsonData.Count > 0)
			{
				buttonContent = _selectFoldersJsonData[0].ToString();
				if (_selectFoldersJsonData.Count > 1)
				{
					buttonContent = $"[{_selectFoldersJsonData.Count}] {buttonContent}...";
				}
			}
			if (GUILayout.Button(buttonContent,GUILayout.Width(300)))
			{
				_foldersWindowRect = new Rect(Event.current.mousePosition, Vector2.one * 200);
				if (_selectFoldersJsonData == null)
				{
					_selectFoldersJsonData = new JsonData();
					_selectFoldersJsonData.SetJsonType(JsonType.Array);
					jsonData[key] = _selectFoldersJsonData;
				}
			}
			key = "Split";
			if (!jsonData.Keys.Contains(key))
			{
				jsonData[key] = false;
			}
			bool isSplit = (bool)jsonData[key] ;
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
			int splitCount =(int)jsonData[key];
			int newSplitCount = EditorGUILayout.IntField(splitCount, GUILayout.Width(70));
			if (splitCount != newSplitCount)
			{
				jsonData[key] = newSplitCount;
			}
		}
	}

}