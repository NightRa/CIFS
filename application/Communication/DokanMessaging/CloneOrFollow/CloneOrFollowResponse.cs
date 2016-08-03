using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.CloneOrFollow
{
    public sealed class CloneOrFollowResponse
    {
        public static byte TypeNum => CloneOrFollowRequest.TypeNum;
        public bool IsReadOnlyFolder { get; }
        public bool IsNameCollision { get; }
        public bool PathToParentDoesntExist { get; }
        public bool MalformedPath { get; }
        public bool RootNotFound { get; }
        public bool RemotePathBroken { get; }

        public CloneOrFollowResponse(bool isReadOnlyFolder, bool isNameCollision, bool pathToParentDoesntExist, bool malformedPath, bool rootNotFound, bool remotePathBroken)
        {
            IsReadOnlyFolder = isReadOnlyFolder;
            IsNameCollision = isNameCollision;
            PathToParentDoesntExist = pathToParentDoesntExist;
            RootNotFound = rootNotFound;
            RemotePathBroken = remotePathBroken;
            MalformedPath = malformedPath;
        }

        public static ParsingResult<CloneOrFollowResponse> Parse(byte[] bytes)
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
                                    bool malformedPath = x == 4;
                                    bool rootNotFound = x == 5;
                                    bool brokenRemotePath = x == 6;
                                    return new CloneOrFollowResponse(isReadOnly, isNameCollision, doesPathToParentDoesntExist, malformedPath, rootNotFound, brokenRemotePath);
                                })));
        }
    }
}
