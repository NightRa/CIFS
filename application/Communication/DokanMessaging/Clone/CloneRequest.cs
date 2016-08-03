using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.Clone
{
    public sealed class CloneRequest : IBinary
    {
        public static byte TypeNum => 10;
        public string HashRemotePath { get; }
        public string LocalPath { get; }

        public CloneRequest(string hashRemotePath, string localPath)
        {
            HashRemotePath = hashRemotePath;
            LocalPath = localPath;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(HashRemotePath.ToBytes())
                .Concat(LocalPath.ToBytes())
                .ToArray();
        }
    }
}
