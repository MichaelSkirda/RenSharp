using RenSharp;

namespace RSTests.RSAssert
{
    [PyImport]
    public class RenSharpAssert
    {
        public MessagesAssert Messages;

        public RenSharpAssert(MessagesAssert messagesAssert)
        {
            Messages = messagesAssert;
        }
    }
}
