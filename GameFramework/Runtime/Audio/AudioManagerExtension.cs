using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public static class AudioManagerExtension
	{
		#region Music Audio Manager
		public static AudioTween MusicPlay(this AudioManager audio,string assetName,bool loop)
		{
			return audio.Play<MusicAudioPlayer>(assetName, loop);
		}
		public static void MusicPause(this AudioManager audio)
		{
			audio.Pause<MusicAudioPlayer>();
		}
		public static void MusicResume(this AudioManager audio)
		{
			audio.Resume<MusicAudioPlayer>();
		}
		public static void MusicStop(this AudioManager audio)
		{
			audio.Stop<MusicAudioPlayer>();
		}
		public static bool GetMusicMute(this AudioManager audio)
		{
			return audio.GetAudioPlayer<MusicAudioPlayer>().Mute;
		}

		public static void SetMusicMute(this AudioManager audio,bool mute)
		{
			audio.GetAudioPlayer<MusicAudioPlayer>().Mute = mute;
		}

		public static float GetMusicVolume(this AudioManager audio, float volume)
		{
			return audio.GetAudioPlayer<MusicAudioPlayer>().Volume;
		}

		public static void SetMusicVolume(this AudioManager audio, float volume)
		{
			audio.GetAudioPlayer<MusicAudioPlayer>().Volume = volume;
		}
		#endregion

		#region Sound Audio Manager
		public static AudioTween SoundPlay(this AudioManager audio, string assetName, bool loop)
		{
			return audio.Play<SoundAudioPlayer>(assetName, loop);
		}
		public static void SoundPause(this AudioManager audio)
		{
			audio.Pause<SoundAudioPlayer>();
		}
		public static void SoundResume(this AudioManager audio)
		{
			audio.Resume<SoundAudioPlayer>();
		}
		public static void SoundStop(this AudioManager audio)
		{
			audio.Stop<SoundAudioPlayer>();
		}

		public static bool GetSoundMute(this AudioManager audio)
		{
			return audio.GetAudioPlayer<SoundAudioPlayer>().Mute;
		}

		public static void SetSoundMute(this AudioManager audio, bool mute)
		{
			audio.GetAudioPlayer<SoundAudioPlayer>().Mute = mute;
		}

		public static float GetSoundVolume(this AudioManager audio)
		{
			return audio.GetAudioPlayer<SoundAudioPlayer>().Volume;
		}

		public static void SetSoundVolume(this AudioManager audio, float volume)
		{
			audio.GetAudioPlayer<SoundAudioPlayer>().Volume = volume;
		}
		#endregion

		#region UISound
		public static bool GetUISoundMute(this AudioManager audio)
		{
			return audio.GetAudioPlayer<UISoundAudioPlayer>().Mute;
		}

		public static void SetUISoundMute(this AudioManager audio, bool mute)
		{
			audio.GetAudioPlayer<UISoundAudioPlayer>().Mute = mute;
		}

		public static float GetUISoundVolume(this AudioManager audio)
		{
			return audio.GetAudioPlayer<UISoundAudioPlayer>().Volume;
		}

		public static void SetUISoundVolume(this AudioManager audio, float volume)
		{
			audio.GetAudioPlayer<UISoundAudioPlayer>().Volume = volume;
		}
		#endregion
	}
}