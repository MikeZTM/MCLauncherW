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
using System.Windows.Shapes;

namespace MCLauncherW
{
    /// <summary>
    /// Interaction logic for Preference.xaml
    /// </summary>
    public partial class Preference : Window
    {
        private MainWindow win;
        private String javaVM;
        private String playerName;
        private String playerPswd;
        private bool x64mode;
        private long memory;
        private bool pswdEnabled;

        public Preference()
        {
            InitializeComponent();
            refreshSettings();
            javaw.Text = javaVM;
            mc_path.Text = Properties.Settings.Default.mcPath;
            if (Environment.Is64BitOperatingSystem) //check x64 os
            {
                x64Check.IsEnabled = true;
                if (Properties.Settings.Default.x64mode)
                {
                    x64Check.IsChecked = true;
                }
            }
            memSlider.Value = Properties.Settings.Default.memory;
        }

        private void closed(object sender, EventArgs e)
        {
            win.Show();
        }

        public void setParent(MainWindow win)
        {
            this.win = win;
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "javaw.exe"; // Default file name 
            dlg.DefaultExt = ".exe"; // Default file extension 
            dlg.Filter = "Executable Binary (.exe)|*.exe"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog(); //show dialog

            if (result == true)
            {
                // Open document 
                javaw.Text = dlg.FileName;
            }
        }

        private void javaw_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.javaVM = javaw.Text;
            Properties.Settings.Default.Save();
        }

        public void refreshSettings()
        {
            javaVM = Properties.Settings.Default.javaVM;
            playerName = Properties.Settings.Default.playerName;
            playerPswd = Properties.Settings.Default.playerPswd;
            x64mode = Properties.Settings.Default.x64mode;
            memory = Properties.Settings.Default.memory;
            pswdEnabled = Properties.Settings.Default.pswdEnabled;
        }

        private void browseMC_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "minecraft.jar"; // Default file name 
            dlg.DefaultExt = ".jar"; // Default file extension 
            dlg.Filter = "Java runnable (.jar)|*.jar"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog(); //show dialog

            if (result == true)
            {
                // Open document 
                mc_path.Text = dlg.FileName;
            }
        }

        private void x64Check_Checked(object sender, RoutedEventArgs e)
        {
            memSlider.Maximum = 4096;
            Properties.Settings.Default.x64mode = true;
            Properties.Settings.Default.Save();
        }

        private void x64Check_Unchecked(object sender, RoutedEventArgs e)
        {
            memSlider.Maximum = 1536;
            Properties.Settings.Default.x64mode = false;
            Properties.Settings.Default.Save();
        }

        private void memSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (memAmountTextField != null)
            {
                memAmountTextField.Content = memSlider.Value;
                Properties.Settings.Default.memory = (long)memSlider.Value;
                Properties.Settings.Default.Save();
            }
        }

        private void mcpath_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.mcPath = mc_path.Text;
            Properties.Settings.Default.Save();
        }
    }
}