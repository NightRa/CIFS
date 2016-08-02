using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Dokan.Messaging.CreateFolder
{
    public sealed class CreateFolderRequest : IBinary
    {
        public static byte TypeNum => 7;
        public string FilePath { get; }

        public CreateFolderRequest(string filePath)
        {
            FilePath = filePath;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(FilePath.ToBytes())
                .ToArray();
        }
    }
}
