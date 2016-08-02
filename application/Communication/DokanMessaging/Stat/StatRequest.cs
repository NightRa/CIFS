using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.Stat
{
    public sealed class StatRequest : IBinary
    {
        public static byte TypeNum => 2;
        public string FilePath { get; }

        public StatRequest(string filePath)
        {
            FilePath = filePath;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(FilePath.ToBytes())
                .ToArray();
        }
    }
}
