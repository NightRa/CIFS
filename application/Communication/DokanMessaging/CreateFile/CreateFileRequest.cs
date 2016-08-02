using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.CreateFile
{
    public sealed class CreateFileRequest : IBinary
    {
        public static byte TypeNum => 6;
        public string FilePath { get; }

        public CreateFileRequest(string filePath)
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
