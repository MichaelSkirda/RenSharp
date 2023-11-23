using RenSharp.Models.Commands;
using System.Collections.Generic;

namespace RenSharp.Core.Repositories
{
	internal class CharacterRepository
	{
		private Dictionary<string, Character> Characters { get; set; } = new Dictionary<string, Character>();
		private static int AnonymousCharacterId { get; set; } = 0;

		internal Character this[string key]
		{
			get
			{
				return Characters[key];
			}
			set
			{
				Characters[key] = value;
			}
		}

		internal Character GetCharacter(string name)
			=> Characters[name];

		internal bool TryGetCharacter(string name, out Character character)
			=> Characters.TryGetValue(name, out character);

		internal string AddCharacter(Character character)
		{
			AnonymousCharacterId++;
			string key = $"_rs_anonymous_character_{AnonymousCharacterId}";
			Characters[key] = character;
			return key;
		}
	}
}
