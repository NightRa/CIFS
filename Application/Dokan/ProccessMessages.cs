using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Agents;
using Agents.AdministratorMessages;
using Agents.DokanMessages;

namespace Dokan
{
    internal static class ProccessMessages
    {
        public static void ProccessMessage(this QuitDokanMessage @this, char driverChar)
        {
            DokanNet.Dokan.Unmount(driverChar);
        }
        public static void ProccessMessage(this DokanThrewExceptionMessage @this, char driverChar, SendMessage<AdministratorMessage> sendMeesage, out bool shouldQuit)
        {
            DokanNet.Dokan.Unmount(driverChar);
            sendMeesage(new InternalErrorMessage(@this.Exception));
            shouldQuit = true;
        }
        public static void ProccessMessage(this UnmountedSuccessfullyMessage @this, SendMessage<AdministratorMessage> sendMeesage, out bool shouldQuit)
        {
            sendMeesage(new DokanExitedSuccessfullyMessage());
            shouldQuit = true;
        }
    }
}
