using System;

namespace Agents
{
    public abstract class Message
    {
        public DateTime MessageTime { get; }
        protected Message()
        {
            MessageTime = DateTime.Now;
        }
        public abstract string AsString();
    }
}
