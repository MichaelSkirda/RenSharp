using System;

namespace RenSharp.Core.Exceptions
{
	internal class UnexpectedEndOfProgramException : Exception
	{
		public UnexpectedEndOfProgramException(string message) : base(message) { }
	}
}
