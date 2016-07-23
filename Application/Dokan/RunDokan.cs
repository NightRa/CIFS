using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DokanNet;
using FileSystem.Entries;
using Agents;
using Agents.AdministratorMessages;
using Agents.DokanMessages;
using Constants;

namespace Dokan
{
    public static class RunDokan
    {
       // char driverChar = 'd';
        //Index index = new Index(null);
       // DokanOptions dokanOptions = DokanOptions.DebugMode;
       // int numThreads = 5;
        public static void MountDokan(Index index, char driverChar, DokanOptions dokanOptions, int numThreads, SendMessage<AdministratorMessage> sendMessage)
        {
            Mail<DokanMessage> dokanMail = new Mail<DokanMessage>();

            Action mountDokan = () =>
            {
                try
                {
                    CifsDriverInstance instance = new CifsDriverInstance(index);
                    instance.Mount(driverChar + ":\\", dokanOptions, numThreads);
                    dokanMail.Publish(new UnmountedSuccessfullyMessage());
                }
                catch (Exception e)
                {
                    dokanMail.Publish(new DokanThrewExceptionMessage(e));
                }
            };

            Action superviseDokan = () => SuperviseDokanLoop(sendMessage, dokanMail, driverChar);

            mountDokan.DoAsyncBackground("DokanThread");
            superviseDokan.DoAsyncBackground("SuperviseDokanLoop");

        }

        public static void SuperviseDokanLoop(SendMessage<AdministratorMessage> sendMessage, Mail<DokanMessage> inbox, char driverChar)
        {
            while (true)
            {
                var maybeMail = inbox.TryGetMessage();
                if (maybeMail.IsNone)
                    Thread.Sleep(Global.AgentSleepTime);
                else
                {
                    var mail = maybeMail.ValueUnsafe;
                    var quitMessage = mail as QuitDokanMessage;
                    var dokanThrewExceptionMessage = mail as DokanThrewExceptionMessage;
                    var dokanUnMountedSuccessfullyMessage = mail as UnmountedSuccessfullyMessage;
                    bool shouldQuit = false;
                    quitMessage?.ProccessMessage(driverChar);
                    dokanThrewExceptionMessage?.ProccessMessage(driverChar, sendMessage, out shouldQuit);
                    dokanUnMountedSuccessfullyMessage?.ProccessMessage(sendMessage, out shouldQuit);
                    if (shouldQuit)
                        return;
                }
            }
        }
    }
}
