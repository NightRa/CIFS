using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static ParsingResult<int> ToInt(this byte[] @this, Box<int> index)
        {
            return ParsingResult.Parse(() =>
            {
                byte b1 = @this[index.Value++];
                byte b2 = @this[index.Value++];
                byte b3 = @this[index.Value++];
                byte b4 = @this[index.Value++];
                return (b1 << 24) + (b2 << 16) + (b3 << 8) + (b4 << 0);
            });
        }
    }
}
