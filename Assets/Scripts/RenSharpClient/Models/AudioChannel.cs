using RenSharp.Models;
using System.Collections.Generic;
using UnityEngine;

namespace RenSharpClient.Models
{
    internal class AudioChannel
    {
        internal AudioChannel(AudioSource audioSource)
        {
            AudioSource = audioSource;
        }

        internal AudioSource AudioSource { get; set; }
        internal AudioItem CurrentPlaying { get; set; }
        internal Queue<AudioItem> Queue { get; set; } = new Queue<AudioItem>();
        internal Queue<AudioItem> PlayedQueue { get; set; } = new Queue<AudioItem>();

        /// <summary>
        /// This property does not belongs to AudioSource/AudioClip Paused, it is like mutex to pause queue
        /// </summary>
        internal bool Paused { get; set; } = true;
        internal bool Loop { get; set; }
    }
}
