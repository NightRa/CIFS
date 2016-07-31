using System;
using Utils.StringUtil;

namespace Utils
{
    public sealed class LogMessage
    {
        public string ThreadName { get; }
        public DateTime Time { get; }
        public string Message { get; }

        public LogMessage(string threadName, DateTime time, string message)
        {
            ThreadName = string.IsNullOrEmpty(threadName) ? "DokanThread" : threadName;
            Time = time;
            Message = message;
        }

        public override string ToString()
        {
            return Time.TimeOfDay.ToString().TakeWithPadding(12) + ": " + ThreadName + " - " + Message;
        }
    }
}