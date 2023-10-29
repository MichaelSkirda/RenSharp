using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenSharp.Models
{
	public class Attributes
	{
		public string this[string key]
		{
			get => values[key];
		}

		private Dictionary<string, string> values = new Dictionary<string, string>();
		public Attributes(IEnumerable<string> attributes) => AddAttributes(attributes);
		public Attributes(Dictionary<string, string> attributes) => AddAttributes(attributes);

		public void AddAttributes(Attributes attributes, bool rewrite = true) => AddAttributes(attributes.KeyValues(), rewrite);
		public void AddDefaultAttributes(Configuration config)
			=> AddAttributes(config.GetDefaultAttrbutes(), rewrite: false);
		private IEnumerable<string> KeyValues() => values.Select(x => $"{x.Key}={x.Value}");

		public void AddAttributes(IEnumerable<string> attributes, bool rewrite = true)
		{
			foreach (string attribute in attributes)
			{
				AddAttribute(attribute, rewrite);
			}
		}
		public void AddAttributes(Dictionary<string, string> attributes)
			=> AddAttributes(attributes.Select(kv => $"{kv.Key}={kv.Value}"));
		

		private void AddAttribute(string attribute, bool rewrite)
		{
			string[] keyValue = attribute.Split('=');
			KeyValuePair<string, string> pair;

			if (keyValue.Length == 1)
				pair = new KeyValuePair<string, string>(keyValue[0], "true");
			else if (keyValue.Length == 2)
				pair = new KeyValuePair<string, string>(keyValue[0], keyValue[1]);
			else
				throw new Exception($"Cannot parse attribute '{attribute}'.");

			if(rewrite)
			{
				values[pair.Key] = pair.Value;
			}
			else
			{
				bool hasValue = values.TryGetValue(pair.Key, out _);
				if(!hasValue)
					values[pair.Key] = pair.Value;
			}
		}

		public string GetAttributeValue(string key) => values[key];

		public int GetDelay()
		{
			string delay = GetAttributeValue("delay");
			return Int32.Parse(delay);
		}

		public bool IsClear()
		{
			string str = GetAttributeValue("no-clear");
			return !bool.Parse(str);
		}
		public string GetSpeaker() => GetAttributeValue("name");
	}
}
