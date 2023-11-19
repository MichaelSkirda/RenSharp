using System;
using System.Collections.Generic;
using RenSharp.Core.Parse;
using RenSharp.Models;
using RenSharp.Models.Commands;

internal static class IfComplexParser
{
    // Todo rename rootIf. Actually it's can be not root.
    // Not root and root if - at all almost same.
    internal static List<Command> Parse(ParserContext ctx, If rootIf)
    {
        List<Command> commands = new List<Command>() { rootIf };
        int endIfLine;
        while (true)
        {
            Command next = ctx.SeekNext();

            if (IsValid(next, rootIf) == false)
            {
                endIfLine = next.Line;
                break;
            }

            List<Command> parsed = ctx.ParseCommands();
            foreach(Command command in parsed)
            {
                if (IsValid(command, rootIf) == false)
                    throw new ArgumentException($"Не получилось прочесть команду '{command.GetType()}'");
            }
            commands.AddRange(parsed);
        }

        foreach(Command command in commands)
        {
            If cmd = command as If;
            if (cmd == null)
                continue;
            if(cmd.Level == rootIf.Level)
            {
                cmd.EndIfLine = endIfLine;
            }
        }
        rootIf.EndIfLine = endIfLine;
        return commands;
    }

    private static bool IsValid(Command command, If rootIf)
    {
        // Any cmd with lower lvl is exit
        if (command.Level < rootIf.Level)
            return false;

        // Allowed only not root if on same level
        if (command.Level == rootIf.Level)
        {
            if (command.IsNot<If>())
                return false;

            if ((command as If).IsRoot)
                return false;
        }
        return true;
    }

}
