using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Agents;
using Agents.AdministratorMessages;
using Constants;
using Hardcodet.Wpf.TaskbarNotification;
using Utils;
using Utils.DoubleUtil;
using Utils.FileSystemUtil;
using static System.Environment.SpecialFolder;
using static Constants.Global;

namespace CifsStartupApp
{
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon = null;
        private Mail<AdministratorMessage> administratorMail = null;
        private Action<string> log; 
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
            Initilization.InitilizeCifs(administratorMail, log);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            log?.Invoke("Shutdown request for App");
            administratorMail?.Publish(new QuitAppMessage());
            notifyIcon?.Dispose();
            var maxWaitTime = TimeSpan.FromSeconds(5);
            if (!Global.DokanSemaphore.WaitOne(maxWaitTime))
                log?.Invoke("Dokan didnt unmount.... :(");
            log?.Invoke("Exiting App");
            base.OnExit(e);
        }
    }
}
