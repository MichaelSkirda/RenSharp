
namespace RenSharpConsole
{
	public interface IFormatter
	{
		string Format(string input);
		string FormatDefault(string input);
		void SetFormat(string format);
	}
}
