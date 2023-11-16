using RenSharp.Core;
using RenSharp.Core.Parse;

namespace RenSharp.Models
{
    public abstract class Command
	{
		public int Level { get; set; }
		public int Line { get; set; }
		public int SourceLine { get; set; }
		public abstract void Execute(RenSharpCore core);
		public virtual Command Rollback(RenSharpCore core) => null;

		public bool IsNot<T>() where T : Command => !(this is T);
		public bool Is<T>() where T : Command => this is T;
		public void SetLine(ParserContext ctx)
		{
			ctx.Line++;
			Line = ctx.Line;
			SourceLine = ctx.SourceLine;
		}
	}
}
