using System.Linq;
using Utils.GeneralUtils;
using Utils.Parsing;

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
