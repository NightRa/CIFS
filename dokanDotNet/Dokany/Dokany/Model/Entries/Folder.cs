using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DokanNet;
using Dokany.Model.IndexExamples;
using Dokany.Model.PathUtils;
using Dokany.Model.Pointers;
using Dokany.Util;

namespace Dokany.Model.Entries
{
    public sealed class Folder : Entry, IDeepCopiable<Folder>
    {
        public readonly Dictionary<Bracket, FileHash> files;
        public readonly Dictionary<Bracket, RemotePath> follows;
        public readonly Dictionary<Bracket, Folder> folders;

        public Folder(Dictionary<Bracket, FileHash> files, Dictionary<Bracket, RemotePath> follows, Dictionary<Bracket, Folder> folders)
        {
            this.files = files;
            this.follows = follows;
            this.folders = folders;
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
            return files.EqualDictionary(other.files) &&
                folders.EqualDictionary(other.folders) &&
                follows.EqualDictionary(other.follows);
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

        public Folder DeepCopy()
        {
            return new Folder(files.DeepCopy(), follows.DeepCopy(), folders.DeepCopy());
        }

    }
}
