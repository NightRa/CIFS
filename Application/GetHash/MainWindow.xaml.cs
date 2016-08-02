using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Communication;
using Communication.DokanMessaging.RootHash;
using Constants;
using Utils.ArrayUtil;
using Utils.FileSystemUtil;

namespace GetHash
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            var communicator = new CommunicationAgent("127.0.0.1", Global.TcpPort, _ => {});
            var response = communicator.GetResponse(new RootHashRequest(), RootHashResponse.Parse);
            if (response.IsError)
                return "Error: " + response.ErrorUnsafe;
            
            return response.ResultUnsafe.RootHash.ToHexa().CombinePathWith(innerPath);
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
