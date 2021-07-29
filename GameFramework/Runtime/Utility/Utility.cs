using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
				if (config == null || config["AppVersion"] == null)
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


		/// <summary>
		/// AES加密
		/// </summary>
		/// <param name="data"></param>
		/// <param name="key"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static string AESEncrypt(string data, string key, CipherMode mode = CipherMode.CBC)
		{
			byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            RijndaelManaged rijndael = new RijndaelManaged();
			rijndael.Key = Encoding.UTF8.GetBytes(key);
			rijndael.Mode = mode;
			rijndael.IV = Encoding.UTF8.GetBytes(key);
			rijndael.Padding = PaddingMode.None;

			int covering = dataBytes.Length % 16;
			int coveringLength = 0;
			if (covering != 0)//手动补位
			{
				coveringLength = 16 - covering;
			}
			int dataLength = dataBytes.Length + coveringLength;
			byte[] inputData = new byte[dataLength];
			Buffer.BlockCopy(dataBytes,0, inputData,0, dataBytes.Length);

			string result = "";
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(),CryptoStreamMode.Write))
				{
					cryptoStream.Write(inputData, 0, inputData.Length);

					byte[] tempData = memoryStream.ToArray();
					result =Convert.ToBase64String(tempData);
				}
			}
			return result;
		}

		/// <summary>
		/// AES解密
		/// </summary>
		/// <param name="data"></param>
		/// <param name="key"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static string AESDecrypt(string data, string key, CipherMode mode = CipherMode.CBC)
		{
			byte[] dataBytes = Convert.FromBase64String(data);
            RijndaelManaged rijndael = new RijndaelManaged();
			rijndael.Key = Encoding.UTF8.GetBytes(key);
			rijndael.IV = Encoding.UTF8.GetBytes(key);
			rijndael.Mode = mode;
			rijndael.Padding = PaddingMode.None;

			byte[] outData = new byte[dataBytes.Length];

			string result = "";
			using (MemoryStream memoryStream = new MemoryStream(dataBytes))
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
				{
					cryptoStream.Read(outData, 0, outData.Length);
					int length = outData.Length;
                    for (int i = 0; i < outData.Length; i++)
                    {
                        if (outData[i] == '\0')
                        {
                            length = i;
                            //Log.Info($"{i} --# {outData[i]}");
                            break;
                        }
                        //Log.Info($"{i}  --> {outData[i]}");
                    }
                    byte[] getData = new byte[length];
					Buffer.BlockCopy(outData,0, getData,0,length);
					result = Encoding.UTF8.GetString(getData);
				}
			}
			return result;
		}

	}

}