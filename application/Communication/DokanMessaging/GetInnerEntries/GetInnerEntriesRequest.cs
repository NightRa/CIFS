using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.GetInnerEntries
{
    public sealed class GetInnerEntriesRequest : IBinary
    {
        public static byte TypeNum => 3;
        public string FilePath { get; }

        public GetInnerEntriesRequest(string filePath)
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
