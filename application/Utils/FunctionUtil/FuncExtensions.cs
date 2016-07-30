using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.FunctionUtil
{
    public static class FuncExtensions
    {
        public static Action<A> CombineWith<A, B>(this Action<B> act2, Func<A, B> func1)
        {
            return a => act2(func1(a));
        }

    }
}
