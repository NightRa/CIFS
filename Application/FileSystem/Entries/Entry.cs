using System;
using System.IO;
using System.Security.AccessControl;
using Constants;
using DokanNet;
using Utils;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace FileSystem.Entries
{
    public abstract class Entry
    {
        public abstract FileInformation GetInfo(string name);

        public abstract FileSystemSecurity GetSecurityInfo();

        public sealed class Info
        {
            public string Name { get; }
            public long Length { get; }
            public bool IsFolder { get; }
            public bool IsFile => !IsFolder;
            public bool IsReadOnly { get; }
            public Info(string name, long length, bool isReadOnly, bool isFolder)
            {
                Name = name;
                Length = length;
                IsReadOnly = isReadOnly;
                IsFolder = isFolder;
            }

            public FileInformation AsFileInfo()
            {
                var info = new FileInformation();
                info.Length = IsFolder ? 0 : Length;
                info.Attributes = IsFolder ? FileAttributes.Directory : FileAttributes.Normal;
                if (IsReadOnly)
                    info.Attributes |= FileAttributes.ReadOnly;
                info.CreationTime = info.LastAccessTime = info.LastWriteTime = Global.FoldersTime;
                info.FileName = Name;
                return info;
            }

            public static ParsingResult<Info> Parse(byte[] bytes, Box<int> index)
            {
                return
                    bytes
                        .GetByte(index)
                        .FlatMap(isFolder =>
                            bytes
                                .GetByte(index)
                                .FlatMap(isReadOnly =>
                                    bytes
                                        .GetLong(index)
                                        .FlatMap(size =>
                                            bytes
                                                .GetString(index)
                                                .Map(name =>
                                                    new Info(name, size, isReadOnly == 0, isFolder == 1)
                                                ))));
            }
        }
    }
}
