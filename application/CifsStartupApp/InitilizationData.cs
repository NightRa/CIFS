﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Agents;
using Agents.AdministratorMessages;
using CifsPreferences;
using FileSystem.Entries;
using Utils.FileSystemUtil;
using Utils.FunctionUtil;
using Utils.GeneralUtils;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static Constants.Global;

namespace CifsStartupApp
{
    public sealed class InitilizationData
    {
        public Action<string> Log { get; }
        private Mail<AdministratorMessage> Inbox { get; }
        public InitilizationData(Mail<AdministratorMessage> mail, Action<string> log)
        {
            Log = log;
            Inbox = mail;
        }

        public void RunAgentLoopAsync(Index index, Preferences preferences)
        {
            Action loopAction = () => AdministratorAgent.Loop(index, preferences, Inbox, Log);
            loopAction.DoAsyncBackground("AdministratorAgentLoopThread", Log);
        }

        public void InitilizeIcon()
        {
            var iconName = nameof(CifsStartupApp) + "." + nameof(Resources) + "." +
                           nameof(Properties.Resources.uBoxIconEmbedded) + ".ico";
            if (!CifsIconPath.DoesFileExists(Log))
                using (var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconName))
                using (var fileStream = File.Create(CifsIconPath))
                    iconStream?.CopyTo(fileStream);
        }

        public void ApplyPreferences(Preferences preferences)
        {
            Log("Applying preferences " + preferences.ToString().Replace(NewLine, " "));
            ChangeOnStartupTo(preferences.OpenOnStartup);
            if (GetPreferences().DriverChar == preferences.DriverChar)
                return;
            App.UnmountDokan(TimeSpan.FromSeconds(15));
            this.RunAgentLoopAsync(GetIndex(), preferences);

            lock (CifsPreferencesDataPath)
                CifsPreferencesDataPath.CreateFile(preferences.ToBytes(), Log);
        }

        public Index GetIndex()
        {
            if (!CifsIndexDataPath.DoesFileExists(Log))
                CifsIndexDataPath.CreateFile(Index.Default().ToBytes(), Log);
            var maybeIndex = Index.Parse(CifsIndexDataPath.ReadAllBytes(), new Box<int>(0));
            var index = Index.Default();
            if (maybeIndex.IsResult)
                index = maybeIndex.ResultUnsafe;
            else
            {
                Log("Index parsing error: " + maybeIndex.ErrorUnsafe);
                CifsIndexDataPath.CreateFile(index.ToBytes(), Log);
            }
            return index;
        }

        public Preferences GetPreferences()
        {
            lock (CifsPreferencesDataPath)
            {
                CifsDirectoryPath.CreateDirectoryIfDoesntExist(FileAttributes.Hidden, Log);
                if (!CifsPreferencesDataPath.DoesFileExists(Log))
                    CifsPreferencesDataPath.CreateFile(Preferences.Default().ToBytes(), Log);
                var maybePreferences = Preferences.Parse(CifsPreferencesDataPath.ReadAllBytes(), new Box<int>(0));
                Preferences preferences = null;
                if (maybePreferences.IsResult)
                    preferences = maybePreferences.ResultUnsafe;
                else
                {
                    preferences = Preferences.Default();
                    Log("Preferences parsing error: " + maybePreferences.ErrorUnsafe);
                    CifsIndexDataPath.CreateFile(preferences.ToBytes(), Log);
                }
                return preferences;
            }
        }

        private static readonly InitilizationData dummy =
            new InitilizationData(new Mail<AdministratorMessage>(_ => { }), _ => { });
        public static Preferences GetPreferencesUnsafe()
        {
            //return Preferences.Default();
            return dummy.GetPreferences();
        }

        public void ApplyPreferencesInitially(Preferences preferences)
        {
            Log("Initilize: applying preferences " + preferences.ToString().Replace(NewLine, " "));
            ChangeOnStartupTo(preferences.OpenOnStartup);
            lock (CifsPreferencesDataPath)
                CifsPreferencesDataPath.CreateFile(preferences.ToBytes(), Log);
        }

        private void ChangeOnStartupTo(bool openOnStartup)
        {
            var startupFiles =
                Startup.GetPath()
                    .GetDirectoryInfo()
                    .EnumerateFiles()
                    .ToArray();
            var isAppOnStartup = startupFiles.Any(f => f.Name.Equals(StartUpShortcutName));
            if (!openOnStartup && isAppOnStartup)
                startupFiles.First(f => f.Name.Equals(StartUpShortcutName)).Delete();
            if (openOnStartup && !isAppOnStartup)
                Assembly.GetExecutingAssembly().Location
                    .CreateShortcut(Startup.GetPath().CombinePathWith(StartUpShortcutName), CifsIconPath, Log);
        }
    }
}
