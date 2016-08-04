using System;

namespace Utils.LogHandling
{
    public sealed class LogMessageEventArgs : EventArgs
    {
        public LogMessage LogMessage { get; }

        public LogMessageEventArgs(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }


    }
}
