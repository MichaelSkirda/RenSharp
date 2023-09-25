using RenSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models.Commands
{
	internal class Pass : Command
	{
		internal override void Execute(RenSharpCore renSharpCore, RenSharpContext context)
		{
            Console.WriteLine("pass");
        } // Do nothing
	}
}
