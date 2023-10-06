using System;

namespace RenSharp.Core.Exceptions
{
	internal class UnexpectedEndOfProgramException : RSException
	{
		public UnexpectedEndOfProgramException(string message) : base(message) { }
	}
}
