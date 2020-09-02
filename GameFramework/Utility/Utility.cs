using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class Utility
    {
        /// <summary>
        /// 获取平台名称
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformName()
        {
            string platformName = "StandaloneWindows";
#if UNITY_IOS
                platformName = "IOS";
#elif UNITY_ANDROID
                platformName = "Android";
#elif UNITY_STANDALONE_OSX
                platformName = "StandaloneOSX";
#elif UNITY_STANDALONE_LINUX
            platformName = "StandaloneLinux";
#endif
            return platformName.ToLower();
        }
    }

}