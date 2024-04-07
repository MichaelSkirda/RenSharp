using System;

namespace RenSharp.Models
{
    public class ImportType
	{
		public string Name { get; set; }
		public Type Type { get; set; }

		public ImportType(Type type, string name)
		{
			Type = type;
			Name = name;
		}

	}
}
