using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static System.Environment;

namespace Agents
{
    public static class ThreadsExtensions
    {
        public static void DoAsyncBackground(this Action @this, string threadName, Action<string> log)
        {
            log("Starting a thread named " + threadName);
            Thread t = new Thread(() =>
            {
                try
                {
                    @this();
                }
                catch (Exception e)
                {
                    log("Thread " + threadName + " threw exception " + NewLine + e.Message);
                }

            });
            t.Name = threadName;
            t.IsBackground = true;
            t.Priority = ThreadPriority.BelowNormal;
            t.Start();
        }


    }
}
