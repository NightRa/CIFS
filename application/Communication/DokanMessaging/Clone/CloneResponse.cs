using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.Clone
{
    public sealed class CloneResponse
    {
        public static byte TypeNum => CloneRequest.TypeNum;
        public bool IsReadOnlyFolder { get; }
        public bool IsNameCollision { get; }
        public bool PathToParentDoesntExist { get; }
        public bool RootNotFound { get; }
        public bool RemotePathBroken { get; }

        public CloneResponse(bool isReadOnlyFolder, bool isNameCollision, bool pathToParentDoesntExist, bool rootNotFound, bool remotePathBroken)
        {
            IsReadOnlyFolder = isReadOnlyFolder;
            IsNameCollision = isNameCollision;
            PathToParentDoesntExist = pathToParentDoesntExist;
            RootNotFound = rootNotFound;
            RemotePathBroken = remotePathBroken;
        }

        public static ParsingResult<CloneResponse> Parse(byte[] bytes)
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
                                    return new CloneResponse(isReadOnly, isNameCollision, doesPathToParentDoesntExist, rootNotFound, brokenRemotePath);
                                })));
        }
    }
}
