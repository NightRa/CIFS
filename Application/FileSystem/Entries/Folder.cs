using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Constants;
using DokanNet;
using FileSystem.Pointers;
using FileSystemBrackets;
using Utils;
using Utils.Binary;
using Utils.DictionaryUtil;
using Utils.FileSystemUtil;
using Utils.IEnumerableUtil;
using Utils.Parsing;
using Utils.StringUtil;
using static System.Environment;

namespace FileSystem.Entries
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
                CreationTime = Global.FoldersTime,
                LastWriteTime = Global.FoldersTime,
                LastAccessTime = Global.FoldersTime,
                Length = 0
            };
        }

        public static Folder Empty
            =>
                new Folder(new Dictionary<Bracket, FileHash>(), new Dictionary<Bracket, RemotePath>(),
                    new Dictionary<Bracket, Folder>());
        public bool IsEmpty => Files.Count == 0 && Follows.Count == 0 && Folders.Count == 0;

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
            return obj is Folder && Equals((Folder)obj);
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
            str.AppendLine(Files.AsString((file, bracket) => file.AsString(bracket.Value)).AddTabs());
            str.AppendLine("Folders");
            str.AppendLine(Folders.AsString((folder, bracket) => NewLine + folder.ToString()).AddTabs());


            return str.ToString();
        }

        public byte[] ToBytes()
        {
            return
                Files.ToBytes(b => b.ToBytes(), f => f.ToBytes())
                    .Concat(Follows.ToBytes(b => b.ToBytes(), r => r.ToBytes()))
                    .Concat(Folders.ToBytes(b => b.ToBytes(), f => f.ToBytes()))
                    .ToArray();
        }

        public static ParsingResult<Folder> Parse(byte[] bytes, Box<int> index)
        {
            return
                bytes.ToDictionary(index, Bracket.Parse, FileHash.Parse).FlatMap(files =>
                    bytes.ToDictionary(index, Bracket.Parse, RemotePath.Parse).FlatMap(follows =>
                        bytes.ToDictionary(index, Bracket.Parse, Folder.Parse).Map(folders =>
                            new Folder(files, follows, folders)
                            )
                        )
                    );
        }
    }
}
