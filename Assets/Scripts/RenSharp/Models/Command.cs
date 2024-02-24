using RenSharp.Core;
using RenSharp.Core.Parse;
using RenSharp.Models.Commands.Json;

namespace RenSharp.Models
{
	public abstract class Command
	{
		public string TypeName => GetType().Name.ToLower();
		public int Level { get; set; }
		public int Line { get; set; }
		public int SourceLine { get; set; }
		public abstract void Execute(RenSharpCore core);
		public virtual Command Rollback(RenSharpCore core) => null;

		public bool Is<T>() where T : Command => this is T;
		public bool IsNot<T>() where T : Command => this is not T;

		public void SetLine(ParserContext ctx)
		{
			ctx.Line++;
			Line = ctx.Line;
			SourceLine = ctx.SourceLine;
		}
		public void SetPosition(Command command)
		{
			Line = command.Line;
			SourceLine = command.SourceLine;
			Level = command.Level;
		}

		public void SetPosition(CommandJson command)
		{
			Line = command.Line;
			SourceLine = command.SourceLine;
			Level = command.Level;
		}

	}
}
