﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.Binary;
using Utils.Parsing;

namespace Dokan.Messaging.Move
{
    public sealed class MoveRequest : IBinary
    {
        public static byte TypeNum => 9;
        public string From { get; }
        public string To { get; }

        public MoveRequest(string @from, string to)
        {
            From = @from;
            To = to;
        }

        public byte[] ToBytes()
        {
            return 
                TypeNum
                .Singleton()
                .Concat(From.ToBytes())
                .Concat(To.ToBytes())
                .ToArray();
        }
    }
}
