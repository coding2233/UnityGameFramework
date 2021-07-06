using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class SoundAudioPlayer : AudioPlayer
	{


		public SoundAudioPlayer(AudioSource audioSource) : base(audioSource)
		{
		}

		/// <summary>
		/// 播放音频
		/// </summary>
		/// <param name="audioClip"></param>
		public override AudioTween Play(AudioClip audioClip,bool loop=false)
		{
			if (_audioSource.isPlaying)
				return _audioTween;

			return base.Play(audioClip, loop);
		}
	}
}