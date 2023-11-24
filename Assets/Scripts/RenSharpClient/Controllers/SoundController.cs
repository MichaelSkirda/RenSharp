using RenSharp.Models;
using RenSharpClient.Models.Commands.Results;
using RenSharpClient.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RenSharpClient.Controllers
{
	public class SoundController : MonoBehaviour
	{
		private List<AudioSource> SoundSources { get; set; } = new List<AudioSource>();

		[SerializeField]
		private SoundStorage SoundStorage;
		[SerializeField]
		private AudioSource AudioSourcePrefab;
		[SerializeField]
		private AudioSource MusicSource;

		public void AddAudio(string name, AudioClip clip)
			=> SoundStorage.AddAudio(name, clip);

		public AudioSource PlaySound(string name)
		{
			AudioSource freeSource = SoundSources.FirstOrDefault(x => x.isPlaying == false);
			AudioClip clip = SoundStorage.GetAudio(name);

			if(freeSource == null)
			{
				freeSource = Instantiate(AudioSourcePrefab);
				SoundSources.Add(freeSource);
			}

			freeSource.clip = clip;
			return freeSource;
		}

		public AudioSource PlayMusic(string name)
		{
			AudioClip clip = SoundStorage.GetAudio(name);
			MusicSource.clip = clip;
			return MusicSource;
		}

		public void PauseMusic() => MusicSource.Pause();
		public void StopMusic() => MusicSource.Stop();
		public void RestartMusic()
		{
			MusicSource.Stop();
			MusicSource.Play();
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
				StartCoroutine(FadeIn(audioSource, fadeIn.Value));
			}

			audioSource.Play();
		}

		private IEnumerator FadeIn(AudioSource audioSource, float duration)
		{
			float currentTime = 0f;
			float start = 0f;
			audioSource.volume = start;

			while (currentTime < duration)
			{
				currentTime += Time.deltaTime;
				audioSource.volume = Mathf.Lerp(start, 1f, currentTime / duration);
				yield return null;
			}
			yield break;
		}
	}
}
