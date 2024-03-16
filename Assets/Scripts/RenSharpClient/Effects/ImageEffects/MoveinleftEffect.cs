using RenSharp;
using System;
using System.Collections;

namespace RenSharpClient.Effects.ImageEffects
{
    public static class MoveinleftEffect
    {
        public static IEnumerator moveinleft(EffectData data)
        {
            float startX = data.PointStorage.Find("leftOutScreen").Point.position.x;
            return MoveinEffect.movein(data, startX);
        }


        [PyImport]
        public static Func<EffectData, IEnumerator> moveinleft(float duration)
        {
            return (data) =>
            {
                data.Duration = duration;
                return moveinleft(data);
            };
        }
    }
}
