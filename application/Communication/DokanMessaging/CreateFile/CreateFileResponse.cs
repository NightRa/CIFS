using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.CreateFile
{
    public sealed class CreateFileResponse
    {
        public static byte TypeNum => CreateFileRequest.TypeNum;
        public bool IsReadOnlyFolder { get; }
        public bool IsNameCollision { get; }
        public bool PathToParentDoesntExist { get; }
        public CreateFileResponse(bool isReadOnlyFolder, bool isNameCollision, bool pathToParentDoesntExist)
        {
            IsReadOnlyFolder = isReadOnlyFolder;
            IsNameCollision = isNameCollision;
            PathToParentDoesntExist = pathToParentDoesntExist;
        }

        public static ParsingResult<CreateFileResponse> Parse(byte[] bytes)
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
                                    return new CreateFileResponse(isReadOnly, isNameCollision, doesPathToParentDoesntExist);
                                })));
        }
    }
}
