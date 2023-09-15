using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenSharp.Models
{
	public class Attributes
	{
		private Dictionary<string, string> values = new Dictionary<string, string>();
		public Attributes(IEnumerable<string> attributes) => AddAttributes(attributes);
		public string GetAttributeValue(string key) => values[key];
		public void AddAttributes(Attributes attributes) => AddAttributes(attributes.KeyValues());
		private IEnumerable<string> KeyValues() => values.Select(x => $"{x.Key}={x.Value}");

		public void AddAttributes(IEnumerable<string> attributes)
		{
			foreach (string attribute in attributes)
			{
				AddAttribute(attribute);
			}
		}

		private void AddAttribute(string attribute)
		{
			string[] keyValue = attribute.Split('=');
			KeyValuePair<string, string> pair;

			if (keyValue.Length == 1)
				pair = new KeyValuePair<string, string>(keyValue[0], "true");
			else if (keyValue.Length == 2)
				pair = new KeyValuePair<string, string>(keyValue[0], keyValue[1]);
			else
				throw new Exception($"Cannot parse attribute '{attribute}'.");

			values[pair.Key] = pair.Value;
		}

		public int GetDelay()
		{
			string delay = GetAttributeValue("delay");
			return Int32.Parse(delay);
		}
		public string GetSpeaker() => GetAttributeValue("name");
	}
}
