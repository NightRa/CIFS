using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Utils.OptionUtil;
using static System.Environment;

namespace Utils.FunctionUtil
{
    public static class FuncExtensions
    {
        public static void CatchErrors(this Action @this, Action<string> log, string message)
        {
            try
            {
                @this();
            }
            catch (Exception e)
            {
                log(message + NewLine + e);
            }
        }
        public static void DoAsyncBackground(this Action @this, string threadName, Action<string> log)
        {
            log("Starting a thread named " + threadName);
            Thread t = new Thread(() => @this.CatchErrors(log, "Thread " + threadName + "threw an exception"));
            t.Name = threadName;
            t.IsBackground = true;
            t.Priority = ThreadPriority.BelowNormal;
            t.Start();
        }

    }
}
