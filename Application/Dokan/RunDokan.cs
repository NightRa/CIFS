using System;
using System.Threading;
using DokanNet;
using FileSystem.Entries;
using Agents;
using Agents.AdministratorMessages;
using Agents.DokanSupervisorMessages;
using Communication;
using Constants;
using Utils.FunctionUtil;

namespace Dokan
{
    public static class RunDokan
    {
        public static void MountDokan(Index index,
                                      char driverChar,
                                      DokanOptions dokanOptions,
                                      int numThreads,
                                      SendMessage<AdministratorMessage> sendMessage,
                                      Mail<DokanSupervisorMessage> inbox,
                                      Action<string> log)
        {
            Action superviseDokan = () => SuperviseDokanLoop(sendMessage, driverChar, inbox, log);
            Action mountDokan = () => Mount(inbox.Publish, driverChar, index, numThreads, dokanOptions, log);
            mountDokan.DoAsyncBackground("MountDokanThread", log);
            superviseDokan.DoAsyncBackground("SuperviseDokanLoop", log);

        }

        private static void SuperviseDokanLoop(SendMessage<AdministratorMessage> administrator, char driverChar, Mail<DokanSupervisorMessage> inbox, Action<string> log)
        {
            while (true)
            {
                var maybeMail = inbox.TryGetMessage();
                if (maybeMail.IsNone)
                    Thread.Sleep(Global.AgentSleepTime);
                else
                {
                    var mail = maybeMail.ValueUnsafe;
                    
                    var quitMessage = mail as QuitDokanRequestMessage;
                    var dokanThrewExceptionMessage = mail as DokanThrewExceptionMessage;
                    var dokanUnMountedSuccessfullyMessage = mail as UnmountedSuccessfullyMessage;
                    dokanThrewExceptionMessage?.ProccessMessage(administrator);
                    dokanUnMountedSuccessfullyMessage?.ProccessMessage(administrator);
                    quitMessage?.ProccessMessage(driverChar);
                    if (quitMessage != null)
                        break;
                }
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                Mount(_ => {}, 'g', Index.Default(), 5, DokanOptions.DebugMode, _ => { });
            }
            finally
            {
                DokanNet.Dokan.Unmount('g');
            }
        }
        private static void Mount(SendMessage<DokanSupervisorMessage> reportProgress, char driverChar, Index index,
            int numThreads, DokanOptions dokanOptions, Action<string> log)
        {
            try
            {
                var communicator = new CommunicationAgent("77.138.132.84", Global.TcpPort, log);
                //CifsIpfsDriverInstance instance = new CifsIpfsDriverInstance(communicator, log);
                CifsDriverInstance instance = new CifsDriverInstance(index, log);
                log("Trying to mount Dokan: " + driverChar + ":\\");
                instance.Mount(driverChar + ":\\", dokanOptions, numThreads);
                log("Dokan Unmounted");
                reportProgress(new UnmountedSuccessfullyMessage());
            }
            catch (Exception e)
            {
                log("Dokan threw an exception: " + e.Message);
                reportProgress(new DokanThrewExceptionMessage(e));
            }
        }
    }
}
