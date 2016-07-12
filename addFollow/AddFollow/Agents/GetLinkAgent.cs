using System.Threading;
using System.Windows;
using AddFollow.Agents.Messages.IPFS;
using AddFollow.Agents.Messages.Ui;
using AddFollow.Util.Option;
using static AddFollow.Util.Option.Opt;

namespace AddFollow.Agents
{
    public sealed class GetLinkAgent
    {
        private readonly MessageQueue<UiMessage, IPFSMessage> messageQueue;

        public GetLinkAgent(MessageQueue<UiMessage, IPFSMessage> messageQueue)
        {
            this.messageQueue = messageQueue;
        }

        public void RunAsync()
        {
            var thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            while (true)
            {
                var maybeUiMessage = messageQueue.GetMessage();
                if (maybeUiMessage.IsSome)
                {
                    var message = maybeUiMessage.ValueUnsafe;
                    if (message is QuitMessage)
                        return;
                    if (message is GetLinkMessage)
                        SendLinkToIPFS((GetLinkMessage)message);
                    else
                        MessageBox.Show("Unkown message type, Programming Exception!");
                }

                var maybeCifsMessage = MaybeGetIPFSMessage();
                if (maybeCifsMessage.IsSome)
                    messageQueue.SendMessage(maybeCifsMessage.ValueUnsafe);
                Thread.Sleep(10);
            }
        }

        private Option<IPFSMessage> MaybeGetIPFSMessage()
        {
            //return Opt.Some<IPFSMessage>();
            return None<IPFSMessage>();
        }

        private void SendLinkToIPFS(GetLinkMessage getLinkMessage)
        {
            return;
            //throw new NotImplementedException();
        }
    }
}
