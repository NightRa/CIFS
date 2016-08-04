using System;
using System.Collections.Generic;
using System.Linq;
using Utils.ArrayUtil;
using Utils.IEnumerableUtil;
using static System.Environment;
using static System.StringSplitOptions;

namespace Utils.StringUtil
{
    public static class StringExtensions
    {
        public static string AddTabs(this string @this, string tab = "    ")
        {
            return
                @this
                .Split(NewLine.Singleton(), None)
                .Select(x => tab + x)
                .MkString(NewLine);
        }
        public static string TakeWithPadding(this string @this, int amountToTake, char pad = ' ')
        {
            var chars = 
                @this
                .AsEnumerable()
                .Concat(Enumerable.Repeat(pad, amountToTake))
                .Take(amountToTake);
            return new string(chars.ToArray());
        }
    }
}
