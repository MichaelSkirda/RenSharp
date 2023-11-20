using RenSharpClient.Storage;
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

		public void PlaySound(string name)
		{
			AudioSource freeSource = SoundSources.FirstOrDefault(x => x.isPlaying == false);
			AudioClip clip = SoundStorage.GetAudio(name);

			if(freeSource == null)
			{
				freeSource = Instantiate(AudioSourcePrefab);
				SoundSources.Add(freeSource);
			}

			freeSource.clip = clip;
			freeSource.Play();
		}

		public void PlayMusic(string name)
		{
			AudioClip clip = SoundStorage.GetAudio(name);
			MusicSource.clip = clip;
			MusicSource.Play();
		}

		public void PauseMusic() => MusicSource.Pause();
		public void StopMusic() => MusicSource.Stop();
		public void RestartMusic()
		{
			MusicSource.Stop();
			MusicSource.Play();
		}

	}
}
