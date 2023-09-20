using System;
using System.Collections;
using System.Collections.Generic;
using RenSharp;
using RenSharp.Core;
using UnityEngine;

public class CallbackExample
{
    [Callback("gotoLine")]
    public static void FuncGotoLine(RenSharpCore renSharp, string[] args)
    {
        if (args.Length != 1)
            throw new ArgumentException("Wrong count of arguments");
        int line = Int32.Parse(args[0]);
        renSharp.Goto(line);
    }
}
