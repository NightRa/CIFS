using System;
using System.Collections.Concurrent;
using Utils.OptionUtil;
using static Utils.OptionUtil.Opt;

namespace Agents
{
    public delegate void SendMessage<in TMessage>(TMessage message)
        where TMessage : Message;
    public delegate void MessageEventHandler(Message message);
    public sealed class Mail<TMessage>
        where TMessage : Message
    {
        private ConcurrentQueue<TMessage> Inbox { get; }

        public event MessageEventHandler OnMessageRecieved;
        public event MessageEventHandler OnMessageSent;

        public Mail(Action<string> log)
        {
            this.Inbox = new ConcurrentQueue<TMessage>();
            log("Creating Mail for messages of type " + typeof (TMessage).FullName);
            Func<Message, string> showMessage = message => message.MessageTime.TimeOfDay.ToString() + " - " + message.AsString();
            this.OnMessageRecieved += message => log("Recieved " + showMessage(message));
            this.OnMessageSent += message => log("Sent " + showMessage(message));
        }

        public Option<TMessage> TryGetMessage()
        {
            TMessage message;
            if (Inbox.TryDequeue(out message)) {
                OnMessageRecieved?.Invoke(message);
                return Some(message);
            }
            return None<TMessage>();
        }

        public SendMessage<TMessage> Publish => message =>
        {
            OnMessageSent?.Invoke(message);
            this.Inbox.Enqueue(message);
        };
    }
}
 