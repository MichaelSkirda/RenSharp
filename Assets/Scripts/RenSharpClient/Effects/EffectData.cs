using RenSharp.Core;
using RenSharpClient.Storage;
using System;
using UnityEngine.UI;

namespace RenSharpClient.Effects
{
    public class EffectData
	{
        public bool IsAppear { get; set; }
        public float Duration { get; set; }
        public float targetX { get; set; }
        public Image Image { get; set; }
        public RenSharpCore Core { get; set; }
        public PointStorage PointStorage { get; set; }
        public Action FinishCallback { get; set; }
    }
}
