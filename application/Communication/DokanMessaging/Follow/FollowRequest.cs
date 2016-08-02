using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.Follow
{
    public sealed class FollowRequest : IBinary
    {
        public static byte TypeNum => 13;
        public string FilePath { get; }

        public FollowRequest(string filePath)
        {
            FilePath = filePath;
        }

        public byte[] ToBytes()
        {
            return TypeNum.Singleton().Concat(FilePath.ToBytes()).ToArray();
        }
    }
}
