using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Wanderer.GameFramework
{
    public class AddressablesAssetsHelper : IAssetsHelper
    {

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _sceneInstanceAsync = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
        private Dictionary<string, UnityEngine.Object> _objectAsync = new Dictionary<string, UnityEngine.Object>();

        public List<string> AllAssetPaths => null;

        public void Clear()
        {
            foreach (var item in _objectAsync)
            {
                Addressables.Release(item.Value);
            }
            _objectAsync.Clear();

            foreach (var item in _sceneInstanceAsync)
            {
                Addressables.UnloadSceneAsync(item.Value);
            }
            _sceneInstanceAsync.Clear();
        }

        public void LoadAsset<T>(string assetName, Action<T> callback) where T : UnityEngine.Object
        {
           Addressables.LoadAssetAsync<T>(assetName).Completed += (handle)=> {
               var @object = handle.Result;
               CheckAsset(assetName, @object);
               callback?.Invoke(@object); 
           };
        }

        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(assetName);
            var @object = handle.WaitForCompletion();
            CheckAsset(assetName, @object);
            return @object;
        }


        public T[] FindAssets<T>(List<string> tags) where T : UnityEngine.Object
        {
            var handle= Addressables.LoadAssetsAsync<T>(tags, (tObject) =>
            {
            },Addressables.MergeMode.Intersection);

            var resultList= handle.WaitForCompletion();
            T[] result = new T[resultList.Count];
            resultList.CopyTo(result,0);
            return result;
        }

        public void FindAssets<T>(List<string> tags, Action<T[]> callback) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetsAsync<T>(tags, (tObject) =>
            {
            }, Addressables.MergeMode.Intersection);

            handle.Completed += (resultHandle) => {
                var resultList = resultHandle.Result;
                T[] result = new T[resultList.Count];
                resultList.CopyTo(result, 0);
                callback?.Invoke(result);
            };
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, Action<AsyncOperation> callback)
        {
            if (!_sceneInstanceAsync.ContainsKey(sceneName))
            {
                var handle = Addressables.LoadSceneAsync(sceneName, mode);
                _sceneInstanceAsync.Add(sceneName,handle);
                handle.Completed += (sceneHandle) => { callback?.Invoke(sceneHandle.Result.ActivateAsync()); };
            }
        }

        public void Preload(Action<float> progressCallback)
        {
            progressCallback?.Invoke(1.0f);
        }

        public void SetResource(Action callback)
        {
            callback?.Invoke();
        }

        public void UnloadAsset(string assetName)
        {
            if (_objectAsync.ContainsKey(assetName))
            {
                var @object = _objectAsync[assetName];
                _objectAsync.Remove(assetName);
                if (@object != null)
                {
                    Addressables.Release(@object);
                }
            }
        }

        public void UnloadAssetBunlde(string assetBundleName, bool unload = false)
        {
        }

        public AsyncOperation UnloadSceneAsync(string sceneName)
        {
            if (_sceneInstanceAsync.ContainsKey(sceneName))
            {
                var handle = _sceneInstanceAsync[sceneName];
                _sceneInstanceAsync.Remove(sceneName);
                var unloadHandle = Addressables.UnloadSceneAsync(handle);
                return unloadHandle.Result.ActivateAsync();
            }
            return null;
        }

        private void CheckAsset(string assetName, UnityEngine.Object @object)
        {
            if (!_objectAsync.ContainsKey(assetName))
            {
                _objectAsync.Add(assetName, @object);
            }
            else
            {
                Log.Warning($"The same resource file was loaded earlier : {assetName}");
            }
        }
    }
}
