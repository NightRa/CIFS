using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Dokan.Messaging.Flush
{
    public sealed class FlushResponse
    {
        public static byte TypeNum => FlushRequest.TypeNum;

        public static ParsingResult<FlushResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes
                    .GetByte(index)
                    .Map(num => num.HasToBe(TypeNum))
                    .Map(_ => new FlushResponse());
        }
    }
}
