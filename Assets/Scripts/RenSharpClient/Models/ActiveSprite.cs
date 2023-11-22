using RenSharp.Models;
using UnityEngine;

namespace RenSharpClient.Models
{
	public class ActiveSprite
	{
		public GameObject obj { get; set; }
		public string Name { get; set; }
		public string Details { get; set; }
		public Attributes Attributes { get; set; }
	}
}
