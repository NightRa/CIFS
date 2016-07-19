﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using DokanNet;
using Dokany.Model.Entries;
using Dokany.Model.PathUtils;
using Dokany.Model.Pointers;
using Dokany.Util;
using Dokany.Util.OptionUtil;
using static Dokany.Util.Global;
using FileAccess = DokanNet.FileAccess;

namespace Dokany.CifsDriver
{
    public sealed class CifsDriverInstance : IDokanOperations
    {
        private const FileAccess DataAccess = FileAccess.ReadData | FileAccess.WriteData | FileAccess.AppendData |
                                              FileAccess.Execute |
                                              FileAccess.GenericExecute | FileAccess.GenericWrite | FileAccess.GenericRead;
        public Index Index { get; set; }
        public Folder MainFolder => Index.MainFolder;
        public CifsDriverInstance(Index index)
        {
            this.Index = index;
        }
        private static void WriteToLog(string data)
        {
            if (data.Contains("*"))
                Console.WriteLine(data);
        }

        public NtStatus CreateFile(string fileName, FileAccess fileAccess, FileShare share, FileMode mode,
            FileOptions options,
            FileAttributes attributes, DokanFileInfo info)
        {
            WriteToLog("CreateFile " + fileName);
            var maybeAccess = EntryAccessBrackets.FromPath(fileName);
            if (fileName.EndsWith("desktop.ini"))
                return NtStatus.Success;
            if (info.DeleteOnClose)
                WriteToLog("**** CreateFile: Delete on close, " + fileName);

            bool readWriteAttributes = (fileAccess & DataAccess) == 0;
            var folder = MainFolder.GetFolder(fileName.AsBrackets());
            var file = maybeAccess.FlatMap(MainFolder.GetFile);
            var pathExists = file.IsSome || folder.IsSome;


            if (info.IsDirectory)
            {
                switch (mode)
                {
                    case FileMode.CreateNew:
                        if (folder.IsSome)
                            return DokanResult.FileExists;
                        MainFolder.AddOrUpdateFolder(maybeAccess.ValueUnsafe, Folder.Empty);
                        return NtStatus.Success;
                    case FileMode.Open:
                        if (folder.IsNone)
                            return DokanResult.PathNotFound;
                        return NtStatus.Success;
                    default:
                        return NtStatus.Success;
                }
            }

            // info: File
            switch (mode)
            {
                case FileMode.Open:
                    if (!pathExists)
                        return DokanResult.FileNotFound;
                    if (folder.IsSome || readWriteAttributes)
                    {
                        info.IsDirectory = folder.IsSome;
                        info.Context = new object();
                    }
                    return NtStatus.Success;
                case FileMode.CreateNew:
                    if (pathExists)
                        return DokanResult.FileExists;
                    break;
                case FileMode.Truncate:
                    if (!pathExists)
                        return DokanResult.FileNotFound;
                    break;
            }
            info.Context = new object();
            if (maybeAccess.IsSome)
                lock (Rand)
                    MainFolder.AddOrUpdateFile(maybeAccess.ValueUnsafe, new FileHash(Hash.Random(60, Rand)));

            return NtStatus.Success;

        }

        public void Cleanup(string fileName, DokanFileInfo info)
        {
            WriteToLog("Cleanup " + fileName);
            info.Context = null;

            if (info.DeleteOnClose)
            {
                WriteToLog("**** Cleanup with delete request: " + fileName + ", isDirectory: " + info.IsDirectory);
                EntryAccessBrackets.FromPath(fileName).Iter(access =>
                {
                    if (info.IsDirectory)
                        MainFolder.DeleteFolder(access);
                    else
                        MainFolder.DeleteFile(access);
                });
            }
        }

        public void CloseFile(string fileName, DokanFileInfo info)
        {
            WriteToLog("CloseFile " + fileName);
            if (info.DeleteOnClose)
            {
                WriteToLog("**** Close with delete request: " + fileName + ", isDirectory: " + info.IsDirectory);
            }
            info.Context = null;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, DokanFileInfo info)
        {
            WriteToLog("************ ReadFile " + fileName + ", Buffer length: " + buffer.Length);
            var readBytes = 0;
            EntryAccessBrackets.FromPath(fileName)
                .FlatMap(access => MainFolder.GetFile(access))
                .Iter(file =>
            {
                var bytes = file.Hash.Bits.AsHexBytes();
                var length = Math.Min(buffer.Length, bytes.Length);
                Array.Copy(bytes, offset, buffer, 0, length);
                readBytes = length;
            });
            bytesRead = readBytes;
            return NtStatus.Success;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, DokanFileInfo info)
        {
            WriteToLog("****** WriteFile " + fileName + ", buffer length: " + buffer.Length);
         //   int writtenBytes = 0;

       //     EntryAccessBrackets.FromPath(fileName).Iter(access =>
       //         MainFolder.GetFile(access).Iter(file =>
       //             writtenBytes = file.Write(buffer, offset, info.WriteToEndOfFile)
       //             )
       //         );

            bytesWritten = buffer.Length;
            return NtStatus.Success;
        }

        public NtStatus FlushFileBuffers(string fileName, DokanFileInfo info)
        {
            WriteToLog("***** FlushFileBuffers " + fileName);
            return NtStatus.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, DokanFileInfo info)
        {
            WriteToLog("GetFileInformation " + fileName);
            var fileInformation = new FileInformation
            {
                FileName = "", Attributes = FileAttributes.Directory, CreationTime = TempTime, LastWriteTime = TempTime, LastAccessTime = TempTime, Length = 0
            };

            fileInfo =
                EntryAccessBrackets.FromPath(fileName)
                .FlatMap(access => MainFolder.GetNamedEntry(access, info.IsDirectory))
                .Map(ne => ne.Entry.GetInfo(ne.Name))
                .OrElse(fileInformation);

            return NtStatus.Success;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, DokanFileInfo info)
        {
            WriteToLog("*** ---- FindFiles " + fileName);
            var brackets = fileName.AsBrackets();
            var folder = MainFolder.GetFolder(brackets);
            if (folder.IsSome)
                files = folder.ValueUnsafe
                    .GetInnerEntries()
                    .Select(nm => nm.Entry.GetInfo(nm.Name))
                    .Concat(GetEmptyDirectoryDefaultFiles())
                    .ToArray();
            else
                files = GetEmptyDirectoryDefaultFiles();

            return NtStatus.Success;
        }

        private static IList<FileInformation> GetEmptyDirectoryDefaultFiles()
            => new[] {
                new FileInformation { FileName = ".", Attributes = FileAttributes.Directory, CreationTime = DateTime.Today, LastWriteTime = DateTime.Today, LastAccessTime = DateTime.Today },
                new FileInformation { FileName = "..", Attributes = FileAttributes.Directory, CreationTime = DateTime.Today, LastWriteTime = DateTime.Today, LastAccessTime = DateTime.Today }
            };

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, DokanFileInfo info)
        {
            WriteToLog("*** SetFileAttributes " + fileName);
            return NtStatus.Success;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, DokanFileInfo info)
        {
            WriteToLog("*** SetFileTime " + fileName);
            return NtStatus.Success;
        }

        public NtStatus DeleteFile(string fileName, DokanFileInfo info)
        {
            WriteToLog("*** DeleteFile " + fileName);
            var access = EntryAccessBrackets.FromPath(fileName);
            var fileExists = access.FlatMap(MainFolder.GetFile).IsSome;
            if (!fileExists)
                return DokanResult.FileNotFound;
            access.Iter(MainFolder.DeleteFile);
            return NtStatus.Success;
        }

        public NtStatus DeleteDirectory(string fileName, DokanFileInfo info)
        {
            WriteToLog("*** DeleteFile " + fileName);
            var access = EntryAccessBrackets.FromPath(fileName);
            var folderExists = MainFolder.GetFolder(fileName.AsBrackets()).IsSome;
            if (!folderExists)
                return DokanResult.FileNotFound;
            var isEmpty = MainFolder.GetFolder(fileName.AsBrackets()).ValueUnsafe.IsEmpty;
            if (!isEmpty)
                return DokanResult.DirectoryNotEmpty;
            access.Iter(MainFolder.DeleteFolder);
            return NtStatus.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, DokanFileInfo info)
        {
            WriteToLog($"****** MoveFile old: {oldName}, new: {newName}, replace: {replace}");

            EntryAccessBrackets.FromPath(oldName)
                .Iter(oldAccess => EntryAccessBrackets.FromPath(newName)
                    .Iter(newAccess => MainFolder.MoveFile(oldAccess, newAccess))
                 );

            return NtStatus.Success;
        }

        public NtStatus SetEndOfFile(string fileName, long length, DokanFileInfo info)
        {
            WriteToLog("****** SetEndOfFile " + fileName);
            return NtStatus.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, DokanFileInfo info)
        {
            WriteToLog("***** SetAllocationSize " + fileName);
            return NtStatus.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            WriteToLog("**** LockFile " + fileName);
            return NtStatus.Success;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            WriteToLog("**** UnlockFile " + fileName);
            return NtStatus.Success;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, DokanFileInfo info)
        {
            WriteToLog("GetDiskFreeSpace");
            totalNumberOfBytes = 1024L*1024L;
            freeBytesAvailable = totalNumberOfBytes;
            totalNumberOfFreeBytes = totalNumberOfBytes;
            return NtStatus.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, DokanFileInfo info)
        {
            WriteToLog("GetVolumeInformation");
            volumeLabel = "DOKAN";
            fileSystemName = "NTFS";

            features = FileSystemFeatures.CasePreservedNames | FileSystemFeatures.CaseSensitiveSearch | FileSystemFeatures.PersistentAcls | FileSystemFeatures.SupportsRemoteStorage | FileSystemFeatures.UnicodeOnDisk;
            return NtStatus.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, DokanFileInfo info)
        {
            WriteToLog("***** GetFileSecurity " + fileName);
            security = new FileSecurity();
            try
            {
                var maybeEntry =
                    EntryAccessBrackets.FromPath(fileName)
                        .FlatMap(access => MainFolder.GetEntry(access));
                if (maybeEntry.IsNone)
                    return DokanResult.PathNotFound;

                security = maybeEntry.ValueUnsafe.GetSecurityInfo();
                return NtStatus.Success;
            }
            catch (UnauthorizedAccessException)
            {
                security = null;
                return DokanResult.AccessDenied;
            }
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, DokanFileInfo info)
        {
            WriteToLog("****** SetFileSecurity " + fileName);
            return NtStatus.Success;
        }

        public NtStatus Mounted(DokanFileInfo info)
        {
            WriteToLog("Mounted");
            return NtStatus.Success;
        }

        public NtStatus Unmounted(DokanFileInfo info)
        {
            WriteToLog("Unmounted");
            return NtStatus.Success;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, DokanFileInfo info)
        {
            WriteToLog("****** FindStreams " + fileName);
            streams = new FileInformation[0];
            return NtStatus.Success;
        }
    }
}
