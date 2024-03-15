using RenSharp.Core;

namespace RenSharp.Models.Commands
{
    public class Nop : Command
    {
        public readonly string Message;

        public Nop()
        {
            Message = "No message provided.";
        }

        public Nop(string message)
        {
            Message = message;
        }

        public override void Execute(RenSharpCore core)
        {
            // Do nothing
        }
    }
}
