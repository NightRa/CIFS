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

        public const string Base58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxy";

        public static byte[] FromBase58(this string s)
        {
            throw new KeyNotFoundException();
        }
        public static byte[] FromHex(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
