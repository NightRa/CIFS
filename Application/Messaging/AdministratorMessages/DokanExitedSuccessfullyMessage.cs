using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agents.AdministratorMessages
{
    public sealed class DokanExitedSuccessfullyMessage : AdministratorMessage
    {
        public override string AsString()
        {
            return "Dokan exited successfully";
        }
    }
}
