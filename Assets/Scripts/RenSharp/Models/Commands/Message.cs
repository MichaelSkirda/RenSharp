using RenSharp.Core;

namespace RenSharp.Models.Commands
{
	public class Message : Command, IRollbackable
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
			MessageResult result = WriteMessage(core);
			core.Context.MessageHistory.Push(result);
		}

		protected MessageResult WriteMessage(RenSharpCore core)
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


			float? delay = characterAttributes.GetNw();

			if (delay == null || delay < 0)
				config?.Writer?.Write(result);
			else
			{
				// Auto after writen read next command
				config?.Writer?.Write(result, delay.Value, () =>
				{
					try
					{
						core.ReadNext();
					}
					catch
					{
						// WARNING empty catch.
						// important to not fall!
						// because callback will be called from coroutine in Unity implemetantion
					}
				});
			}

			return result;
		}

		public Command Rollback(RenSharpCore core)
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
