using System;

namespace RenSharp.Core.Exceptions
{
    internal class WrongArgumentNumberException : Exception
    {
        public WrongArgumentNumberException(string message) : base(message) { }
        public WrongArgumentNumberException(string message, Exception innerException) : base(message, innerException) { }
    }
}
