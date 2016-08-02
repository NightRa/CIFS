using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.Delete
{
    public sealed class DeleteResponse
    {

        public static byte TypeNum => DeleteRequest.TypeNum;
        public bool IsReadOnly { get; }
        public bool PathDoesntExist { get; }
        public DeleteResponse(bool isReadOnly, bool pathDoesntExist)
        {
            IsReadOnly = isReadOnly;
            PathDoesntExist = pathDoesntExist;
        }

        public static ParsingResult<DeleteResponse> Parse(byte[] bytes)
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
                                        bool pathDoesntExist = x == 2;
                                        return new DeleteResponse(isReadOnly, pathDoesntExist);
                                    })));
        }
    }
}
