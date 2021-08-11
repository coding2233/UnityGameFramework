using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.Text;
using UnityEditor.AddressableAssets.Build;

namespace Wanderer.GameFramework
{
    public class AssetFileWatcher
    {
        private static FileSystemWatcher _fileWatcher;
        private static bool _isRuning = false;

        [InitializeOnLoadMethod]
        private static void RunAssetFileWatcher()
        {
            //Debug.Log($"AssetFileWatcher.RunAssetFileWatcher");
            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Path = Path.Combine(Application.dataPath, "Game");
            _fileWatcher.IncludeSubdirectories = true;

            _fileWatcher.Created += (sender, e) => { UpdateAddressables(e); };
            //_fileWatcher.Deleted += (sender, e) => { UpdateAddressables(e); };
            _fileWatcher.Renamed += (sender, e) => { UpdateAddressables(e); };

            _fileWatcher.EnableRaisingEvents = true;

            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (!_isRuning)
                return;
            AssetDatabase.Refresh();
#if ADDRESSABLES_SUPPORT
            AddressablesEditor.SetAddressablesAssets();
#endif
            _isRuning = false;
        }

        private static void UpdateAddressables(FileSystemEventArgs e)
        {
            //Debug.Log($"File watcher: {e.FullPath}");
            if (_isRuning)
                return;
            _isRuning = true;
        }
    }
}
