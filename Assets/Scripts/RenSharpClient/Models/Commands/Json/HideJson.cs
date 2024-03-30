using RenSharp.Models;
using RenSharp.Models.Commands.Json;

namespace RenSharpClient.Models.Commands.Json
{
    public class HideJson : CommandJson
    {
        public string Name { get; set; }
        public Attributes Attributes { get; set; }
    }
}
