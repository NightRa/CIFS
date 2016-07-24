using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileSystemBrackets;
using Utils;
using Utils.Binary;
using Utils.Parsing;

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

        private bool Equals(RemotePath other)
        {
            return Equals(Root, other.Root) && Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is RemotePath && Equals((RemotePath) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Root != null ? Root.GetHashCode() : 0)*397) ^ (Path != null ? Path.GetHashCode() : 0);
            }
        }

        public byte[] ToBytes()
        {
            return Root.ToBytes().Concat(Path.ToBytes()).ToArray();
        }

        public static ParsingResult<RemotePath> Parse(byte[] bytes, Box<int> index)
        {
            return MutablePtr.Parse(bytes, index).FlatMap(root =>
                Brackets.Parse(bytes, index).Map(path =>
                    new RemotePath(root, path)));
        }
    }
}
