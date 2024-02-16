using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RenSharpClient.Storage
{
	public class SoundStorage : MonoBehaviour
	{
		[SerializeField]
		private List<SoundItem> Data;

		public AudioClip GetAudio(string name)
		{
			SoundItem audio = Data.FirstOrDefault(x => x.Name == name);
			if (audio.AudioClip == null)
				throw new ArgumentException($"Аудио с названием '{name}' не найдено.");

			return audio.AudioClip;
		}

		public IEnumerable<AudioClip> GetAudio(IEnumerable<string> names)
		{
            foreach(string name in names)
			{
				yield return GetAudio(name);
			}
        }

		public void AddAudio(string name, AudioClip clip)
		{
			Data.RemoveAll(x =>  x.Name == name);
			var item = new SoundItem() { Name = name, AudioClip = clip };
			Data.Add(item);
		}
	}
}
