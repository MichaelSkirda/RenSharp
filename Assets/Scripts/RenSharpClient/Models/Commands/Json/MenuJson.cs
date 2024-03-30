using RenSharp.Models.Commands.Json;
using RenSharp.RenSharpClient.Models;
using System.Collections.Generic;

namespace RenSharpClient.Models.Commands.Json
{
    public class MenuJson : CommandJson
    {
        public IEnumerable<MenuButton> Buttons { get; set; }
    }
}
