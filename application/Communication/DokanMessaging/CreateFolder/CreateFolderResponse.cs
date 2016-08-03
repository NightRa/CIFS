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
        public CreateFolderResponse(bool isReadOnly, bool nameCollosion, bool pathToParentDoesntExist)
        {
            IsReadOnly = isReadOnly;
            NameCollosion = nameCollosion;
            PathToParentDoesntExist = pathToParentDoesntExist;
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
                                        return new CreateFolderResponse(isReadOnly, isNameCollision, pathToParentDoesntExist);
                                    })));
        }
    }
}
