using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Communication.DokanMessaging.RootHash
{
    public sealed class RootHashResponse
    {
        public static byte TypeNum => RootHashRequest.TypeNum;
        public string RootHash { get; }

        public RootHashResponse(string rootHash)
        {
            RootHash = rootHash;
        }

        public static ParsingResult<RootHashResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes.GetByte(index)
                    .FlatMap(num =>
                        num
                            .HasToBe(TypeNum)
                            .FlatMap(_ =>
                                bytes
                                    .GetString(index)
                                    .Map(b => new RootHashResponse(b))));
        }

        public override string ToString()
        {
            return $"RootHash: {RootHash}";
        }
    }
}
