using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.OptionUtil;
using Utils.Parsing;
using static Utils.OptionUtil.Opt;

namespace Utils.Binary
{
    public static class CharBinary
    {
        public static byte[] ToBytes(this char @this)
        {
            //throw new ArgumentException();
            return ((int)@this).ToBytes();
        }

        public static ParsingResult<char> ToChar(this byte[] @this, Box<int> index)
        {
            return  @this.ToInt(index).Map(num => (char)num);
        }
    }
}
