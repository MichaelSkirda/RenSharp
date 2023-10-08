using RenSharp.Core;
using RenSharp.Models;

namespace Assets.Scripts.RenSharpClient.Commands
{
	internal class Show : Command
	{
		public string Name { get; set; }

		public Show(string name)
		{
			Name = name;
		}

		internal override void Execute(RenSharpCore core)
		{
			throw new System.NotImplementedException();
		}
	}
}
