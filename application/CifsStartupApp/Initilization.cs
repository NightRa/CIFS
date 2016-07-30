using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Agents;
using Agents.AdministratorMessages;
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
        public static void InitilizeCifs(Mail<AdministratorMessage> mail, Action<string> log)
        {
            InitilizeIcon(log);
            var preferences = GetPreferences(log);
            ApplyPreferences(preferences, log);
            var index = GetIndex(log);

            Action loopAction = () => AdministratorAgent.Loop(index, preferences, mail, log);
            loopAction.DoAsyncBackground("AdministratorAgentLoopThread");
        }

        private static void InitilizeIcon(Action<string> log)
        {
            var iconName = nameof(CifsStartupApp) + "." + nameof(Resources) + "." +
                           nameof(Properties.Resources.EmbeddedIcon) + ".ico";
            if (!CifsIconPath.DoesFileExists(log))
                using (var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconName))
                using (var fileStream = File.Create(CifsIconPath))
                    iconStream.CopyTo(fileStream);
        }

        private static void ApplyPreferences(Preferences preferences, Action<string> log)
        {
            log("Applying preferences " + preferences.ToString().Replace(Environment.NewLine, " "));
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
                    .CreateShortcut(Startup.GetPath().CombinePathWith(StartUpShortcutName), CifsIconPath, log);
        }

        private static Index GetIndex(Action<string> log)
        {
            if (!CifsIndexDataPath.DoesFileExists(log))
                CifsIndexDataPath.CreateFile(Index.Default().ToBytes(), log);
            var maybeIndex = Index.Parse(CifsIndexDataPath.ReadAllBytes(), new Box<int>(0));
            var index = Index.Default();
            if (maybeIndex.IsResult)
                index = maybeIndex.ResultUnsafe;
            else
            {
                log("Index parsing error: " + maybeIndex.ErrorUnsafe);
                CifsIndexDataPath.CreateFile(index.ToBytes(), log);
            }
            return index;
        }

        private static Preferences GetPreferences(Action<string> log)
        {
            if (!CifsPreferencesDataPath.DoesFileExists(log))
                CifsPreferencesDataPath.CreateFile(Preferences.Default().ToBytes(), log);
            var maybePreferences = Preferences.Parse(CifsPreferencesDataPath.ReadAllBytes(), new Box<int>(0));
            var preferences = Preferences.Default();
            if (maybePreferences.IsResult)
                preferences = maybePreferences.ResultUnsafe;
            else
            {
                log("Preferences parsing error: " + maybePreferences.ErrorUnsafe);
                CifsIndexDataPath.CreateFile(preferences.ToBytes(), log);
            }
            return preferences;
        }

        public static void ShowCifsInExplorer()
        {
            var path = GetPreferences(_ => { }).DriverChar + ":\\";
            Action openExplorer = () => Process.Start(path);
            openExplorer.DoAsyncBackground("CifsExplorerProcessOpener");
        }

        public static void EditPreferences()
        {
            Action editPreferences = () => MessageBox.Show("Editing preferences :O");
            editPreferences.DoAsyncBackground("EditPreferences");
        }

        public static void CloseApp()
        {
            Application.Current.Shutdown();
        }
    }
}
