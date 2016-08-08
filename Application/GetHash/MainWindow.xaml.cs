using System;
using System.Windows;
using CifsStartupApp;
using Communication;
using Communication.DokanMessaging.RootHash;
using Constants;
using Utils.FileSystemUtil;
using Utils.IEnumerableUtil;
using Utils.Parsing;

namespace GetHash
{
    public partial class MainWindow : Window
    {
        private CommunicationAgent Communicator { get; }
        public Action<string> Log { get; }
        public MainWindow()
        {
            InitializeComponent();
            Log = _ => { };
            var preferences = InitilizationData.GetPreferencesUnsafe();
            this.Communicator = new CommunicationAgent(preferences.IndexIp, Global.TcpPort, Log);
            this.AddressTextBox.Text = GetHashAddress(Environment.GetCommandLineArgs());
        }

        private string GetHashAddress(string[] commandLineArgs)
        {
            if (commandLineArgs.Length != 2)
                return "Internal error, needs 2 command line args";
            var innerPath = commandLineArgs[1].Split('\\').Tail().MkString("/");
           var response = Communicator.GetResponse(new RootHashRequest(), RootHashResponse.Parse);
            if (response.IsError)
                return "Error: " + response.ErrorUnsafe;
          //  var response = Parse.Return(new RootHashResponse("Qus37j9dj9ddm2lx92ld02kd2c"));
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
