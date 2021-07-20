using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class EditorFormView
    {
        struct FormHeaderInfo
        {
            public string Title;
            public float Width;
            public JsonType JsonType;
            public Action<JsonData,float> OnDrawItem;
        }

        private List<FormHeaderInfo> _formHeader=new List<FormHeaderInfo>();
        private JsonData _data;
		private JsonData _newData;
		private Vector2 _scrollView = Vector2.zero;

        private float _spacePixels = 15;
        private List<float> _verticalLineX = new List<float>();

        public EditorFormView(JsonData jsonData)
        {
            if (jsonData == null)
            {
                jsonData = new JsonData();
                jsonData.SetJsonType(JsonType.Array);
            }
			_newData = new JsonData();
			_data = jsonData;
        }

        public EditorFormView SetTitle(string title, float width, JsonType jsonType,Action<JsonData,float> onDrawItem)
        {
            _formHeader.Add(new FormHeaderInfo() { Title = title, Width = width, JsonType = jsonType, OnDrawItem=onDrawItem });
            return this;
        }


        public void OnDrawLayout()
        {
            if (_formHeader.Count == 0)
                return;
		

			_scrollView = GUILayout.BeginScrollView(_scrollView,"HelpBox");

			GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));
			_verticalLineX.Clear();
			foreach (var item in _formHeader)
			{
				GUILayout.Space(_spacePixels);
				GUILayout.Label(item.Title, GUILayout.Width(item.Width));
				GUILayout.Space(_spacePixels);
				Rect latRect = GUILayoutUtility.GetLastRect();
				_verticalLineX.Add(latRect.x + latRect.width);
				//GUILayout.Space(_spacePixels);
			}
			GUILayout.EndHorizontal();

			int configCount = _data.Count;
            if (configCount > 0)
            {
                for (int i = 0; i < configCount; i++)
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width));
					JsonData itemRowData = _data[i];
                    for (int j = 0; j < _formHeader.Count; j++)
                    {
						FormHeaderInfo itemHeader = _formHeader[j];
						JsonData itemData = null;
						if (itemRowData.ContainsKey(itemHeader.Title))
						{
							itemData = itemRowData[itemHeader.Title];
						}
						if (itemData == null)
						{
							itemData = new JsonData();
							itemData.SetJsonType(itemHeader.JsonType);
							itemRowData[itemHeader.Title] = itemData;
						}

						GUILayout.Space(_spacePixels);
						if (itemHeader.OnDrawItem != null)
						{
							itemHeader.OnDrawItem(itemData, itemHeader.Width);
						}
						else
						{
							DrawJsonItem(itemHeader, itemData);
						}
						GUILayout.Space(_spacePixels);
					}

					GUILayout.Space(_spacePixels);
					if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
						if (EditorUtility.DisplayDialog("Waring", "Are you sure to delete the data?", "Yes", "No"))
						{
							_data.Remove(_data[i]);
							GUIUtility.ExitGUI();
						}
                    }
                    GUILayout.EndHorizontal();
                }
            }


			//Add new data
			GUILayout.FlexibleSpace();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect.y += lastRect.height;
			lastRect.width = Screen.width;
			lastRect.height = 1.0f;
			EditorGUI.DrawRect(lastRect, Color.gray);
			//GUILayout.Space(_spacePixels);
			GUILayout.BeginHorizontal("HelpBox", GUILayout.Width(Screen.width));
			for (int j = 0; j < _formHeader.Count; j++)
			{
				FormHeaderInfo itemHeader = _formHeader[j];
				JsonData itemData = null;
				if (!_newData.ContainsKey(itemHeader.Title))
				{
					itemData = new JsonData();
					itemData.SetJsonType(itemHeader.JsonType);
					_newData[itemHeader.Title] = itemData;
				}
				else
				{
					itemData = _newData[itemHeader.Title];
				}

				GUILayout.Space(_spacePixels);
				if (itemHeader.OnDrawItem != null)
				{
					itemHeader.OnDrawItem(itemData, itemHeader.Width);
				}
				else
				{
					DrawJsonItem(itemHeader, itemData);
				}
				GUILayout.Space(_spacePixels);
			}

			
			GUILayout.Space(_spacePixels);
			if (GUILayout.Button("+", GUILayout.Width(30)))
			{
				_data.Add(_newData);
				_newData = new JsonData();
				//_data.Remove(_data[i]);
				//GUIUtility.ExitGUI();
			}
			GUILayout.EndHorizontal();

			//Draw Vertical Line
			foreach (var item in _verticalLineX)
			{
				Rect lineRect = new Rect(item, 0, 1, Screen.height);
				EditorGUI.DrawRect(lineRect, new Color(0.2196079f, 0.2196079f, 0.2196079f, 1.0f) * 1.5f);
			}
			GUILayout.EndScrollView();
		}

		private void DrawJsonItem(FormHeaderInfo header,JsonData jsonData)
		{
			IJsonWrapper jsonDataWrapper = jsonData as IJsonWrapper;

			var layoutOption = GUILayout.Width(header.Width);

			switch (jsonData.GetJsonType())
			{
				//case JsonType.Object:
				//	if (JsonDataObjectEditor(value))
				//	{
				//		updateValue = value;
				//	}
				//	break;
				//case JsonType.Array:
				//	if (JsonDataArrayEditor(value))
				//	{
				//		updateValue = value;
				//	}
				//	break;
				case JsonType.String:
					string stringValue = (string)jsonData;
					string newStringValue = EditorGUILayout.TextField(stringValue, layoutOption);
					if (stringValue != newStringValue)
					{
						jsonDataWrapper.SetString(newStringValue);
					}
					break;
				case JsonType.Int:
					int intValue = (int)jsonData;
					int newIntValue = EditorGUILayout.IntField(intValue, layoutOption);
					if (intValue != newIntValue)
					{
						jsonDataWrapper.SetInt(newIntValue);
					}
					break;
				case JsonType.Long:
					long longValue = (long)jsonData;
					long newLongValue = EditorGUILayout.LongField(longValue, layoutOption);
					if (newLongValue != longValue)
					{
						jsonDataWrapper.SetLong(newLongValue);
					}
					break;
				case JsonType.Double:
					double doubleValue = (double)jsonData;
					double newDubleValue = EditorGUILayout.DoubleField(doubleValue, layoutOption);
					if (doubleValue != newDubleValue)
					{
						jsonDataWrapper.SetDouble(newDubleValue);
					}
					break;
				case JsonType.Boolean:
					bool boolValue = (bool)jsonData;
					bool newboolValue = EditorGUILayout.Toggle(boolValue, layoutOption);
					if (boolValue != newboolValue)
					{
						jsonDataWrapper.SetBoolean(newboolValue);
					}
					break;

				default:
					break;
			}
		}

		////»æÖÆjsondata
		//private void DrawJsonData(FormHeaderInfo header,JsonData jsonData)
		//{
		//	string key = "AssetBundleName";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = "Game";
		//	}
		//	string content = jsonData[key].ToString();
		//	string newContent = EditorGUILayout.TextField(content, GUILayout.Width(150));
		//	if (!content.Equals(newContent))
		//	{
		//		jsonData[key] = newContent;
		//	}

		//	key = "Annotation";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = "";
		//	}
		//	content = jsonData[key].ToString();
		//	newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
		//	if (!content.Equals(newContent))
		//	{
		//		jsonData[key] = newContent;
		//	}

		//	key = "Variant";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = "";
		//	}
		//	content = jsonData[key].ToString();
		//	newContent = EditorGUILayout.TextField(content, GUILayout.Width(100));
		//	if (!content.Equals(newContent))
		//	{
		//		jsonData[key] = newContent;
		//	}

		//	key = "Filter";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = 1024;
		//	}
		//	int filter = (int)jsonData[key];
		//	int newFilter = EditorGUILayout.MaskField(filter, _assetFilter, GUILayout.Width(100));
		//	if (filter != newFilter)
		//	{
		//		jsonData[key] = newFilter;
		//	}

		//	key = "SearchInFolders";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = new JsonData();
		//		jsonData[key].SetJsonType(JsonType.Array);
		//	}
		//	JsonData selectFoldersJsonData = jsonData[key];
		//	content = selectFoldersJsonData.ToJson();
		//	newContent = EditorGUILayout.TextField(content, GUILayout.Width(300));
		//	if (!content.Equals(newContent))
		//	{
		//		var jd = JsonMapper.ToObject(newContent);
		//		if (jd != null)
		//		{
		//			jsonData[key] = jd;
		//		}
		//	}
		//	//if (selectFoldersJsonData != null && selectFoldersJsonData.Count > 0)
		//	//{
		//	//	buttonContent = selectFoldersJsonData[0].ToString();
		//	//	if (selectFoldersJsonData.Count > 1)
		//	//	{
		//	//		buttonContent = $"[{selectFoldersJsonData.Count}] {buttonContent}...";
		//	//	}
		//	//}
		//	//if (GUILayout.Button(buttonContent,GUILayout.Width(300))&&!_haspopupFoldersWindow)
		//	//{
		//	//	_foldersWindowRect = new Rect(Event.current.mousePosition, Vector2.one * 200);
		//	//	_selectFoldersJsonData = selectFoldersJsonData;
		//	//	if (_selectFoldersJsonData == null)
		//	//	{
		//	//		_selectFoldersJsonData = new JsonData();
		//	//		_selectFoldersJsonData.SetJsonType(JsonType.Array);
		//	//		jsonData[key] = _selectFoldersJsonData;
		//	//	}
		//	//}
		//	key = "Split";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = false;
		//	}
		//	bool isSplit = (bool)jsonData[key];
		//	bool newIsSplit = EditorGUILayout.Toggle(isSplit, GUILayout.Width(100));
		//	if (isSplit != newIsSplit && !_haspopupFoldersWindow)
		//	{
		//		jsonData[key] = newIsSplit;
		//	}

		//	key = "SplitCount";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = 1;
		//	}
		//	int splitCount = (int)jsonData[key];
		//	int newSplitCount = EditorGUILayout.IntField(splitCount, GUILayout.Width(100));
		//	if (splitCount != newSplitCount)
		//	{
		//		jsonData[key] = newSplitCount;
		//	}

		//	key = "ForceUpdate";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = true;
		//	}
		//	bool forceUpdate = (bool)jsonData[key];
		//	bool newForceUpdate = EditorGUILayout.Toggle(forceUpdate, GUILayout.Width(110));
		//	if (forceUpdate != newForceUpdate)
		//	{
		//		jsonData[key] = newForceUpdate;
		//	}

		//	key = "Preload";
		//	if (!jsonData.Keys.Contains(key))
		//	{
		//		jsonData[key] = false;
		//	}
		//	bool preloadUpdate = (bool)jsonData[key];
		//	bool newPreloadUpdate = EditorGUILayout.Toggle(preloadUpdate, GUILayout.Width(70));
		//	if (preloadUpdate != newPreloadUpdate)
		//	{
		//		jsonData[key] = newPreloadUpdate;
		//	}
		//}


	}
}
