using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddFollow.Agents.Messages.IPFS
{
    public interface IPFSMessage : Message
    {
        string MessageToUser();
    }
}
