using RenSharp.Interfaces;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharpClient
{
	internal class Writer : IWriter
	{
		public void Write(Message message)
		{
            Console.WriteLine(message.Speech);
        }
	}
}
