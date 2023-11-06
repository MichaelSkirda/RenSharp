using RenSharp.Models;

namespace Assets.Scripts.RenSharpClient.Models.Commands.Results
{
	internal class ImageResult
	{
		public string Name { get; set; }
		public string Details { get; set; }
		public int? Width { get; set; }
		public int? Height { get; set; }
		public float Zoom { get; set; }
	}
}
