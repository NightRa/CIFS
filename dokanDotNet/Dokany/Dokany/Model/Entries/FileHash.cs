using System;
using System.IO;
using DokanNet;
using Dokany.Model.Pointers;
using Dokany.Util;

namespace Dokany.Model.Entries
{
    public sealed class FileHash : Entry, IDeepCopiable<FileHash>
    {
        public readonly Hash hash;

        public FileHash(Hash hash)
        {
            this.hash = hash;
        }

        public override FileInformation GetInfo(string name)
        {
            return new FileInformation
            {
                FileName = name,
                Attributes = FileAttributes.Normal,
                CreationTime = Global.TempTime,
                LastWriteTime = Global.TempTime,
                LastAccessTime = Global.TempTime,
                Length = hash.bits.Length
            };
        }

        private bool Equals(FileHash other)
        {
            return hash.Equals(other.hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is FileHash && Equals((FileHash) obj);
        }

        public override int GetHashCode()
        {
            return hash.GetHashCode();
        }

        public override string ToString()
        {
            return $"File: {hash}";
        }

        public FileHash DeepCopy()
        {
            return new FileHash(hash);
        }

        public int Write(byte[] buffer, long offset, bool rewriteAll)
        {
            if (rewriteAll)
                Array.Clear(hash.bits, 0, hash.bits.Length);
            var length = (int)Math.Min(buffer.Length, buffer.LongLength - offset);
            if (length > 0)
                Array.Copy(buffer, 0, hash.bits, offset, length);
            return length;
        }
    }
}
