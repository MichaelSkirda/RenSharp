using RenSharp.Models;
using RenSharpClient.Models.Commands.Results;
using RenSharpClient.Storage;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace RenSharpClient.Controllers
{
	public class SoundController : MonoBehaviour
	{
		[SerializeField]
		private SoundStorage SoundStorage;
		[SerializeField]
		private AudioSource MusicSource;
		[SerializeField]
		private AudioSource SoundSource;

		public void AddAudio(string name, AudioClip clip)
			=> SoundStorage.AddAudio(name, clip);

		public AudioSource PlaySound(string name)
		{
			AudioClip clip = SoundStorage.GetAudio(name);
			SoundSource.clip = clip;
			return SoundSource;
		}

		public AudioSource PlayMusic(string name)
		{
			AudioClip clip = SoundStorage.GetAudio(name);
			MusicSource.clip = clip;
			return MusicSource;
		}

		internal void Play(PlayResult audio)
		{
			string name = audio.Name;
			string channel = audio.Channel;
			Attributes attributes = audio.Attributes;

			AudioSource audioSource;
			if (channel == "music")
				audioSource = PlayMusic(name);
			else if (channel == "sound")
				audioSource = PlaySound(name);
			else
				throw new InvalidOperationException($"Неизвестный аудио канал '{channel}'. Используйте 'play music' или 'play sound'.");

			float? fadeIn = attributes.GetFloatOrNull("fadein");
			if(fadeIn != null)
			{
				StartCoroutine(FadeIn(audioSource, fadeIn.Value, target: 1f));
			}

			audioSource.Play();
		}
		internal void Pause(StopResult audio)
		{
			string channel = audio.Channel;
			Attributes attributes = audio.Attributes;
			AudioSource audioSource;

			if (channel == "music")
				audioSource = MusicSource;
			else if (channel == "sound")
				audioSource = SoundSource;
			else
				throw new InvalidOperationException($"Неизвестный аудио канал '{channel}'. Используйте 'play music' или 'play sound'.");

			float? fadeOut = attributes.GetFloatOrNull("fadeout");
			if (fadeOut != null)
			{
				StartCoroutine(FadeIn(audioSource, fadeOut.Value, target: 0f));
			}
		}

		private IEnumerator FadeIn(AudioSource audioSource, float duration, float target)
		{
			float currentTime = 0f;
			float start = 0f;
			audioSource.volume = start;

			while (currentTime < duration)
			{
				currentTime += Time.deltaTime;
				audioSource.volume = Mathf.Lerp(start, target, currentTime / duration);
				yield return null;
			}
			yield break;
		}
	}
}
