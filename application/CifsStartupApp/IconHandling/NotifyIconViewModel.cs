using System.Windows.Input;

namespace CifsStartupApp.IconHandling
{
    public class NotifyIconViewModel
    {
        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitCifsCommand =>
            new DelegateCommand(App.CloseApp, App.IsDokanRunning);
        public ICommand ShowCifsInExplorerCommand =>
            new DelegateCommand(App.ShowCifsInExplorer, App.IsDokanRunning);
        public ICommand EditPreferencesCommand =>
            new DelegateCommand(App.EditPreferences, App.IsDokanRunning);
    }
}
