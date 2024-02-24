using System.Collections.Generic;
using RenSharp.Models;
using RenSharp.Models.Save;

namespace RenSharp.Core.Save
{
    public class SaveModel
    {
        // Core
        public bool IsPaused { get; set; }
        public bool HasStarted { get; set; }

        // Program
        public int Line { get; set; }

        // Context
        public IEnumerable<MessageResult> MessageHistory { get; set; }
        public IEnumerable<Command> RollbackStack { get; set; }
        public IEnumerable<StackFrame> CallStack { get; set; }
        public StackFrame CurrentFrame { get; set; }

        // PyEvaluator
        public Dictionary<string, object> Variables { get; set; }


        public SaveModel() { }
        public SaveModel(SaveModelJson model, IEnumerable<Command> rollbackStack)
        {
            IsPaused = model.IsPaused;
            HasStarted = model.HasStarted;
            Line = model.Line;
            MessageHistory = model.MessageHistory;
            RollbackStack = rollbackStack;
            CallStack = model.CallStack;
            CurrentFrame = model.CurrentFrame;
            Variables = model.Variables;
        }
    }
}
