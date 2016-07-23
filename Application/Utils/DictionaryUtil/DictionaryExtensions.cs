using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.IEnumerableUtil;

namespace Utils.DictionaryUtil
{
    public static class DictionaryExtensions
    {
        public static string AsString<K, V>(this Dictionary<K, V> @this, Func<V, K, string> asString)
        {
            var str = new StringBuilder();

            @this.Keys
                .Iter(k => str.AppendLine(k.ToString() + ": " + asString(@this[k], k)));

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

    }
}
