using System;
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
        public Index Index { get; set; }
        public Folder MainFolder => Index.MainFolder;
        public CifsDriverInstance(Index index)
        {
            this.Index = index;
        }
        private static void WriteToLog(string data)
        {
            //if (data.Contains("*"))
                Console.WriteLine(data);
        }

        public NtStatus CreateFile(string fileName, FileAccess fileAccess, FileShare share, FileMode mode, FileOptions options,
            FileAttributes attributes, DokanFileInfo info)
        {
            WriteToLog("CreateFile " + fileName);
            if (!fileName.EndsWith("desktop.ini"))
            {
                if (info.DeleteOnClose)
                    WriteToLog("**** CreateFile: Delete on close, " + fileName);
                if (info.Context == null)
                    info.Context = new object();
                switch (mode)
                {
                    case FileMode.CreateNew:
                    case FileMode.Create:
                    case FileMode.OpenOrCreate:
                        EntryAccessBrackets.FromPath(fileName).Iter(access =>
                        {
                            if (info.IsDirectory)
                            {
                                if (MainFolder.GetFolder(access.AsBrackets()).IsNone)
                                    MainFolder.AddOrUpdateFolder(access, Folder.Empty);
                            }
                            else
                            {
                                if (MainFolder.GetFile(access).IsNone)
                                    lock (Rand)
                                        MainFolder.AddOrUpdateFile(access, new FileHash(Hash.Random(60, Rand)));
                            }
                        });
                        break;
                    case FileMode.Open:
                    case FileMode.Truncate:
                    case FileMode.Append:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

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
                //info.Context = null;
            }
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
            WriteToLog("FindFiles " + fileName);
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
            info.Context = null;
            EntryAccessBrackets.FromPath(fileName).Iter(fileAccess => MainFolder.DeleteFile(fileAccess));
            return NtStatus.Success;
        }

        public NtStatus DeleteDirectory(string fileName, DokanFileInfo info)
        {
            WriteToLog("*** DeleteDirectory " + fileName);
            info.Context = null;
            EntryAccessBrackets.FromPath(fileName).Iter(fileAccess => MainFolder.DeleteFolder(fileAccess));
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
            WriteToLog("LockFile " + fileName);
            return NtStatus.Success;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            WriteToLog("UnlockFile " + fileName);
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
