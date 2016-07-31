using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utils
{
    public static class MonitorExtensions
    {
        public static bool IsHeld(this object @this, int timeInMs = 0)
        {
            switch (Monitor.TryEnter(@this, timeInMs))
            {
                case true:
                    Monitor.Exit(@this);
                    return false;
                default:
                    return true;
            }
        }
    }

}
