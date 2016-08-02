using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.Binary;
using Utils.Parsing;

namespace Dokan.Messaging.Flush
{
    public sealed class FlushRequest : IBinary
    {
        public static byte TypeNum => 1;

        public byte[] ToBytes()
        {
            return TypeNum.Singleton();
        }
    }
}
