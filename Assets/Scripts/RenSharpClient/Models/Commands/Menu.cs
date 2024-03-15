using RenSharp.Core;
using RenSharp.Models;
using RenSharp.Models.Commands;
using RenSharp.RenSharpClient.Models;
using RenSharpClient.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenSharpClient.Models.Commands
{
	public class Menu : CommandPushable, IRollbackable
	{
		public ICollection<MenuButton> Buttons { get; set; } = new List<MenuButton>();
		public Message Message { get; set; }
		public MenuController Controller { get; set; }

		public Menu(MenuController controller)
		{
			Controller = controller;
		}

		public override void Execute(RenSharpCore core)
		{
			if (Message != null)
				Message.Execute(core);

			core.Pause();

			if (Buttons.Count <= 0)
				throw new InvalidOperationException("У блока меню не объявлены кнопки.");
			IEnumerable<MenuButton> activeButtons = Buttons
				.Where(btn => core.Context.Evaluate<bool>(btn.Predicate));

			Controller.Clear();
			Controller.Show(activeButtons, core);
		}

		public override void Push(Stack<int> stack, RenSharpContext ctx)
		{
			stack.Push(0);
		}

		public override bool TryPush(Stack<int> stack, RenSharpContext ctx)
		{
			stack.Push(0);
			return true;
		}

		public Command Rollback(RenSharpCore core)
		{
			var rollback = new MenuRollback(Controller);
			rollback.SetPosition(this);
			return rollback;
		}

    }
}
