using System;
using System.IO;
using System.Windows;
using Communication;
using Communication.DokanMessaging.CloneOrFollow;
using Constants;
using Utils.IEnumerableUtil;
using Utils.Parsing;

namespace AddFollow
{
    public partial class MainWindow : Window
    {
        CommunicationAgent Communicator = new CommunicationAgent(Global.LocalHost, Global.TcpPort, _ => { });

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            Gui.IconRemover.RemoveIcon(this);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FollowButton_OnClick(object sender, RoutedEventArgs e)
        {
            var localPath = GetLocalPath();
            if (!Directory.Exists(localPath))
                return;
            var hashRemotePath = FollowTextBox.Text;
            var isFollow = FollowRadioButton.IsChecked ?? true;
            var request = new CloneOrFollowRequest(isFollow, hashRemotePath, localPath);
            var maybeResponse = Communicator.GetResponse(request, CloneOrFollowResponse.Parse);
            MessageBox.Show(GetMessage(maybeResponse, isFollow));
            this.Close();
        }

        private string GetLocalPath()
        {
            try
            {
                var path = Environment.GetCommandLineArgs()[1];
                if (Directory.Exists(path))
                    return path;
                MessageBox.Show("Path doesn't exist...");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Invalid exe opening...");
                this.Close();
            }
            return "";

        }

        private static string GetMessage(ParsingResult<CloneOrFollowResponse> res, bool isFollow)
        {
            if (res.IsError)
                return "Parsing error... " + res.ErrorUnsafe;
            if (res.ResultUnsafe.IsNameCollision)
                return "Name collision..";
            if (res.ResultUnsafe.IsReadOnlyFolder)
                return "Folder is read-only..";
            if (res.ResultUnsafe.MalformedPath)
                return "Malformed path..";
            if (res.ResultUnsafe.PathToParentDoesntExist)
                return "Path to parent doesnt exist..";
            if (res.ResultUnsafe.RemotePathBroken)
                return "Remote path broken..";
            if (res.ResultUnsafe.RootNotFound)
                return "Root not found..";
            return isFollow ? "Followed successfully" : "Cloned successfully";
        }
    }
}
