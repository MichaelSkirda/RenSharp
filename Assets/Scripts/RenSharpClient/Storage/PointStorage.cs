using RenSharpClient.Storage.Items;
using System.Linq;
using UnityEngine;

namespace RenSharpClient.Storage
{
	internal class PointStorage : MonoBehaviour
	{
		[SerializeField]
		private PointItem[] pointItems;

		public PointItem Find(string name)
			=> pointItems.First(x => x.Name == name);

		private void Start()
		{
			
		}
	}
}
