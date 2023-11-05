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

		public bool TryGetValue(string key, out string result)
			=> values.TryGetValue(key, out result);

		public int? GetIntOrNull(string key)
		{
			bool notFound = !values.TryGetValue(key, out string str);
			if (notFound)
				return null;

			bool notParsed = !Int32.TryParse(str, out int result);
			if (notParsed)
				return null;
			return result;
		}

		private Dictionary<string, string> values { get; set; } = new Dictionary<string, string>();

		public Attributes() { }
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
		

		public void AddAttribute(string attribute, bool rewrite)
		{
			string[] keyValue = attribute.Split('=');
			KeyValuePair<string, string> pair;

			if (keyValue.Length == 1)
				pair = new KeyValuePair<string, string>(keyValue[0], "true");
			else if (keyValue.Length == 2)
				pair = new KeyValuePair<string, string>(keyValue[0], keyValue[1]);
			else
				throw new Exception($"Cannot parse attribute '{attribute}'.");

			AddAttribute(pair.Key, pair.Value, rewrite);
		}

		public void AddAttribute(string name, string value, bool rewrite)
		{
			if(rewrite)
			{
				values[name] = value;
			}
			else
			{
				bool hasValue = values.ContainsKey(name);
				if(hasValue == false)
					values[name] = value;
			}
		}

		public string GetAttributeValue(string key) => values[key];
		public bool ContainsKey(string key) => values.ContainsKey(key);

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
