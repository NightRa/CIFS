using System.Collections.Generic;
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
            var bytes = new List<byte>(amount);
            for (int i = 0; i < amount; i++)
            {
                var maybeByte = @this.GetByte(index);
                if (maybeByte.IsError)
                    return Parse.Error<byte[]>("Array parsing error at index " + i + ": " + maybeByte.ErrorUnsafe);
                bytes.Add(maybeByte.ResultUnsafe);
            }
            return Parse.Return(bytes.ToArray());
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
