using RenSharpClient.Models;
using RenSharp.Core.Parse;
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

                if(channel.AudioSource.isPlaying == false && channel.Paused == false)
                    NextTrack(channel);

                SetVolume(channel);
            }
        }

        private void NextTrack(AudioChannel channel)
        {
            if (channel.CurrentPlaying != null)
                channel.PlayedQueue.Enqueue(channel.CurrentPlaying);

            bool queueEmpty = !channel.Queue.Any();
            bool playedEmpty = !channel.PlayedQueue.Any();
            bool loop = channel.Loop;

            if (queueEmpty && playedEmpty)
                return; // Channel have no audio

            if(queueEmpty && loop)
            {
                channel.Queue = channel.PlayedQueue;
                channel.PlayedQueue = new Queue<AudioItem>();
            }

            AudioItem clip = channel.Queue.Dequeue();
            ReplaceCurrentPlaying(channel, clip);
        }

        private void SetVolume(AudioChannel channel)
        {
            AudioSource audioSource = channel.AudioSource;
            AudioItem clip = channel.CurrentPlaying;

            float fadeout = clip?.Attributes?.GetFloatOrNull("fadeout") ?? 0;
            float fadein = clip?.Attributes?.GetFloatOrNull("fadein") ?? 0;
            float volume = clip?.Attributes?.GetVolume() ?? 1f;

            if (fadein <= 0 && fadeout <= 0)
            {
                audioSource.volume = volume;
                return;
            }


            float length = audioSource.clip.length;
            float time = audioSource.time;
            float timeLeft = length - time;


            if (time < fadein)
            {
                audioSource.volume = Mathf.Lerp(0, volume, time / fadein);
            }
            else if (timeLeft < fadeout)
            {
                audioSource.volume = timeLeft / fadeout * volume;
            }
            else
            {
                audioSource.volume = volume;
            }
        }

        internal void Play(PlayResult audio)
		{
            AudioChannel channel = ChannelController.GetChannel(audio.Channel);

            IEnumerable<AudioItem> clips = SoundStorage.GetAudio(audio.ClipNames)
                .Select(x => new AudioItem() { Clip = x, Attributes = audio.Attributes });
            AudioItem clip = clips.First();

            channel.Paused = true;

            var queue = new Queue<AudioItem>(clips.Skip(1));

            channel.Queue.Clear();
            channel.Queue = queue;
            ReplaceCurrentPlaying(channel, clip);
            channel.Paused = false; // Actually not necessary, but I'm scared to delete this line
		}

        internal void Enqueue(PlayResult audio)
        {
            AudioChannel channel = ChannelController.GetChannel(audio.Channel);

            IEnumerable<AudioItem> clips = SoundStorage.GetAudio(audio.ClipNames)
                .Select(x => new AudioItem() { Clip = x, Attributes = audio.Attributes });

            channel.Paused = true;
            foreach(AudioItem clip in clips)
            {
                channel.Queue.Enqueue(clip);
            }
            channel.Paused = false;
        }

        private void ReplaceCurrentPlaying(AudioChannel channel, AudioItem clip)
        {
            channel.Paused = true;

            channel.AudioSource.Stop();
            channel.AudioSource.clip = clip.Clip;
            channel.CurrentPlaying = clip;

            float? fadeIn = clip.Attributes?.GetFloatOrNull("fadein");
            if (fadeIn != null && fadeIn > 0)
                channel.AudioSource.volume = 0;
            else
                channel.AudioSource.volume = clip.Attributes.GetVolume();

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

        // Well... Use this code late to pause/unpause logic
		/* internal void UnPause(string channelName)
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
        } */

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
