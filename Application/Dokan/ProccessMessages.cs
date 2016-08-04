using Agents;
using Agents.AdministratorMessages;
using Agents.DokanSupervisorMessages;

namespace Dokan
{
    internal static class ProccessMessages
    {
        public static void ProccessMessage(this QuitDokanRequestMessage @this, char driverChar)
        {
            DokanNet.Dokan.Unmount(driverChar);
        }
        public static void ProccessMessage(this DokanThrewExceptionMessage @this, SendMessage<AdministratorMessage> sendMeesage)
        {
            sendMeesage(new InternalErrorMessage(@this.Exception));
        }
        public static void ProccessMessage(this UnmountedSuccessfullyMessage @this, SendMessage<AdministratorMessage> sendMeesage)
        {
            sendMeesage(new DokanExitedSuccessfullyMessage());
        }
    }
}
