using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddFollow.Agents.Messages.IPFS
{
    public class FollowOkMessage : IPFSMessage
    {
        public string MessageToUser()
        {
            return "Fllow link is OK";
        }
    }
}
