using System;

namespace Agents.DokanSupervisorMessages
{
    public sealed class DokanThrewExceptionMessage : DokanSupervisorMessage
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
