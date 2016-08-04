using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.RandomUtils
{
    public static class RandomExtensions
    {
        private static char NextChar(this Random @this)
        {
            unchecked
            {
                return (char) @this.Next(1, 1000);
            }
        }

        public static string NextString(this Random @this, int length)
        {
            var chs =
                Enumerable.Repeat(0, length)
                    .Select(_ => @this.NextChar())
                    .ToArray();
            return new string(chs);
        }

        public static byte[] NextBytes(this Random @this, int length)
        {
            var bytes = new byte[length];
            @this.NextBytes(bytes);
            return bytes;
        }
    }
}
