using System;
using System.Windows;
using Communication;
using Communication.DokanMessaging.CloneOrFollow;
using Constants;

namespace AddFollow
{
    public partial class MainWindow : Window
    {
        CommunicationAgent Communicator = new CommunicationAgent(Global.LocalHost, Global.TcpPort, _ => {});
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
            var localPath = ""; //Environment.GetCommandLineArgs()[1]
            var hashRemotePath = FollowTextBox.Text;
            var isFollow = FollowRadioButton.IsChecked ?? true;
            var request = new CloneOrFollowRequest(isFollow, hashRemotePath, localPath);
            var maybeResponse = Communicator.GetResponse(request, CloneOrFollowResponse.Parse);
            //if (maybeResponse.IsError)

        }
    }
}
