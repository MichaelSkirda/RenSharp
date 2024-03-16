using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RenSharpClient.Effects.ImageEffects
{
    internal static class MoveinEffect
    {
        public static IEnumerator movein(EffectData data, float startX)
        {
            float time = 0;
            float duration = data.Duration;
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
            rect.position = new Vector2(targetX, y);
        }
    }
}
