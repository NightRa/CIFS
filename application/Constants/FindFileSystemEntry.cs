using System;
using System.IO;
using System.Linq;
using Utils.FileSystemUtil;
using Utils.OptionUtil;
using static System.Environment.SpecialFolder;
using static Constants.Global;
using static Utils.OptionUtil.Opt;

namespace Constants
{
    public static class FindFileSystemEntry
    {
        public static FileInfo FindFilePath()
        {
            if (CifsPreferencesDataPath.DoesFileExists())
                return CifsPreferencesDataPath.GetFileInfo();
            if (CifsIndexDataPath.DoesFileExists())
                return CifsIndexDataPath.GetFileInfo();
            var maybeFile1 = Desktop.GetPath().TryFindFileInFolder();
            var maybeFile2 = MyMusic.GetPath().TryFindFileInFolder();
            var maybeFile3 = MyPictures.GetPath().TryFindFileInFolder();
            var maybeFile4 = MyDocuments.GetPath().TryFindFileInFolder();
            return 
                maybeFile1.OrElse(
                    maybeFile2.OrElse(
                        maybeFile3.OrElse(
                            maybeFile4.OrElse(
                                new FileInfo("")))));
        }

        public static DirectoryInfo FindDirectoryPath()
        {
            if (CifsDirectoryPath.DoesFolderExists())
                return CifsDirectoryPath.GetDirectoryInfo();
            return MyDocuments.GetPath().GetDirectoryInfo();
        }

        private static Option<FileInfo> TryFindFileInFolder(this string folderPath)
        {
            if (folderPath.DoesFolderExists())
                if (folderPath.GetDirectoryInfo().EnumerateFiles().Any())
                    return Some(folderPath.GetDirectoryInfo().EnumerateFiles().First());
            return None<FileInfo>();
        }
    }
}
