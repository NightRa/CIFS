using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Dokan.Messaging.RootHash
{
    public sealed class RootHashRequest : IBinary
    {
        public static byte TypeNum => 0;
        public byte[] ToBytes()
        {
            return TypeNum.Singleton();
        }
    }
}
