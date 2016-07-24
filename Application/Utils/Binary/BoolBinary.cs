using Utils.ArrayUtil;
using Utils.OptionUtil;
using Utils.Parsing;
using static Utils.OptionUtil.Opt;

namespace Utils.Binary
{
    public static class BoolBinary
    {
        public static byte[] ToBytes(this bool @this)
        {
            byte bit = (byte)(@this ? 1 : 0);
            return bit.Singleton();
        }

        public static ParsingResult<bool> ToBool(this byte[] @this, Box<int> index)
        {
            return ParsingResult.Parse(() =>
            {
                byte value = @this[index.Value++];
                switch (value)
                {
                    case 0:
                        return ParsingResult.Pure(false);
                    case 1:
                        return ParsingResult.Pure(true);
                    default:
                        return ParsingResult.Error<bool>("");
                }
            }).Flatten();
        } 
    }
}
