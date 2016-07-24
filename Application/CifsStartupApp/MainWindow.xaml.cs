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
using CifsPreferences;
using FileSystem.Entries;
using Utils;
using Utils.FileSystemUtil;
using static Constants.Global;

namespace CifsStartupApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!CifsDirectoryPath.DoesFolderExists())
                CifsDirectoryPath.CreateDirectory();
            if (!CifsPreferencesDataPath.DoesFileExists())
                CifsPreferencesDataPath.CreateFile(Preferences.Default().ToBytes());
            if (!CifsIndexDataPath.DoesFileExists())
                CifsIndexDataPath.CreateFile(Index.Default().ToBytes());
            var maybePreferences = Preferences.Parse(CifsPreferencesDataPath.ReadAllBytes(), new Box<int>(0));
            var maybeIndex = Index.Parse(CifsIndexDataPath.ReadAllBytes(), new Box<int>(0));
           // var p
        }
    }
}
