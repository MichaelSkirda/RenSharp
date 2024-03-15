using RenSharp;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RenSharpClient.Effects.ImageEffects
{
    public static class MoveinrightEffect
    {

        public static IEnumerator moveinright(EffectData data)
        {
            float time = 0;
            float duration = data.Duration;
            float startX = data.PointStorage.Find("rightOutScreen").Point.position.x;
            float targetX = data.targetX;
            float deltaX = startX - targetX;
            float currentX;

            Image sprite = data.Image;

            RectTransform rect = sprite.GetComponent<RectTransform>();

            float y = rect.rect.height / 2;

            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);

            while (time < duration)
            {
                currentX = startX - (time / duration * deltaX);
                rect.position = new Vector2(currentX, y);

                time += Time.deltaTime;
                yield return null;
            }
            Debug.Log("Finished!");
            rect.position = new Vector2(targetX, y);
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
