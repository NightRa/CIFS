using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.DoubleUtil
{
    public static class DoubleExtensions
    {
        public static int ToInt(this double @this)
        {
            return Convert.ToInt32(@this);
        }
    }
}
