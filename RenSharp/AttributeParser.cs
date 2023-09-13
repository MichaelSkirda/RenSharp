using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp
{
	internal static class AttributeParser
	{
		internal static int GetDelay(this Attributes attributes)
		{
			string delay = attributes.GetAttributeValue("delay");
			return Int32.Parse(delay);
		}
	}
}
