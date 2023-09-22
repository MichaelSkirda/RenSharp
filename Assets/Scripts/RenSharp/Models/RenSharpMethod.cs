using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RenSharp.Models
{
	public class RenSharpMethod
	{
		public string Namespace { get; set; }
		public MethodInfo MethodInfo { get; set; }

		public RenSharpMethod(MethodInfo methodInfo, string ns = "")
		{
			MethodInfo = methodInfo;
			Namespace = ns;
		} 
	}
}
