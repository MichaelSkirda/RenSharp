
namespace RenSharpConsole
{
	public interface IFormatter
	{
		string Format(string input);
		string FormatDefault(string input);

		string GetCurrentColor();
		void SetFormat(string format);
	}
}
