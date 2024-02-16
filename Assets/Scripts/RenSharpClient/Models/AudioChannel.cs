using RenSharp.Models;
using System.Collections.Generic;
using UnityEngine;

namespace RenSharpClient.Models
{
    internal class AudioChannel
    {
        public AudioChannel(AudioSource audioSource)
        {
            AudioSource = audioSource;
            Queue = new Queue<AudioClip>();
        }

        internal AudioSource AudioSource { get; set; }
        internal Queue<AudioClip> Queue { get; set; } = new Queue<AudioClip>();
        internal Queue<AudioClip> PlayedQueue { get; set; } = new Queue<AudioClip>();
        internal Attributes Attributes { get; set; }

        /// <summary>
        /// This property does not belongs to AudioSource/AudioClip Paused, it is like mutex to pause queue
        /// </summary>
        internal bool Paused { get; set; } = true;
        internal bool Loop { get; set; }
    }
}
