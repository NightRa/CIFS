using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace CifsStartupApp
{
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            var index = Initilization.InitilizeCifs(s => MessageBox.Show(s));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon?.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
