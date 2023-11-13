namespace RenSharp.RenSharpClient.Models
{
	public class MenuButton
	{
		public string Text { get; set; }
		public string Predicate { get; set; }

		public MenuButton(string text)
		{
			Text = text;
			Predicate = null;
		}

		public MenuButton(string text, string predicate)
		{
			Text = text;
			Predicate = predicate;
		}
	}
}
