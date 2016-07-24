using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Environment;

namespace Utils.FileSystemUtil
{
    public static class FileSystemExtensions
    {
        public static string GetPath(this SpecialFolder @this)
        {
            return GetFolderPath(@this);
        }

        public static DirectoryInfo GetDirectoryInfo(this string @this)
        {
            return new DirectoryInfo(@this);
        }

        public static FileInfo GetFileInfo(this string @this)
        {
            return new FileInfo(@this);
        }

        public static string CombinePathWith(this string path, string bracket)
        {
            return Path.Combine(path, bracket);
        }

        public static bool DoesFileExists(this string filePath)
        {
            return File.Exists(filePath);
        }
        public static void CreateFile(this string filePath, byte[] bytes)
        {
            using (var file = File.Create(filePath, bytes.Length))
                file.Write(bytes, 0, bytes.Length);
        }
        public static bool DoesFolderExists(this string folderPath)
        {
            return Directory.Exists(folderPath);
        }
        public static void CreateDirectory(this string folderPath, FileAttributes additionalAttributes)
        {
            var dir = Directory.CreateDirectory(folderPath);
            dir.Attributes |= additionalAttributes;
        }
        public static byte[] ReadAllBytes(this string filePath)
        {
            return File.ReadAllBytes(filePath);
        }
    }
}
