namespace RenSharp.Models.Commands.Json
{
    public class MessageRollbackJson : CommandJson
    {
        public string Speech { get; set; }
        public string Character { get; set; }
        public Attributes Attributes { get; set; }
    }
}
