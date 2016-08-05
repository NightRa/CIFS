using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CifsStartupApp;
using Constants;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using Utils.ConcurrencyUtils;

namespace GetAddressShellExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class GetHashAddressHandler : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return Global.DokanRunningObject.IsHeld();
            /*var items = SelectedItemPaths.ToArray();
            if (items.Length != 1)
                return false;
            var selectedPath = items.First();
            if (selectedPath.Length == 0)
                return false;
            var preferences = InitilizationData.GetPreferencesUnsafe();
            return selectedPath.First() == preferences.DriverChar;*/
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Click += (sender, args) => MessageBox.Show("Got hash");
            menu.Items.Add("Get Hash");
            return menu;
        }
    }
}
