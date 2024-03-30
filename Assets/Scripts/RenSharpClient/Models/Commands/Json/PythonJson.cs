using RenSharp.Models.Commands.Json;
using System.Collections.Generic;

namespace RenSharpClient.Models.Commands.Json
{
    public class PythonJson : CommandJson
    {
        public IEnumerable<string> Commands { get; set; }
    }
}
