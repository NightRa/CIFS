using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Environment;

namespace Utils.FunctionUtil
{
    public static class FuncExtensions
    {
        public static Action<A> CombineWith<A, B>(this Action<B> act2, Func<A, B> func1)
        {
            return a => act2(func1(a));
        }
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

    }
}
