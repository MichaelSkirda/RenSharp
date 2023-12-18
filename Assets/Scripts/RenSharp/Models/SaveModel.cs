using System;
using System.Collections.Generic;

namespace RenSharp.Models
{
	public class SaveModel
	{
		// Core
		public bool IsPaused { get; set; }
		public bool HasStarted { get; set; }

		// Program
		public int Line { get; set; }

		// Context
		public IEnumerable<MessageResult> MessageHistory { get; set; }
		public IEnumerable<StackFrame> CallStack { get; set; }
		public IEnumerable<Command> RollbackStack { get; set; }
		public IEnumerable<int> LevelStack { get; set; }

		public string ToJson()
		{
			throw new NotImplementedException();
		}
	}
}
