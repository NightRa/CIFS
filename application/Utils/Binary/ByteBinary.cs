using System.Collections.Generic;
using System.Linq;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Utils.Binary
{
    public static class ByteBinary
    {
        public static ParsingResult<byte> GetByte(this byte[] @this, Box<int> index)
        {
            if (index.Value >= 0 && index.Value < @this.Length)
                return Parse.Return(@this[index.Value++]);
            var errorMessage = "Get byte when index out of range: " + index.Value + ", range: 0.." + (@this.Length - 1);
            return Parse.Error<byte>(errorMessage);
        }

        public static ParsingResult<byte[]> GetBytes(this byte[] @this, Box<int> index, int amount)
        {
            return
                Enumerable.Repeat(0, amount)
                    .Select(_ => @this.GetByte(index))
                    .Flatten()
                    .Map(Enumerable.ToArray);
        }

        public static ParsingResult<byte> HasToBe(this byte actual, byte expected)
        {
            if (actual == expected)
                return Parse.Return(actual);
            return
                Parse.Error<byte>("Error: actual: " + actual + ", expected: " + expected);
        }
    }
}
