﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Core.Exceptions
{
	internal class LabelNotExists : RSException
	{
		public LabelNotExists(string message) : base(message) { }
	}
}
