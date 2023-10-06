using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core.Exceptions
{
	internal class RSException : Exception
	{
		public RSException(string message) : base(message) { }
	}
}
