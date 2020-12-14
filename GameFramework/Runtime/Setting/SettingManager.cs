//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #设置管理器模块# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月22日 14点27分# </time>
//-----------------------------------------------------------------------

using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using OdinSerializer;

namespace Wanderer.GameFramework
{
    public sealed class SettingManager : GameFrameworkModule
    {
		public override int Priority => 1000;

		#region 属性
		//普通的设置
		private Dictionary<Type, Action<string, IConvertible>> _normalSet = new Dictionary<Type, Action<string, IConvertible>>();
        //普通的获取
        private Dictionary<Type, Func<string, IConvertible,IConvertible>> _normalGet = new Dictionary<Type, Func<string, IConvertible,IConvertible>>();
		//配置文件的路径
		private const string _settingDirectory = ".userdata/";
		private string _settingFilePath;
		public string SettingFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(_settingFilePath))
				{
					_settingFilePath = Path.Combine(Application.persistentDataPath, _settingDirectory);
					if (!Directory.Exists(_settingFilePath))
					{
						Directory.CreateDirectory(_settingFilePath);
					}
				}
				return _settingFilePath;
			}
		}
		#endregion

		public SettingManager()
        {
            // normal set
            _normalSet.Add(typeof(int), (key, value) => {
                PlayerPrefs.SetInt(key, (int)value);
            });

            _normalSet.Add(typeof(float), (key, value) => {
                PlayerPrefs.SetFloat(key, (float)value);
            });

            _normalSet.Add(typeof(string), (key, value) => {
                PlayerPrefs.SetString(key, (string)value);
            });

            
            //normal get
            _normalGet.Add(typeof(int), (key,defaultValue) => {
                return PlayerPrefs.GetInt(key, (int)defaultValue);
            });

            _normalGet.Add(typeof(float), (key, defaultValue) => {
                return PlayerPrefs.GetFloat(key,(float)defaultValue);
            });

            _normalGet.Add(typeof(string), (key,defaultValue) => {
                return PlayerPrefs.GetString(key,(string)defaultValue);
            });

			//设置默认的质量
			SetQualityFromUserData();
        }

		/// <summary>
		/// 设置数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
        public void Set<T>(string key, T value)
        {
            if (_normalSet.TryGetValue(typeof(T), out Action<string, IConvertible> setAction))
            {
                setAction.Invoke(key, (IConvertible)value);
            }
            else
            {
                if (value is object)
                {
					SetObject(key, value);
				}
            }
        }

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T Get<T>(string key,T defaultValue=default(T))
		{
			if (_normalGet.TryGetValue(typeof(T), out Func<string, IConvertible,IConvertible> getAction))
			{
				return (T)getAction.Invoke(key, (IConvertible)defaultValue);
			}
			else
			{
				return GetObject<T>(key, defaultValue);
			}
		}

		/// <summary>
		/// 是否包含数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Has(string key)
		{
			string filePath = GetSettingFilePath(key);
			return PlayerPrefs.HasKey(key) || File.Exists(filePath);
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		public void Delete(string key)
		{
			//删除PlayerPrefs
			if (PlayerPrefs.HasKey(key))
				PlayerPrefs.DeleteKey(key);
			//删除文件
			string filePath = GetSettingFilePath(key);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		/// <summary>
		/// 删除所有的数据
		/// </summary>
		public void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
			string[] files = Directory.GetFiles(SettingFilePath);
			if (files != null)
			{
				foreach (var item in files)
				{
					if(File.Exists(item))
						File.Delete(item);
				}
			}
		}

		/// <summary>
		/// 获取对象数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetObject<T>(string key, T defaultValue= default(T))
		{
			string filePath = GetSettingFilePath(key);
			if (File.Exists(filePath))
			{
				byte[] buffer= File.ReadAllBytes(filePath);
				T getValue = SerializationUtility.DeserializeValue<T>(buffer, DataFormat.Binary);
				return getValue;
			}
			return defaultValue;
		}

		/// <summary>
		/// 保存对象数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		public void SetObject<T>(string key, T value)
		{
			string filePath = GetSettingFilePath(key);
			using (FileStream fileStream = new FileStream(filePath,FileMode.OpenOrCreate))
			{
				fileStream.Flush();
				byte[] buffer = SerializationUtility.SerializeValue(value, DataFormat.Binary);
				fileStream.Write(buffer, 0, buffer.Length);
				fileStream.Close();
			}
		}

		#region 质量
		/// <summary>
		/// 质量信息
		/// </summary>
		public int Quality
		{
			get
			{
				int level = QualitySettings.GetQualityLevel();
				//获取质量信息
				level = Get<int>("QualitySettings.QualityLevel", level);
				return level;
			}
			set
			{
				int currentLevel = QualitySettings.GetQualityLevel();
				if (currentLevel != value)
				{
					QualitySettings.SetQualityLevel(value);
					//保存质量信息
					Set<int>("QualitySettings.QualityLevel", value);
				}
			}
		}
		/// <summary>
		/// 从本地数据设置质量
		/// </summary>
		public void SetQualityFromUserData()
		{
			Quality = Quality;
		}
		#endregion

		#region 震动
		/// <summary>
		/// 震动是否启用
		/// </summary>
		public bool Vibrate 
		{
			get
			{
				return Get<bool>("UnityEngine.Vibrate",true);
			}
			set
			{
				Set<bool>("UnityEngine.Vibrate",value);
			}
		}

		/// <summary>
		/// 调用移动端的震动
		/// </summary>
		public void HandheldVibrate()
		{
			if (Vibrate)
			{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
				Handheld.Vibrate();
#endif
			}
		}
		#endregion

		#region System

		/// <summary>
		/// 获取网络的状态信息
		/// </summary>
		public NetworkReachability InternetReachability
		{
			get
			{
				return Application.internetReachability;
			}
		}

		/// <summary>
		/// 获取网络状态信息
		/// </summary>
		/// <returns></returns>
		public string GetInternetReachability()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return "无网络";
			}
			//Check if the device can reach the internet via a carrier data network
			else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
			{
				return "移动数据";
			}
			//Check if the device can reach the internet via a LAN
			else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
			{
				return "WIFI";
			}
			return null;
		}

		/// <summary>
		/// 获取当前的时间戳 s数
		/// </summary>
		/// <returns></returns>
		public string GetTimeStamp()
		{
			var t = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0); 
			return Convert.ToInt64(t.TotalSeconds).ToString();
		}

		#endregion


		#region 内部函数
		/// <summary>
		/// 获取设置文件的路径
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private string GetSettingFilePath(string key)
		{
			string filePath = Path.Combine(SettingFilePath, key);
			return filePath;
		}
#endregion
		public override void OnClose()
        {
			//暂时不清理SettingManager的数据。 其他模块可能在OnClose中保存数据

			//_normalSet.Clear();
			//_normalGet.Clear();
		}
    }
}
