using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.OptionUtil;
using static Utils.OptionUtil.Opt;

namespace Utils.Binary
{
    public static class CharBinary
    {
        public static byte[] ToBytes(this char @this)
        {
            return Convert.ToByte(@this).Singleton();
        }

        public static Option<char> FromBytes(this byte[] @this, ref int index)
        {
            return Some(Convert.ToChar(@this[index++]));
        }
    }
}
