using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.RootHash
{
    public sealed class RootHashRequest : IBinary
    {
        public static byte TypeNum => 0;
        public byte[] ToBytes()
        {
            return TypeNum.Singleton();
        }
    }
}
