using RenSharp.Models;
using RenSharpClient.Models;
using RenSharpClient.Models.Commands.Results;
using RenSharpClient.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RenSharpClient.Controllers
{
	public class SoundController : MonoBehaviour
	{
		[SerializeField]
		private SoundStorage SoundStorage;
		[SerializeField]
		private AudioSource AudioSourcePrefab;
		[SerializeField]
		private Transform NewSourcesParent;

		private AudioChannelController ChannelController;


        private void Start()
        {
            ChannelController = new AudioChannelController();

            AudioSource musicSource = Instantiate(AudioSourcePrefab, NewSourcesParent);
            AudioSource soundSource = Instantiate(AudioSourcePrefab, NewSourcesParent);

            var musicChannel = new AudioChannel(musicSource) { Loop = true };
            var soundChannel = new AudioChannel(soundSource);

            ChannelController.CreateChannel("music", musicChannel);
            ChannelController.CreateChannel("sound", soundChannel);
        }

        private void Update()
        {
            foreach (KeyValuePair<string, AudioChannel> kv in ChannelController.Channels)
            {
                AudioChannel channel = kv.Value;
                if (channel.Paused)
                    continue;

                AudioSource audioSource = channel.AudioSource;
                if (audioSource.isPlaying)
                    continue;

                channel.PlayedQueue.Enqueue(audioSource.clip);
                Queue<AudioClip> queue = channel.Queue;

                if (queue.Count <= 0)
                {
                    if(channel.Loop)
                    {
                        channel.Queue = channel.PlayedQueue;
                        channel.PlayedQueue.Clear();
                    }
                    else
                    {
                        channel.Paused = true;
                        continue;
                    }
                }

                Attributes attributes = channel.Attributes;
                AudioClip clip = queue.Dequeue();

                Play(channel, clip, attributes, clearQueue: false);
            }
        }

        internal void CreateChannel(string name, bool loop)
		{
			AudioSource audioSource = Instantiate(AudioSourcePrefab, NewSourcesParent);
			audioSource.loop = loop;

			var channel = new AudioChannel(audioSource);
			try
			{
                ChannelController.CreateChannel(name, channel);
            }
			catch(Exception ex)
			{
				Destroy(audioSource);
				throw ex;
			}

        }

		internal void Play(PlayResult audio, bool clearQueue = false)
		{
			AudioChannel channel = ChannelController.GetChannel(audio.Channel);
            AudioClip clip = SoundStorage.GetAudio(audio.Name);
            Attributes attributes = audio.Attributes;
            Play(channel, clip, attributes, clearQueue);
		}

        private void Play(AudioChannel channel, AudioClip clip, Attributes attributes, bool clearQueue = false)
        {
            channel.Paused = true;
            AudioSource audioSource = channel.AudioSource;
            channel.Attributes = attributes;

            if (clearQueue)
                channel.Queue.Clear();

            channel.AudioSource.Stop();
            channel.AudioSource.clip = clip;

            float? fadeIn = attributes.GetFloatOrNull("fadein");
            if (fadeIn != null)
            {
                float volume = channel.Attributes.GetVolume();
                StartCoroutine(Fade(audioSource, fadeIn.Value, start: 0f, target: volume));
            }
            channel.Paused = false;
            channel.AudioSource.Play();
        }

        internal void Stop(StopResult audio)
        {
            AudioChannel channel = ChannelController.GetChannel(audio.Channel);
            channel.Paused = true; // Preventing turning on next from queue, but not stop clip
            channel.Queue.Clear();

            AudioSource audioSource = channel.AudioSource;

            float? fadeOut = audio.Attributes.GetFloatOrNull("fadeout");
            if (fadeOut != null)
            {
                StartCoroutine(Fade(audioSource, fadeOut.Value, start: audioSource.volume, target: 0f, audio => audio.Stop()));
            }
            else
            {
                audioSource.Stop();
            }
        }

		internal void UnPause(string channelName)
		{
            AudioChannel channel = ChannelController.GetChannel(channelName);
            AudioSource audioSource = channel.AudioSource;

            float? fadeIn = channel.Attributes.GetFloatOrNull("fadein");
            if (fadeIn != null)
            {
                float volume = channel.Attributes.GetVolume();
                StartCoroutine(Fade(audioSource, fadeIn.Value, start: 0f, target: volume));
            }

            channel.Paused = false;
			audioSource.UnPause();
        }

        internal void Pause(StopResult audio)
		{
            AudioChannel channel = ChannelController.GetChannel(audio.Channel);
            channel.Paused = true; // Preventing turning on next from queue, but not stop clip

            AudioSource audioSource = channel.AudioSource;

            float? fadeOut = audio.Attributes.GetFloatOrNull("fadeout");
			if (fadeOut != null)
			{
				StartCoroutine(Fade(audioSource, fadeOut.Value, start: audioSource.volume, target: 0f, audio => audio.Pause()));
			}
			else
			{
				audioSource.Pause();
			}
		}

        public void AddAudio(string name, AudioClip clip)
            => SoundStorage.AddAudio(name, clip);

        private IEnumerator Fade(AudioSource audioSource, float duration, float start, float target,
            Action<AudioSource> callback = null)
        {
            float currentTime = 0f;
            audioSource.volume = start;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, target, currentTime / duration);
                yield return null;
            }
            if (callback != null)
                callback(audioSource);
            yield break;
        }
    }
}
