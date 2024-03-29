﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RenSharp.Models
{
	public class Attributes
	{
		public Dictionary<string, string> values { get; set; } = new Dictionary<string, string>();
		public string this[string key]
		{
			get => values[key];
		}

		public Attributes() { }
		public Attributes(IEnumerable<string> attributes)
		{
			if (attributes == null)
				attributes = new List<string>();
			AddAttributes(attributes);
		}
		public Attributes(Dictionary<string, string> attributes)
		{
			AddAttributes(attributes);
		}
		public static Attributes Empty() => new Attributes();

		public void Remove(string key)
			=> values.Remove(key);

		public bool TryGetValue(string key, out string result)
			=> values.TryGetValue(key, out result);

		public string GetValueOrNull(string key)
		{
			bool hasValue = TryGetValue(key, out string result);
			if (hasValue)
				return result;
			return null;
		}
		
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

		public float? GetFloatOrNull(string key)
		{
			bool notFound = !values.TryGetValue(key, out string str);
			if (notFound)
				return null;

			try
			{
				float result = float.Parse(str, CultureInfo.InvariantCulture);
				return result;
			}
			catch
			{
				return null;
			}
		}

		public bool? GetBoolOrNull(string key)
		{
            bool notFound = !values.TryGetValue(key, out string value);
            if (notFound)
                return null;

            try
            {
                bool result = bool.Parse(value);
                return result;
            }
            catch
            {
				if (value == "1" || value.Trim() == string.Empty)
					return true;
                return null;
            }
        }

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

		public void AddAttribute(string key, string value, bool rewrite)
		{
			if(rewrite)
			{
				values[key] = value;
			}
			else
			{
				bool hasValue = values.ContainsKey(key);
				if(hasValue == false)
					values[key] = value;
			}
		}

		public string GetAttributeValue(string key) => values[key];
		public bool ContainsKey(string key) => values.ContainsKey(key);

		public int GetDelay()
		{
			string delay = GetAttributeValue("delay");
			return Int32.Parse(delay);
		}

        public float? GetVolumeOrDefault()
        {
            float? volume = GetFloatOrNull("volume");
            if (volume < 0f)
                volume = 0f;
            else if (volume > 1f)
                volume = 1f;
            return volume;
        }

        public float GetVolume()
		{
			float volume = GetVolumeOrDefault() ?? 1f;
			return volume;
		}

		public float GetCurrentTrackVolumeOrVolume()
		{
			float volume = GetFloatOrNull("current-track-volume") ?? GetVolume();
			return volume;
		}

        public bool IsClear()
		{
			string str = GetAttributeValue("no-clear");
			return !bool.Parse(str);
		}
		public string GetSpeaker() => GetAttributeValue("name");

        internal float? GetNw()
        {
			float? delay = GetFloatOrNull("nw");

			if (delay == null && values.ContainsKey("nw"))
				delay = 1;

			return delay;
        }
    }
}
