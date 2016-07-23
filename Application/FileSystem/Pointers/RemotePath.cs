using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileSystemBrackets;

namespace FileSystem.Pointers
{
    public sealed class RemotePath
    {
        public MutablePtr Root { get; }
        public Brackets Path { get; }

        public RemotePath(MutablePtr root, Brackets path)
        {
            Root = root;
            Path = path;
        }
    }
}
