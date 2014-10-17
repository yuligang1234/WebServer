using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;using System.Windows.Forms;
using System.Configuration;

namespace WebServer
{
    public partial class StartForm : BaseForm
    {

        public StartForm()
        {
            //应用皮肤
            ApplySkin("McSkin");
            InitializeComponent();
            //加载默认的路径、端口、浏览器
            TxtUrl.Text = GetAppConfig("url");
            TxtPort.Text = GetAppConfig("port");
            TxtBrowse.Text = GetAppConfig("browse");
        }

        /// <summary>
        ///  网站浏览
        /// </summary>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-19 20:38:54
        private void button2_Click(object sender, EventArgs e)
        {
            string url = TxtUrl.Text;//浏览的网站的地址
            string str = "/";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请选择网站目录!");
            }
            else
            {
                //获取WebDev.WebServer路径
                string devServer = GetDevServer();
                if (File.Exists(devServer))
                {
                    string portNumber = GetPortNumber();
                    SaveAppConfig("port", portNumber);//保存端口
                    StartProcess(devServer, portNumber, url, str);
                }
                else
                {
                    MessageBox.Show("ASP.NET服务环境没有安装，请安装!");
                }
            }
        }

        /// <summary>
        ///  获取WebDev.WebServer路径
        /// </summary>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-19 20:47:22
        private string GetDevServer()
        {
            string str = string.Format("{0}\\Microsoft Shared\\DevServer\\10.0\\WebDev.WebServer40.exe", Environment.GetEnvironmentVariable("CommonProgramFiles"));
            if (!File.Exists(str))
            {
                str = string.Format("{0}\\Microsoft Shared\\DevServer\\10.0\\WebDev.WebServer40.exe", Environment.GetEnvironmentVariable("CommonProgramFiles(x86)"));
            }
            else if (!File.Exists(str))
            {
                str = string.Format("{0}\\Microsoft Shared\\DevServer\\11.0\\WebDev.WebServer40.exe", Environment.GetEnvironmentVariable("CommonProgramFiles"));
                if (!File.Exists(str))
                {
                    str = string.Format("{0}\\Microsoft Shared\\DevServer\\11.0\\WebDev.WebServer40.exe", Environment.GetEnvironmentVariable("CommonProgramFiles(x86)"));
                }
            }
            return str;
        }

        /// <summary>
        ///  获取端口号，如果没有设置，默认是22222
        /// </summary>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-19 20:52:48
        private string GetPortNumber()
        {
            TcpListener tcpListener;
            bool flag = false;
            int port = 22222;//默认端口号是22222
            try
            {
                //获取端口号
                int customPort = port;
                Int32.TryParse(TxtPort.Text, out customPort);
                tcpListener = new TcpListener(IPAddress.Any, customPort);
                tcpListener.ExclusiveAddressUse = true;
                tcpListener.Start();
                port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
                tcpListener.Stop();
            }
            catch (SocketException)
            {
                flag = (MessageBox.Show("端口错误，将自动获取端口？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK);
            }
            if (flag)//如果端口号错误或者被使用，自动回去未使用的端口号
            {
                tcpListener = new TcpListener(IPAddress.Any, 0);
                tcpListener.Start();
                port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
                tcpListener.Stop();
            }
            return port.ToString();
        }

        /// <summary>
        ///  开始浏览网站（DevServer的格式）
        /// </summary>
        /// <param name="cassiniExecutable"></param>
        /// <param name="portNumber"></param>
        /// <param name="physPath"></param>
        /// <param name="appName"></param>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-19 21:20:16
        private void StartProcess(string cassiniExecutable, string portNumber, string physPath, string appName)
        {
            Process process = new Process();
            process.StartInfo.FileName = cassiniExecutable;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = string.Format("/port:{0} /path:\"{1}\" /vpath:\"{2}\"", portNumber, physPath,
                appName);
            process.Start();
            DevServerProcess devServerProcess = new DevServerProcess();
            devServerProcess.ShowDialog();
            Process process1 = new Process();
            process1.StartInfo.UseShellExecute = true;
            process1.StartInfo.FileName = string.Format("http://localhost:{0}{1}", portNumber, appName);
            if (string.IsNullOrEmpty(TxtBrowse.Text))
            {
                process1.Start();//执行浏览操作
            }
            else
            {
                try
                {
                    Process.Start(TxtBrowse.Text, process1.StartInfo.FileName);
                }
                catch (Exception)
                {
                    process1.Start();//执行浏览操作
                }
            }
            SaveAppConfig("browse", TxtBrowse.Text);
        }

        /// <summary>
        ///  网站URL
        /// </summary>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-19 21:27:29
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择网站路径";
            dialog.SelectedPath = TxtUrl.Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TxtUrl.Text = dialog.SelectedPath;
                SaveAppConfig("url", TxtUrl.Text);//报错路径
            }
        }

        /// <summary>
        ///  根据key读取value
        /// </summary>
        /// <param name="key">The key.</param>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-20 09:59:25
        private string GetAppConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        ///  保存配置文件
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-20 10:12:26
        private void SaveAppConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        ///  浏览器路径
        /// </summary>
        /// Author  : 俞立钢
        /// Company : 绍兴标点电子技术有限公司
        /// Created : 2014-08-20 14:24:23
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择浏览器";
            dialog.Filter = "运行文件|*.exe";//过滤格式
            dialog.InitialDirectory = TxtBrowse.Text;//默认路径
            dialog.Multiselect = false;//不可多选
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TxtBrowse.Text = dialog.FileName;
                SaveAppConfig("browse", TxtBrowse.Text);
            }
        }



    }
}
