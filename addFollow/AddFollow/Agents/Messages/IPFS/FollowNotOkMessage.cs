using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddFollow.Agents.Messages.IPFS
{
    public class FollowNotOkMessage : IPFSMessage
    {
        public readonly string message;

        public FollowNotOkMessage(string message)
        {
            this.message = message;
        }

        public string MessageToUser()
        {
            return "Follow link isn't OK: " + message;
        }
    }
}
