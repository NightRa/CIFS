using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Constants;
using Utils.ArrayUtil;
using Utils.IEnumerableUtil;
using static System.Environment;
using Utils.StringUtil;

namespace CifsPreferences
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Preferences currentPrefferences;
        public Preferences OriginalPreferences { get; set; }

        public Preferences CurrentPrefferences
        {
            get { return currentPrefferences; }
            set
            {
                currentPrefferences = value;
                ApplyButton.IsEnabled = !currentPrefferences.Equals(OriginalPreferences);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.OriginalPreferences = Preferences.Default();
            this.CurrentPrefferences = Preferences.Default();
            DriverChars.Items.Clear();
            Global.AvailableDriverChars()
                .Where(ch => ch != currentPrefferences.DriverChar)
                .Select(ch => new DriverCharComboBoxItem(ch, OnDriverCharChanged))
                .Concat(new DriverCharComboBoxItem(currentPrefferences.DriverChar, OnDriverCharChanged) { IsSelected = true }.Singleton())
                .OrderBy(ch => ch.DriverChar)
                .Iter(ch => DriverChars.Items.Add(ch));
            this.MountOnStartupCheckBox.IsChecked = currentPrefferences.OpenOnStartup;
        }



        protected override void OnSourceInitialized(EventArgs e)
        {
            Gui.IconRemover.RemoveIcon(this);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            var preferences = this.CurrentPrefferences;
            this.Close();
            MessageBox.Show("Preferences Applied:" + NewLine + preferences.ToString().AddTabs());
        }
        

        private void MountOnStartupCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CurrentPrefferences = CurrentPrefferences.WithOpenOnStartup(MountOnStartupCheckBox.IsChecked ?? true);
        }

        private void OnDriverCharChanged(char ch)
        {
            CurrentPrefferences = CurrentPrefferences.WithDriverChar(ch);
        }
    }
}
