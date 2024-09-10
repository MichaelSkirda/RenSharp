using RenSharp;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RenSharpClient.Effects.ImageEffects
{
    public static class DissolveEffect
    {
        public static IEnumerator Dissolve(EffectData data)
        {
            float time = 0;
            float duration = data.Duration;
            Color startValue;
            Color targetColor;
            Image sprite = data.Image;

            if (data.IsAppear)
            {
                startValue = sprite.color;
                startValue.a = 0;
                targetColor = sprite.color;
            }
            else
            {
                startValue = sprite.color;
                targetColor = new Color(0, 0, 0);
            }

            while (time < duration)
            {
                sprite.color = Color.Lerp(startValue, targetColor, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            sprite.color = targetColor;

            if (data.IsAppear == false)
                UnityEngine.Object.Destroy(sprite);

            data.FinishCallback?.Invoke();
        }


        [PyImport]
        public static Func<EffectData, IEnumerator> dissolve(float duration)
        {
            return (data) =>
            {
                data.Duration = duration;
                return Dissolve(data);
            };
        }
    }
}
