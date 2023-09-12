using RenSharp.Models;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RenSharp.Core
{
    internal class RenSharpReader
    {
        internal static List<Command> ParseCode(string path)
        {
            var lines = File.ReadAllLines(path).ToList();
            return ParseCode(lines);
        }

        internal static List<Command> ParseCode(List<string> code)
        {
            RemoveComments(code);
            List<Command> commands = new List<Command>();
            RenSharpContext context = new RenSharpContext();

            int line = 0;
            for (int i = 0; i < code.Count; i++)
            {
                string codeLine = code[i];
                if (NotCommand(codeLine))
                    continue;
                line++;

                try
                {
                    Command command = ParseCommand(codeLine, commands);
                    Validate(command, context, codeLine);
                    context.Level = command.Level;
                    command.Line = line;
                    commands.Add(command);
                }
                catch (Exception ex)
                {
                    throw new Exception($" at line {i}", ex);
                }
            }

            return commands;
        }

        internal static Command ParseCommand(string line, List<Command> commands)
        {
            string[] words = line.Trim().Split(' ');
            string keyword = "";
            string firstArgument = "";
            List<string> args = new List<string>();

            try
            {
                keyword = words[0];
                firstArgument = words[1];
                args = words.Skip(1).ToList();
            }
            catch
            {

            }

            Command command = null;

            if (keyword == "label")
            {
                command = new Label(firstArgument);
            }
            else if (keyword == "character")
            {
                command = new Character(firstArgument, args);
            }
            else if (line.Trim().StartsWith('"'))
            {
                string message = line.GetStringBetween("\"", "\"");
                command = new Message(message, character: "", effects: args);
            }
            else if (keyword == "goto")
            {
                command = new Goto(firstArgument);
            }
            else if (IsCharacter(commands, keyword))
            {
                string message = line.GetStringBetween("\"", "\"");
                command = new Message(message, character: keyword, effects: args);
            }

            if (command == null)
                throw new Exception($"Cannot parse command '{line}'");

            command.Level = GetCommandLevel(line);

            return command;
        }

        private static bool IsCharacter(List<Command> commands, string name)
        {
            IEnumerable<Character> characters = commands.OfType<Character>();
            return characters.Any(x => x.Name == name);
        }

        private static void Validate(Command command, RenSharpContext context, string codeLine)
        {
            if (command.Level <= 0)
                throw new Exception($"Command '{codeLine}' not valid! Tabulation can not be less than zero!");

            if (command.Level >= context.Level + 2)
                throw new Exception($"Command '{codeLine}' not valid! Tabulation can not be higher by two then previous!");
        }

        internal static int GetCommandLevel(string command)
        {
            int level = 0;
            foreach (char chr in command)
            {
                level++;
                if (chr != '\t')
                    break;
            }
            return level;
        }

        internal static void RemoveComments(List<string> code)
        {
            code.ForEach(x => x = x.DeleteAfter("//"));
        }

        private static bool NotCommand(string str)
        {
            return string.IsNullOrEmpty(str.Trim());
        }

    }
}
