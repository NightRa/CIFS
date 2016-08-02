using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.WriteFile
{
    public sealed class WriteFileResponse
    {
        public static byte TypeNum => WriteFileRequest.TypeNum;
        public bool IsWriteSuccess { get; }
        public bool IsFileReadonly { get; }
        public bool FileDoesntExist { get; }

        public WriteFileResponse(bool isWriteSuccess, bool isFileReadonly, bool fileDoesntExist)
        {
            IsWriteSuccess = isWriteSuccess;
            IsFileReadonly = isFileReadonly;
            FileDoesntExist = fileDoesntExist;
        }

        public static ParsingResult<WriteFileResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes.GetByte(index)
                    .FlatMap(num =>
                        num.HasToBe(TypeNum)
                            .FlatMap(_ =>
                                bytes.GetByte(index)
                                    .Map(x =>
                                    {
                                        bool isWriteSuccess = x == 0;
                                        bool isReadOnly = x == 1;
                                        bool doesntExist = x == 2;
                                        return new WriteFileResponse(isWriteSuccess, isReadOnly, doesntExist);
                                    })));
        }
    }
}
