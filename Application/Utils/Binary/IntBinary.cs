using Utils.GeneralUtils;
using Utils.Parsing;

namespace Utils.Binary
{
    public static class IntBinary
    {
        public static byte[] ToBytes(this int @this)
        {
            byte b1 = (byte)((@this >> 24) & 0xFF);
            byte b2 = (byte)((@this >> 16) & 0xFF);
            byte b3 = (byte)((@this >> 8) & 0xFF);
            byte b4 = (byte)((@this >> 0) & 0xFF);
            return new[] {b1, b2, b3, b4};
        }

        public static ParsingResult<int> GetInt(this byte[] @this, Box<int> index)
        {
            return
                @this
                    .GetBytes(index, 4)
                    .Map(bytes =>
                        (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + (bytes[3] << 0));
        }
        public static ParsingResult<int> HasToBe(this int actual, int expected)
        {
            if (actual == expected)
                return Parse.Return(actual);
            return
                Parse.Error<int>("Error: actual: " + actual + ", expected: " + expected);
        }
    }
}
