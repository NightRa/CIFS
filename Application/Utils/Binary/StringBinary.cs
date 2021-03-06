﻿using System;
using System.Linq;
using Utils.GeneralUtils;
using Utils.Parsing;
using static System.Text.Encoding;

namespace Utils.Binary
{
    public static class StringBinary
    {
        public static byte[] ToBytes(this string @this)
        {
            @this = @this.Replace('\\', '/');
            var chars = @this.ToCharArray();
            var bytes = UTF8.GetBytes(chars);
            return bytes.Length.ToBytes().Concat(bytes).ToArray();
        }

        public static ParsingResult<string> GetString(this byte[] @this, Box<int> index)
        {
            try
            {
                return
                    @this
                        .GetInt(index)
                        .Map(numOfBytes =>
                        {
                            var chs = UTF8.GetChars(@this, index.Value, numOfBytes);
                            index.Value += numOfBytes;
                            return chs;
                        })
                        .Map(chs => new string(chs))
                        .Map(str => str.Replace('/', '\\'));
            }
            catch (Exception e)
            {
                return Parse.Error<string>("Parsing error: " + e);
            }
        }
    }
}
