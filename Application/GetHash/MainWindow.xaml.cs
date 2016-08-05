using System;
using System.Windows;
using Communication;
using Communication.DokanMessaging.RootHash;
using Constants;
using Utils.FileSystemUtil;
using Utils.IEnumerableUtil;

namespace GetHash
{
    public partial class MainWindow : Window
    {
        private readonly CommunicationAgent communicator = new CommunicationAgent("77.138.132.84", Global.TcpPort, _ => { });
        public MainWindow()
        {
            InitializeComponent();
            this.AddressTextBox.Text = GetHashAddress(Environment.GetCommandLineArgs());
        }

        private string GetHashAddress(string[] commandLineArgs)
        {
            if (commandLineArgs.Length != 2)
                return "Internal error, needs 2 command line args";
            var innerPath = commandLineArgs[1].Split('\\').Tail().MkString("/");
            var response = communicator.GetResponse(new RootHashRequest(), RootHashResponse.Parse);
            if (response.IsError)
                return "Error: " + response.ErrorUnsafe;
            return response.ResultUnsafe.RootHash + "/" + innerPath;
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
