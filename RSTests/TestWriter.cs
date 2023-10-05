using RenSharp.Interfaces;
using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTests
{
	internal class TestWriter : IWriter
	{
		internal List<Message> WritedMessage { get; set; } = new List<Message>();

		public void Write(Message message)
		{
			WritedMessage.Add(message);
		}
	}
}
