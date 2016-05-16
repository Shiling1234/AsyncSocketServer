using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using AsyncSocketServer.Common;
using Microsoft.Win32;
using PublicLibrary.Model;


namespace AsyncSocketServer.View
{
    /// <summary>
    /// Other.xaml 的交互逻辑
    /// </summary>
    
    public partial class Other : UserControl
    {
        public Other()
        {
            InitializeComponent();
            App.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            if (t != null)
            {
                t.Abort();
                t.Join();
            }
            client.Close();
          
        
            client = null;
        }
        Thread t;
        private bool firstTime = true;
        ObservableCollection<ClientInfo> clientInfos=new ObservableCollection<ClientInfo>();
        private UdpClient client=new UdpClient();
        IPEndPoint multicast = new IPEndPoint(IPAddress.Parse("234.5.6.7"),8888);
        int times = 1;
        private void Other_OnLoaded(object sender, RoutedEventArgs e)
        {

            if (times==1)
            {
                times++;
                firstTime = false;  
                return;
            }
            if (times == 2)
            {
             
                times++;
                this.allClientListView.ItemsSource = clientInfos;
                client.JoinMulticastGroup(IPAddress.Parse("234.5.6.7"));
                t = new Thread(new ThreadStart(ReceieveData));
                t.IsBackground = true;
                t.Start();
                byte[] buf = Encoding.Default.GetBytes("Hello from multicast");
           
                App.SplitSendDataUdp(client, multicast, buf, 1000, 2, 0);

            }

          
            //     client.Send(buf, buf.Length, multicast);
            //Thread.Sleep(1000);


        }
        Regex ipRegex = new Regex(@"^\d+\.\d+\.\d+\.\d+$");
        IPEndPoint any = null;
        private void ReceieveData()
        {
            while (true)
            {
                if (client == null)
                {
                    return;
                }
                    if (client.Available <= 0) continue;
                
                byte[] buf;
                try
                {
                    buf = client.Receive(ref any);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    client.Close();
                 
                    client = null;
                    return;
                }
                string msg = Encoding.Default.GetString(buf);
                if (ipRegex.IsMatch(msg))
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        clientInfos.Add(new ClientInfo() {Ip = msg});
                    }));
                }
                else if (msg == "nextDestop")
                {
                    SendDestop(new object());
                }
                else if (msg == "nextFilePart")
                {
                    SendFile(new object());
                }
                else if (msg == "nextFilePart")
                {
                   
                        SendFile(new object());
                    

                }

                else
                {
                    string[] validMsg = msg.Split(new char[] {'|'});
                    if (validMsg[0] == "Destop")
                    {
                        SplitSendDestop(destop, int.Parse(validMsg[1]));
                    }
                    else if(validMsg[0]=="File")
                    {
                        SplitSendFile(fileBytes,int.Parse(validMsg[1]));
                    }
                }

            }
        }

        private void OnShareScreenMouseDown(object sender, MouseButtonEventArgs e)
        {

            
            Thread t=new Thread(new ParameterizedThreadStart(this.SendDestop));
            t.Start(0);
        }

        private byte[] destop;
        private void SendDestop(object obj)
        {

                //int packetIndex = (int) obj;
                MemoryStream memoryStream = null;
                memoryStream=   ScreenShotEx.CreateBitmapSourceFromBitmap();
               
                destop = memoryStream.ToArray();
               memoryStream.Dispose();
                memoryStream.Close();
                SplitSendDestop(destop,0);
   
               // client.Send(destop, destop.Length,multicast);
        }

        private void SplitSendDestop(byte[] destop,int packetIndex)
        {
      
            App.SplitSendDataUdp(client, multicast, destop, 1000, 1, packetIndex);
         
        }
        string path = string.Empty;
        private void OnGroupSendFile(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd=new OpenFileDialog();
          bool? result=  ofd.ShowDialog();

            if (result.Value == true)
            {
                path = ofd.FileName;
            }

            Thread t = new Thread(new ParameterizedThreadStart(this.SendFile));
            t.Start(0);

          
        }

       

        private bool fileSendFinish=false;
        private byte[] fileBytes;
        private int fileCursor=0;    
        private void SendFile(object obj)
        {
            
            byte[] bytes = new byte[1024];
            using (FileStream fs = new FileStream(path,FileMode.OpenOrCreate,FileAccess.ReadWrite))
            {
                int len;
                fs.Seek(fileCursor, 0);
                len = fs.Read(bytes, 0, bytes.Length);
                fileBytes=new byte[len];
                Array.Copy(bytes,0,fileBytes,0,len);
                if (len > 0)
                {
                    SplitSendFile(bytes, 0);
                    fileCursor += len;
                }
                else
                {
                    fileCursor = 0;
                    fileSendFinish = true;
                    byte[] fileFinish = Encoding.Default.GetBytes("FileTransFinish");
                    App.SplitSendDataUdp(client, multicast, fileFinish, 1000, 4, 0);
                }


            }
          
        }

        private void SplitSendFile(byte[] bytes, int p)
        { 
            App.SplitSendDataUdp(client, multicast, fileBytes, 1000, 3, p);
        }
    }

    public class PacketMsg
    {
        int packetNum;
        int packetIndex;

        public PacketMsg(int packetNum, int packetIndex)
        {
            this.packetNum = packetNum;
            this.packetIndex = packetIndex;
        }
    }
}
