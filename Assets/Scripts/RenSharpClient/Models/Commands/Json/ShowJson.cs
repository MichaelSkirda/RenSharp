using RenSharp.Models;
using RenSharp.Models.Commands.Json;

namespace RenSharpClient.Models.Commands.Json
{
    public class ShowJson : CommandJson
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Attributes Attributes { get; set; }
    }
}
