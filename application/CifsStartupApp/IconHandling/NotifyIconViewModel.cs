using System.Windows.Input;

namespace CifsStartupApp.IconHandling
{
    public class NotifyIconViewModel
    {
        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitCifsCommand =>
            new DelegateCommand(Initilization.CloseApp, () => true);
        public ICommand ShowCifsInExplorerCommand =>
            new DelegateCommand(Initilization.ShowCifsInExplorer, () => true);
        public ICommand EditPreferencesCommand =>
            new DelegateCommand(Initilization.EditPreferences, () => true);
    }
}
