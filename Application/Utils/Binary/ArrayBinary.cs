using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Utils.Binary
{
    public static class ArrayBinary
    {
        public static byte[] ToBytes<T>(this T[] @this, EncodeFunc<T> encode)
        {
            return 
                @this
                .Select(encode.Invoke)
                .Aggregate(@this.Length.ToBytes().AsEnumerable(), (acc, bytes) => acc.Concat(bytes))
                .ToArray();
        }

        public static ParsingResult<T[]> GetArray<T>(this byte[] @this, Box<int> index, ParseFunc<T> parse)
        {
            return 
                @this.GetInt(index)
                .FlatMap(amount =>
                    Enumerable.Repeat(0, amount)
                    .Select(_ => parse(@this, index))
                    .Flatten())
                .Map(Enumerable.ToArray);
        }
    }
}
