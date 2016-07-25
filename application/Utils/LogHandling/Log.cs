using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Utils.IEnumerableUtil;
using Utils.LogHandling;

namespace Utils
{
    public sealed class Log
    {
        private ConcurrentBag<LogMessage> Messages { get; }

        public event EventHandler<LogMessageEventArgs> MessageAdded;

        public Log()
        {
            Messages = new ConcurrentBag<LogMessage>();
        }

        public void AddMessage(string message)
        {
            var logMessage = new LogMessage(Thread.CurrentThread.Name, DateTime.Now, message);
            Messages.Add(logMessage);
            MessageAdded?.Invoke(this, new LogMessageEventArgs(logMessage));
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            Messages
                .OrderBy(m => m.Time)
                .Iter(m => str.AppendLine(m.ToString()));

            return str.ToString();
        }

        public static Action<string> InitilizeInteractive(string logFileLocation)
        {
            var log = new Log();
           
            var logFile = File.CreateText(logFileLocation);
            log.MessageAdded += (sender, args) =>
            {
                logFile.WriteLine(args.LogMessage.ToString());
                logFile.Flush();
            };
            return log.AddMessage;
        }
    }
}
