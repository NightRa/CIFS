using System;
using System.IO;
using System.Linq;
using System.Threading;
using Utils.FileSystemUtil;
using static System.Environment.SpecialFolder;

namespace Constants
{
    public static class Global
    {
        public static DateTime FilesTime => new DateTime(2016, 7, 21);
        public static DateTime FoldersTime => new DateTime(2016, 7, 20);
        public static TimeSpan AgentSleepTime => TimeSpan.FromMilliseconds(4.0);

        public static readonly Semaphore DokanSemaphore = new Semaphore(initialCount: 1, maximumCount: 1, name: "IsDokanAliveSemaphore");

        public static string CifsDirectoryPath => MyDocuments.GetPath().CombinePathWith("CIFS");
        public static string CifsIconPath => CifsDirectoryPath.CombinePathWith("CifsIcon.ico");
        public static string CifsPreferencesDataPath => CifsDirectoryPath.CombinePathWith("Preferences.dat");
        public static string CifsIndexDataPath => CifsDirectoryPath.CombinePathWith("Index.dat");
        public static char[] AvailableDriverChars()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var driverChars = DriveInfo.GetDrives().Select(d => d.Name.First()).ToArray();
            return chars.Where(c => !driverChars.Contains(c)).ToArray();
        }

        public static char DefaultDriverChar => AvailableDriverChars().Last();
        public static string StartUpShortcutName => "CIFS.lnk";
    }
}
