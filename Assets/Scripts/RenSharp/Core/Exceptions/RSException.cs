using System;

namespace RenSharp.Core.Exceptions
{
	public class RSException : Exception
	{
		public RSException(string message) : base(message) { }
	}
}
