using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using Communication;
using Communication.DokanMessaging.CreateFile;
using Communication.DokanMessaging.CreateFolder;
using Communication.DokanMessaging.Delete;
using Communication.DokanMessaging.Flush;
using Communication.DokanMessaging.GetInnerEntries;
using Communication.DokanMessaging.Move;
using Communication.DokanMessaging.ReadFile;
using Communication.DokanMessaging.Stat;
using Communication.DokanMessaging.WriteFile;
using Constants;
using DokanNet;
using Utils.ConcurrencyUtils;
using static Constants.Global;
using FileAccess = DokanNet.FileAccess;

namespace Dokan
{
    public sealed class CifsIpfsDriverInstance : IDokanOperations
    {
        private const FileAccess DataAccess = FileAccess.ReadData       |
                                              FileAccess.WriteData      |
                                              FileAccess.AppendData     |
                                              FileAccess.Execute        |
                                              FileAccess.GenericExecute |
                                              FileAccess.GenericWrite   |
                                              FileAccess.GenericRead;
        public CommunicationAgent Communicator { get; }
        public Action<string> Log { get; }

        private NtStatus WriteToLog(NtStatus result, string message)
        {
            Log(message);
            return result;
        }
        public CifsIpfsDriverInstance(CommunicationAgent communicator, Action<string> log)
        {
            Communicator = communicator;
            Log = log;
        }

        public NtStatus CreateFile(string fileName, FileAccess access, FileShare share, FileMode mode,
            FileOptions options,
            FileAttributes attributes, DokanFileInfo info)
        {
            Log("CreateFile " + fileName + ", delete on close: " + info.DeleteOnClose);
            if (fileName.EndsWith("desktop.ini"))
                return DokanResult.Success;
            bool readWriteAttributes = (access & DataAccess) == 0;
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            var stat = maybeStat.ResultUnsafe;
            if (info.IsDirectory)
            {
                switch (mode)
                {
                    case FileMode.CreateNew:
                        if (stat.IsFolder)
                            return DokanResult.FileExists;
                        var res = Communicator.GetResponse(new CreateFolderRequest(fileName), CreateFolderResponse.Parse);
                        if (res.IsError)
                            return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + res.ErrorUnsafe);
                        if (res.ResultUnsafe.NameCollosion)
                            return WriteToLog(DokanResult.AlreadyExists, "Name collision in create folder! " + fileName);
                        if (res.ResultUnsafe.IsReadOnly)
                            return WriteToLog(DokanResult.AccessDenied, "Create folder in read-only folder!! " + fileName);
                        if (res.ResultUnsafe.PathToParentDoesntExist)
                            return WriteToLog(DokanResult.PathNotFound, "Create folder Path not found....! " + fileName);
                        if (res.ResultUnsafe.InvalidName)
                            return WriteToLog(DokanResult.Error, "Create folder Invalid name....! " + fileName);
                        return DokanResult.Success;
                    case FileMode.Open:
                        if (stat.IsFile)
                            return DokanResult.PathNotFound;
                        return DokanResult.Success;
                    default:
                        return DokanResult.Success;
                }
            }
            // file
            switch (mode)
            {
                case FileMode.Open:
                    if (!stat.EntryExists)
                        return DokanResult.FileNotFound;
                    if (stat.IsFolder || readWriteAttributes)
                    {
                        info.IsDirectory = stat.IsFolder;
                        info.Context = new object();
                    }
                    return DokanResult.Success;
                case FileMode.CreateNew:
                    if (stat.EntryExists)
                        return DokanResult.FileExists;
                    break;
                case FileMode.Truncate:
                    if (!stat.EntryExists)
                        return DokanResult.FileNotFound;
                    break;
            }
            info.Context = new object();
            var creationRes = Communicator.GetResponse(new CreateFileRequest(fileName), CreateFileResponse.Parse);
            if (creationRes.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + creationRes.ErrorUnsafe);
            if (creationRes.ResultUnsafe.IsReadOnlyFolder)
                return WriteToLog(DokanResult.AccessDenied, "Create file in read-only folder!! " + fileName);
            if (creationRes.ResultUnsafe.PathToParentDoesntExist)
                return WriteToLog(DokanResult.PathNotFound, "Create file Path not found....! " + fileName);
            if (creationRes.ResultUnsafe.IsNameCollision)
                return WriteToLog(DokanResult.AlreadyExists, "Name collision in create file! " + fileName);
            if (creationRes.ResultUnsafe.InvalidName)
                return WriteToLog(DokanResult.Error, "Invalid name in create file! " + fileName);
            return DokanResult.Success;
        }

        public void Cleanup(string fileName, DokanFileInfo info)
        {
            Log("Cleanup " + fileName + ", delete request: " + info.DeleteOnClose);
            info.Context = null;

            if (info.DeleteOnClose)
            {
                var deleteResponse = Communicator.GetResponse(new DeleteRequest(fileName), DeleteResponse.Parse);
                if (deleteResponse.IsError)
                    Log("Parsing error!!! " + deleteResponse.ErrorUnsafe);
                else
                {
                    if (deleteResponse.ResultUnsafe.PathDoesntExist)
                        Log("delete request on file that doesnt exist" + fileName);
                    if (deleteResponse.ResultUnsafe.IsReadOnly)
                        Log("delete request when folder is readonly " + fileName);
                }
            }
        }

        public void CloseFile(string fileName, DokanFileInfo info)
        {
            Log("CloseFile " + fileName + ", delete request: " + info.DeleteOnClose);
            info.Context = null;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, DokanFileInfo info)
        {
            Log("ReadFile " + fileName + ", Buffer length: " + buffer.Length + ", offset: " + offset);
            bytesRead = 0;
            var readRequest = new ReadFileRequest(fileName, offset, buffer.Length);
            var res = Communicator.GetResponse(readRequest, ReadFileResponse.Parse);
            if (res.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + res.ErrorUnsafe);
            if (!res.ResultUnsafe.DoesFileExist)
                return WriteToLog(DokanResult.FileNotFound, "Read file when file doesnt exist " + fileName);
            bytesRead = res.ResultUnsafe.BytesRead.Length;
            res.ResultUnsafe.BytesRead.CopyTo(buffer, 0);
            return DokanResult.Success;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, DokanFileInfo info)
        {
            bytesWritten = 0;
            var res = Communicator.GetResponse(new WriteFileRequest(fileName, buffer, offset), WriteFileResponse.Parse);
            if (res.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + res.ErrorUnsafe);
            if (res.ResultUnsafe.FileDoesntExist)
                return WriteToLog(DokanResult.FileNotFound, "Write to file that doesnt exist.. " + fileName);
            if (res.ResultUnsafe.IsFileReadonly)
                return WriteToLog(DokanResult.AccessDenied, "Write to file that is readonly.. " + fileName);
            return DokanResult.Success;
        }

        public NtStatus FlushFileBuffers(string fileName, DokanFileInfo info)
        {
            var res = Communicator.GetResponse(new FlushRequest(), FlushResponse.Parse);
            if (res.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + res.ErrorUnsafe);
            return DokanResult.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, DokanFileInfo info)
        {
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            fileInfo = new FileInformation();
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return WriteToLog(DokanResult.FileNotFound, "Entry doesnt exist....... " + fileName);
            fileInfo = maybeStat.ResultUnsafe.ToFileInfo(fileName);
            return DokanResult.Success;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, DokanFileInfo info)
        {
            Log("FindFiles " + fileName);
            files = DefaultFiles;
            var maybeInnerFiles = Communicator.GetResponse(new GetInnerEntriesRequest(fileName),
                GetInnerEntriesResponse.Parse);
            if (maybeInnerFiles.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeInnerFiles.ErrorUnsafe);
            if (!maybeInnerFiles.ResultUnsafe.DoesFolderExist)
                return WriteToLog(DokanResult.PathNotFound, "FindFiles when folder doesnt exist... " + fileName);
            files = maybeInnerFiles.ResultUnsafe.Infos.Select(i => i.AsFileInfo()).Concat(DefaultFiles).ToArray();
            return DokanResult.Success;
        }
        private static IList<FileInformation> DefaultFiles
            => new[] {
                new FileInformation { FileName = ".", Attributes = FileAttributes.Directory, CreationTime = FoldersTime, LastWriteTime = FoldersTime, LastAccessTime = FoldersTime },
                new FileInformation { FileName = "..", Attributes = FileAttributes.Directory, CreationTime = FoldersTime, LastWriteTime = FoldersTime, LastAccessTime = FoldersTime }
            };


        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, DokanFileInfo info)
        {
            Log("SetFileAttributes " + fileName);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            return DokanResult.Success;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime,
            DokanFileInfo info)
        {
            Log("SetFileTime " + fileName);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            return DokanResult.Success;
        }

        public NtStatus DeleteFile(string fileName, DokanFileInfo info)
        {
            Log("DeleteFile " + fileName);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            if (maybeStat.ResultUnsafe.IsFolder)
                return DokanResult.FileNotFound;
            return DokanResult.Success;
        }

        public NtStatus DeleteDirectory(string fileName, DokanFileInfo info)
        {
            Log("Delete directory " + fileName);
            var maybeInnerFiles = Communicator.GetResponse(new GetInnerEntriesRequest(fileName),
                GetInnerEntriesResponse.Parse);
            if (maybeInnerFiles.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeInnerFiles.ErrorUnsafe);
            if (!maybeInnerFiles.ResultUnsafe.DoesFolderExist)
                return WriteToLog(DokanResult.PathNotFound, "FindFiles when folder doesnt exist... " + fileName);
            if (maybeInnerFiles.ResultUnsafe.Infos.Any())
                return WriteToLog(DokanResult.DirectoryNotEmpty, "Delete folder request on not empty folder " + fileName);
            return DokanResult.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, DokanFileInfo info)
        {
            Log("Move file, old: " + oldName + ", new: " + newName + ", replace: " + replace);
            var maybeNodeDataOld = Communicator.GetResponse(new StatRequest(oldName), StatResponse.Parse);
            var maybeNodeDataNew = Communicator.GetResponse(new StatRequest(newName), StatResponse.Parse);
            if (maybeNodeDataOld.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!!" + maybeNodeDataOld.ErrorUnsafe);
            if (maybeNodeDataNew.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!!" + maybeNodeDataNew.ErrorUnsafe);
            var nodeDataOld = maybeNodeDataOld.ResultUnsafe;
            var nodeDataNew = maybeNodeDataNew.ResultUnsafe;
            bool exist = false;
            if (info.IsDirectory)
                exist = nodeDataNew.IsFolder;
            else
                exist = nodeDataOld.IsFile;

            if (!exist)
            {
                info.Context = null;
                var moveRes = Communicator.GetResponse(new MoveRequest(oldName, newName), MoveResponse.Parse);
                if (moveRes.IsError)
                    return WriteToLog(DokanResult.InternalError, "Parsing error!!!" + moveRes.ErrorUnsafe);
                if (moveRes.ResultUnsafe.SrcDoesntExist)
                    return WriteToLog(DokanResult.FileNotFound, "move file SrcDoesntExist " + moveRes.ErrorUnsafe);
                if (moveRes.ResultUnsafe.SrcOrDesrReadOnly)
                    return WriteToLog(DokanResult.AccessDenied, "move file SrcOrDesrReadOnly " + moveRes.ErrorUnsafe);
                return DokanResult.Success;
            }
            if (replace)
            {
                info.Context = null;
                var deleteRes = Communicator.GetResponse(new DeleteRequest(newName), DeleteResponse.Parse);
                if (deleteRes.IsError)
                    return WriteToLog(DokanResult.InternalError, "Parsing error!!!" + deleteRes.ErrorUnsafe);

                var moveRes = Communicator.GetResponse(new MoveRequest(oldName, newName), MoveResponse.Parse);
                if (moveRes.IsError)
                    return WriteToLog(DokanResult.InternalError, "Parsing error!!!" + moveRes.ErrorUnsafe);
                if (moveRes.ResultUnsafe.SrcDoesntExist)
                    return WriteToLog(DokanResult.FileNotFound, "move file SrcDoesntExist " + moveRes.ErrorUnsafe);
                if (moveRes.ResultUnsafe.SrcOrDesrReadOnly)
                    return WriteToLog(DokanResult.AccessDenied, "move file SrcOrDesrReadOnly " + moveRes.ErrorUnsafe);
                return DokanResult.Success;
            }
            return DokanResult.FileExists;
        }

        public NtStatus SetEndOfFile(string fileName, long length, DokanFileInfo info)
        {
            Log("SetEndOfFile " + fileName + ", length: " + length);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            return DokanResult.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, DokanFileInfo info)
        {
            Log("SetAllocationSize " + fileName + ", length: " + length);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            return DokanResult.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            Log("LockFile " + fileName);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            return DokanResult.Success;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            Log("UnlockFile " + fileName);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            return DokanResult.Success;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes,
            DokanFileInfo info)
        {
            Log("GetDiskFreeSpace");
            totalNumberOfBytes = 1024L * 1024L * 50;
            freeBytesAvailable = totalNumberOfBytes / 2;
            totalNumberOfFreeBytes = freeBytesAvailable;
            return DokanResult.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features,
            out string fileSystemName,
            DokanFileInfo info)
        {
            Log("GetVolumeInformation");
            volumeLabel = "DOKAN";
            fileSystemName = "uBox";

            features = FileSystemFeatures.CasePreservedNames    |
                       FileSystemFeatures.CaseSensitiveSearch   |
                       FileSystemFeatures.PersistentAcls        |
                       FileSystemFeatures.SupportsRemoteStorage |
                       FileSystemFeatures.UnicodeOnDisk;
            return DokanResult.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections,
            DokanFileInfo info)
        {
            Log("GetFileSecurity " + fileName);
            security = new FileSecurity();
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!!" + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            security = maybeStat.ResultUnsafe.FileSecurity(Log);
            return DokanResult.Success;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections,
            DokanFileInfo info)
        {
            Log("SetFileSecurity " + fileName);
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return DokanResult.PathNotFound;
            return DokanResult.Success;
        }

        public NtStatus Mounted(DokanFileInfo info)
        {
            if (Global.DokanRunningObject.IsHeld())
            {
                Log("DEADLOCK: mounting while dokan is running");
                return DokanResult.Error;
            }
            Monitor.Enter(Global.DokanRunningObject);
            Log("Dokan mounted");
            return DokanResult.Success;
        }

        public NtStatus Unmounted(DokanFileInfo info)
        {
            Log("Dokan unmounted");
            Monitor.Exit(Global.DokanRunningObject);
            return DokanResult.Success;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, DokanFileInfo info)
        {
            Log("FindStreams " + fileName);
            streams = new FileInformation[0];
            var maybeStat = Communicator.GetResponse(new StatRequest(fileName), StatResponse.Parse);
            if (maybeStat.IsError)
                return WriteToLog(DokanResult.InternalError, "Parsing error!!! " + maybeStat.ErrorUnsafe);
            if (!maybeStat.ResultUnsafe.EntryExists)
                return WriteToLog(DokanResult.PathNotFound, "Find stream when file not found...");
            return DokanResult.Success;
        }
    }
}
