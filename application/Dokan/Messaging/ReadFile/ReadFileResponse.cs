using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;
using static Utils.Parsing.Parse;

namespace Dokan.Messaging.ReadFile
{
    public sealed class ReadFileResponse
    {
        public static byte TypeNum => ReadFileRequest.TypeNum;
        public bool DoesFileExist { get; }
        public byte[] BytesRead { get; }

        public ReadFileResponse(bool doesFileExist, byte[] bytesRead)
        {
            DoesFileExist = doesFileExist;
            BytesRead = bytesRead;
        }

        public static ParsingResult<ReadFileResponse> Parse(byte[] bytes)
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
                                    .FlatMap(exists =>
                                    {
                                        if (exists != 0)
                                            return Return(new ReadFileResponse(false, new byte[0]));
                                        else
                                            return
                                                bytes
                                                    .GetArray(index, ByteBinary.GetByte)
                                                    .Map(byteArray =>
                                                        new ReadFileResponse(true, byteArray));
                                    })));
        }
    }
}
