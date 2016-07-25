using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Agents;
using CifsPreferences;
using FileSystem.Entries;
using Utils;
using Utils.FileSystemUtil;
using static System.Environment.SpecialFolder;
using static Constants.Global;

namespace CifsStartupApp
{
    public static class Initilization
    {
        public static Index InitilizeCifs(Action<string> errorReports)
        {
            var iconName = nameof(CifsStartupApp) + "." + nameof(Resources) + "." +
                           nameof(Properties.Resources.EmbeddedIcon) + ".ico";
            if (!CifsDirectoryPath.DoesFolderExists())
                CifsDirectoryPath.CreateDirectory(FileAttributes.Hidden);
            if (!CifsIconPath.DoesFileExists())
                using (var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconName))
                using (var fileStream = File.Create(CifsIconPath))
                    iconStream.CopyTo(fileStream);
            var preferences = GetPreferences(errorReports);
            ApplyPreferences(preferences);
            return GetIndex(errorReports);
        }

        private static void ApplyPreferences(Preferences preferences)
        {
            var startupFiles =
                Startup.GetPath()
                    .GetDirectoryInfo()
                    .EnumerateFiles()
                    .ToArray();
            var isAppOnStartup = startupFiles.Any(f => f.Name.Equals(StartUpShortcutName));
            if (!preferences.OpenOnStartup && isAppOnStartup)
                startupFiles.First(f => f.Name.Equals(StartUpShortcutName)).Delete();
            if (preferences.OpenOnStartup && !isAppOnStartup)
                Assembly.GetExecutingAssembly().Location
                    .CreateShortcut(Startup.GetPath().CombinePathWith(StartUpShortcutName), CifsIconPath);
        }

        private static Index GetIndex(Action<string> errorReports)
        {
            if (!CifsIndexDataPath.DoesFileExists())
                CifsIndexDataPath.CreateFile(Index.Default().ToBytes());
            var maybeIndex = Index.Parse(CifsIndexDataPath.ReadAllBytes(), new Box<int>(0));
            var index = Index.Default();
            if (maybeIndex.IsResult)
                index = maybeIndex.ResultUnsafe;
            else
            {
                errorReports(maybeIndex.ErrorUnsafe);
                CifsIndexDataPath.CreateFile(index.ToBytes());
            }
            return index;
        }

        private static Preferences GetPreferences(Action<string> errorReports)
        {
            if (!CifsPreferencesDataPath.DoesFileExists())
                CifsPreferencesDataPath.CreateFile(Preferences.Default().ToBytes());
            var maybePreferences = Preferences.Parse(CifsPreferencesDataPath.ReadAllBytes(), new Box<int>(0));
            var preferences = Preferences.Default();
            if (maybePreferences.IsResult)
                preferences = maybePreferences.ResultUnsafe;
            else
            {
                errorReports(maybePreferences.ErrorUnsafe);
                CifsIndexDataPath.CreateFile(preferences.ToBytes());
            }
            return preferences;
        }

        public static void ShowCifsInExplorer()
        {
            Action openExplorer = () => Process.Start(MyDocuments.GetPath());
            openExplorer.DoAsyncBackground("CifsExplorerProcessOpener");
        }

        public static void EditPreferences()
        {
            Action editPreferences = () => {};
            editPreferences.DoAsyncBackground("EditPreferences");
        }

        public static void CloseApp()
        {
            Application.Current.Shutdown();
        }
    }
}
