﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Agents;
using Agents.AdministratorMessages;
using CifsPreferences;
using Constants;
using Hardcodet.Wpf.TaskbarNotification;
using Utils;
using Utils.DoubleUtil;
using Utils.FileSystemUtil;
using Utils.FunctionUtil;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static Constants.Global;

namespace CifsStartupApp
{
    public partial class App : Application
    {
        private static TaskbarIcon notifyIcon = null;
        private static Mail<AdministratorMessage> administratorMail = null;
        private static Action<string> log = null;
        private static InitilizationData init = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Thread.CurrentThread.Name = "MainAppThread";
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            var logFileLocation = Desktop.GetPath().CombinePathWith("log.txt");
            CifsDirectoryPath.CreateDirectoryIfDoesntExist(FileAttributes.Hidden, _ => { });
            log = Log.InitilizeInteractive(logFileLocation);
            log("Application started");
            administratorMail = new Mail<AdministratorMessage>(log);

            init = new InitilizationData(administratorMail, log);
            init.InitilizeIcon();
            var index = init.GetIndex();
            var preferences = init.GetPreferences();
            init.ApplyPreferencesInitially(preferences);
            init.RunAgentLoopAsync(index, preferences);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            log?.Invoke("Shutdown request for App");
            notifyIcon?.Dispose();
            UnmountDokan(TimeSpan.FromSeconds(15));
            log?.Invoke("Exiting App");
            base.OnExit(e);
        }

        public static void UnmountDokan(TimeSpan timeout)
        {
            administratorMail?.Publish(new QuitAppMessage());
            if (DokanRunningObject.IsHeld(timeout.TotalMilliseconds.ToInt()))
                log?.Invoke("Dokan didnt unmount.... :(");
        }

        public static void CloseApp()
        {
            Delegate shutDown = new Action(() => Application.Current.Shutdown());
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,shutDown);
        }

        public static void ShowCifsInExplorer()
        {
            var path = init.GetPreferences().DriverChar + ":\\";
            Action openExplorer = () => Process.Start(path);
            openExplorer.DoAsyncBackground("CifsExplorerProcessOpener", log);
        }

        public static void EditPreferences()
        {
            Action editPreferences = () =>
            {
                var preferences = init.GetPreferences();
                var window = new PreferencesWindow(preferences, init.ApplyPreferences, log);
                window.Show();
            };
            editPreferences.CatchErrors(log, "Edit preferences");
        }

        public static bool IsDokanRunning()
        {
            return Global.DokanRunningObject.IsHeld();
        }
    }
}
