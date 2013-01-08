using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MCLauncherW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String javaVM;
        private String playerName;
        private String playerPswd;
        private bool x64mode;
        private long memory;
        private bool pswdEnabled;

        public MainWindow()
        {
            InitializeComponent();
            refreshSettings();
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {

        }

        private void preference_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Preference prefer = new Preference();
            prefer.setParent(this);
            prefer.Show();
        }

        public void refreshSettings()
        {
            javaVM = Properties.Settings.Default.javaVM;
            playerName = Properties.Settings.Default.playerName;
            playerPswd = Properties.Settings.Default.playerPswd;
            x64mode=Properties.Settings.Default.x64mode;
            memory=Properties.Settings.Default.memory;
            pswdEnabled=Properties.Settings.Default.pswdEnabled;
        }
    }
}
