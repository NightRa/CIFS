using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AddFollow.Agents.Messages;
using AddFollow.Util.Option;
using static AddFollow.Util.Option.Opt;

namespace AddFollow.Agents
{
    public sealed class MessageQueue<A,B>
        where A: Message
        where B: Message
    {
        private readonly ConcurrentQueue<A> incoming;
        private readonly ConcurrentQueue<B> outgoing;

        public MessageQueue()
        {
            incoming = new ConcurrentQueue<A>();
            outgoing = new ConcurrentQueue<B>();
        }

        public MessageQueue(ConcurrentQueue<A> incoming, ConcurrentQueue<B> outgoing)
        {
            this.incoming = incoming;
            this.outgoing = outgoing;
        }

        public Option<A> GetMessage()
        {
            A result;
            if (incoming.TryDequeue(out result))
                return Some(result);
            return None<A>();
        }
        public void SendMessage(B message)
        {
            outgoing.Enqueue(message);
        }

        public static Tuple<MessageQueue<A, B>, MessageQueue<B, A>> BiDirectional()
        {
            ConcurrentQueue<A> a = new ConcurrentQueue<A>();
            ConcurrentQueue<B> b = new ConcurrentQueue<B>();
            var aToB = new MessageQueue<A, B>(a,b);
            var bToA = new MessageQueue<B, A>(b, a);
            return Tuple.Create(aToB, bToA);
        }

        public A GetMessageBlocking(TimeSpan timeout, TimeSpan sleepTime, A @default)
        {
            DateTime end = DateTime.Now + timeout;

            while (DateTime.Now < end)
            {
                var message = GetMessage();
                if (message.IsSome)
                    return message.ValueUnsafe;

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
                Thread.Sleep(sleepTime);
            }
            return @default;
        }
    }
}
