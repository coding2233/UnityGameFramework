﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #编辑器资源加载类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月25日 12点06分# </time>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;

namespace Wanderer.GameFramework
{
    public class EditorAssetsHelper : IAssetsHelper
    {
        private List<string> _allAssetPaths = new List<string>();
        public List<string> AllAssetPaths
        {
            get
            {
                if (_allAssetPaths.Count == 0 )
                {
					foreach (var item in AssetDatabase.GetAllAssetPaths())
					{
                        _allAssetPaths.Add(item.ToLower());
                    }
                }
                return _allAssetPaths;
            }
        }

        public void SetResource(Action callback)
        {
            callback?.Invoke();
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, Action<AsyncOperation> callback)
        {
            AsyncOperation asyncLoadScene = EditorSceneManager.LoadSceneAsyncInPlayMode(sceneName, new LoadSceneParameters(mode));
            callback(asyncLoadScene);
        }


        public AsyncOperation UnloadSceneAsync(string sceneName)
        {
            return UnitySceneManager.UnloadSceneAsync(sceneName);
        }


        public void LoadAsset<T>(string assetName, Action<T> callback) where T : Object
        {
            callback(AssetDatabase.LoadAssetAtPath<T>(assetName));
        }

        public void UnloadAsset(string assetName)
        {

        }

        public void UnloadAssetBunlde(string assetBundleName, bool unload = false)
        {

        }

        public void Clear()
        {
        }

		public T LoadAsset<T>(string assetName) where T : Object
		{
            return AssetDatabase.LoadAssetAtPath<T>(assetName);
        }

		public void Preload(Action<float> progressCallback)
		{
            progressCallback?.Invoke(1.0f);
        }
	}
}

#endif
