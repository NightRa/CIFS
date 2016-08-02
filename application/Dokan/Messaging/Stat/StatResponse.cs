using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Constants;
using Dokan.Messaging.RootHash;
using DokanNet;
using Utils;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;
using static Utils.Parsing.Parse;

namespace Dokan.Messaging.Stat
{
    public sealed class StatResponse
    {
        public static byte TypeNum => StatRequest.TypeNum;
        public bool EntryExists { get; }
        public bool IsFolder { get; }
        public long FileLength { get; }
        public bool IsFile { get; }
        public bool IsReadOnly { get; }

        public StatResponse(bool entryExists, bool isFolder, long fileLength, bool isReadOnly)
        {
            EntryExists = entryExists;
            IsFolder = isFolder && entryExists;
            FileLength = fileLength;
            IsReadOnly = isReadOnly;
            IsFile = !isFolder && entryExists;
        }

        public static ParsingResult<StatResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes
                    .GetByte(index)
                    .FlatMap(num => num.HasToBe(TypeNum)
                        .FlatMap(_ =>
                            bytes.GetByte(index)
                                .FlatMap(x =>
                                {
                                    if (x != 0)
                                        return Return(new StatResponse(false, false, 0, false));
                                    else
                                        return
                                            bytes
                                                .GetByte(index)
                                                .FlatMap(isFolder =>
                                                    bytes
                                                        .GetByte(index)
                                                        .FlatMap(isReadOnly =>
                                                            bytes.GetLong(index)
                                                                .Map(length =>
                                                                    new StatResponse(true, isFolder == 1, length,
                                                                        isReadOnly == 0))));
                                })));
        }

        public FileInformation ToFileInfo(string fileName)
        {
            return new FileInformation
            {
                FileName = fileName,
                CreationTime = Global.FilesTime,
                LastAccessTime = Global.FilesTime,
                LastWriteTime = Global.FilesTime,
                Length = IsFolder ? 0 : FileLength
            };
        }

        public FileSystemSecurity FileSecurity(Action<string> log)
        {
            if (IsFolder && IsReadOnly)
                return Global.ReadOnlyDirectory(log).GetAccessControl();
            if (IsFolder && !IsReadOnly)
                return Global.RegularDirectory(log).GetAccessControl();
            if (IsFile && IsReadOnly)
                return Global.ReadOnlyFile(log).GetAccessControl();
            //if (!IsFile && !IsReadOnly)
            return Global.RegularFile(log).GetAccessControl();

        }
    }
}
