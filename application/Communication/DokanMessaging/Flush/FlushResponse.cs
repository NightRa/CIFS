using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.Flush
{
    public sealed class FlushResponse
    {
        public static byte TypeNum => FlushRequest.TypeNum;

        public static ParsingResult<FlushResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes
                    .GetByte(index)
                    .Map(num => num.HasToBe(TypeNum))
                    .Map(_ => new FlushResponse());
        }
    }
}
