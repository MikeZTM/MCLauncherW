using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
    /// GetUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class GetUpdate : Window
    {
        public GetUpdate()
        {
            InitializeComponent();
            SetLanguageDictionary();
            this.downloadingLable.Content = this.Resources["downloading"].ToString() + "0%";
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

        private MainWindow win;
        private string download;
        WebClient wc = new WebClient();

        private void Window_Closed(object sender, EventArgs e)
        {
            wc.CancelAsync();
        }

        public void setParent(MainWindow par)
        {
            this.win = par;
        }

        public void setDownload(string down)
        {
            this.download = down;
        }

        public void selfUpdateStart()
        {
            try
            {
                wc.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc.Proxy = WebRequest.DefaultWebProxy;

                string filename = "new.exe";
                wc.DownloadFileAsync(new Uri(download), filename);
                //wc.DownloadFile(download, filename);

                wc.Dispose();
            }
            catch
            {
                throw new Exception("networkerror");
            }
        }

        public void minecraftUpdateStart()
        {
            try
            {
                wc.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc.Proxy = WebRequest.DefaultWebProxy;

                string filename = "update.zip";
                wc.DownloadFileAsync(new Uri(download), filename);
                //wc.DownloadFile(download, filename);

                wc.Dispose();
            }
            catch
            {
                throw new Exception("networkerror");
            }
        }

        void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show(this.Resources["downloadCanceled"].ToString());
            else
            {
                MessageBox.Show(this.Resources["downloadFinish"].ToString());
                if (download.Contains("MCLauncherW.exe"))
                {
                    string filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "killmyself.bat");
                    using (StreamWriter bat = File.CreateText(filename))
                    {
                        // 自升级
                        bat.WriteLine(string.Format(@"
@echo off
:selfkill
attrib -a -r -s -h ""{0}""
del ""{0}""
if exist ""{0}"" goto selfkill
copy /y ""new.exe"" ""{0}""
del ""new.exe""
""{0}""

del %0
", AppDomain.CurrentDomain.FriendlyName));
                    }

                    // 启动自删除批处理文件
                    ProcessStartInfo info = new ProcessStartInfo(filename);
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(info);

                    // 强制关闭当前进程
                    Environment.Exit(0);
                }
                else
                {
                    String path = Properties.Settings.Default.mcPath;
                    path = path.Substring(0, path.Length - 18);
                    Update.updateFile("update.zip", path);
                    Properties.Settings.Default.minecraftVersion = MainWindow.newMinecraftVersion;
                    Properties.Settings.Default.Save();
                }
            }
            this.win.Show();
            this.Close();
        }

        void wc_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.win.Hide();
            downloadingProcess.Value = e.ProgressPercentage;
            this.downloadingLable.Content = this.Resources["downloading"].ToString() + downloadingProcess.Value.ToString() + "%";
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            wc.CancelAsync();
        }
    }
}
