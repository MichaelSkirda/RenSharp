﻿using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core
{
	public class ReaderContext
	{
		internal List<string> SourceCode = new List<string>();
		internal List<Command> Commands = new List<Command>();
		internal int Line { get; set; }
		internal string LineText => SourceCode[Line - 1];

		private Func<ReaderContext, List<Command>> _parseFunc;
		private Func<ReaderContext, Command> _parseSingle;

		internal Func<ReaderContext, List<Command>> ParseFunc
		{
			set => _parseFunc = value;
		}

		internal Func<ReaderContext, Command> ParseSingleFunc
        {
			set => _parseSingle = value;
        }

		internal List<Command> ParseCommands() => _parseFunc(this);
		internal Command ParseSingle() => _parseSingle(this);
	}
}
