using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Models
{
	public class MessageResult
	{
		public string RawLine { get; set; }
		public string Speech { get; set; }
		public string Character { get; set; }
		public Attributes Attributes { get; set; }
	}
}
