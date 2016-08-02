using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.ArrayUtil
{
    public static class ArrayExtensions
    {
        public static T[] Singleton<T>(this T @this) => new[] { @this };
        public static byte[] AsHexBytes(this byte[] @this)
        {
            var str = BitConverter.ToString(@this).Replace("-", string.Empty);
            var bytes = Encoding.ASCII.GetBytes(str);
            return bytes;
        }
        public static bool ArrayEquals<T>(this T[] @this, T[] @that, Func<T, T, bool> areEqual)
        {
            if (@this.Length == @that.Length)
                for (int i = 0; i < @this.Length; i++)
                    if (!areEqual(@this[i], @that[i]))
                        return false;
            return true;
        }
        public static string ToHexa(this byte[] @this)
        {
            return BitConverter.ToString(@this).Replace("-", string.Empty);
        }
    }
}
