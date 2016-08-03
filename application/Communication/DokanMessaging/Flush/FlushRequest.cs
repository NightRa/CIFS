using Utils.ArrayUtil;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.Flush
{
    public sealed class FlushRequest : IBinary
    {
        public static byte TypeNum => 1;

        public byte[] ToBytes()
        {
            return TypeNum.Singleton();
        }
    }
}
