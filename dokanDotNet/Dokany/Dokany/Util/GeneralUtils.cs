using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static System.StringSplitOptions;

namespace Dokany.Util
{
    public static class GeneralUtils
    {
        public static T[] Singleton<T>(this T @this) => new []{@this};

        public static byte[] AsHexBytes(this byte[] @this)
        {
            var str = BitConverter.ToString(@this).Replace("-", string.Empty);
            var bytes = Encoding.ASCII.GetBytes(str);
            return bytes;
        }

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

        public static bool EqualDictionary<K, V>(this Dictionary<K, V> @this, Dictionary<K, V> @that)
        {
            if (@this.Keys.All(@that.ContainsKey))
                if (@that.Keys.All(@this.ContainsKey))
                    if (@this.Keys.All(key => @this[key].Equals(@that[key])))
                        if (@that.Keys.All(key => @this[key].Equals(@that[key])))
                            return true;
            return false;
        }

        public static bool ArrayEquals<T>(this T[] @this, T[] @that, Func<T, T, bool> areEqual)
        {
            if (@this.Length == @that.Length)
                for (int i = 0; i < @this.Length; i++)
                    if (!areEqual(@this[i], @that[i]))
                        return false;
            return true;
        }

        public static int EnumerableHashCode<T>(this IEnumerable<T> @this)
        {
            unchecked
            {
                return @this.Aggregate(0, (current, item) => current*31 + item.GetHashCode());
            }
        }

        public static string AddTabs(this string @this)
        {
            return 
                @this
                .Split(Environment.NewLine.Singleton(), None)
                .Select(x => "    " + x)
                .MkString(Environment.NewLine);
        }

        public static string AsString<K, V>(this Dictionary<K, V> @this, Func<V, K, string> asString)
        {
            var str = new StringBuilder();

            @this.Keys
                .Iter(k => str.AppendLine(k.ToString() + ": " + asString(@this[k], k)));

            return str.ToString();
        }

        public static void Iter<T>(this IEnumerable<T> @this, Action<T> act)
        {
            foreach (var item in @this)
                act(item);
        }
    }
}
