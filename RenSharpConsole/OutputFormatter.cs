using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharpConsole
{
	public class OutputFormatter : IFormatter
	{
		internal string Code { get; set; }

		public OutputFormatter()
		{
			Code = "15"; // White
		}

		public string Format(string input)
		{
			return $"\u001b[38;5;{Code}m{input}";
		}

		public string FormatDefault(string input)
		{
			return $"\u001b[38;5;{7}m{input}";
		}

		public void SetFormat(string format)
		{
			Code = format;
		}
	}
}
