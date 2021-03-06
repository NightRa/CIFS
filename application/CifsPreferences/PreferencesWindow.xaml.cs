﻿using System;
using System.Linq;
using System.Net;
using System.Windows;
using Constants;
using Utils.ArrayUtil;
using Utils.IEnumerableUtil;
using Utils.StringUtil;
using static System.Environment;

namespace CifsPreferences
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        private Preferences currentPrefferences;
        public Preferences OriginalPreferences { get; }

        public Preferences CurrentPrefferences
        {
            get { return currentPrefferences; }
            set
            {
                currentPrefferences = value;
                bool isValidIp = IsValidIp(currentPrefferences.IndexIp);
                bool preferencasChanged = !currentPrefferences.Equals(OriginalPreferences);
                ApplyButton.IsEnabled = isValidIp && preferencasChanged;
            }
        }
        public Action<Preferences> OnPreferencesApplied { get; }
        public Action<string> Log { get; }

        public PreferencesWindow(Preferences originialPreferences, Action<Preferences> onPreferencesApplied, Action<string> log)
        {
            InitializeComponent();
            Log = log;
            this.OnPreferencesApplied = onPreferencesApplied;
            this.OriginalPreferences = originialPreferences;
            this.CurrentPrefferences = originialPreferences;
            DriverChars.Items.Clear();
            Global.AvailableDriverChars()
                .Where(ch => ch != currentPrefferences.DriverChar)
                .Select(ch => new DriverCharComboBoxItem(ch, OnDriverCharChanged))
                .Concat(new DriverCharComboBoxItem(currentPrefferences.DriverChar, OnDriverCharChanged) { IsSelected = true }.Singleton())
                .OrderBy(ch => ch.DriverChar)
                .Iter(ch => DriverChars.Items.Add(ch));
            this.MountOnStartupCheckBox.IsChecked = currentPrefferences.OpenOnStartup;
            this.IpTextBox.Text = currentPrefferences.IndexIp;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            OnPreferencesApplied(this.CurrentPrefferences);
            this.Close();
            MessageBox.Show("Preferences Applied:" + NewLine + CurrentPrefferences.ToString().AddTabs());
        }

        private void MountOnStartupCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CurrentPrefferences = CurrentPrefferences.WithOpenOnStartup(MountOnStartupCheckBox.IsChecked ?? false);
        }

        private void OnDriverCharChanged(char ch)
        {
            CurrentPrefferences = CurrentPrefferences.WithDriverChar(ch);
        }

        private void IpTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CurrentPrefferences = CurrentPrefferences.WithIp(IpTextBox.Text);
        }

        private bool IsValidIp(string str)
        {
            IPAddress ip;
            return IPAddress.TryParse(str, out ip);
        }
    }
}
