using Assets.Scripts.RenSharpClient.Storage;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RenSharpClient.Controllers
{
	public class SoundController : MonoBehaviour
	{
		private AudioSource MusicSource { get; set; }
		private List<AudioSource> SoundSources { get; set; }

		[SerializeField]
		private SoundStorage SoundStorage { get; set; }
		[SerializeField]
		private AudioSource AudioSourcePrefab { get; set; }

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
