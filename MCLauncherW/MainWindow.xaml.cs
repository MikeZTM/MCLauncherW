﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.IO;

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
            SetLanguageDictionary();
            playerNameTextField.Text = playerName;
            if (playerName == "")
            {
                javaVM = javaAutoDetect();
                if (javaVM == string.Empty)
                {
                    MessageBox.Show("没有找到Java路径！请手动设置！", "错误！", MessageBoxButton.OK, MessageBoxImage.Error);
                    javaVM = Properties.Settings.Default.javaVM;
                }
                else
                {
                    Properties.Settings.Default.javaVM = javaVM;
                    Properties.Settings.Default.Save();
                }
            }
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
                MessageBox.Show("未安装Java！");
                return findJavaPath;
            }
            else
            {
                string javaVersion = pregkey.GetValue("CurrentVersion", "0").ToString();
                //textBox1.AppendText("Java版本：" + javaVersion + "\r\n");
                pregkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\" + javaVersion, false);
                if (pregkey == null)
                {
                    MessageBox.Show("未安装Java！");
                    return findJavaPath;
                }
                else
                {
                    string javaPath = pregkey.GetValue("JavaHome", "0").ToString();
                    //textBox1.AppendText(javaPath + "\r\n");
                    string javaw = javaPath + "\\" + "bin\\javaw.exe";
                    if (javaw.Contains(" (x86)"))
                    {
                        //textBox1.AppendText("32位Java路径：" + javaw + "\r\n");
                        string java64w = javaw.Replace(" (x86)", "");
                        //textBox1.AppendText("64位Java路径" + java64w + "\r\n");
                        if (File.Exists(java64w))
                        {
                            findJavaPath = java64w;
                            //textBox1.AppendText("系统中存在64位Java！将优先选用！\r\n");
                        }
                        else
                            if (File.Exists(javaw))
                            {
                                findJavaPath = javaw;
                                //textBox1.AppendText("系统中存在32位Java！\r\n");
                            }
                            else
                            {
                                findJavaPath = string.Empty;
                                //textBox1.AppendText("没有检测到Java！请手动选择。\r\n");
                            }
                    }
                    else
                    {
                        //textBox1.AppendText("Java路径：" + javaw + "\r\n");
                        if (File.Exists(javaw))
                        {
                            findJavaPath = javaw;
                            //textBox1.AppendText("系统中存在Java！\r\n");
                        }
                        else
                        {
                            findJavaPath = string.Empty;
                            //textBox1.AppendText("没有检测到Java！请手动选择。\r\n");
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
            x64mode = Properties.Settings.Default.x64mode;
            memory = Properties.Settings.Default.memory;
            mcPath = Properties.Settings.Default.mcPath;

        }
    }
}
