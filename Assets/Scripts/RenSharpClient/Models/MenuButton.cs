namespace RenSharp.RenSharpClient.Models
{
	public class MenuButton
	{
		public string Text { get; set; }
		public string Label { get; set; }
		public string Predicate { get; set; }

		public MenuButton(string text, string label)
		{
			Text = text;
			Label = label;
			Predicate = "True";
		}

		public MenuButton(string text, string label, string predicate)
		{
			Text = text;
			Label = label;
			Predicate = predicate;
		}
	}
}
