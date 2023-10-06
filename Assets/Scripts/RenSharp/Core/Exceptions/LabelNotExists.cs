using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core.Exceptions
{
	public class LabelNotExists : RSException
	{
		public LabelNotExists(string message) : base(message) { }
	}
}
