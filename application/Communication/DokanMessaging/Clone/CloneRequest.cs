using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.Clone
{
    public sealed class CloneRequest : IBinary
    {
        public static byte TypeNum => 11;
        public string HashPath { get; }

        public CloneRequest(string hashPath)
        {
            HashPath = hashPath;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(HashPath.ToBytes())
                .ToArray();
        }
    }
}
