using System;

namespace RenSharp.Core.Exceptions
{
	public class RenSharpPausedException : Exception
	{
		public RenSharpPausedException() { }
		public RenSharpPausedException(string message) : base(message) { }
	}
}
