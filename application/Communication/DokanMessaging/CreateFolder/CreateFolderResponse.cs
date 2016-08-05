using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.CreateFolder
{
    public sealed class CreateFolderResponse
    {

        public static byte TypeNum => CreateFolderRequest.TypeNum;
        public bool IsReadOnly { get; }
        public bool NameCollosion { get; }
        public bool PathToParentDoesntExist { get; }
        public bool InvalidName { get; }
        public CreateFolderResponse(bool isReadOnly, bool nameCollosion, bool pathToParentDoesntExist, bool invalidName)
        {
            IsReadOnly = isReadOnly;
            NameCollosion = nameCollosion;
            PathToParentDoesntExist = pathToParentDoesntExist;
            InvalidName = invalidName;
        }

        public static ParsingResult<CreateFolderResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes
                    .GetByte(index)
                    .FlatMap(num =>
                        num.HasToBe(TypeNum)
                            .FlatMap(_ =>
                                bytes
                                    .GetByte(index)
                                    .Map(x =>
                                    {
                                        bool isReadOnly = x == 1;
                                        bool isNameCollision = x == 2;
                                        bool pathToParentDoesntExist = x == 3;
                                        bool invalidName = x == 4;
                                        return new CreateFolderResponse(isReadOnly, isNameCollision, pathToParentDoesntExist, invalidName);
                                    })));
        }

        public override string ToString()
        {
            return $"IsReadOnly: {IsReadOnly}, NameCollosion: {NameCollosion}, PathToParentDoesntExist: {PathToParentDoesntExist}, InvalidName: {InvalidName}";
        }
    }
}
