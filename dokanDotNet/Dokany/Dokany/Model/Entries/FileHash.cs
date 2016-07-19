using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using DokanNet;
using Dokany.Model.Pointers;
using Dokany.Util;

namespace Dokany.Model.Entries
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
                CreationTime = Global.TempTime,
                LastWriteTime = Global.TempTime,
                LastAccessTime = Global.TempTime,
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
            return obj is FileHash && Equals((FileHash) obj);
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
                Environment.SpecialFolder.DesktopDirectory
                .GetPath()
                .GetDirectoryInfo()
                .EnumerateFiles()
                .Last()
                .GetAccessControl();
            //return new FileInfo().GetAccessControl();
            /*WindowsIdentity id = WindowsIdentity.GetCurrent();
            FileSystemAccessRule rule = new FileSystemAccessRule(id.Name, FileSystemRights.FullControl,
                AccessControlType.Allow);
            FileSecurity security = new FileSecurity();
            security.AddAccessRule(rule);
            return security;*/
        }

        public string AsString(string name)
        {
            return "File " + name + " - " + Hash.ToString();
        }
    }
}
