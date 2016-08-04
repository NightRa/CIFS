using System;
using System.Windows.Controls;

namespace CifsPreferences
{
    internal sealed class DriverCharComboBoxItem : ComboBoxItem
    {
        public char DriverChar { get; }

        public DriverCharComboBoxItem(char driverChar, Action<char> onSelectedChanged)
        {
            DriverChar = driverChar;
            this.Content = driverChar;
            this.Unselected += (sender, args) => onSelectedChanged(driverChar);
            this.Selected += (sender, args) => onSelectedChanged(driverChar);
        }
    }
}
