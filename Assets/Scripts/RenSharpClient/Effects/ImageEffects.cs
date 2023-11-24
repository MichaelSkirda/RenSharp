using RenSharp;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RenSharpClient.Effects
{
	public static class ImageEffects
	{
		public static IEnumerator Dissolve(Image sprite, EffectData data)
		{
			float time = 0;
			float duration = data.Duration;
			Color startValue;
			Color targetColor;

			if(data.IsAppear)
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
		}

		[PyImport]
		public static Func<Image, EffectData, IEnumerator> Dissolve(float duration)
		{
			return (image, data) =>
			{
				data.Duration = duration;
				return Dissolve(image, data);
			};
		}
	}
}
