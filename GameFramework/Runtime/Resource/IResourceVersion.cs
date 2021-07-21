using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public abstract class IResourceVersion
    {
        public T Cast<T>() where T : IResourceVersion
        {
            return this as T;
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        /// <param name="needUpdate"></param>
        public abstract void CheckUpdate(Action<bool> needUpdate);

        /// <summary>
        /// 更新资源
        /// </summary>
        /// <param name="callback">下载回调[进度(0-1)，大小(KB),速度(KB/S),剩余时间(s)]</param>
        /// <param name="downloadComplete">下载完成</param>
        /// <param name="errorCallback">下载错误</param>
        /// <returns></returns>
        public abstract void UpdateResource(Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback);

    }
}
