using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.IEnumerableUtil
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> @this)
        {
            return @this.Skip(1);
        }
        public static string MkString<T>(this IEnumerable<T> @this, string seperator, string start = "", string end = "")
        {
            StringBuilder str = new StringBuilder();
            str.Append(start);
            var values = @this.ToArray();
            int i = 0;
            while (true)
            {
                if (i >= values.Length)
                    break;
                str.Append(values[i]);
                if (i < values.Length - 1)
                    str.Append(seperator);
                i++;
            }
            str.Append(end);
            return str.ToString();
        }
        public static int EnumerableHashCode<T>(this IEnumerable<T> @this)
        {
            unchecked
            {
                return @this.Aggregate(0, (current, item) => current * 31 + item.GetHashCode());
            }
        }
        public static void Iter<T>(this IEnumerable<T> @this, Action<T> act)
        {
            foreach (var item in @this)
                act(item);
        }
    }
}
