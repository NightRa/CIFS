using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.CreateFolder
{
    public sealed class CreateFolderRequest : IBinary
    {
        public static byte TypeNum => 7;
        public string FilePath { get; }

        public CreateFolderRequest(string filePath)
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
