//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #音频管理器# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月22日 00点37分# </time>
//-----------------------------------------------------------------------

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public sealed class AudioManager : GameFrameworkModule,IUpdate
    {
        #region 属性
        //资源管理器
        private ResourceManager _resourceManager;
		//所有的音频播放器
		Dictionary<Type, AudioPlayer> _audioPlayers = new Dictionary<Type, AudioPlayer>();
        //背景音乐
		//默认loop ,只支持1个实例
        private MusicAudioPlayer _musicAudioPlayer;
		//音效
		//默认不循环，不支持loop,只支持1个实例，不支持打断
		private SoundAudioPlayer _soundAudioPlayer;
		//ui音效
		//默认不循环，支持loop,支持多个实例,支持回调
		private UISoundAudioPlayer _uiSoundAudioPlayer;
		//音频资源
		private Dictionary<string, AudioClip> _audioClipSources = new Dictionary<string, AudioClip>();

		private bool _mute;
		/// <summary>
		/// 静音
		/// </summary>
		public bool Mute 
		{
			get
			{
				//获取数据
				//_mute=GameFrameworkMode.GetModule<SettingManager>().Get<bool>($"AudioManager.AudioPlayer.Mute", _mute);
				return _mute;
			}
			set
			{
				_mute = value;

				foreach (var item in _audioPlayers.Values)
				{
					item.Mute = _mute;
				}

				//保存数据
				//GameFrameworkMode.GetModule<SettingManager>().Set<bool>($"AudioManager.AudioPlayer.Mute", _mute);
			}
		}

		private float _volume = 1.0f;
		/// <summary>
		/// 音量
		/// </summary>
		public float Volume
		{
			get
			{
				//获取数据
				_volume = GameFrameworkMode.GetModule<SettingManager>().Get<float>($"AudioManager.AudioPlayer.Volume", _volume);

				return _volume;
			}
			set
			{
				_volume = value;
				foreach (var item in _audioPlayers.Values)
				{
					item.Volume = _volume;
				}

				//保存数据
				GameFrameworkMode.GetModule<SettingManager>().Set<float>($"AudioManager.AudioPlayer.Volume", _volume);
			}
		}
		#endregion

		public AudioManager()
        {
            _resourceManager = GameFrameworkMode.GetModule<ResourceManager>();
        }

		public void OnUpdate()
		{
			foreach (var item in _audioPlayers)
			{
				item.Value.Update();
			}
		}

		#region 外部接口

		///// <summary>
		///// //Gets setup information from user data.
		///// </summary>
		//public void SettingFromUserData()
		//{
		//	Mute = Mute;
		//	Volume = Volume;
		//}

		/// <summary>
		/// 设置默认声音播放器
		/// </summary>
		/// <param name="backgroundMusic"></param>
		/// <param name="uiSound"></param>
		/// <param name="soundEffect"></param>
		public void SetDefaultAudioSource(AudioSource backgroundMusic = null, AudioSource uiSound = null, AudioSource sound = null)
		{
			_musicAudioPlayer = new MusicAudioPlayer(backgroundMusic);
			_soundAudioPlayer = new SoundAudioPlayer(sound);
			_uiSoundAudioPlayer = new UISoundAudioPlayer(uiSound);

			_audioPlayers.Add(typeof(MusicAudioPlayer), _musicAudioPlayer);
			_audioPlayers.Add(typeof(SoundAudioPlayer), _soundAudioPlayer);
			_audioPlayers.Add(typeof(UISoundAudioPlayer), _uiSoundAudioPlayer);

			//设置默认信息
			//SettingFromUserData();
		}

		/// <summary>
		/// 添加音频播放器
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		public void AddAudiPlayer<T>(T t) where T : AudioPlayer
		{
			if (!_audioPlayers.ContainsKey(typeof(T)))
			{
				_audioPlayers.Add(typeof(T), t);
			}
		}

		/// <summary>
		/// 设置静音
		/// </summary>
		/// <param name="mute"></param>
		public void SetMute<T>(bool mute) where T : AudioPlayer
		{
			GetAudioPlayer<T>().Mute = mute;
		}

		/// <summary>
		/// 设置音量
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		public void SetVolume<T>(float volume) where T : AudioPlayer
		{
			GetAudioPlayer<T>().Volume = volume;
		}

		/// <summary>
		/// 播放音频
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assetName"></param>
		/// <param name="loop"></param>
		public void Play<T>(string assetName, bool loop = false) where T : AudioPlayer
		{
			AudioClip audioClip = GetAudioClip(assetName);
			GetAudioPlayer<T>().Play(audioClip, loop);
		}

		/// <summary>
		/// 暂停
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Pause<T>() where T : AudioPlayer
		{
			GetAudioPlayer<T>().Pause();
		}


		/// <summary>
		/// 恢复
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Resume<T>() where T : AudioPlayer
		{
			GetAudioPlayer<T>().Resume();
		}

		/// <summary>
		/// 停止
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Stop<T>() where T : AudioPlayer
		{
			GetAudioPlayer<T>().Stop();
		}


		/// <summary>
		/// 获取AudioPlayer
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetAudioPlayer<T>() where T : AudioPlayer
		{
			if (_audioPlayers.TryGetValue(typeof(T), out AudioPlayer audioPlayer))
			{
				return audioPlayer as T;
			}
			return default(T);
		}

	
		/// <summary>
		/// 播放UI音效
		/// </summary>
		/// <param name="assetName"></param>
		public AudioTween UISoundPlay(string assetName,bool loop=false)
		{
			AudioClip audioClip = GetAudioClip(assetName);
			return _uiSoundAudioPlayer.Play(audioClip, loop);
		}

		/// <summary>
		/// 暂停UI音效
		/// </summary>
		public void UISoundPause(AudioTween audioTween)
		{
			_uiSoundAudioPlayer.Pasue(audioTween);
		}

		/// <summary>
		/// UI音效恢复
		/// </summary>
		public void UISoundResume(AudioTween audioTween)
		{
			_uiSoundAudioPlayer.Resume(audioTween);
		}

		/// <summary>
		/// 停止UI音效
		/// </summary>
		public void UISoundStop(AudioTween audioTween)
		{
			_uiSoundAudioPlayer.Stop(audioTween);
		}

		/// <summary>
		/// 停止所有的UI音效
		/// </summary>
		public void UISoundStopAll()
		{
			_uiSoundAudioPlayer.StopAll();
		}

		#endregion

		#region 内部函数
		//获取音频
		private AudioClip GetAudioClip(string assetName)
		{
			AudioClip audioClip = null;
			if (_audioClipSources.TryGetValue(assetName, out audioClip))
			{
				audioClip = _resourceManager.LoadAssetSync<AudioClip>(assetName);
				_audioClipSources.Add(assetName, audioClip);
			}
			return audioClip;
		}
		#endregion

		public override void OnClose()
        {
			_audioClipSources.Clear();
			foreach (var item in _audioPlayers)
			{
				item.Value.Close();
			}
			_audioPlayers.Clear();
		}

	
	}
}
