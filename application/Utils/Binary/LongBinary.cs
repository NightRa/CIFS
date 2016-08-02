using System.Linq;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Utils.Binary
{
    public static class LongBinary
    {
        public static byte[] ToBytes(this long @this)
        {
            var mostSagnificantInt = (int) (@this >> 32);
            var leastSagnificantInt = (int)(@this & 0x00000000FFFFFFFFL);
            return mostSagnificantInt.ToBytes().Concat(leastSagnificantInt.ToBytes()).ToArray();
        }

        public static ParsingResult<long> GetLong(this byte[] @this, Box<int> index)
        {
            return
                @this
                .GetInt(index)
                .FlatMap(mostSagnificantInt =>
                            @this
                            .GetInt(index)
                            .Map(leastSagnificantInt => leastSagnificantInt + ((long)mostSagnificantInt << 32)));
        }
    }
}
