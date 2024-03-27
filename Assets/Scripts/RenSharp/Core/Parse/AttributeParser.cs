using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenSharp.Core.Parse
{
	internal static class AttributeParser
	{

		internal static Attributes ParseAttributes(IEnumerable<string> keys, IEnumerable<string> words)
		{
			var attributes = new Dictionary<string, string>();

			// Null to fall with exception if first word not keyword
			string currentKey = null;

			foreach (string word in words)
			{
				if (string.IsNullOrEmpty(word))
					continue;
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
