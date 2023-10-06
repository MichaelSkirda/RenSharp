using RenSharp.Models.Commands;
using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RenSharp.Core.Exceptions;

namespace RenSharp.Core.Read
{
	internal class RenSharpValidator
	{
		private Configuration Config { get; set; }

		internal RenSharpValidator(Configuration config)
		{
			Config = config;
		}

		internal void Validate(Command command, Command previousCmd)
		{
			if (command.Level <= 0)
				throw new Exception($"Command '{command.GetType()}' not valid. Tabulation can not be less than zero.");

			if (previousCmd == null)
				return;

			if (command.Level >= previousCmd.Level + 2)
				throw new Exception($"Command '{command.GetType()} not valid. Tabulation can not be higher by two then previous.");

			if (Config.CanPush(previousCmd) == false)
			{
				if (previousCmd.Level < command.Level)
					throw new Exception($"Command '{previousCmd.GetType()}' can not push tabulation.");
			}

			if (Config.IsMustPush(previousCmd))
			{
				if (previousCmd.Level >= command.Level)
					throw new Exception($"Command '{previousCmd.GetType()}' must use tab on next line.");
			}

			if (command is Init && command.Level != 1)
				throw new ArgumentException($"Команда 'Init' должна быть корневой и не иметь отступов.");

		}

		internal void Validate(List<Command> commands)
		{
			IEnumerable<string> repeatingNames = commands
				.OfType<Label>()
				.GroupBy(x => x.Name)
				.Where(x => x.Count() > 1)
				.Select(x => x.Key);

			if(repeatingNames.Count() > 0)
			{
				throw new ArgumentException(Messages.ReapitingLabelNames(repeatingNames));
			}

			Label start = commands
				.OfType<Label>()
				.FirstOrDefault(x => x.Name == "start");

			if (start == null)
				throw new LabelNotExists(Messages.StartNotExist);
			
		}
	}
}
