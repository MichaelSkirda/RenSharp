using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core
{
	internal class ReaderContext
	{
		internal List<string> SourceCode = new List<string>();
		internal List<Command> Commands = new List<Command>();
		internal int Line { get; set; }
		internal string LineText => SourceCode[Line - 1];
	}
}
