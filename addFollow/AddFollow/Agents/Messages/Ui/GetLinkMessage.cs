using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddFollow.Agents.Messages.Ui
{
    public class GetLinkMessage : UiMessage
    {
        public readonly string linkMessage;

        public GetLinkMessage(string linkMessage)
        {
            this.linkMessage = linkMessage;
        }
    }
}
