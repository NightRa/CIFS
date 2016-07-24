using System;

namespace Agents.DokanMessages
{
    public sealed class DokanThrewExceptionMessage : DokanMessage
    {
        public Exception Exception { get; }

        public DokanThrewExceptionMessage(Exception exception)
        {
            Exception = exception;
        }

        public override string AsString()
        {
            return "Dokan threw exception: " + Environment.NewLine + this.Exception;
        }
    }
}
