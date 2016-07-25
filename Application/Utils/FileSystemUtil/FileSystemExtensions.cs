using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IWshRuntimeLibrary;
using static System.Environment;
using File = System.IO.File;

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

        public static bool DoesFileExists(this string filePath, Action<string> log)
        {
            var doesFileExist = File.Exists(filePath);
            log("Folder " + filePath + " " + (doesFileExist ? "" : "doesn't ") + "exist");
            return doesFileExist;
        }
        public static void CreateFile(this string filePath, byte[] bytes, Action<string> log)
        {
            log("Creating file " + filePath);
            using (var file = File.Create(filePath, bytes.Length))
                file.Write(bytes, 0, bytes.Length);
        }
        public static bool DoesFolderExists(this string folderPath, Action<string> log)
        {
            var doesFolderExist = Directory.Exists(folderPath);
            log("Folder " + folderPath + " " + (doesFolderExist ? "" : "doesn't ") + "exist");
            return doesFolderExist;
        }
        public static void CreateDirectory(this string folderPath, FileAttributes additionalAttributes, Action<string> log)
        {
            log("Creating directory " + folderPath);
            var dir = Directory.CreateDirectory(folderPath);
            dir.Attributes |= additionalAttributes;
        }
        public static void CreateDirectoryIfDoesntExist(this string folderPath, FileAttributes additionalAttributes, Action<string> log)
        {
            if (!folderPath.DoesFolderExists(log))
                folderPath.CreateDirectory(FileAttributes.Hidden, log);
        }
        public static byte[] ReadAllBytes(this string filePath)
        {
            return File.ReadAllBytes(filePath);
        }


        public static void CreateShortcut(this string executablePath, string destinationPath, string iconPath, Action<string> log)
        {
            log("Creating shortcut to " + executablePath + " at " + destinationPath);
            var wsh = new IWshShell_Class();
            var shortcut = wsh.CreateShortcut(destinationPath) as IWshShortcut;
            shortcut.TargetPath = executablePath;
            shortcut.IconLocation = iconPath;
            shortcut.Save();
        }
    }
}
