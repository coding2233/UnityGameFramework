//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #音频管理器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月22日 00点37分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
	public sealed class AudioManager:GameFrameworkModule 
	{
		#region 属性
		//资源管理器
		private ResourceManager _resourceManager;
		//背景音乐
		private AudioSource _backgroundMusic;
		//ui音效
		private AudioSource _uiSound;
		//音效
		private AudioSource _soundEffect;

		private readonly Dictionary<string, AudioClip> _backGroundMusicClips = new Dictionary<string, AudioClip>();
		private readonly Dictionary<string, AudioClip> _uiSoundClips = new Dictionary<string, AudioClip>();
		private readonly Dictionary<string, AudioClip> _soundEffectClips = new Dictionary<string, AudioClip>();

		#endregion

		public AudioManager()
		{
			_resourceManager = GameFrameworkMode.GetModule<ResourceManager>();
		}

		#region 外部接口
		/// <summary>
		/// 设置默认声音播放器
		/// </summary>
		/// <param name="backgroundMusic"></param>
		/// <param name="uiSound"></param>
		/// <param name="soundEffect"></param>
		public void SetDefaultAudioSource(AudioSource backgroundMusic = null, AudioSource uiSound=null, AudioSource soundEffect=null)
		{
			_backgroundMusic = backgroundMusic;
			_uiSound = uiSound;
			_soundEffect = soundEffect;
		}

		/// <summary>
		/// 添加背景音乐
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void AddBackroundMusic(string assetBundleName,string audioClipPath)
		{
			AddAuioClip(assetBundleName,audioClipPath, _backGroundMusicClips);
		}
		/// <summary>
		/// 添加UI音效
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void AddUISound(string assetBundleName,string audioClipPath)
		{
			AddAuioClip(assetBundleName,audioClipPath, _uiSoundClips);
		}
		/// <summary>
		/// 添加音效
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void AddSoundEffect(string assetBundleName,string audioClipPath)
		{
			AddAuioClip(assetBundleName, audioClipPath, _soundEffectClips);
		}
		
		/// <summary>
		/// 播放在GameObject上的声音
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void PlayGameObjectSound(GameObject go,string assetBundleName,string audioClipPath)
		{
			AudioSource audioSource=go.GetComponent<AudioSource>();
			if (audioSource==null)
			{
				audioSource = go.AddComponent<AudioSource>();
			}

			audioSource.clip = _resourceManager?.LoadAsset<AudioClip>(assetBundleName,audioClipPath);
			if (audioSource.clip != null)
				audioSource.Play();
		}
		/// <summary>
		/// 移除音乐
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void RemoveGameObjectSound(GameObject go)
		{
			AudioSource audioSource= go.GetComponent<AudioSource>();
			if (audioSource!=null)
				MonoBehaviour.Destroy(audioSource);
		}


		/// <summary>
		/// 播放背景音乐
		/// </summary>
		/// <param name="audioClipPath">音频的资源路径</param>
		public void PlayBackgroundMusic(string audioClipPath,bool addAudioClip=false)
		{
            if(_backGroundMusicClips.ContainsKey(audioClipPath))
			    PlayAudioClip(audioClipPath, _backGroundMusicClips, _backgroundMusic);
		}

		/// <summary>
		/// 停止背景音乐
		/// </summary>
		public void StopBakgroundMusic()
		{
			StopAudioClip(_backgroundMusic);
		}

		/// <summary>
		/// 移除背景音乐
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void RemoveBackgroundMusic(string audioClipPath)
		{
			RemoveAudioClip(audioClipPath, _backGroundMusicClips);
		}

		/// <summary>
		/// 播放ui音效
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void PlayUISound(string audioClipPath,bool addAudioClip=false)
		{
            if(_uiSoundClips.ContainsKey(audioClipPath))
			    PlayAudioClip(audioClipPath, _uiSoundClips, _uiSound);
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void StopUISoud()
		{
			StopAudioClip(_uiSound);
		}

		/// <summary>
		/// 移除ui音效
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void RemoveUISound(string audioClipPath)
		{
			RemoveAudioClip(audioClipPath, _uiSoundClips);
		}

		/// <summary>
		/// 播放音效
		/// </summary>
		/// <param name="audioClipPath"></param>
		/// <param name="addAudioClip"></param>
		public void PlaySoundEffect(string audioClipPath,bool addAudioClip=false)
		{
			if (_soundEffectClips.ContainsKey(audioClipPath))
			    PlayAudioClip(audioClipPath, _soundEffectClips, _soundEffect);
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void StopSoundEffect()
		{
			StopAudioClip(_soundEffect);
		}

		/// <summary>
		/// 移除音效
		/// </summary>
		/// <param name="audioClipPath"></param>
		public void RemoveSoundEffect(string audioClipPath)
		{
			RemoveAudioClip(audioClipPath,_soundEffectClips);
		}

		#endregion

		#region 内部函数

		//添加音频
		private bool AddAuioClip(string assetBundleName,string audioClipPath, Dictionary<string, AudioClip> clips)
		{
			if (!clips.ContainsKey(audioClipPath))
			{
				AudioClip audioClip = _resourceManager?.LoadAsset<AudioClip>(assetBundleName,audioClipPath);
				if (audioClip != null)
				{
					clips.Add(audioClipPath, audioClip);
					return true;
				}
			}

			return false;
		}

		private void PlayAudioClip(string audioClipPath,Dictionary<string,AudioClip> audioClips,AudioSource audioSource)
		{
			AudioClip audioClip;
			if (audioClips.TryGetValue(audioClipPath, out audioClip))
			{
				if (audioSource != null && (audioSource.clip != audioClip || !audioSource.isPlaying))
				{
					audioSource.clip = audioClip;
					audioSource.Play();
				}
			}
		}

		private void StopAudioClip(AudioSource audioSource)
		{
			audioSource?.Stop();
		}

		private void RemoveAudioClip(string audioClipPath, Dictionary<string, AudioClip> audioClips)
		{
			AudioClip audioClip;
			if (audioClips.TryGetValue(audioClipPath,out audioClip))
			{
				audioClips.Remove(audioClipPath);
				audioClip = null;
			}
		}

		#endregion

		public override void OnClose()
		{
		}
	}
}
