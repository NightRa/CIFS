using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agents.DokanSupervisorMessages
{
    public sealed class QuitDokanRequestMessage : DokanSupervisorMessage   
    {
        public override string AsString()
        {
            return "Quit Dokan is requested";
        }
    }
}
