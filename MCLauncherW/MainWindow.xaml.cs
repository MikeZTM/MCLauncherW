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
using System.Diagnostics;
using System.Threading;

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
        private String mcPath;
        private bool x64mode;
        private long memory;

        public MainWindow()
        {
            InitializeComponent();
            refreshSettings();
            playerNameTextField.Text = playerName;
            SetLanguageDictionary();
        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("en_us.xaml",
                                  UriKind.Relative);
                    break;
                case "zh-CN":
                    dict.Source = new Uri("zh_cn.xaml",
                                       UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("en_us.xaml",
                                      UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        } 

        private void start_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.playerName = playerNameTextField.Text;
            Properties.Settings.Default.Save();

            refreshSettings();
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = javaVM;
            String memString = "-Xms" + memory + "m";
            p.StartInfo.Arguments = memString;
            String path = mcPath.Substring(0, mcPath.Length - 13);
            p.StartInfo.EnvironmentVariables.Remove("APPDATA");
            p.StartInfo.EnvironmentVariables.Add("APPDATA", mcPath.Substring(0, mcPath.Length - 29));
            String cp = "";
            if (Properties.Settings.Default.HighEnabled)
            {
                cp = " -cp \"" + path + "minecraft_high.jar;" + path + "lwjgl.jar;" + path + "lwjgl_util.jar;" + path + "jinput.jar\"";
            }
            else
            {
                cp = " -cp \"" + path + "minecraft.jar;" + path + "lwjgl.jar;" + path + "lwjgl_util.jar;" + path + "jinput.jar\"";
            }
            p.StartInfo.Arguments += cp;
            String dcp = " -Djava.library.path=\"" + path + "natives\"";
            p.StartInfo.Arguments += dcp;
            p.StartInfo.Arguments += " net.minecraft.client.Minecraft ";

            try
            {
                if (Properties.Settings.Default.passwordEnabled)
                {
                    String loginSession = Login.login(playerNameTextField.Text, playerPswd);
                    if (loginSession != "")
                    {
                        p.StartInfo.Arguments += loginSession;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    p.StartInfo.Arguments += "\"" + playerNameTextField.Text + "\"";
                }
                p.Start();

                Application.Current.Shutdown();
            }
            catch (Exception)
            {

            }
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
            x64mode = Properties.Settings.Default.x64mode;
            memory = Properties.Settings.Default.memory;
            mcPath = Properties.Settings.Default.mcPath;

        }
    }
}
