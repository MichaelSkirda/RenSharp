using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp
{
	internal static class DefaultConfigurationExtension
	{
		public static void UseDefaultSkips(this Configuration config)
		{
			config.Skip<Label>();
			config.Skip<Goto>();
		}
	}
}
