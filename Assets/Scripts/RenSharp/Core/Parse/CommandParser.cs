using RenSharp.Models;
using RenSharp.Models.Commands;
using RenSharp.Models.Parse;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RenSharp.Core.Parse
{
    internal static class CommandParser
    {
        internal static Label ParseLabel(string[] args) => new Label(args[1]);
        internal static Pass ParsePass(string[] args) => args.Length == 1 && args[0] == "pass"
            ? new Pass() : throw new ArgumentException($"Can not parse pass at line '{string.Join(" ", args)}'");
        [Obsolete]
        internal static Callback ParseCallback(string[] args) => new Callback(string.Join(" ", args.Skip(1)));
        internal static While ParseWhile(string[] args) => new While(string.Join(" ", args.Skip(1)));
        internal static Python ParsePythonStart(string[] args)
            => args.Length == 1 && args[0] == "python" ? new Python() : throw new ArgumentException($"Can not parse python start block. Value: '{args.ToWord()}'");
        internal static Python ParsePythonSingle(string[] args)
            => new Python() { Commands = new List<string>() { args.Skip(1).ToWord() } };
        internal static Return ParseReturn(string[] args)
        {
            string expression;
            if (args.Length <= 0)
                throw new ArgumentException($"Can not parse '{args.ToWord()}'");
            if (args.Length == 1)
                return new Return("", isSoft: false);

            if (args[0] == "soft")
            {
                expression = args.Skip(2).ToWord();
                return new Return(expression, isSoft: true);
            }
            else if (args[0] == "hard")
            {
                expression = args.Skip(2).ToWord();
                return new Return(expression, isSoft: false);
            }
            else
            {
                expression = args.Skip(1).ToWord();
                return new Return(expression, isSoft: false);
            }
        }
        internal static If ParseIf(string[] args)
        {
            bool isRoot;
            if (args[0] == "if")
                isRoot = true;
            else if (args[0] == "else")
                isRoot = false;
            else
                throw new ArgumentException($"Can not parse If {string.Join(" ", args)}");

            string expression;
            if (isRoot)
                expression = string.Join(" ", args.Skip(1));
            else
                expression = string.Join(" ", args.Skip(2));

            If command = new If(expression, isRoot);
            return command;
        }

        internal static Message ParseMessage(string[] words, Configuration config)
        {
            string line = string.Join(" ", words);

            StringFirstQuotes quotes = RegexMethods.BetweenQuotesFirst(line);

            string command = quotes.Before;
            string text = quotes.Between;
            string attrs = quotes.After;

            Attributes attributes = AttributeParser.ParseAttributes(new string[] { "delay", "name", "nw" }, attrs.Split(' '));
            string character = command
                .Split(' ')
                .Skip(1)
                .FirstOrDefault();

            var message = new Message(text, character, attributes);

            // They won't rewrite current attributes
            List<string> defaultAttributes = new List<string>()
            {
                config.GetDefaultKeyValueString("delay"),
                config.GetDefaultKeyValueString("name")
            };
            message.Attributes.AddAttributes(defaultAttributes, rewrite: false);

            return message;
        }

        

		internal static Repeat ParseRepeat(string[] words)
        {
            string expression = string.Join(" ", words.Skip(1));

            return new Repeat(expression);
        }

        internal static Init ParseInit(string[] words)
        {
            if (words.Length < 1 || words.Length > 3)
                throw new ArgumentException($"Init must have between zero and two argument. '{words.ToWord()}'");
            if (words[0] != "init")
                throw new ArgumentException($"Can not parse '{words.ToWord()}' command.");

            int priority = 0;
            if (words.Length > 1)
                priority = int.Parse(words[1]);

            if (priority < -999 || priority > 999)
                throw new SyntaxErrorException("Приоритет блока 'init' не может быть более 999 или менее -999.");

            // Silence!
            string lastWord = words.Last();
            if (lastWord == "python" || lastWord == "python:")
                return new Init(priority, isPython: true);
            return new Init(priority, isPython: false);
        }

        internal static Jump ParseGoto(string[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("Call can not contains less than 2 arguments");
            if (args[1] == "expression")
                return new Jump(args.Skip(2).ToWord(), evaluate: true);

            if (args.Length != 2)
                throw new ArgumentException("Call without expression keyword can contains only 1 argument (label name).");

            return new Jump($"{args[1]}", evaluate: false);
        }
        internal static Call ParseCall(string[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("Call can not contains less than 1 arguments");
            if (args[1] == "expression")
                return new Call(args.Skip(2).ToWord(), evaluate: true); // skip words 'call', 'expression'

            if (args.Length != 2)
                throw new ArgumentException("Call without expression keyword can contains only 1 argument (label name).");

            return new Call($"{args[1]}", evaluate: false);
        }

        internal static Define ParseDefine(string[] words)
            => new Define(string.Join(' ', words.Skip(1)));

        internal static Exit ParseExit() => new Exit();
    }
}
