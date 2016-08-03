using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Communication.DokanMessaging.CloneOrFollow
{
    public sealed class CloneOrFollowRequest : IBinary
    {
        public static byte TypeNum => 10;
        public bool IsFollow { get; }
        public string HashRemotePath { get; }
        public string LocalPath { get; }

        public CloneOrFollowRequest(bool isFollow, string hashRemotePath, string localPath)
        {
            HashRemotePath = hashRemotePath;
            LocalPath = localPath;
            IsFollow = isFollow;
        }

        public byte[] ToBytes()
        {
            var isFollow = (byte) (IsFollow ? 1 : 0);
            return
                TypeNum
                .Singleton()
                .Concat(isFollow.Singleton())
                .Concat(HashRemotePath.ToBytes())
                .Concat(LocalPath.ToBytes())
                .ToArray();
        }
    }
}
