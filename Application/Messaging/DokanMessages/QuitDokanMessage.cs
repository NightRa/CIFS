using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agents.DokanMessages
{
    public sealed class QuitDokanMessage : DokanMessage
    {
        public override string AsString()
        {
            return "Quit Dokan";
        }
    }
}
