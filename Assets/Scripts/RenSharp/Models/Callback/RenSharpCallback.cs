using System;

namespace RenSharp.Models.Callback
{
    public class RenSharpCallback
    {
        public Action Callback { get; set; }
        public bool CallIfRollbackUsed { get; set; }

        public RenSharpCallback(bool callIfRollbackUsed, Action callback)
        {
            CallIfRollbackUsed = callIfRollbackUsed;
            Callback = callback;
        }
    }
}
