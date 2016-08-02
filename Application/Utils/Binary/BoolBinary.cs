using Utils.ArrayUtil;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Utils.Binary
{
    public static class BoolBinary
    {
        public static byte[] ToBytes(this bool @this)
        {
            byte bit = (byte) (@this ? 1 : 0);
            return bit.Singleton();
        }

        public static ParsingResult<bool> GetBool(this byte[] @this, Box<int> index)
        {
            return
                @this
                    .GetByte(index)
                    .FlatMap(num =>
                    {
                        switch (num)
                        {
                            case 0:
                                return Parse.Return(false);
                            case 1:
                                return Parse.Return(true);
                            default:
                                return Parse.Error<bool>("Cannot conver to boolean: " + num);
                        }
                    });
        }
    }
}
