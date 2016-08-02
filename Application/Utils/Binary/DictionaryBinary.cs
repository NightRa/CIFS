using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.GeneralUtils;
using Utils.IEnumerableUtil;
using Utils.Parsing;

namespace Utils.Binary
{
    public static class DictionaryBinary
    {
        public static byte[] ToBytes<TKey, TValue>(this Dictionary<TKey, TValue> @this, EncodeFunc<TKey> encodeKey, EncodeFunc<TValue> encodeValue)
        {
            return
                @this
                .Count
                .ToBytes()
                .Concat(@this.Keys.SelectMany(encodeKey.Invoke))
                .Concat(@this.Values.SelectMany(encodeValue.Invoke))
                .ToArray();
        }

        public static ParsingResult<Dictionary<TKey, TValue>> GetDictionary<TKey, TValue>(this byte[] @this,
            Box<int> index, ParseFunc<TKey> parseKey, ParseFunc<TValue> parseValue)
        {
            return
                @this
                    .GetInt(index)
                    .FlatMap(amount =>
                        Enumerable
                            .Repeat(0, amount)
                            .Select(_ => parseKey(@this, index))
                            .Flatten()
                            .FlatMap(keys =>
                                Enumerable
                                    .Repeat(0, amount)
                                    .Select(_ => parseValue(@this, index))
                                    .Flatten()
                                    .Map(values =>
                                    {
                                        var dictionary = new Dictionary<TKey, TValue>(amount);
                                        keys.ZipIter(values, dictionary.Add);
                                        return dictionary;
                                    })));
        }
    }
}
