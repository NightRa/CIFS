using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Agents;
using Agents.AdministratorMessages;
using CifsPreferences;
using CifsStartupApp;
using SharpShell.Interop;
using SharpShell.SharpIconOverlayHandler;

namespace OverlayIcon
{
    [ComVisible(true)]
    public class FollowOverlayIcon : SharpIconOverlayHandler
    {
        protected override int GetPriority() => 90;

        protected override bool CanShowOverlay(string path, FILE_ATTRIBUTE attributes)
        {
            if (path.Length == 0)
                return false;
            var driveChar = Char.ToUpper(path.First());
            var preferences = GetCifsPreferences();
            var cifsDriverChar = Char.ToUpper(preferences.DriverChar);
            return driveChar == cifsDriverChar;
        }

        private Preferences GetCifsPreferences()
        {
            Action<string> log = _ => { };
            var init = new InitilizationData(new Mail<AdministratorMessage>(log), log);
            return init.GetPreferences();
        }

        protected override Icon GetOverlayIcon() => Properties.Resources.FollowIcon;
    }
}
