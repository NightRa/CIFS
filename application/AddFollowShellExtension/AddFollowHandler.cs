using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AddFollowShellExtension.Properties;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace AddFollowShellExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Drive)]
    public class AddFollowHandler : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            var items = SelectedItemPaths.ToArray();
            if (items.Length != 1)
                return false;
            var selectedPath = items.First();
            if (selectedPath.Length == 0)
                return false;
            //var preferences = InitilizationData.GetPreferencesUnsafe();
            return selectedPath.First() == 'C'; //preferences.DriverChar;  
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            const string message = "Add follow";
            var image = Resources.uBoxIcon;
            var thumbnail = image.GetThumbnailImage(16, 16, () => true, IntPtr.Zero);
            EventHandler onClick = (sender, args) => MessageBox.Show("Added follow");
            menu.Items.Add(message);
            menu.Click += onClick;
            return menu;
        }
    }
}
