﻿using Microsoft.Scripting.Actions;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.RenSharpClient
{
	internal static class AttributeParser
	{

		internal static Attributes ParseAttributes(IEnumerable<string> keys, IEnumerable<string> words)
		{
			var attributes = new Dictionary<string, string>();

			string currentKey = null ;

			foreach (string word in words)
			{
				if (keys.Contains(word))
				{
					currentKey = word;
					attributes[currentKey] = string.Empty;
					continue;
				}

				if (currentKey == null)
					throw new ArgumentException($"Unexpected attribute '{word}'.");

				// It is OK not string builder. Most of case there is only one value.
				attributes[currentKey] += word;
			}

			return new Attributes(attributes);
		}

	}
}
