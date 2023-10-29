using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RenSharpClient.Storage
{
	public class SoundStorage : MonoBehaviour
	{
		[SerializeField]
		private SoundItem[] Data;

		public AudioClip GetAudio(string name)
		{
			SoundItem audio = Data.FirstOrDefault(x => x.Name == name);
			if (audio.AudioClip == null)
				throw new ArgumentException($"Аудио с названием '{name}' не найдено.");

			return audio.AudioClip;
		}
	}
}
