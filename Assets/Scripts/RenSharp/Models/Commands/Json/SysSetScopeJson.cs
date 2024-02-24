using System.Collections.Generic;

namespace RenSharp.Models.Commands.Json
{
    public class SysSetScopeJson : CommandJson
    {
        public Dictionary<string, object> variables { get; set; }
    }
}
