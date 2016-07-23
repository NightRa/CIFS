﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utils.FileSystemUtil;
using static System.Environment.SpecialFolder;

namespace Constants
{
    public static class Global
    {
        public static DateTime FilesTime => new DateTime(2016, 7, 21);
        public static DateTime FoldersTime => new DateTime(2016, 7, 20);
        public static TimeSpan AgentSleepTime => TimeSpan.FromMilliseconds(4.0);

        public static string CifsDirectoryPath => ProgramFilesX86.GetPath().CombinePathWith("CIFS");
        public static string CifsPreferencesDataPath => CifsDirectoryPath.CombinePathWith("Preferences.dat");
        public static string CifsIndexDataPath => CifsDirectoryPath.CombinePathWith("Index.dat");
        public static char[] AvailableDriverChars()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var driverChars = DriveInfo.GetDrives().Select(d => d.Name.First()).ToArray();
            return chars.Where(c => !driverChars.Contains(c)).ToArray();
        }

        public static char DefaultDriverChar => AvailableDriverChars().First();
    }
}
