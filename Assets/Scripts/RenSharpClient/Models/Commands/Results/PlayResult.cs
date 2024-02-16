using RenSharp.Models;
using System.Collections.Generic;

namespace RenSharpClient.Models.Commands.Results
{
	public class PlayResult
	{
		public IEnumerable<string> ClipNames { get; set; }
		public string Channel { get; set; }
		public Attributes Attributes { get; set; }
	}
}
