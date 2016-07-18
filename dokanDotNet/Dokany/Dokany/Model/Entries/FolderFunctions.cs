using System.Collections.Generic;
using System.Linq;
using Dokany.Model.PathUtils;
using Dokany.Util.OptionUtil;

namespace Dokany.Model.Entries
{
    public static class FolderFunctions
    {
        public static Option<Folder> GetFolder(this Folder @this, Brackets bracketsToFolder)
        {
            var folder = @this;
            foreach (var bracket in bracketsToFolder)
                if (folder.Folders.ContainsKey(bracket))
                    folder = folder.Folders[bracket];
                else
                    return Opt.None<Folder>();
            return Opt.Some(folder);
        }
        public static Option<FileHash> GetInnerFile(this Folder @this, Bracket fileName)
        {
            if (@this.Files.ContainsKey(fileName))
                return Opt.Some(@this.Files[fileName]);
            else
                return Opt.None<FileHash>();
        }
        public static IEnumerable<NamedEntry> GetInnerEntries(this Folder @this)
        {
            foreach (var fileName in @this.Files.Keys)
                yield return new NamedEntry(@this.Files[fileName], fileName.Value); ;
            foreach (var folderName in @this.Folders.Keys)
                yield return new NamedEntry(@this.Folders[folderName], folderName.Value);

        }
        public static Option<FileHash> GetFile(this Folder @this, EntryAccessBrackets accessBrackets)
        {
            return @this.GetFolder(accessBrackets.BracketsUptoParentFolder)
                .FlatMap(folder => folder.GetInnerFile(accessBrackets.EntryBracket));
        }
        public static void DeleteFile(this Folder @this, EntryAccessBrackets accessBrackets)
        {
            @this.GetFolder(accessBrackets.BracketsUptoParentFolder)
                .Iter(folder => folder.Files.Remove(accessBrackets.EntryBracket));
        }
        public static void DeleteFolder(this Folder @this, EntryAccessBrackets accessBrackets)
        {
            @this.GetFolder(accessBrackets.BracketsUptoParentFolder)
                .Iter(folder => folder.Folders.Remove(accessBrackets.EntryBracket));
        }
        public static void AddOrUpdateFile(this Folder @this, EntryAccessBrackets accessBrackets, FileHash file)
        {
            @this.GetFolder(accessBrackets.BracketsUptoParentFolder)
                .Iter(folder => folder.Files[accessBrackets.EntryBracket] = file);
        }
        public static void AddOrUpdateFolder(this Folder @this, EntryAccessBrackets accessBrackets, Folder newFolder)
        {
            @this.GetFolder(accessBrackets.BracketsUptoParentFolder)
                .Iter(folder => folder.Folders[accessBrackets.EntryBracket] = newFolder);
        }
        public static void MoveFile(this Folder @this, EntryAccessBrackets oldName, EntryAccessBrackets newName)
        {
            @this.GetFile(oldName)
                .Iter(file => @this.AddOrUpdateFile(newName, file));
            @this.DeleteFile(oldName);
        }
        public static void MoveFolder(this Folder @this, EntryAccessBrackets oldName, EntryAccessBrackets newName)
        {
            @this.GetFolder(oldName.AsBrackets())
                .Iter(folder => @this.AddOrUpdateFolder(newName, folder));
            @this.DeleteFolder(oldName);
        }

        public static Option<Entry> GetEntry(this Folder @this, EntryAccessBrackets access, bool isDirectory)
        {
            var folder = @this.GetFolder(access.AsBrackets());
            var file = @this.GetFile(access);
            if (isDirectory && folder.IsSome)
                return folder;
            if (!isDirectory && file.IsSome)
                return file;
            return Opt.None<Entry>();
        }
        public static Option<Entry> GetEntry(this Folder @this, EntryAccessBrackets access)
        {
            var folder = @this.GetFolder(access.AsBrackets());
            var file = @this.GetFile(access);
            if (folder.IsSome)
                return folder;
            if (file.IsSome)
                return file;
            return Opt.None<Entry>();
        }

        public static Option<NamedEntry> GetNamedEntry(this Folder @this, EntryAccessBrackets access, bool isDirectory)
        {
            return GetEntry(@this, access, isDirectory)
                .Map(entry => new NamedEntry(entry, access.EntryBracket.Value));
        }
        public static IEnumerable<FileHash> AllInnerFiles(this Folder @this)
        {
            return @this.Files.Values.Concat(@this.Folders.Values.SelectMany(AllInnerFiles));
        }
    }
}
