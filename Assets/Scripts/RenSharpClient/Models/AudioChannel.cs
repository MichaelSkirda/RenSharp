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
        internal Queue<AudioClip> Queue { get; set; }
        internal Queue<AudioClip> PlayedQueue { get; set; }
        internal Attributes Attributes { get; set; }
        internal bool Paused { get; set; }
        internal bool Loop { get; set; }
    }
}
