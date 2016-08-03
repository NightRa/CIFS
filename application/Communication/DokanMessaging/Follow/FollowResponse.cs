using Communication.DokanMessaging.CreateFile;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.Follow
{
    public sealed class FollowResponse
    {
        public static byte TypeNum => FollowRequest.TypeNum;
        public bool IsReadOnlyFolder { get; }
        public bool IsNameCollision { get; }
        public bool PathToParentDoesntExist { get; }
        public bool RootNotFound { get; }
        public bool RemotePathBroken { get; }

        public FollowResponse(bool isReadOnlyFolder, bool isNameCollision, bool pathToParentDoesntExist, bool rootNotFound, bool remotePathBroken)
        {
            IsReadOnlyFolder = isReadOnlyFolder;
            IsNameCollision = isNameCollision;
            PathToParentDoesntExist = pathToParentDoesntExist;
            RootNotFound = rootNotFound;
            RemotePathBroken = remotePathBroken;
        }

        public static ParsingResult<FollowResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes
                    .GetByte(index)
                    .FlatMap(num =>
                        num.HasToBe(TypeNum)
                            .FlatMap(_ => bytes.GetByte(index)
                                .Map(x =>
                                {
                                    bool isReadOnly = x == 1;
                                    bool isNameCollision = x == 2;
                                    bool doesPathToParentDoesntExist = x == 3;
                                    bool rootNotFound = x == 4;
                                    bool brokenRemotePath = x == 5;
                                    return new FollowResponse(isReadOnly, isNameCollision, doesPathToParentDoesntExist, rootNotFound, brokenRemotePath);
                                })));
        }
    }
}
