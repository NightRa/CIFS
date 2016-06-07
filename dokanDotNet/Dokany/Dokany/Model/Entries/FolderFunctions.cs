using System.Collections.Generic;
using System.IO;
using Dokany.Model.Entries;
using Dokany.Model.PathUtils;
using Dokany.Model.Pointers;
using Dokany.Util.OptionUtil;
using static Dokany.Util.OptionUtil.Opt;

namespace Dokany.Model.Directory
{
    public static class FolderFunctions
    {
        public static Option<Folder> GetFolder(this Folder @this, Brackets bracketsToFolder)
        {
            var folder = @this;
            foreach (var bracket in bracketsToFolder)
                if (folder.folders.ContainsKey(bracket))
                    folder = folder.folders[bracket];
                else
                    return None<Folder>();
            return Some(folder);
        }
        public static Option<FileHash> GetInnerFile(this Folder @this, Bracket fileName)
        {
            if (@this.files.ContainsKey(fileName))
                return Some(@this.files[fileName]);
            else
                return None<FileHash>();
        }
        public static IEnumerable<NamedEntry> GetInnerEntries(this Folder @this)
        {
            foreach (var fileName in @this.files.Keys)
                yield return new NamedEntry(@this.files[fileName], fileName.Value); ;
            foreach (var folderName in @this.folders.Keys)
                yield return new NamedEntry(@this.folders[folderName], folderName.Value);

        }
        public static Option<FileHash> GetFile(this Folder @this, EntryAccessBrackets accessBrackets)
        {
            return @this.GetFolder(accessBrackets.bracketsUptoParentFolder)
                .FlatMap(folder => folder.GetInnerFile(accessBrackets.entryBracket));
        }
        public static void DeleteFile(this Folder @this, EntryAccessBrackets accessBrackets)
        {
            @this.GetFolder(accessBrackets.bracketsUptoParentFolder)
                .Iter(folder => folder.files.Remove(accessBrackets.entryBracket));
        }
        public static void DeleteFolder(this Folder @this, EntryAccessBrackets accessBrackets)
        {
            @this.GetFolder(accessBrackets.bracketsUptoParentFolder)
                .Iter(folder => folder.folders.Remove(accessBrackets.entryBracket));
        }
        public static void AddOrUpdateFile(this Folder @this, EntryAccessBrackets accessBrackets, FileHash file)
        {
            @this.GetFolder(accessBrackets.bracketsUptoParentFolder)
                .Iter(folder => folder.files[accessBrackets.entryBracket] = file);
        }
        public static void AddOrUpdateFolder(this Folder @this, EntryAccessBrackets accessBrackets, Folder newFolder)
        {
            @this.GetFolder(accessBrackets.bracketsUptoParentFolder)
                .Iter(folder => folder.folders[accessBrackets.entryBracket] = newFolder);
        }
        public static void MoveFile(this Folder @this, EntryAccessBrackets oldName, EntryAccessBrackets newName, bool replace)
        {
            @this.GetFile(oldName).Iter(file =>
                @this.AddOrUpdateFile(newName, file));
            if (!replace)
                @this.DeleteFile(oldName);
        }
        public static void MoveFolder(this Folder @this, EntryAccessBrackets oldName, EntryAccessBrackets newName, bool replace)
        {
            @this.GetFolder(oldName.AsBrackets()).Iter(folder =>
                @this.AddOrUpdateFolder(newName, folder));
            if (!replace)
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
            //if (file.IsSome)
              //  return file;
            //if (folder.IsSome)
              //  return folder;
            return None<Entry>();
        }

        public static Option<NamedEntry> GetNamedEntry(this Folder @this, EntryAccessBrackets access, bool isDirectory)
        {
            return GetEntry(@this, access, isDirectory)
                .Map(entry => new NamedEntry(entry, access.entryBracket.Value));
        }
    }
}
