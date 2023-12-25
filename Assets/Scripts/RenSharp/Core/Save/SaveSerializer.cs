using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Text;

namespace RenSharp.Core.Save
{
	internal static class SaveSerializer
	{
		private static string valueDelimiter = "---";
		private static string categoryDelimiter = "###";
		private static string newLine = Environment.NewLine;

		internal static string Serialize(SaveModel save)
		{
			// Header
			var data = new StringBuilder(DateTime.Now.ToString("G"));
			data.AppendLine(save.IsPaused.ToString());
			data.AppendLine(save.HasStarted.ToString());
			data.AppendLine(save.Line.ToString());


			// Message History


			// Rollback stack
			data.AppendLine(categoryDelimiter + "_ROLLBACK_STACK");

			foreach (Command command in save.RollbackStack)
			{
				data.AppendLine(Serialize(command));
				data.AppendLine(valueDelimiter);
			}

			// Callstack
			// Callstack last (or first) is CurrentFrame
			data.AppendLine(categoryDelimiter + "_CALLSTACK");

			foreach (StackFrame frame in save.CallStack)
			{
				data.AppendLine(Serialize(frame));
				data.AppendLine(valueDelimiter);
			}
			data.AppendLine(Serialize(save.CurrentFrame));

			throw new NotImplementedException();
		}

		private static string Serialize(Command command)
		{
			Type commandType = command.GetType();
			var result = new StringBuilder(
				  commandType.Name + newLine
				+ command.Level + newLine
				+ command.Line + newLine
				+ command.SourceLine
				);

			if(commandType == typeof(Call))
			{
				Call cmd = (Call)command;
				result.Append(
					  cmd.Expression + newLine
					+ cmd.Evaluate);
			}
			else if(commandType == typeof(Callback))
			{
				Callback cmd = (Callback)command;
			}
			else if (commandType == typeof(Define))
			{
				Define cmd = (Define)command;
			}
			else if (commandType == typeof(Exit))
			{
				Exit cmd = (Exit)command;
			}
			else if (commandType == typeof(If))
			{
				If cmd = (If)command;
			}
			else if (commandType == typeof(Init))
			{
				Init cmd = (Init)command;
			}
			else if (commandType == typeof(Jump))
			{
				Jump cmd = (Jump)command;
			}
			else if (commandType == typeof(Label))
			{
				Label cmd = (Label)command;
			}
			else if (commandType == typeof(Message))
			{
				Message cmd = (Message)command;
			}
			else if (commandType == typeof(MessageRollback))
			{
				MessageRollback cmd = (MessageRollback)command;
			}
			else if (commandType == typeof(Pass))
			{
				Pass cmd = (Pass)command;
			}
			else if (commandType == typeof(Python))
			{
				Python cmd = (Python)command;
			}
			else if (commandType == typeof(Repeat))
			{
				Repeat cmd = (Repeat)command;
			}
			else if (commandType == typeof(SysSetScope))
			{
				SysSetScope cmd = (SysSetScope)command;
			}
			else if (commandType == typeof(While))
			{
				While cmd = (While)command;
			}
			else
			{
				throw new InvalidCastException
					($"Can not parse command {commandType.Name} at line {command.SourceLine} during loading save.");
			}

			return result.ToString();
		}

		private static string Serialize(StackFrame stackFrame)
		{
			return
				stackFrame.Line
				+ newLine
				+ string.Join(',', stackFrame.LevelStack);
		}

		internal static SaveModel Deserialize(string data)
		{
			string[] lines = data.Split('\n');

			DateTime saveDate = DateTime.Parse(lines[0]);
			bool isPaused = bool.Parse(lines[1]);
			bool hasStarted = bool.Parse(lines[2]);
			int line = int.Parse(lines[3]);

			throw new NotImplementedException();
		}
	}
}
