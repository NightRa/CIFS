using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Constants;
using DokanNet;
using FileSystem.Pointers;
using FileSystemBrackets;
using Utils.FileSystemUtil;
using static System.Environment;

namespace FileSystem.Entries
{
    public sealed class FileHash : Entry
    {
        public Hash Hash { get; }

        public FileHash(Hash hash)
        {
            Hash = hash;
        }

        public override FileInformation GetInfo(string name)
        {
            return new FileInformation
            {
                FileName = name,
                Attributes = FileAttributes.Normal,
                CreationTime = Global.FilesTime,
                LastWriteTime = Global.FilesTime,
                LastAccessTime = Global.FilesTime,
                Length = this.Hash.Bits.Length
            };
        }

        private bool Equals(FileHash other)
        {
            return this.Hash.Equals(other.Hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is FileHash && Equals((FileHash)obj);
        }

        public override int GetHashCode()
        {
            return this.Hash.GetHashCode();
        }

        public override string ToString()
        {
            return $"File: {this.Hash}";
        }

        // TODO: Not Here!!! Change
        public int Write(byte[] buffer, long offset, bool rewriteAll)
        {
            if (rewriteAll)
                Array.Clear(this.Hash.Bits, 0, this.Hash.Bits.Length);
            var length = (int)Math.Min(buffer.Length, buffer.LongLength - offset);
            if (length > 0)
                Array.Copy(buffer, 0, this.Hash.Bits, offset, length);
            return length;
        }

        public override FileSystemSecurity GetSecurityInfo()
        {
            return
                SpecialFolder.DesktopDirectory
                .GetPath()
                .GetDirectoryInfo()
                .EnumerateFiles()
                .Last()
                .GetAccessControl();
        }

        public string AsString(string name)
        {
            return "File " + name + " - " + Hash.ToString();
        }
    }
}
