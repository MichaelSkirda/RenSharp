using RenSharp.Core;

namespace RenSharp.Models.Commands
{
	public class Message : Command
	{
		public string Speech { get; set; }
		public string Character { get; set; }
		public Attributes Attributes { get; set; }

		public Message(string speech, string character, Attributes attributes)
		{
			Speech = speech;
			Character = character;
			Attributes = attributes;
		}

        // TODO: bugfix!
        // Message save into MessageHistory
		// not interpolated value.
		// Need to save preview or interpolated value.

        public override void Execute(RenSharpCore core)
		{
			Configuration config = core.Configuration;
			Attributes characterAttributes = Attributes;
			characterAttributes.AddAttributes(core.GetCharacterAttributes(Character));
			characterAttributes.AddDefaultAttributes(config);

			var result = new MessageResult()
			{
				Speech = core.Context.InterpolateString(Speech),
				Character = Character,
				Attributes = characterAttributes
			};

			core.Context.MessageHistory.Push(result);
            config?.Writer?.Write(result);
		}

		public override Command Rollback(RenSharpCore core)
		{
            var command = new MessageRollback(Speech, Character, Attributes)
            {
                Line = Line,
                SourceLine = SourceLine,
                Level = Level
            };

            return command;
		}
	}
}
