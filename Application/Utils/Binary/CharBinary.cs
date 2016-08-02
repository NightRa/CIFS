using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.FunctionUtil;
using Utils.GeneralUtils;
using Utils.OptionUtil;
using Utils.Parsing;
using static System.Text.Encoding;
using static Utils.OptionUtil.Opt;

namespace Utils.Binary
{
    public static class CharBinary
    {
        public static byte[] ToBytes(this char @this)
        {
            return @this.ToString().ToBytes();
        }

        public static ParsingResult<char> GetChar(this byte[] @this, Box<int> index)
        {
            return
                @this
                    .GetString(index)
                    .FlatMap(s =>
                        s.Length.HasToBe(1)
                            .Map(_ => s.First()));
        }
    }
}
