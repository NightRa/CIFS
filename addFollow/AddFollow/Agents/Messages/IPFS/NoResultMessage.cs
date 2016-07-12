using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddFollow.Agents.Messages.IPFS
{
    public class NoResultMessage : IPFSMessage
    {
        public string MessageToUser()
        {
            return "No result found in time.. :(";
        }
    }
}
