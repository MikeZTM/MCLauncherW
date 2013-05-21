using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Xml;

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
        private string currentMinecraftVersion;
        public static string newMinecraftVersion;
        private bool x64mode;
        private long memory;
        bool isSelfUpdate = false;
        bool isMinecraftUpdate = false;
        string currentSelfVersion = "1.0.1.2";
        string selfDownload = string.Empty;
        string minecraftDownload = string.Empty;
        string updateCheckURL = "http://mclauncherw.sinaapp.com/MCLauncherW.xml";
        

        public MainWindow()
        {
            InitializeComponent();
            refreshSettings();
            SetLanguageDictionary();
            playerNameTextField.Text = playerName;
            if (playerName == "")
            {
                javaVM = javaAutoDetect();
                if (javaVM == string.Empty)
                {
                    MessageBox.Show(this.Resources["notFundMessageLine1"].ToString() + "\r\n" + this.Resources["notFundMessageLine2"].ToString(), this.Resources["errorMessageTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    javaVM = Properties.Settings.Default.javaVM;
                    this.Hide();
                    Preference prefer = new Preference();
                    prefer.setParent(this);
                    prefer.Show();
                }
                else
                {
                    Properties.Settings.Default.javaVM = javaVM;
                    Properties.Settings.Default.Save();
                }
            }
            selfUpdate();
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

        private string javaAutoDetect()
        {
            RegistryKey pregkey;
            string findJavaPath = string.Empty;

            pregkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment", false);
            if (pregkey == null)
            {
                return findJavaPath;
            }
            else
            {
                string javaVersion = pregkey.GetValue("CurrentVersion", "0").ToString();
                pregkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\" + javaVersion, false);
                if (pregkey == null)
                {
                    return findJavaPath;
                }
                else
                {
                    string javaPath = pregkey.GetValue("JavaHome", "0").ToString();
                    string javaw = javaPath + "\\" + "bin\\javaw.exe";
                    if (javaw.Contains(" (x86)"))
                    {
                        string java64w = javaw.Replace(" (x86)", "");
                        if (File.Exists(java64w))
                        {
                            findJavaPath = java64w;
                        }
                        else
                            if (File.Exists(javaw))
                            {
                                findJavaPath = javaw;
                            }
                            else
                            {
                                findJavaPath = string.Empty;
                            }
                    }
                    else
                    {
                        if (File.Exists(javaw))
                        {
                            findJavaPath = javaw;
                        }
                        else
                        {
                            findJavaPath = string.Empty;
                        }
                    }
                }
            }
            pregkey.Close();
            return findJavaPath;
        }

        public void refreshSettings()
        {
            javaVM = Properties.Settings.Default.javaVM;
            playerName = Properties.Settings.Default.playerName;
            playerPswd = Properties.Settings.Default.playerPswd;
            currentMinecraftVersion = Properties.Settings.Default.minecraftVersion;
            x64mode = Properties.Settings.Default.x64mode;
            memory = Properties.Settings.Default.memory;
            mcPath = Properties.Settings.Default.mcPath;

        }

        public void checkUpdate()
        {
            string newSelfVersion = string.Empty;
            try
            {
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(updateCheckURL);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);
                XmlNode list = xmlDoc.SelectSingleNode("Update");
                foreach (XmlNode node in list)
                {
                    if (node.Name == "Soft" && node.Attributes["Name"].Value.ToLower() == "MCLauncherW".ToLower())
                    {
                        foreach (XmlNode xml in node)
                        {
                            if (xml.Name == "Version")
                                newSelfVersion = xml.InnerText;
                            else
                                selfDownload = xml.InnerText;
                        }
                    }
                    if (node.Name == "Soft" && node.Attributes["Name"].Value.ToLower() == "Minecraft".ToLower())
                    {
                        foreach (XmlNode xml in node)
                        {
                            if (xml.Name == "Version")
                                newMinecraftVersion = xml.InnerText;
                            else
                                minecraftDownload = xml.InnerText;
                        }
                    }
                }

                Version ver = new Version(newSelfVersion);
                Version verson = new Version(currentSelfVersion);
                int tm = verson.CompareTo(ver);

                if (tm >= 0)
                    isSelfUpdate = false;
                else
                    isSelfUpdate = true;

                ver = new Version(newMinecraftVersion);
                verson = new Version(currentMinecraftVersion);
                tm = verson.CompareTo(ver);

                if (tm >= 0)
                    isMinecraftUpdate = false;
                else
                    isMinecraftUpdate = true;
            }
            catch (Exception ex)
            {
                throw new Exception("networkerror");
            }
        }

        private void selfUpdate()
        {
            try
            {
                checkUpdate();
            }
            catch (Exception ex)
            {
                if (ex.Message == "networkerror")
                {
                    MessageBox.Show(this.Resources["networkError"].ToString(), this.Resources["errorMessageTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (isSelfUpdate)
            {
                if (MessageBox.Show(this.Resources["haveNewSelfUpdates"].ToString(), this.Resources["haveNewUpdatesTitle"].ToString(), MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    try
                    {
                        this.Hide();
                        GetUpdate updateForm = new GetUpdate();
                        updateForm.Show();
                        updateForm.setParent(this);
                        updateForm.setDownload(selfDownload);
                        updateForm.selfUpdateStart();
                        updateForm.Show();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "networkerror")
                        {
                            MessageBox.Show(this.Resources["networkError"].ToString(), this.Resources["errorMessageTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                    minecraftUpdate();
            }
        }

        public void minecraftUpdate()
        {
            if (isMinecraftUpdate)
            {
                if (MessageBox.Show(this.Resources["haveNewMinecraftUpdates"].ToString(), this.Resources["haveNewUpdatesTitle"].ToString(), MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    try
                    {
                        this.Hide();
                        GetUpdate updateForm = new GetUpdate();
                        updateForm.Show();
                        updateForm.setParent(this);
                        updateForm.setDownload(minecraftDownload);
                        updateForm.minecraftUpdateStart();
                        updateForm.Show();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "networkerror")
                        {
                            MessageBox.Show(this.Resources["networkError"].ToString(), this.Resources["errorMessageTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }
}
