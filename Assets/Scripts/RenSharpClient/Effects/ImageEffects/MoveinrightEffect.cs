﻿using RenSharp;
using System;
using System.Collections;
namespace RenSharpClient.Effects.ImageEffects
{
    public static class MoveinrightEffect
    {

        public static IEnumerator moveinright(EffectData data)
        {
            float startX = data.PointStorage.Find("rightOutScreen").Point.position.x;
            return MoveinEffect.movein(data, startX);
        }


        [PyImport]
        public static Func<EffectData, IEnumerator> moveinright(float duration)
        {
            return (data) =>
            {
                data.Duration = duration;
                return moveinright(data);
            };
        }

    }
}
