using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.AddressableAssets.Addressables;

namespace Wanderer.GameFramework
{
    public class AddressableVersion : IResourceVersion
    {
        private bool _isCheckUpdate;
        private AsyncOperationHandle<List<string>> _checkHandle;

        public override async void CheckUpdate(Action<bool> needUpdate)
        {
            var initHandle = Addressables.InitializeAsync();
            await initHandle.Task;
            if (_isCheckUpdate)
            {
                Addressables.Release(_checkHandle);
                _isCheckUpdate = false;
            }
            _checkHandle =  Addressables.CheckForCatalogUpdates(false);
            _isCheckUpdate = true;
            await _checkHandle.Task;
            needUpdate?.Invoke(_checkHandle.Result.Count>0);
        }

        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="callback">���ػص�[����(0-1)����С(KB),�ٶ�(KB/S),ʣ��ʱ��(s)]</param>
        /// <param name="downloadComplete">�������</param>
        /// <param name="errorCallback">���ش���</param>
        /// <returns></returns>
        public override async void UpdateResource(Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback,string label)
        {
            try
            {
                if (_isCheckUpdate)
                {
                    bool hasLabel = !string.IsNullOrEmpty(label);

                    if (_checkHandle.Result.Count > 0)
                    {
                        var updateHandle = Addressables.UpdateCatalogs(_checkHandle.Result, false);
                        await updateHandle.Task;
                        var locators = updateHandle.Result;
                        HashSet<object> downloadKeys = new HashSet<object>();
                        long totalDownloadSize = 0;
                        foreach (var locator in locators)
                        {
                            var sizeHandle = Addressables.GetDownloadSizeAsync(locator.Keys);
                            await sizeHandle.Task;
                            long downloadSize = sizeHandle.Result;
                            if (downloadSize > 0)
                            {
                                if (hasLabel)
                                {
                                    foreach (var key in locator.Keys)
                                    {
                                        if (key.ToString().Equals(label))
                                        {
                                            totalDownloadSize += downloadSize;
                                            downloadKeys.Add(key);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    totalDownloadSize += downloadSize;
                                    foreach (var key in locator.Keys)
                                    {
                                        downloadKeys.Add(key);
                                    }
                                }
                            }
                        }
                        if (totalDownloadSize > 0)
                        {
                            double downloadKBSize = totalDownloadSize / 1024.0f;
                            float downloadStartTime = Time.realtimeSinceStartup;
                            var downloadHandle = Addressables.DownloadDependenciesAsync(downloadKeys, MergeMode.Union);
                            while (!downloadHandle.IsDone)
                            {
                                float percentage = downloadHandle.PercentComplete;
                                double useTime = Time.realtimeSinceStartup - downloadStartTime;
                                double downloadSpeed = ((double)percentage * downloadKBSize) / useTime;
                                float remainingTime =(float)((downloadKBSize / downloadSpeed)/ downloadSpeed- useTime);
                                callback?.Invoke(percentage, downloadKBSize, downloadSpeed, remainingTime);
                                await Task.Delay(100);
                            }
                            Addressables.Release(downloadHandle);
                        }
                        downloadComplete?.Invoke();
                        Addressables.Release(updateHandle);
                    }
                    Addressables.Release(_checkHandle);
                    _isCheckUpdate = false;
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e.Message, e.ToString());
            }
        }

    }

}