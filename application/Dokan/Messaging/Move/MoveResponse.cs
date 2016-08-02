﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Dokan.Messaging.Move
{
    public sealed class MoveResponse
    {

        public static byte TypeNum => MoveRequest.TypeNum;
        public bool SrcDoesntExist { get; }
        public bool SrcOrDesrReadOnly { get; }
        public MoveResponse(bool srcDoesntExist, bool srcOrDesrReadOnly)
        {
            SrcOrDesrReadOnly = srcOrDesrReadOnly;
            SrcDoesntExist = srcDoesntExist;
        }

        public static ParsingResult<MoveResponse> Parse(byte[] bytes)
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
                                        bool srcOrDesrReadOnly = x == 1;
                                        bool srcDoesntExist = x == 2;
                                        return new MoveResponse(srcDoesntExist, srcOrDesrReadOnly);
                                    })));
        }
    }
}
