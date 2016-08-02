using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Dokan.Messaging.RootHash
{
    public sealed class RootHashResponse
    {
        public static byte TypeNum => RootHashRequest.TypeNum;
        public byte[] RootHash { get; }

        public RootHashResponse(byte[] rootHash)
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
                                    .GetBytes(index, 32)
                                    .Map(b => new RootHashResponse(b))));
        }
    }
}
