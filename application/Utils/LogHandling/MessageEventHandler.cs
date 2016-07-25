using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
