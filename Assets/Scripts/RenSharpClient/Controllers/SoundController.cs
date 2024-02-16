using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharpClient.Models;
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

            CreateChannel("music", loop: true);
            CreateChannel("sound", loop: false);
            CreateChannel("voice", loop: false);
        }

        private void Update()
        {
            foreach (KeyValuePair<string, AudioChannel> kv in ChannelController.Channels)
            {
                AudioChannel channel = kv.Value;
                if (channel.Paused)
                    continue;

                AudioSource audioSource = channel.AudioSource;
                Attributes attributes;

                if (audioSource.isPlaying)
                {
                    attributes = channel.Attributes;
                    float? fadeout = attributes?.GetFloatOrNull("fadeout");
                    if (fadeout == null || fadeout.Value <= 0)
                        continue;

                    float timeLeft = audioSource.clip.length - audioSource.time;

                    if (timeLeft < fadeout)
                    {
                        float volume = attributes.GetVolume();
                        audioSource.volume = timeLeft / fadeout.Value * volume;
                    }

                    continue;
                }

                if(audioSource.clip != null)
                    channel.PlayedQueue.Enqueue(audioSource.clip);

                if (channel.Queue.Count <= 0)
                {
                    if (channel.PlayedQueue.Any() == false)
                        continue;

                    if(channel.Loop)
                    {
                        channel.Queue = channel.PlayedQueue;
                        channel.PlayedQueue = new Queue<AudioClip>();
                    }
                    else
                    {
                        channel.Paused = true;
                        continue;
                    }
                }

                AudioClip clip = channel.Queue.Dequeue();

                attributes = channel.Attributes;
                audioSource.volume = attributes.GetVolume();

                ReplaceCurrentPlaying(channel, clip, attributes);
            }
        }

		internal void Play(PlayResult audio)
		{
            AudioChannel channel = ChannelController.GetChannel(audio.Channel);
            IEnumerable<AudioClip> clips = SoundStorage.GetAudio(audio.ClipNames);

            AudioClip clip = clips.First();
            Queue<AudioClip> queue = new Queue<AudioClip>(clips.Skip(1));

            channel.Paused = true;
            channel.Queue.Clear();
            channel.Queue = queue;
            ReplaceCurrentPlaying(channel, clip, audio.Attributes);
            channel.Paused = false; // Actually not necessary, but I'm scared to delete this line
		}

        internal void Enqueue(AudioChannel channel, IEnumerable<AudioClip> clips)
        {
            channel.Paused = true;
            foreach(AudioClip clip in clips)
            {
                channel.Queue.Enqueue(clip);
            }
            channel.Paused = false;
        }

        private void ReplaceCurrentPlaying(AudioChannel channel, AudioClip clip, Attributes attributes)
        {
            channel.Paused = true;
            AudioSource audioSource = channel.AudioSource;
            channel.Attributes = attributes;

            channel.AudioSource.Stop();
            channel.AudioSource.clip = clip;

            float? fadeIn = attributes?.GetFloatOrNull("fadein");
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

        internal void CreateChannel(string name, bool loop)
        {
            if (RegexMethods.IsValidCharacterName(name) == false)
                throw new ArgumentException("Name of new channel must be word without whitespace characters." +
                    " You can use underscores ('_') as word delimeters.", nameof(name));
            AudioSource audioSource = Instantiate(AudioSourcePrefab, NewSourcesParent);
            audioSource.loop = false; // SoundController have loop mechanism with queue

            var channel = new AudioChannel(audioSource);
            channel.Loop = loop;
            channel.Paused = false;

            try
            {
                ChannelController.CreateChannel(name, channel);
            }
            catch
            {
                Destroy(audioSource);
                throw;
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
