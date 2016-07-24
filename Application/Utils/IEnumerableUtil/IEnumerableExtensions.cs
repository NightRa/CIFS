using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;

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
        public static void ZipIter<T1, T2>(this IEnumerable<T1> @this, IEnumerable<T2> other, Action<T1, T2> act)
        {
            var one = @this.ToArray();
            var two = other.ToArray();
            if (one.Length != two.Length)
                throw new ArgumentException("Zip's inputs are of different length: one - " + one.Length + ", two - " + two.Length);
            for (int i = 0; i < one.Length; i++)
                act(one[i], two[i]);
        }
    }
}
