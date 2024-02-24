using System.Collections.Generic;

namespace RenSharp.Models.Save
{
    public class SaveModelJson
    {
        // Core
        public bool IsPaused { get; set; }
        public bool HasStarted { get; set; }

        // Program
        public int Line { get; set; }

        // Context
        public IEnumerable<MessageResult> MessageHistory { get; set; }
        public IEnumerable<object> RollbackStack { get; set; }
        public IEnumerable<StackFrame> CallStack { get; set; }
        public StackFrame CurrentFrame { get; set; }

        // PyEvaluator
        public Dictionary<string, object> Variables { get; set; }
    }
}
