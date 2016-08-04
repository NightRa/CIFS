using System.IO;

namespace Constants
{
    public static class FindFileSystemEntry
    {
        public static FileInfo FindFilePath()
        {
            return Global.RegularFile(_ => {});
        }

        public static DirectoryInfo FindDirectoryPath()
        {
            return Global.RegularDirectory(_ => { });
        }
    }
}
