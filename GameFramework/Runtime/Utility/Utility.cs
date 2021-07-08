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
			string platformName = "";
#if UNITY_IOS
            platformName = "iOS";
#elif UNITY_ANDROID
            platformName = "Android";
#elif UNITY_STANDALONE_OSX
            platformName = "OSX";
#elif UNITY_STANDALONE_LINUX
            platformName = "Linux";
#elif UNITY_STANDALONE_WIN
			platformName = "Windows";
#elif UNITY_WEBGL
			platformName="WebGL";
#endif
			return platformName.ToLower();
		}

		/// <summary>
		/// 获取运行平台
		/// </summary>
		/// <returns></returns>
		public static RuntimePlatform GetRuntimePlatform()
		{
			return Application.platform;
		}

		/// <summary>
		/// 获取运行时间的平台名称
		/// </summary>
		/// <param name="target">编辑器默认以目标平台为准</param>
		/// <returns></returns>
		public static string GetRuntimePlatformName(bool target = true)
		{
			string platformName = "";
			switch (Application.platform)
			{
				case RuntimePlatform.OSXEditor:
					platformName = target ? GetPlatformName() : "OSX";
					break;
				case RuntimePlatform.OSXPlayer:
					platformName = "OSX";
					break;
				case RuntimePlatform.WindowsPlayer:
					platformName = "Windows";
					break;
				case RuntimePlatform.WindowsEditor:
					platformName = target ? GetPlatformName() : "Windows";
					break;
				case RuntimePlatform.IPhonePlayer:
					platformName = "iOS";
					break;
				case RuntimePlatform.Android:
					platformName = "Android";
					break;
				case RuntimePlatform.LinuxPlayer:
					platformName = "Linux";
					break;
				case RuntimePlatform.LinuxEditor:
					platformName = target ? GetPlatformName() : "Linux";
					break;
				case RuntimePlatform.WebGLPlayer:
					platformName = "WebGL";
					break;
				default:
					platformName = Application.platform.ToString();
					break;
			}
			return platformName.ToLower();
		}

		/// <summary>
		/// AppVersion 从Config里面取,避免其他地方发布的时候，将版本号覆盖
		/// </summary>
		public static string AppVersion
		{
			get
			{
				var config = GameFrameworkMode.GetModule<ConfigManager>();
				if(config==null|| config["AppVersion"]==null)
				{
					return Application.version;
				}
				return (string)config["AppVersion"];
			}
		}

		/// <summary>
		/// 资源加载方案
		/// </summary>
		public static string ResoucePlan
		{
			get
			{
#if ADDRESSABLES_SUPPORT
				return "Addressables";
#else
				return "AssetBundle";
#endif
			}
		}

		/// <summary>
		/// App
		/// </summary>

		public static string AppServerName
		{
			get
			{
#if TEST
				return "Test";
#else
				return "Official";
#endif
			}
		}

		/// <summary>
		/// 只读路径
		/// </summary>
		public static string ReadOnlyPath
		{
			get
			{
				return Application.streamingAssetsPath;
			}
		}

		/// <summary>
		/// 读写路径
		/// </summary>
		public static string ReadWritePath
		{
			get
			{
				return Application.persistentDataPath;
			}
		}

		/// <summary>
		/// 数据路径
		/// </summary>
		public static string DataPath
		{
			get
			{
				return Application.dataPath;
			}
		}

		/// <summary>
		/// 缓存路径
		/// </summary>
		public static string CachePath
		{
			get
			{
				return Application.temporaryCachePath;
			}
		}



	}

}