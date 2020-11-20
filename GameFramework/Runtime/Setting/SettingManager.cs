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
		/// 获取对象数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetObject<T>(string key, T defaultValue= default(T))
		{
			string filePath = Path.Combine(SettingFilePath, key);
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
			string filePath = Path.Combine(SettingFilePath, key);
			using (FileStream fileStream = new FileStream(filePath,FileMode.OpenOrCreate))
			{
				fileStream.Flush();
				byte[] buffer = SerializationUtility.SerializeValue(value, DataFormat.Binary);
				fileStream.Write(buffer, 0, buffer.Length);
				fileStream.Close();
			}
		}

		public override void OnClose()
        {
        }
    }
}
