using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets;
using System.Text;
using UnityEditor.AddressableAssets.Settings;

namespace Wanderer.GameFramework
{
    public class AddressablesEditor
    {
        [MenuItem("Tools/Addressables/Build Content")]
        public static void BuildContent()
        {
            AddressableAssetSettings.BuildPlayerContent();
        }

        [MenuItem("Tools/Addressables/Check Update")]
        public static void CheckUpdate()
        {
            string binPath = ContentUpdateScript.GetContentStateDataPath(false);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entries = ContentUpdateScript.GatherModifiedEntries(settings, binPath);
            StringBuilder @string = new StringBuilder();
            foreach (var item in entries)
            {
                @string.AppendLine(item.address);
            }
            //将被修改过的资源单独分组
            var groupName = string.Format("UpdateGroup_{0}", System.DateTime.Now.ToString("yyyyMMdd"));
            ContentUpdateScript.CreateContentUpdateGroup(settings, entries, groupName);
            Debug.Log($"Update content:{@string}");
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Addressables/Build Update")]
        public static void BuildUpdate()
        {
            var binPath = ContentUpdateScript.GetContentStateDataPath(false);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressablesPlayerBuildResult result = ContentUpdateScript.BuildContentUpdate(settings, binPath);
            Debug.Log("BuildFinish path = " + settings.RemoteCatalogBuildPath.GetValue(settings));
            AssetDatabase.Refresh();
        }
    }
}