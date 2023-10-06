using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core.Exceptions
{
	public class RSException : Exception
	{
		public RSException(string message) : base(message) { }
	}
}
