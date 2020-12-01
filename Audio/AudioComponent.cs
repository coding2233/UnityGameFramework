using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Wanderer.GameFramework
{
	public class AudioComponent : MonoBehaviour
	{
		//声音混合器
		[SerializeField]
		private AudioMixer _audioMixer;

		private void Awake()
		{
			//获取所有的音频混合组
			List<AudioMixerGroup> amgs = new List<AudioMixerGroup>();
			amgs.AddRange(_audioMixer.FindMatchingGroups("Master"));
			//music
			AudioSource asMusic = gameObject.AddComponent<AudioSource>();
			asMusic.volume = 0.3f;
			asMusic.playOnAwake = false;
			asMusic.loop = true;
			asMusic.outputAudioMixerGroup = amgs.Find(x => x.name.Equals("Music"));
			//sound
			AudioSource asSound = gameObject.AddComponent<AudioSource>();
			asSound.playOnAwake = false;
			asSound.outputAudioMixerGroup = amgs.Find(x => x.name.Equals("Sound"));
			//uiSound
			AudioSource asUISound = gameObject.AddComponent<AudioSource>();
			asUISound.playOnAwake = false;
			asUISound.outputAudioMixerGroup = amgs.Find(x => x.name.Equals("UISound"));

			//设置音频管理器
			GameFrameworkMode.GetModule<AudioManager>().SetDefaultAudioSource(asMusic,asUISound,asSound);
		}
	}
}