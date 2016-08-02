using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Dokan.Messaging.ReadFile
{
    public sealed class ReadFileRequest : IBinary
    {
        public static byte TypeNum => 4;
        public string FilePath { get; }
        public long Offset { get; }
        public int AmountToRead { get; }

        public ReadFileRequest(string filePath, long offset, int amountToRead)
        {
            FilePath = filePath;
            Offset = offset;
            AmountToRead = amountToRead;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(FilePath.ToBytes())
                .Concat(Offset.ToBytes())
                .Concat(AmountToRead.ToBytes())
                .ToArray();
        }
    }
}
