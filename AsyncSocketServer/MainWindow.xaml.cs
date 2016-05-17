using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using AsyncSocketServer.AsyncSocketCore;
using PublicLibrary;


namespace AsyncSocketServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            App.server = new Server(2, 1024 * 1024);
            this.Loaded += MainWindow_Loaded;
            InitializeComponent();        
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartServer));
        }

        private void StartServer(object state)
        {
            App.server.Init();
           
           App.server.Start(new IPEndPoint(IPAddress.Parse("192.168.0.201"), 5555));
         //   App.server.Start(new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[2], 5555));
            App.server.StartAccept(null);
        }


        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.Key.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 701);
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.Key.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 702);
        }

        private void OnCloseRemoteDestop(object sender, MouseButtonEventArgs e)
        {
          //2
            string sendMsg = "CloseDestopImage";
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 707);
        }

        private void OnOpenRemoteDestop(object sender, MouseButtonEventArgs e)
        {
            string sendMsg = "DestopImage";
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 700);
            
        }
    }
}
