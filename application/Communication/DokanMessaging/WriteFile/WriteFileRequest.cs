using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.WriteFile
{
    public sealed class WriteFileRequest : IBinary
    {
        public static byte TypeNum => 5;
        public string FilePath { get; }
        public byte[] BufferToWrite { get; }
        public long Offset { get; }

        public WriteFileRequest(string filePath, byte[] bufferToWrite, long offset)
        {
            FilePath = filePath;
            BufferToWrite = bufferToWrite;
            Offset = offset;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(FilePath.ToBytes())
                .Concat(Offset.ToBytes())
                .Concat(BufferToWrite.ToBytes(ArrayExtensions.Singleton))
                .ToArray();
        }
    }
}
