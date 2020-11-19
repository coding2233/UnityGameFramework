using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Wanderer.GameFramework
{
	public class UISoundAudioPlayer : AudioPlayer
	{
		//ui音效的队列
		private Queue<AudioSource> _uiSoundQueue = new Queue<AudioSource>();
		//uisound正在播放的列表
		private Dictionary<AudioTween, AudioSource> _uiSoundActiveSource = new Dictionary<AudioTween, AudioSource>();
		//uisound暂停的资源列表
		private HashSet<AudioTween> _uiSoundPauseSource = new HashSet<AudioTween>();

		public UISoundAudioPlayer(AudioSource audioSource) : base(audioSource)
		{
		}

		/// <summary>
		/// 播放音频
		/// </summary>
		/// <param name="audioClip"></param>
		public override AudioTween Play(AudioClip audioClip, bool loop)
		{
			var audioTween = AudioTween.Get(audioClip.name);
			var uiAudioSource = GetUIAudioSource();
			uiAudioSource.loop = loop;
			_uiSoundActiveSource.Add(audioTween, uiAudioSource);
			return audioTween;
		}

		public override void Update()
		{
			if (_uiSoundActiveSource.Count > 0)
			{
				foreach (var item in _uiSoundActiveSource)
				{
					if (!item.Value.loop && !item.Value.isPlaying && !_uiSoundPauseSource.Contains(item.Key))
					{
						item.Key.Complete();
						item.Value.clip = null;
						//回收
						_uiSoundQueue.Enqueue(item.Value);
						AudioTween.Release(item.Key);
						//移除
						_uiSoundActiveSource.Remove(item.Key);
					}
				}
			}
		}

		public override void Pause()
		{
		}

		public void Pasue(AudioTween tween)
		{
			if (_uiSoundActiveSource.TryGetValue(tween, out AudioSource audioSource))
			{
				if (audioSource.isPlaying)
				{
					audioSource.Pause();
					tween.Pause();
					_uiSoundPauseSource.Add(tween);
				}
			}
		}

		public override void Resume()
		{
		}

		public void Resume(AudioTween tween)
		{
			if (_uiSoundActiveSource.TryGetValue(tween, out AudioSource audioSource))
			{
				if (!audioSource.isPlaying)
				{
					audioSource.UnPause();
					tween.Resume();
					_uiSoundPauseSource.Remove(tween);
				}
			}
		}

		public override void Stop()
		{
		}

		public void Stop(AudioTween tween)
		{
			if (_uiSoundActiveSource.TryGetValue(tween, out AudioSource audioSource))
			{
				if (audioSource.clip!=null)
				{
					audioSource.Stop();
					tween.Stop();
					
					audioSource.clip = null;
				}
				if (_uiSoundPauseSource.Contains(tween))
					_uiSoundPauseSource.Remove(tween);
				//回收
				_uiSoundQueue.Enqueue(audioSource);
				AudioTween.Release(tween);
				//移除
				_uiSoundActiveSource.Remove(tween);
			}
		}

		public void StopAll()
		{
			if (_uiSoundActiveSource.Count >0)
			{
				foreach (var item in _uiSoundActiveSource)
				{
					Stop(item.Key);
				}
				_uiSoundActiveSource.Clear();
			}
		}

		public override void Close()
		{
			StopAll();
		}


		#region 内部函数
		/// <summary>
		/// 获取UIAudio的Source
		/// </summary>
		/// <returns></returns>
		private AudioSource GetUIAudioSource()
		{
			AudioSource uiAuidoSource = null;
			if (_uiSoundQueue.Count > 0)
			{
				uiAuidoSource = _uiSoundQueue.Dequeue();
			}
			if (uiAuidoSource == null)
			{
				uiAuidoSource = _audioSource.Clone();
			}
			return uiAuidoSource;
		}
		#endregion
	}
}