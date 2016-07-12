using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dokany.Util
{
    public static class GeneralUtils
    {
        public static T[] Singleton<T>(this T @this)
        {
            T[] array = new T[1];
            array[0] = @this;
            return array;
        }
        public static byte[] AsHexBytes(this byte[] @this)
        {
            var str = BitConverter.ToString(@this).Replace("-", string.Empty);
            var bytes = Encoding.ASCII.GetBytes(str);
            return bytes;
        }
        public static T[] RevTail<T>(this T[] @this)
        {
            Debug.Assert(@this.Length > 0, "RevTail length = 0");
            return @this.Take(@this.Length - 1).ToArray();
        }

        public static string MkString<T>(this IEnumerable<T> @this, string seperator, string start = "", string end = "")
        {
            StringBuilder str = new StringBuilder();
            str.Append(start);
            var values = @this.ToArray();
            int i = 0;
            while (i < values.Length - 1)
                str.Append(values[i++] + seperator);
            if (i < values.Length)
                str.Append(values[i]);
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

        public static Dictionary<K, V> DeepCopy<K, V>(this Dictionary<K, V> @this)
            where K: IDeepCopiable<K>
            where V: IDeepCopiable<V>

        {
            Dictionary<K, V> dict = new Dictionary<K, V>(@this.Count);
            foreach (var pair in @this)
                dict.Add(pair.Key.DeepCopy(), pair.Value.DeepCopy());
            return dict;
        }

        public static int EnumerableHashCode<T>(this IEnumerable<T> @this)
        {
            unchecked
            {
                int hashCode = 0;
                foreach (var item in @this)
                    hashCode = hashCode*31 + item.GetHashCode();
                return hashCode;

            }
        }
    }
}
