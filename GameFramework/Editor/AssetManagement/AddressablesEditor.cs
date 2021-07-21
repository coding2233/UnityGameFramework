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

namespace Wanderer.GameFramework
{
    public class AddressablesEditor
    {
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
                        //将被修改过的资源单独分组
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

        public static void ShellBuildPlayerContent(string activeProfileId = "Default")
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null && settings.BuildRemoteCatalog)
            {
                var profileId = settings.profileSettings.GetProfileId(activeProfileId);
                settings.activeProfileId = profileId;
            }
            AddressableAssetSettings.BuildPlayerContent();
            EditorUtility.SetDirty(settings);
            AssetDatabase.Refresh();
        }

    }
}