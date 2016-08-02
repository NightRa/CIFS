using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.GeneralUtils;
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
            var chars = @this.ToCharArray();
            var bytes = UTF8.GetBytes(chars);
            return bytes.Length.ToBytes().Concat(bytes).ToArray();
                ;
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
                        .Map(chs => new string(chs));
            }
            catch (Exception e)
            {
                return Parse.Error<string>("Parsing error: " + e);
            }
        }
    }
}
