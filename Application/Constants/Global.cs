﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using Utils.FileSystemUtil;
using static System.Environment.SpecialFolder;

namespace Constants
{
    public static class Global
    {
        public static string StartUpShortcutName => "uBox.lnk";
        public static string LocalHost => "127.0.0.1";
        public static int TcpPort => 8008;
        public static DateTime FilesTime => new DateTime(2016, 7, 21);
        public static DateTime FoldersTime => new DateTime(2016, 7, 20);
        public static TimeSpan AgentSleepTime => TimeSpan.FromMilliseconds(4.0);

        public static readonly object DokanRunningObject = new object();
        public static readonly string CifsDirectoryPath = MyDocuments.GetPath().CombinePathWith("uBox");
        public static readonly string CifsIconPath = CifsDirectoryPath.CombinePathWith("uBoxIcon.ico");
        public static readonly string CifsPreferencesDataPath = CifsDirectoryPath.CombinePathWith("Preferences.dat");
        public static readonly string CifsIndexDataPath = CifsDirectoryPath.CombinePathWith("Index.dat");

        public static char[] AvailableDriverChars()
        {
            const string chars = "BCDEFGHIJKLMNOPQRSTUVWXYZ"; /* 'A' is not allowed! */
            var driverChars = DriveInfo.GetDrives().Select(d => d.Name.First()).ToArray();
            return chars.Where(c => !driverChars.Contains(c)).ToArray();
        }
        public static char DefaultDriverChar => AvailableDriverChars().First();

        public static DirectoryInfo ReadOnlyDirectory(Action<string> log)
        {
            var path = CifsDirectoryPath.CombinePathWith("ReadOnlyDir");
            if (!path.DoesFolderExists(log))
                path.CreateDirectory(FileAttributes.ReadOnly, log);
            return path.GetDirectoryInfo();
        }
        public static FileInfo ReadOnlyFile(Action<string> log)
        {
            var path = CifsDirectoryPath.CombinePathWith("ReadOnlyFile.txt");
            if (!path.DoesFileExists(log))
                path.CreateFile(new byte[0], log);
            path.GetFileInfo().Attributes |= FileAttributes.ReadOnly;
            return path.GetFileInfo();
        }
        public static DirectoryInfo RegularDirectory(Action<string> log)
        {
            var path = CifsDirectoryPath.CombinePathWith("RegularDir");
            if (!path.DoesFolderExists(log))
                path.CreateDirectory(FileAttributes.Directory, log);
            return path.GetDirectoryInfo();
        }
        public static FileInfo RegularFile(Action<string> log)
        {
            var path = CifsDirectoryPath.CombinePathWith("RegularFile.txt");
            if (!path.DoesFileExists(log))
                path.CreateFile(new byte[0], log);
            path.GetFileInfo().Attributes |= FileAttributes.Normal;
            return path.GetFileInfo();
        }
    }
}
