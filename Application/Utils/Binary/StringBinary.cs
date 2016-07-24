using System;
using System.Collections.Generic;
using System.Linq;
using Utils.ArrayUtil;
using Utils.OptionUtil;
using Utils.Parsing;
using static System.Text.Encoding;
using static System.Text.UnicodeEncoding;
using static Utils.OptionUtil.Opt;
using Convert = System.Convert;

namespace Utils.Binary
{
    public static class StringBinary
    {
        public static byte[] ToBytes(this string @this)
        {
            return @this.ToCharArray().ToBytes(ch => ch.ToBytes());
        }

        public static ParsingResult<string> ParseToString(this byte[] @this, Box<int> index)
        {
            return 
                @this
                .ToArray(index, CharBinary.ToChar)
                .Map(chArray => new string(chArray));
        }
    }
}
