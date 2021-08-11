using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Wanderer.GameFramework
{

    public class AssetGroupEditor:EditorWindow
    {
        private const string _configName = "AssetGroupEditor.json";
        public static string ConfigName => _configName;

        private JsonData _config;
  
        private Vector2 _scrollView = Vector2.zero;
		private string[] _assetFilter;
        private string[] _labels = new string[] { "none" };
        private List<string> _profileVariables = new List<string> { "LocalBuildPath","LocalLoadPath" };

        private EditorMenuItemView _menuItem;
        private EditorFormView _editorForm;

		[MenuItem("Tools/Asset Management/Asset Group #&G")]
        private static void OpenWindow()
        {
            GetWindow<AssetGroupEditor>("Asset Group Editor");
        }

        private void OnFocus()
        {
            _assetFilter = AssetFilterEditor.GetAssetFilters().ToArray();
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                _labels = settings.GetLabels().ToArray();
                _profileVariables = settings.profileSettings.GetVariableNames();
            }
        }

        private void OnEnable()
        {
            _config = ProjectSettingsConfig.LoadJsonData(_configName);
            if (_config == null)
            {
                _config = new JsonData();
                _config.SetJsonType(JsonType.Array);
            }

            //var settings = AddressableAssetSettingsDefaultObject.Settings;
            //if (settings != null)
            //{
            //    _labels = settings.GetLabels().ToArray();
            //    _profileVariables = settings.profileSettings.GetVariableNames();
            //}

            OnMenuInit();
            //_assetFilter = AssetFilterEditor.GetAssetFilters().ToArray();
			_editorForm = new EditorFormView(_config);
            OnFormInit();
		}

        private void OnMenuInit()
        {
            _menuItem = new EditorMenuItemView();
            _menuItem.SetMenuItem("File", new string[] { "Save Config","Set Addressables", "Exit" }, (itemIndex) => {
                if (itemIndex == 0)
                {
                    ProjectSettingsConfig.SaveJsonData(_configName, _config);
                    EditorUtility.DisplayDialog("Save Config", "Data saved successfully!", "OK");
                }
                else if (itemIndex == 1)
                {
                    AddressablesEditor.SetAddressablesAssets();
                    EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                    //var settings = AddressableAssetSettingsDefaultObject.Settings;
                    //if (settings != null)
                    //{
                    //    string binPath = ContentUpdateScript.GetContentStateDataPath(false);
                    //    if (_config != null && _config.Count > 0)
                    //    {
                    //        for (int i = 0; i < _config.Count; i++)
                    //        {
                    //            JsonData item = _config[i];
                    //            string groupName = item["GroupName"].ToString();
                    //            var group = settings.FindGroup(groupName);
                    //            if (group == null)
                    //            {
                    //                group = settings.CreateGroup(groupName,false,false,false,null);
                    //            }

                    //            ContentUpdateGroupSchema cugSchema = group.GetSchema<ContentUpdateGroupSchema>();
                    //            if (cugSchema == null)
                    //            {
                    //                cugSchema = group.AddSchema<ContentUpdateGroupSchema>();
                    //            }
                    //            cugSchema.StaticContent = ((int)item["UpdateRestriction"] == 1);
                    //            BundledAssetGroupSchema bagSchema =group.GetSchema<BundledAssetGroupSchema>();
                    //            if (bagSchema == null)
                    //            {
                    //                bagSchema= group.AddSchema<BundledAssetGroupSchema>();
                    //            }
                    //            bagSchema.BuildPath.SetVariableByName(settings, item["BuildPath"].ToString());
                    //            bagSchema.LoadPath.SetVariableByName(settings, item["LoadPath"].ToString());
                    //            if (cugSchema.StaticContent)
                    //            {
                    //                bagSchema.UseAssetBundleCrc = false;
                    //                bagSchema.UseAssetBundleCrcForCachedBundles = false;
                    //            }

                    //            //Filter
                    //            StringBuilder filterBuilder = new StringBuilder();
                    //            for (int filterIndex = 0; filterIndex < item["Filter"].Count; filterIndex++)
                    //            {
                    //                filterBuilder.Append($"t:{item["Filter"][filterIndex].ToString()} ");
                    //            }
                    //            //SearchInFolders
                    //            List<string> folders = new List<string>();
                    //            for (int folderIndex = 0; folderIndex < item["SearchInFolders"].Count; folderIndex++)
                    //            {
                    //                folders.Add(item["SearchInFolders"][folderIndex].ToString());
                    //            }
                    //            //Labels
                    //            List<string> labels = new List<string>();
                    //            for (int labelIndex = 0; labelIndex < item["Labels"].Count; labelIndex++)
                    //            {
                    //                labels.Add(item["Labels"][labelIndex].ToString());
                    //            }

                    //            //Find All Asset
                    //            var findAssets = AssetDatabase.FindAssets(filterBuilder.ToString(), folders.ToArray());
                    //            for (int findIndex = 0; findIndex < findAssets.Length; findIndex++)
                    //            {
                    //                string guid = findAssets[findIndex];
                    //                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    //                if (AssetDatabase.IsValidFolder(assetPath)|| assetPath.EndsWith(".cs"))
                    //                {
                    //                    continue;
                    //                }
                    //                var entry = group.GetAssetEntry(guid);
                    //                if (entry == null)
                    //                {
                    //                    entry = settings.CreateOrMoveEntry(guid, group);
                    //                }
                    //                entry.labels.Clear();
                    //                foreach (var itemLabel in labels)
                    //                {
                    //                    entry.SetLabel(itemLabel, true);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    EditorUtility.SetDirty(settings);
                    //    AssetDatabase.Refresh();

                    //    EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                    //}
                }
                else
                {
                    Close();
                }
            },50)
                .SetMenuItem("Tools",new string[] { "Filter Edit","Labels Edit","Addressables Edit" },(itemIndex) => 
                {
                    if (itemIndex == 0)
                    {
                        EditorApplication.ExecuteMenuItem("Tools/Asset Management/Asset Filter");
                    }
                    else if (itemIndex == 1|| itemIndex==2)
                    {
                        EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                    }
                });
        }

		private void OnFormInit()
		{
			_editorForm.SetTitle("GroupName", 150, JsonType.String, null)
				.SetTitle("Description", 100, JsonType.String, null)
				//.SetTitle("Variant", 100, JsonType.String, null)
				.SetTitle("Filter", 100, JsonType.Array, (jsonData,width)=> {
                    string buttonText = "None";
                    HashSet<string> selectFilter = new HashSet<string>();
                    if (jsonData!=null && jsonData.Count>0)
                    {
                        if (jsonData.Count == 1)
                        {
                            buttonText = jsonData[0].ToString();
                        }
                        else
                        {
                            buttonText = "Mixed...";
                        }
                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            selectFilter.Add(jsonData[i].ToString());
                        }
                    }
                    if (GUILayout.Button(buttonText, EditorStyles.toolbarDropDown,GUILayout.Width(width)))
                    {
                        GenericMenu gm = new GenericMenu();
                        for (int i = 0; i < _assetFilter.Length; i++)
                        {
                            string itemFilter = _assetFilter[i];
                            bool filterOn = selectFilter.Contains(itemFilter);
                            gm.AddItem(new GUIContent(itemFilter), filterOn, () => {
                                if (filterOn)
                                {
                                    jsonData.Remove(itemFilter);
                                    selectFilter.Remove(itemFilter);
                                }
                                else
                                {
                                    selectFilter.Add(itemFilter);
                                    jsonData.Add(itemFilter);
                                }
                            });
                        }
                        gm.ShowAsContext();
                    }
				})
				.SetTitle("SearchInFolders", 120, JsonType.Array, (jsonData, width) => {
                    string buttonText = "No folder selected";
                    if (jsonData != null && jsonData.Count > 0)
                    {
                        if (jsonData.Count == 1)
                        {
                            buttonText = jsonData[0].ToString();
                        }
                        else
                        {
                            buttonText = "Mixed...";
                        }
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
				})
                .SetTitle("Labels",100,JsonType.Array,(jsonData,width)=> {
                    string buttonText = "None";
                    HashSet<string> selectLabels = new HashSet<string>();
                    if (jsonData != null && jsonData.Count > 0)
                    {
                        if (jsonData.Count == 1)
                        {
                            buttonText = jsonData[0].ToString();
                        }
                        else
                        {
                            buttonText = "Mixed...";
                        }
                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            selectLabels.Add(jsonData[i].ToString());
                        }
                    }
                    if (GUILayout.Button(buttonText, EditorStyles.toolbarDropDown, GUILayout.Width(width)))
                    {
                        GenericMenu gm = new GenericMenu();
                        for (int i = 0; i < _labels.Length; i++)
                        {
                            string itemLabel = _labels[i];
                            bool labelOn = selectLabels.Contains(itemLabel);
                            gm.AddItem(new GUIContent(itemLabel), labelOn, () => {
                                if (labelOn)
                                {
                                    jsonData.Remove(itemLabel);
                                    selectLabels.Remove(itemLabel);
                                }
                                else
                                {
                                    selectLabels.Add(itemLabel);
                                    jsonData.Add(itemLabel);
                                }
                            });
                        }
                        gm.ShowAsContext();
                    }

                })
                .SetTitle("BuildPath",150,JsonType.String,(jsonData,width)=> {
                    if (jsonData == null)
                    {
                        jsonData = new JsonData("RemoteBuildPath");
                    }
                    string buildPath =jsonData.ToString();
                    int buildPathIndex = _profileVariables.IndexOf(buildPath);
                    int newBuildPathIndex = EditorGUILayout.Popup(buildPathIndex, _profileVariables.ToArray(),GUILayout.Width(width));
                    if (newBuildPathIndex != buildPathIndex)
                    {
                        buildPath = _profileVariables[newBuildPathIndex];
                        (jsonData as IJsonWrapper).SetString(buildPath);
                    }
                })
                .SetTitle("LoadPath", 150, JsonType.String, (jsonData, width) => {
                    if (jsonData == null)
                    {
                        jsonData = new JsonData("RemoteLoadPath");
                    }
                    string loadPath = jsonData.ToString();
                    int loadPathIndex = _profileVariables.IndexOf(loadPath);
                    int newLoadPathIndex = EditorGUILayout.Popup(loadPathIndex, _profileVariables.ToArray(),GUILayout.Width(width));
                    if (newLoadPathIndex != loadPathIndex)
                    {
                        loadPath = _profileVariables[newLoadPathIndex];
                        (jsonData as IJsonWrapper).SetString(loadPath);
                    }
                })
                .SetTitle("UpdateRestriction",200,JsonType.Int,(jsonData,width)=> {
                    int updateType = (int)jsonData;
                    int newUpdateType =EditorGUILayout.Popup(updateType,new string[] {"Can Change Post Release", "Cannot Change Post Release" },GUILayout.Width(width));
                    if (newUpdateType != updateType)
                    {
                        (jsonData as IJsonWrapper).SetInt(newUpdateType);
                    }
                })
                ;
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