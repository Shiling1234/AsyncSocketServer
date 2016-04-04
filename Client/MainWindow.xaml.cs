using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;
using Client.ClientProtocol;
using Client.Common;
using Microsoft.Win32;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
          

            App.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            App.client.Connect(IPAddress.Parse("127.0.0.1"), 5555);
            //App.client.Connect(new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[2], 5555));
            #region 发送文件
            //SendFileData("c:\\");
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReceiveCommadFromServer));
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReceiveBroadCastMsg));

            #endregion
        }
        IPEndPoint multicast = new IPEndPoint(IPAddress.Parse("234.5.6.7"), 5566);
        private int udpDataOffset = 0;
        private bool continulUdp = true;
        private int bufferOffsetUdp;
        private int needID =1;
        private bool currentFile = true;
        private string currentOperate = string.Empty;
        string savePath = string.Empty;
        //中间缓冲层
        private byte[] bufferUdp = new byte[1024 * 1024];
        private void ReceiveBroadCastMsg(object state)
        {
            UdpClient client = new UdpClient(7788);

            client.JoinMulticastGroup(IPAddress.Parse("234.5.6.7"));
            //  
            IPEndPoint multicast = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                byte[] bytes = new byte[1024 * 3];
                int len;


                while ((len = (bytes = client.Receive(ref multicast)).Length) > 5 * sizeof(int))
                {
                    continulUdp = true;
                    Buffer.BlockCopy(bytes, 0, bufferUdp, bufferOffsetUdp, len);
                    bufferOffsetUdp += len;
                   
                    //一次数据可以有多个包，不断处理
                    while (continulUdp)
                    {

                        int singlePacketLen = BitConverter.ToInt32(bytes, 0);
                        int totalLen = BitConverter.ToInt32(bytes, 4);
                        int ID = BitConverter.ToInt32(bytes, 8);
                        Console.WriteLine(ID);
                        int maxID = BitConverter.ToInt32(bytes, 12);
                        Console.WriteLine(maxID);
                        int messageType = BitConverter.ToInt32(bytes, 16);

                        if (bufferOffsetUdp >= singlePacketLen)
                        {

                            if (ID != needID)
                            {
                                byte[] getNextBytes = Encoding.Default.GetBytes(needID.ToString());
                                client.Send(getNextBytes, getNextBytes.Length, multicast);
                                bufferOffsetUdp -= len;
                                break;
                            }



                            if (ID == 1)
                            {
                                data = new byte[totalLen - 20 * maxID];
                            }

                            Array.Copy(bufferUdp, 20, data, udpDataOffset, singlePacketLen - 20);
                            udpDataOffset += singlePacketLen - 20;
                            //复制完毕,缓冲池前移动
                            Array.Copy(bufferUdp, singlePacketLen, bufferUdp, 0,
                                len - singlePacketLen);
                            //读取完成后将offset前移。
                            bufferOffsetUdp -= singlePacketLen;


                            //  i need next packet
                            if (ID < maxID)
                            {
                                if (messageType == 1)
                                {
                                    currentOperate = "Destop";
                                }
                                else if(messageType ==3)
                                {
                                    currentOperate = "File";
                                }
                                byte[] getNextBytes = Encoding.Default.GetBytes(currentOperate+"|"+ID.ToString());
                                client.Send(getNextBytes, getNextBytes.Length, multicast);
                                
                                needID = ID+1;
                            }
                            if (ID == maxID)
                            {
                                needID = 1;
                                //偏移量置0
                                udpDataOffset = 0;
                            

                                switch (messageType)
                                {

                                    case 1:

                                        MemoryStream stream = new MemoryStream(data);
                                        var bitmap = new BitmapImage();
                                        bitmap.BeginInit();
                                        bitmap.StreamSource = stream;
                                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                        bitmap.EndInit();
                                        bitmap.Freeze();
                                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { RemoteDestopImg.Source = bitmap; }));
                                        byte[] getNextDestop = Encoding.Default.GetBytes("nextDestop");
                                        client.Send(getNextDestop, getNextDestop.Length, multicast);
                                        break;
                                    case 2:
                                        if (Encoding.Default.GetString(data) == "Hello from multicast")
                                        {
                                            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
                                            Regex ipRegex = new Regex(@"^\d+\.\d+\.\d+\.\d+$");
                                            foreach (IPAddress address in addresses)
                                            {
                                                bool b = ipRegex.IsMatch(address.ToString());
                                                if (b)
                                                {
                                                    byte[] sendBytes = Encoding.Default.GetBytes(address.ToString());
                                                    client.Send(sendBytes, sendBytes.Length, multicast);
                                                }
                                            }
                                        }
                                        break;
                                    case  3:
                                        
                                        if (currentFile == true)
                                        {
                                            MessageBox.Show("来自发送方的文件,请选择保存目录");
                                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                            {
                                                SaveFileDialog sf = new SaveFileDialog();
                                                bool? result = sf.ShowDialog(this);
                                                savePath = sf.FileName;
                                            }));
                                          
                                            currentFile = false;
                                        }
                                        using (FileStream fs = new FileStream(savePath, FileMode.Append, FileAccess.Write))
                                        {
                                            if (savePath != null)
                                            {
                                                fs.Write(data, 0, data.Length);
                                            }
                                        }
                                         byte[] getNextFilePart = Encoding.Default.GetBytes("nextFilePart");
                                         client.Send(getNextFilePart, getNextFilePart.Length, multicast);
                                        break;
                                    case 4:
                                        MessageBox.Show("文件发送完毕");
                                        currentFile = true;
                                        break;

                                }

                            }
                            //当前处理后的长度不够单包大小了，不用解析了。

                        }
                        else
                        {
                            continulUdp = false;
                        }
                    }

                }
            }

           
        }

        private byte[] data;
        private int dataOffset = 0;//不断的加
        private bool continueDeal = true;
        private int bufferOffset;//不断的减，将拿到的数据暂存起来

        //中间缓存层，数据来了先放进bytes，之后再往data里放。超过single不到2个包的剩余部分放里边
        private byte[] buffer = new byte[1024 * 1024];

        private void ReceiveCommadFromServer(object state)
        {

            while (true)
            {

                byte[] bytes = new byte[1024 * 3];
                int len;


                while ((len = App.client.Receive(bytes)) > 5 * sizeof(int))
                {
                    continueDeal = true;
                    Buffer.BlockCopy(bytes, 0, buffer, bufferOffset, len);
                    bufferOffset += len;
                    //一次数据可以有多个包，不断处理
                    while (continueDeal)
                    {

                        int singlePacketLen = BitConverter.ToInt32(bytes, 0);
                        int totalLen = BitConverter.ToInt32(bytes, 4);
                        int ID = BitConverter.ToInt32(bytes, 8);
                        int maxID = BitConverter.ToInt32(bytes, 12);
                        int messageType = BitConverter.ToInt32(bytes, 16);
                        if (bufferOffset >= singlePacketLen)
                        {

                            if (ID == 1)
                            {
                                data = new byte[totalLen - 20 * maxID];
                            }

                            Array.Copy(buffer, 20, data, dataOffset, singlePacketLen - 20);
                            dataOffset += singlePacketLen - 20;
                            //复制完毕,缓冲池前移动
                            Array.Copy(buffer, singlePacketLen, buffer, 0,
                                len - singlePacketLen);
                            //读取完成后将offset前移。
                            bufferOffset -= singlePacketLen;
                            if (ID == maxID)
                            {

                                //偏移量置0
                                dataOffset = 0;

                                switch (messageType)
                                {

                                    case 1:
                                        break;
                                    case 200:
                                        string path = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("客户端收到发送文件列表请求,请求目录{0}", path);
                                        SendFileData(path);
                                        Console.WriteLine("客户端发送目录：{0}\t下文件完毕", path);
                                        break;
                                    case 201:
                                        string DownLoadfile = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("请求下载文件{0}", DownLoadfile);
                                        try
                                        {
                                            new FileProtocol().SendFile(DownLoadfile);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                        Console.WriteLine("客户端发送文件：{0}\t完毕", DownLoadfile);
                                        break;
                                    case 202:
                                        string runFile = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("请求运行文件{0}", runFile);
                                        new FileProtocol().RunFile(runFile);
                                        Console.WriteLine("客户端运行文件：{0}\t完毕", runFile);
                                        break;
                                    case 203:
                                        string getFileDir = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("请求打开文件所在目录{0}", getFileDir);
                                        new FileProtocol().OpenFileDir(getFileDir);
                                        Console.WriteLine("客户端打开文件目录：{0}\t完毕", getFileDir);
                                        break;
                                    case 204:
                                        string delFile = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("请求删除{0}", delFile);
                                        new FileProtocol().DeleteFile(delFile);
                                        Console.WriteLine("客户端删除文件：{0}\t完毕", delFile);
                                        break;
                                    case 205:
                                        string ReNameFile = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("请求重命名{0}", ReNameFile);
                                        new FileProtocol().ReNameFile(ReNameFile);
                                        Console.WriteLine("客户端重命名文件：{0}\t完毕", ReNameFile);
                                        break;
                                    case 300:
                                        string registryKey = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("客户端收到发送注册表列表请求,请求目录{0}", registryKey);
                                        new RegistryProtocol().SendRegistry(registryKey);
                                        break;

                                    case 301:
                                        string createregistry = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("客户端收到新建注册表请求,请求目录{0}", createregistry);
                                        new RegistryProtocol().CreateNewRegedit(createregistry);
                                        break;
                                    case 302:
                                        string delregistry = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("客户端收到删除注册表请求,请求目录{0}", delregistry);
                                        new RegistryProtocol().DeleteRegedit(delregistry);
                                        break;
                                    case 303:
                                        string updateregistry = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("客户端收到更新注册表请求,请求目录{0}", updateregistry);
                                        new RegistryProtocol().UpdateRegedit(updateregistry);
                                        break;
                                    case 304:
                                        string renameregistry = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("客户端收到重命名注册表请求,请求目录{0}", renameregistry);
                                        new RegistryProtocol().RenameRegedit(renameregistry);
                                        break;
                                    case 400:
                                        string requestMsg = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", requestMsg);
                                        new ProcessProtocol().SendRequsetMsg(requestMsg);
                                        break;
                                    case 401:
                                        string killProcessId = System.Text.Encoding.Default.GetString(data);
                                        new ProcessProtocol().KillProcess(killProcessId);
                                        break;
                                    case 402:
                                        string refreshProcessId = System.Text.Encoding.Default.GetString(data);
                                        new ProcessProtocol().RefreshProcess(refreshProcessId);
                                        break;
                                    case 403:
                                        string porcessDir = System.Text.Encoding.Default.GetString(data);
                                        new ProcessProtocol().GotoProcessDir(porcessDir);
                                        break;
                                    case 404:
                                        string restartPorcess = System.Text.Encoding.Default.GetString(data);
                                        new ProcessProtocol().RestartProcess(restartPorcess);
                                        break;
                                    case 405:
                                        string openArtibuteDia = System.Text.Encoding.Default.GetString(data);
                                        new ProcessProtocol().OpenProcessAtributeDialg(openArtibuteDia);
                                        break;
                                    case 500:
                                        string serverInfo = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", serverInfo);
                                        new ServerProtocol().SendRequsetMsg(serverInfo);
                                        break;
                                    //KAISHI  TINGZHI   ZANTING  JIXU
                                    case 501:
                                        string startServer = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", startServer);
                                        new ServerProtocol().StartServer(startServer);
                                        break;
                                    case 502:
                                        string stopServer = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", stopServer);
                                        new ServerProtocol().StopServer(stopServer);
                                        break;
                                    case 503:
                                        string pauseServer = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", pauseServer);
                                        new ServerProtocol().PauseServer(pauseServer);
                                        break;
                                    case 504:
                                        string continueServer = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", continueServer);
                                        new ServerProtocol().ContinueServer(continueServer);
                                        break;
                                    case 505:
                                        string refreshServer = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", refreshServer);
                                        new ServerProtocol().RefreshServer(refreshServer);
                                        break;
                                    case 506:
                                        string serverAtrribute = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求{0}", serverAtrribute);
                                        new ServerProtocol().GetServerAtrribute(serverAtrribute);
                                        break;
                                    case 507:
                                        string manualServer = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求更改服务为手动");
                                        new ServerProtocol().SetServerToManual(manualServer);
                                        break;
                                    case 508:
                                        string autolServer = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求更改服务为自动");
                                        new ServerProtocol().SetServerToAuto(autolServer);
                                        break;
                                    case 600:
                                        string startupInfo = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求开机启动项");
                                        new StartUpItemProtocol().SendRequsetMsg(startupInfo);
                                        break;
                                    case 601:
                                        string getStartupPos = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求打开开机启动项位置");
                                        new StartUpItemProtocol().OpenStartupItemDir(getStartupPos);
                                        break;
                                    case 602:
                                        string getStartupAtrribute = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求打开开机启动项属性");
                                        new StartUpItemProtocol().OpenStartupItemAtrribute(getStartupAtrribute);
                                        break;
                                    case 603:
                                        string forbiddenStartUpItem = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求打开开机启动项属性");
                                        new StartUpItemProtocol().ForbiddenStartUpItem(forbiddenStartUpItem);
                                        break;
                                    case 700:
                                        string destop = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请求远程桌面");
                                        new RemoteDestopProtocol().SendRemoteDestop();
                                        break;
                                    case 701:
                                        string keyDownMsg = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请键盘按下{0}", keyDownMsg);
                                        new RemoteDestopProtocol().KeyDown(keyDownMsg);
                                        break;
                                    case 702:
                                        string keyUpMsg = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请键盘松开{0}", keyUpMsg);
                                        new RemoteDestopProtocol().KeyUp(keyUpMsg);
                                        break;
                                    case 703:
                                        string MouseLeftDownMsg = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请鼠标左键按下{0}", MouseLeftDownMsg);
                                        new RemoteDestopProtocol().MouseLeftDown(MouseLeftDownMsg);
                                        break;
                                    case 704:
                                        string MouseLeftUpMsg = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请左键松开{0}", MouseLeftUpMsg);
                                        new RemoteDestopProtocol().MouseLeftUp(MouseLeftUpMsg);
                                        break;
                                    case 705:
                                        string MouseRightDownMsg = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请右键按下{0}", MouseRightDownMsg);
                                        new RemoteDestopProtocol().MouseRightDown(MouseRightDownMsg);
                                        break;
                                    case 706:
                                        string MouseRightUpnMsg = System.Text.Encoding.Default.GetString(data);
                                        Console.WriteLine("服务端请右键松开{0}", MouseRightUpnMsg);
                                        new RemoteDestopProtocol().MouseRightUp(MouseRightUpnMsg);
                                        break;
                                }

                            }
                            //当前处理后的长度不够单包大小了，不用解析了。

                        }
                        else
                        {
                            continueDeal = false;
                        }
                    }

                }
            }
        }



        private void SendFileData(string dir)
        {
            FileProtocol p = new FileProtocol();
            Stream s = p.PacketData(dir);
            p.SplitSendData(App.client, s, 200, 200);
        }
    }
}
