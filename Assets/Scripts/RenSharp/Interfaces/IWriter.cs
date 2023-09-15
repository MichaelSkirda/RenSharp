using RenSharp.Models.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenSharp.Interfaces
{
	public interface IWriter
	{
		void Write(Message message);
	}
}
