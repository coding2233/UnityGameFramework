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
  
        private Vector2 _scrollView = Vector2.zero;
		private string[] _assetFilter;

        private EditorMenuItemView _menuItem;
        private EditorFormView _editorForm;

		[MenuItem("Tools/Assets Management/Asset Group #&G")]
        private static void OpenWindow()
        {
            GetWindow<AssetGroupEditor>("Asset Group Editor");
        }

        private void OnFocus()
        {
            _assetFilter = AssetFilterEditor.GetAssetFilters().ToArray();
        }

        private void OnEnable()
        {
            _config = ProjectSettingsConfig.LoadJsonData(_configName);
            if (_config == null)
            {
                _config = new JsonData();
                _config.SetJsonType(JsonType.Array);
            }

            OnMenuInit();
            _assetFilter = AssetFilterEditor.GetAssetFilters().ToArray();
			_editorForm = new EditorFormView(_config);
            OnFormInit();
		}

        private void OnMenuInit()
        {
            _menuItem = new EditorMenuItemView();
            _menuItem.SetMenuItem("File", new string[] { "Save Config", "Exit" }, (itemIndex) => {
                if (itemIndex==0)
                {
                    ProjectSettingsConfig.SaveJsonData(_configName, _config);
                    EditorUtility.DisplayDialog("Save Config", "Data saved successfully!", "OK");
                }
            },50)
                .SetMenuItem("Tools",new string[] { "Filter Edit" },(itemIndex) => { EditorApplication.ExecuteMenuItem("Tools/Assets Management/Asset Filter"); });
        }

		private void OnFormInit()
		{
			_editorForm.SetTitle("GroupName", 150, JsonType.String, null)
				.SetTitle("Description", 100, JsonType.String, null)
				//.SetTitle("Variant", 100, JsonType.String, null)
				.SetTitle("Filter", 100, JsonType.Int, (jsonData,width)=> {
					int filter = (int)jsonData;
					int newFilter = EditorGUILayout.MaskField(filter, _assetFilter, GUILayout.Width(100));
					if (filter != newFilter)
					{
						(jsonData as IJsonWrapper).SetInt(newFilter);
					}
				})
				.SetTitle("SearchInFolders", 120, JsonType.Array, (jsonData, width) => {
                    string buttonText = "No folder selected";
                    if (jsonData != null && jsonData.Count > 0)
                    {
                        buttonText = $"[{jsonData.Count}]{jsonData[0].ToString()}";
                        if (buttonText.Length > 13)
                        {
                            buttonText = buttonText.Substring(0, 13);
                        }
                        buttonText += "...|...";
                    }
                    if (GUILayout.Button(buttonText, EditorStyles.toolbarDropDown, GUILayout.Width(width)))
                    {
                        List<string> folders = new List<string>();
                        if (jsonData != null && jsonData.Count > 0)
                        {
                            for (int i = 0; i < jsonData.Count; i++)
                            {
                                folders.Add(jsonData[i].ToString());
                            }
                        }
                        SelectFoldersEditor.OpenWindow(folders.ToArray(),(folders)=> {
                            jsonData.Clear();
                            foreach (var item in folders)
                            {
                                jsonData.Add(item);
                            }
                        });
                    }
				});
		}

        private void OnDisable()
        {
            _config = null;
            _editorForm = null;
        }


        private void OnGUI()
        {
            _menuItem.OnDrawLayout();
            _editorForm.OnDrawLayout();
		}

	}

}