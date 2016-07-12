using System;
using System.Windows;
using System.Windows.Input;
using AddFollow.Agents;
using AddFollow.Agents.Messages.IPFS;
using AddFollow.Agents.Messages.Ui;
using static System.TimeSpan;

namespace AddFollow
{
    //TODO: Message proccessing, Speak with IPFS
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double ScreenWidth => SystemParameters.PrimaryScreenWidth;
        public double ScreenHeight => SystemParameters.PrimaryScreenHeight;
        public double SizeFactor = 5.1;
        public MainWindow()
        {
            InitializeComponent();
            this.Width = 300;
            this.Height = 132;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FollowButton_OnClick(object sender, RoutedEventArgs e)
        {
            var link = this.FollowTextBox.Text;
            var before = this.Cursor;
            this.Cursor = Cursors.Wait;
            var ipfsMessage = FollowLink(link);
            this.Cursor = before;
            ProcessMessage(ipfsMessage);
        }

        private void ProcessMessage(IPFSMessage ipfsMessage)
        {
            MessageBox.Show(ipfsMessage.MessageToUser());
        }
        
        private IPFSMessage FollowLink(string link)
        {
            var duplex = MessageQueue<UiMessage, IPFSMessage>.BiDirectional();
            var mail = duplex.Item2;
            var getLinkAgent = new GetLinkAgent(duplex.Item1);
            getLinkAgent.RunAsync();
            mail.SendMessage(new GetLinkMessage(link));
            var result = mail.GetMessageBlocking(timeout: FromSeconds(10), sleepTime: FromMilliseconds(10), @default: new NoResultMessage());
            mail.SendMessage(new QuitMessage());
            return result;
        }
    }
}
