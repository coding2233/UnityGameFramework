//设置类的扩展 主要为支持不支持泛型的脚本接口的调用
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public static class SettingManagerExtension
	{
		#region set
		/// <summary>
		/// 设置int
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetInt(this SettingManager setting, string key, int value)
		{
			setting.Set(key, value);
		}

		/// <summary>
		/// 设置float
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetFloat(this SettingManager setting, string key, float value)
		{
			setting.Set(key, value);
		}

		/// <summary>
		/// 设置string
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetString(this SettingManager setting, string key, string value)
		{
			setting.Set(key, value);
		}

		/// <summary>
		/// 设置object
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetObject(this SettingManager setting, string key, object value)
		{
			setting.Set(key, value);
		}

		#endregion


		#region get
		/// <summary>
		/// 获取int
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static object GetInt(this SettingManager setting, string key, int value = 0)
		{
			return setting.Get(key, value);
		}

		/// <summary>
		/// 获取float
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static object GetFloat(this SettingManager setting, string key, float value = 0)
		{
			return setting.Get(key, value);
		}

		/// <summary>
		/// 获取string
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static object GetString(this SettingManager setting, string key, string value = null)
		{
			return setting.Get(key, value);
		}
		/// <summary>
		/// 获取object
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static object GetObject(this SettingManager setting, string key, object value=null)
		{
			return setting.Get(key, value);
		}
		#endregion

	}
}