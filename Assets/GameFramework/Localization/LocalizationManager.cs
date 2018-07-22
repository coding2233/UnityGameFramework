//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #本地化管理器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月22日 10点09分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
	public sealed class LocalizationManager : GameFrameworkModule
	{
		#region 属性
		/// <summary>
		/// 语言
		/// </summary>
		public Language Language { get; set; }
		/// <summary>
		/// 系统语言
		/// </summary>
		public Language SystemLanguage { get; set; }

		//本地化的key-值对应
		private Dictionary<string, string> _localizationStrings;
		#endregion
		
		public LocalizationManager()
		{
			_localizationStrings = new Dictionary<string, string>();
		}

		#region 外部接口
		//设置本地语言的字典
		public void SetLocalizationStrings(Dictionary<string, string> localizationStrings)
		{
			_localizationStrings = localizationStrings;
		}
		//添加本地化的key值
		public void AddLocalizationString(string key, string value)
		{
			if (!_localizationStrings.ContainsKey(key))
				_localizationStrings.Add(key, value);
		}

		/// <summary>
		/// 获取某一个值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string Get(string key)
		{
			_localizationStrings.TryGetValue(key, out key);
			return key;
		}

		/// <summary>
		/// 更换本来的类string对应
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void ChangeLocalizationString(string key, string value)
		{
			if (_localizationStrings.ContainsKey(key))
				_localizationStrings[key] = value;
			else
				_localizationStrings.Add(key, value);
		}

		#endregion

		public override void OnClose()
		{
			_localizationStrings.Clear();
		}
	}
}
