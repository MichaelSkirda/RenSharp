using RenSharp.Models;
using System;

namespace RenSharp.Interfaces
{
	public interface IWriter
	{
		void Write(MessageResult message);
		void Write(MessageResult message, float delay, Action Callback);
	}
}
