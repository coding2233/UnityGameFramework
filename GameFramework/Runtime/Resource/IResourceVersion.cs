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
        /// ������
        /// </summary>
        /// <param name="needUpdate"></param>
        public abstract void CheckUpdate(Action<bool> needUpdate);

        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="callback">���ػص�[����(0-1)����С(KB),�ٶ�(KB/S),ʣ��ʱ��(s)]</param>
        /// <param name="downloadComplete">�������</param>
        /// <param name="errorCallback">���ش���</param>
        /// <returns></returns>
        public abstract void UpdateResource(Action<float, double, double, float> callback, Action downloadComplete, Action<string, string> errorCallback);

    }
}
