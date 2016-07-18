using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using DokanNet;
using Dokany.Model.IndexExamples;
using Dokany.Model.PathUtils;
using Dokany.Model.Pointers;
using Dokany.Util;
using static System.Environment;

namespace Dokany.Model.Entries
{
    public sealed class Folder : Entry
    {
        public Dictionary<Bracket, FileHash> Files { get; }
        public Dictionary<Bracket, RemotePath> Follows { get; }
        public Dictionary<Bracket, Folder> Folders { get; }

        public Folder(Dictionary<Bracket, FileHash> files, Dictionary<Bracket, RemotePath> follows, Dictionary<Bracket, Folder> folders)
        {
            this.Files = files;
            this.Follows = follows;
            this.Folders = folders;
        }


        public override FileInformation GetInfo(string folderName)
        {
            return new FileInformation
            {
                FileName = folderName,
                Attributes = FileAttributes.Directory,
                CreationTime = Global.TempTime,
                LastWriteTime = Global.TempTime,
                LastAccessTime = Global.TempTime,
                Length = 0
            };
        }

        public static Folder Empty => Examples.EmptyFolder();

        private bool Equals(Folder other)
        {
            return Files.EqualDictionary(other.Files) &&
                Folders.EqualDictionary(other.Folders) &&
                Follows.EqualDictionary(other.Follows);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Folder && Equals((Folder) obj);
        }

        public override int GetHashCode()
        {
            return this.AllInnerFiles().EnumerableHashCode();
        }

        public override FileSystemSecurity GetSecurityInfo()
        {
            return 
                SpecialFolder.MyDocuments
                .GetPath()
                .GetDirectoryInfo()
                .GetAccessControl();

        }

        public override string ToString()
        {
            var str = new StringBuilder();

            str.AppendLine("Files: ");
            str.AppendLine(Files.AsString((file,bracket) => file.AsString(bracket.Value)).AddTabs());
            str.AppendLine("Folders");
            str.AppendLine(Folders.AsString((folder, bracket) => NewLine + folder.ToString()).AddTabs());


            return str.ToString();
        }
    }
}
