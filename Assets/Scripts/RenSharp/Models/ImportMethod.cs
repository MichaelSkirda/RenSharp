using System.Reflection;

namespace RenSharp.Models
{
	public class ImportMethod
	{
		public string Name { get; set; }
		public MethodInfo MethodInfo { get; set; }

		public ImportMethod(MethodInfo methodInfo, string name = "")
		{
			MethodInfo = methodInfo;
			Name = name;
		} 
	}
}
