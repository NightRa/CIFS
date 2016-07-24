using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agents
{
    public static class ThreadsExtensions
    {
        public static void DoAsyncBackground(this Action @this, string threadName)
        {
            Thread t = new Thread(() => @this());
            t.Name = threadName;
            t.IsBackground = true;
            t.Priority = ThreadPriority.BelowNormal;
            t.Start();
        }


    }
}
