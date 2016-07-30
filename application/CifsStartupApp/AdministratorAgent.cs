using System;
using System.Threading;
using System.Windows;
using Agents;
using Agents.AdministratorMessages;
using Agents.DokanSupervisorMessages;
using CifsPreferences;
using Constants;
using Dokan;
using DokanNet;
using FileSystem.Entries;
using static System.Environment;

namespace CifsStartupApp
{
    public static class AdministratorAgent
    {
        public static void Loop(Index index, Preferences preferences, Mail<AdministratorMessage> @inbox, Action<string> log)
        {
            var dokanMail = new Mail<DokanSupervisorMessage>(log);
            RunDokan.MountDokan(index, preferences.DriverChar, DokanOptions.DebugMode, 5, inbox.Publish, dokanMail, log);

            while (true)
            {
                var maybeMessage = inbox.TryGetMessage();
                if (maybeMessage.IsNone)
                    Thread.Sleep(Global.AgentSleepTime);
                else
                {
                    var message = maybeMessage.ValueUnsafe;
                    var internalErrorMessage = message as InternalErrorMessage;
                    var dokanExitedMessage = message as DokanExitedSuccessfullyMessage;
                    var quitMessage = message as QuitAppMessage;
                    internalErrorMessage?.ProcessMessage(log);
                    dokanExitedMessage?.ProcessMessage(log);
                    quitMessage?.ProcessMessage(dokanMail.Publish ,log);
                    if (quitMessage != null)
                        break;
                }
            }
        }

        private static void ProcessMessage(this QuitAppMessage @this, SendMessage<DokanSupervisorMessage> dokan, Action<string> log)
        {
            log("Quitting App requested");
            dokan(new QuitDokanRequestMessage());
        }

        private static void ProcessMessage(this InternalErrorMessage @this, Action<string> log)
        {
            var message = "An internal error occurred. closing CIFS :(";
            message += NewLine;
            message += @this.Exception.Message;
            log(message);
            MessageBox.Show(message);
            Application.Current.Dispatcher.InvokeShutdown();
        }
        private static void ProcessMessage(this DokanExitedSuccessfullyMessage @this, Action<string> log)
        {
            return;
        }
    }
}
