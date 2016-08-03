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
using Communication.DokanMessaging.CloneOrFollow;
using Constants;

namespace AddFollow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
