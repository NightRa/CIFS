using System;
using System.Windows;
using Communication;
using Communication.DokanMessaging.RootHash;
using Utils.FileSystemUtil;

namespace GetHash
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.AddressTextBox.Text = GetHashAddress(Environment.GetCommandLineArgs());
        }

        private string GetHashAddress(string[] commandLineArgs)
        {
            if (commandLineArgs.Length != 2)
                return "Internal error, needs 2 command line args";
            var innerPath = commandLineArgs[1];
            var communicator = new CommunicationAgent("77.138.132.84", 8008, _ => { });
            var response = communicator.GetResponse(new RootHashRequest(), RootHashResponse.Parse);
            if (response.IsError)
                return "Error: " + response.ErrorUnsafe;
            
            return response.ResultUnsafe.RootHash.CombinePathWith(innerPath).Replace('\\','/');
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            Gui.IconRemover.RemoveIcon(this);
        }

        private void CopyToClipBoardButton_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.AddressTextBox.Text, TextDataFormat.Text);
        }
    }
}
