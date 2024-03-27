using System.Collections.Generic;
using System.Linq;

namespace RenSharp.Models.Commands.Json
{
	public class PythonJson : CommandJson
	{
		public IEnumerable<string> Commands { get; set; } = Enumerable.Empty<string>();
	}
}
