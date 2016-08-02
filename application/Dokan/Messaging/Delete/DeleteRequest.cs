using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;
using Utils.Parsing;

namespace Dokan.Messaging.Delete
{
    public sealed class DeleteRequest : IBinary
    {
        public static byte TypeNum => 8;
        public string FilePath { get; }

        public DeleteRequest(string filePath)
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
