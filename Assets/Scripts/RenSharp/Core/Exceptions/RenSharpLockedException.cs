using System;

namespace RenSharp.Core.Exceptions
{
	public class RenSharpLockedException : Exception
	{
		public RenSharpLockedException() { }
		public RenSharpLockedException(string message) : base(message) { }
	}
}
