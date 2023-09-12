using RenSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharpClient
{
	internal class Writer : IWriter
	{
		public void Write(string speech)
		{
            Console.WriteLine(speech);
        }
	}
}
