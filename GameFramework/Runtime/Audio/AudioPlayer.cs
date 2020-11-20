using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public abstract class AudioPlayer
	{
		protected AudioSource _audioSource;
		//默认的音量
		protected float _nativeVolume=1.0f;
		//静音
		protected bool _mute = false;
		//声音状态
		protected AudioTween _audioTween = null;

		/// <summary>
		/// 静音
		/// </summary>
		public virtual bool Mute
		{
			get
			{
				return _mute;
			}
			set
			{
				_audioSource.mute = value;
				if (_mute != value)
				{
					_mute = value;
					//保存数据
					GameFrameworkMode.GetModule<SettingManager>().Set<bool>($"AudioManager.AudioPlayer.{GetType().Name}.Mute", _mute);
				}
			}
		}

		/// <summary>
		/// 音量
		/// </summary>
		public virtual float Volume
		{
			get
			{
				return _audioSource.volume;
			}
			set
			{
				if (_nativeVolume * value != _audioSource.volume)
				{
					_audioSource.volume = _nativeVolume * value;
					//保存数据
					GameFrameworkMode.GetModule<SettingManager>().Set<float>($"AudioManager.AudioPlayer.{GetType().Name}.Volume", _audioSource.volume);
				}
			}
		}

		/// <summary>
		/// 正在播放
		/// </summary>
		public virtual bool IsPlaying
		{
			get
			{
				return _audioSource.isPlaying;
			}
		}

		/// <summary>
		/// 是否暂停
		/// </summary>
		public virtual bool IsPause
		{
			get;
			protected set;
		}

		/// <summary>
		/// 是否循环
		/// </summary>
		public virtual bool IsLoop
		{
			get
			{
				return _audioSource.loop;
			}
		}

		/// <summary>
		/// 设置声音播放器
		/// </summary>
		/// <param name="audioSource"></param>
		public AudioPlayer(AudioSource audioSource)
		{
			_audioSource = audioSource;
			_nativeVolume = audioSource.volume;
			IsPause = false;
			//获取默认的设置信息
			_mute = GameFrameworkMode.GetModule<SettingManager>().Get<bool>($"AudioManager.AudioPlayer.{GetType().Name}.Mute", _mute);
			_audioSource.volume = GameFrameworkMode.GetModule<SettingManager>().Get<float>($"AudioManager.AudioPlayer.{GetType().Name}.Volume", _audioSource.volume);
			//设置默认的数据
			Mute = _mute;
			Volume = _audioSource.volume;
		}

		/// <summary>
		/// 监听是否完成
		/// </summary>
		public virtual void Update()
		{
			if (!IsLoop)
			{
				if (_audioSource!=null&&!IsPlaying&&!IsPause&&_audioTween!=null)
				{
					_audioSource.clip = null;
					_audioTween.Complete();
					AudioTween.Release(_audioTween);
					_audioTween = null;
				}
			}
		}
	
		/// <summary>
		/// 播放音频
		/// </summary>
		/// <param name="audioClip"></param>
		public virtual AudioTween Play(AudioClip audioClip,bool loop=true)
		{
			var audioTween = AudioTween.Get(audioClip.name);
			_audioSource.clip = audioClip;
			_audioSource.loop = loop;
			_audioSource.Play();
			IsPause = false;
			return audioTween;
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public virtual void Pause()
		{
			if (_audioSource.isPlaying)
			{
				_audioSource.Pause();
				_audioTween?.Pause();
				IsPause = true;
			}
		}

		/// <summary>
		/// 恢复
		/// </summary>
		public virtual void Resume()
		{
			if (!_audioSource.isPlaying)
			{
				_audioSource.UnPause();
				_audioTween?.Resume();
				IsPause = false;
			}
		}

		/// <summary>
		/// 停止
		/// </summary>
		public virtual void Stop()
		{
			if (_audioSource.clip!=null)
			{
				_audioSource.Stop();
				_audioTween?.Stop();
				_audioSource.clip = null;
			}
			if (_audioTween != null)
			{
				AudioTween.Release(_audioTween);
				_audioTween = null;
			}
			IsPause = false;
		}

		public virtual void Close()
		{
			Stop();
		}

	}

	/// <summary>
	/// AudioTween
	/// </summary>
	public class AudioTween
	{
		private static Dictionary<int, AudioTween> _activeAudioTween = new Dictionary<int, AudioTween>();
		public static AudioTween Get(string name)
		{
			int id = UnityEngine.Random.Range(0, 1000);
			while (_activeAudioTween.ContainsKey(id))
			{
				id = UnityEngine.Random.Range(0, 1000);
			}
			var at = Pool<AudioTween>.Get();
			at.Flush();
			at.Id = id;
			at.Name = name;
			return at;
		}
		public static void Release(AudioTween at)
		{
			foreach (var item in _activeAudioTween)
			{
				if (item.Value == at)
				{
					_activeAudioTween.Remove(item.Key);
				}
			}
		}

		private Action _onPause;
		private Action _onResume;
		private Action _onStop;
		private Action _onComplete;

		public int Id { get; internal set; }

		public string Name { get; internal set; }

		public void Flush()
		{
			Name = null;
			Id = -1;
			_onPause = null;
			_onResume = null;
			_onStop = null;
			_onComplete = null;
		}

		public void Pause()
		{ 
			_onPause?.Invoke(); 
		}

		public void Resume()
		{
			_onResume?.Invoke();
		}

		public void Stop()
		{
			_onStop?.Invoke();
		}

		public void Complete()
		{
			_onComplete?.Invoke();
		}

		public AudioTween OnPause(Action onPause)
		{
			_onPause = onPause;
			return this;
		}

		public AudioTween OnResume(Action onResume)
		{
			_onResume = onResume;
			return this;
		}

		public AudioTween OnStop(Action onStop)
		{
			_onStop = onStop;
			return this;
		}

		public AudioTween OnComplete(Action onComplete)
		{
			_onComplete = onComplete;
			return this;
		}

	}
	
}