//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #设置管理器模块# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月22日 14点27分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
	public sealed class SettingManager : GameFrameworkModule
	{
        #region 属性
        private bool _debugEnable = true;
        private GameObject _debuger;
        #endregion
        
        /// <summary>
        /// 调试器可见性
        /// </summary>
        public bool DebugEnable
        {
            get
            {
                return _debugEnable;
            }
            set
            {
                _debugEnable = value;
                if (_debuger != null)
                    _debuger.SetActive(value);
            }
        }

        public void SetDebuger(GameObject debuger)
        {
            _debuger = debuger;
        }

        public int GetQuality()
		{
			return PlayerPrefs.GetInt("QualitySettings", (int)QualitySettings.GetQualityLevel());
		}

		public void SetQuality(int level)
		{
			PlayerPrefs.SetInt("QualitySettings", level);
		}

		public float GetAllSoundVolume()
		{
			return PlayerPrefs.GetFloat("AllSoundVolume", 1.0f);
		}

		public void SetAllSoundVolume(float volume)
		{
			PlayerPrefs.SetFloat("AllSoundVolume", volume);
		}

		public float GetBackgrounddMusicVolumme()
		{
			return PlayerPrefs.GetFloat("BackgroundMusicVolume", 1.0f);
		}

		public void SetBackgroundMusicVolume(float volume)
		{
			PlayerPrefs.SetFloat("BackgroundMusicVolume", volume);
		}

		public void SetUISoundVolume(float volume)
		{
			PlayerPrefs.SetFloat("UISoundVolume", volume);
		}

		public float GetUISoundVolume()
		{
			return PlayerPrefs.GetFloat("UISoundVolume", 1.0f);

		}

		public void SetSoundEffectVolume(float volume)
		{
			PlayerPrefs.SetFloat("SoundEffectVolume", volume);
		}

		public float GetSoundEffectVolumme()
		{
			return PlayerPrefs.GetFloat("SoundEffectVolume", 1.0f);
		}


		public void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public int GetInt(string key)
		{
			return PlayerPrefs.GetInt(key, 0);
		}

		public void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public float GetFloat(string key)
		{
			return PlayerPrefs.GetFloat(key, 0.0f);
		}

		public void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public string GetString(string key)
		{
			return PlayerPrefs.GetString(key, "");
		}


		public override void OnClose()
		{
		}
	}
}
