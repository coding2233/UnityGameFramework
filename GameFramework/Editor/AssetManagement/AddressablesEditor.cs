using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets;
using System.Text;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using LitJson;

namespace Wanderer.GameFramework
{
    public class AddressablesEditor
    {
        //资源管理插件
        public static IAssetGroupPlugin AssetGroupPlugin;

        public static string ShellBuild(string activeProfileId = "Default")
        {
            bool buildPlayerContent = true;
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null && settings.BuildRemoteCatalog)
            {
                var profileId = settings.profileSettings.GetProfileId(activeProfileId);
                settings.activeProfileId = profileId;
                string binPath = ContentUpdateScript.GetContentStateDataPath(false);
                if (File.Exists(binPath))
                {
                    //Debug.Log($"binPath: {binPath}");
                    buildPlayerContent = false;

                    //Check update
                    var entries = ContentUpdateScript.GatherModifiedEntries(settings, binPath);
                    if (entries.Count > 0)
                    {
                        StringBuilder @string = new StringBuilder();
                        foreach (var item in entries)
                        {
                            @string.AppendLine(item.address);
                        }
                        //�����޸Ĺ�����Դ��������
                        var groupName = string.Format("UpdateGroup_{0}#", System.DateTime.Now.ToString("yyyyMMdd"));
                        ContentUpdateScript.CreateContentUpdateGroup(settings, entries, groupName);
                        var group= settings.FindGroup(groupName);
                        BundledAssetGroupSchema bagSchema = group.GetSchema<BundledAssetGroupSchema>();
                        if (bagSchema == null)
                        {
                            bagSchema = group.AddSchema<BundledAssetGroupSchema>();
                        }
                        var defultBAGSchema = settings.DefaultGroup.GetSchema<BundledAssetGroupSchema>();
                        
                        bagSchema.BuildPath.SetVariableByName(settings, defultBAGSchema.BuildPath.GetValue(settings));
                        bagSchema.LoadPath.SetVariableByName(settings, defultBAGSchema.LoadPath.GetValue(settings));
                        Debug.Log($"Update content:{@string}");
                        EditorUtility.SetDirty(settings);
                        AssetDatabase.Refresh();
                    }

                    //Build update
                    ContentUpdateScript.BuildContentUpdate(settings, binPath);
                }
            }
            if (buildPlayerContent)
            {
                AddressableAssetSettings.BuildPlayerContent();
            }

            AssetDatabase.Refresh();

            if (settings != null && settings.BuildRemoteCatalog)
                return settings.RemoteCatalogBuildPath.GetValue(settings);

            return "";
        }


        [MenuItem("Tools/Asset Management/Addressables Player Content")]
        private static void BuildPlayerContent()
        {
            ShellBuildPlayerContent();
        }

        public static void ShellBuildPlayerContent(string activeProfileId = "Default")
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                var profileId = settings.profileSettings.GetProfileId(activeProfileId);
                settings.activeProfileId = profileId;

                AddressableAssetSettings.CleanPlayerContent(settings.ActivePlayerDataBuilder);
            }
            AddressableAssetSettings.BuildPlayerContent();
            EditorUtility.SetDirty(settings);
            AssetDatabase.Refresh();
            //BuildPlayerWindow.RegisterBuildPlayerHandler
            //BuildPlayerWindow.DefaultBuildMethods.BuildPlayer();
        }

        /// <summary>
        /// ����Addressable��������Դ
        /// </summary>
        [MenuItem("Tools/Asset Management/Assets To Addressables")]
        public static void SetAddressablesAssets()
        {
            if(AssetGroupPlugin!=null)
            {
                AssetGroupPlugin.MakeAssetGroup();
                AssetDatabase.Refresh();
            }

            JsonData config = ProjectSettingsConfig.LoadJsonData(AssetGroupEditor.ConfigName);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                string binPath = ContentUpdateScript.GetContentStateDataPath(false);
                if (config != null && config.Count > 0)
                {
                    for (int i = 0; i < config.Count; i++)
                    {
                        JsonData item = config[i];
                        string groupName = item["GroupName"].ToString();
                        var group = settings.FindGroup(groupName);
                        if (group == null)
                        {
                            group = settings.CreateGroup(groupName, false, false, false, null);
                        }

                        ContentUpdateGroupSchema cugSchema = group.GetSchema<ContentUpdateGroupSchema>();
                        if (cugSchema == null)
                        {
                            cugSchema = group.AddSchema<ContentUpdateGroupSchema>();
                        }
                        cugSchema.StaticContent = ((int)item["UpdateRestriction"] == 1);
                        BundledAssetGroupSchema bagSchema = group.GetSchema<BundledAssetGroupSchema>();
                        if (bagSchema == null)
                        {
                            bagSchema = group.AddSchema<BundledAssetGroupSchema>();
                        }
                        bagSchema.BuildPath.SetVariableByName(settings, item["BuildPath"].ToString());
                        bagSchema.LoadPath.SetVariableByName(settings, item["LoadPath"].ToString());
                        if (cugSchema.StaticContent)
                        {
                            bagSchema.UseAssetBundleCrc = false;
                            bagSchema.UseAssetBundleCrcForCachedBundles = false;
                        }

                        //Filter
                        StringBuilder filterBuilder = new StringBuilder();
                        for (int filterIndex = 0; filterIndex < item["Filter"].Count; filterIndex++)
                        {
                            filterBuilder.Append($"t:{item["Filter"][filterIndex].ToString()} ");
                        }
                        //SearchInFolders
                        List<string> folders = new List<string>();
                        for (int folderIndex = 0; folderIndex < item["SearchInFolders"].Count; folderIndex++)
                        {
                            folders.Add(item["SearchInFolders"][folderIndex].ToString());
                        }
                        //Labels
                        List<string> labels = new List<string>();
                        for (int labelIndex = 0; labelIndex < item["Labels"].Count; labelIndex++)
                        {
                            labels.Add(item["Labels"][labelIndex].ToString());
                        }

                        //Find All Asset
                        var findAssets = AssetDatabase.FindAssets(filterBuilder.ToString(), folders.ToArray());
                        for (int findIndex = 0; findIndex < findAssets.Length; findIndex++)
                        {
                            string guid = findAssets[findIndex];
                            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                            if (AssetDatabase.IsValidFolder(assetPath) || assetPath.EndsWith(".cs"))
                            {
                                continue;
                            }
                            var entry = group.GetAssetEntry(guid);
                            if (entry == null)
                            {
                                entry = settings.CreateOrMoveEntry(guid, group);
                            }
                            entry.labels.Clear();
                            foreach (var itemLabel in labels)
                            {
                                entry.SetLabel(itemLabel, true);
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(settings);
                AssetDatabase.Refresh();
            }
        }

    }
}