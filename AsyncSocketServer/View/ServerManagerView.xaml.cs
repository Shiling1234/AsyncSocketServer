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
using System.Windows.Threading;
using PublicLibrary.Model;

namespace AsyncSocketServer.View
{
    /// <summary>
    /// ServerManagerView.xaml 的交互逻辑
    /// </summary>
    public partial class ServerManagerView : UserControl
    {
        public ServerManagerView()
        {
            InitializeComponent();
        }
        private bool firstTime = true;

        private void RefleshServerList()
        {
            App.server.ProtocolIvokeElment.serverProtocol.GetServerList += onGetServerList;
            string sendMsg = "ServerList";
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 500);
        }

        private void ServerManagerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
            {
                firstTime = false;
                return;
            }
            RefleshServerList();
        }

        private void onGetServerList(object sender, System.Collections.ObjectModel.ObservableCollection<PublicLibrary.Model.ServerInfo> e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.ServerListView.ItemsSource = e;
            }));
        }

        private void OnServerStartClick(object sender, RoutedEventArgs e)
        {
            //code 501
            ServerInfo s = this.ServerListView.SelectedItem as ServerInfo;
            string sendMsg = s.Name;
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 501);
            }
            RefleshServerList();
        }

        private void OnServerStopClick(object sender, RoutedEventArgs e)
        {
            ServerInfo s = this.ServerListView.SelectedItem as ServerInfo;
            
            string sendMsg = s.Name;
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 502);
            }
            RefleshServerList();
        }

        private void OnServerPauseClick(object sender, RoutedEventArgs e)
        {
            ServerInfo s = this.ServerListView.SelectedItem as ServerInfo;
            string sendMsg = s.Name;
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 503);
            }
            RefleshServerList();
        }

        private void OnServerContinueClick(object sender, RoutedEventArgs e)
        {
            ServerInfo s = this.ServerListView.SelectedItem as ServerInfo;
            string sendMsg = s.Name;
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 504);
            }
            RefleshServerList();
        }

        private void OnServerRefelshClick(object sender, RoutedEventArgs e)
        {
            RefleshServerList();
        }

        private void OnServerAttributeClick(object sender, RoutedEventArgs e)
        {
            ServerInfo s = this.ServerListView.SelectedItem as ServerInfo;
           
            string sendMsg = s.Path;

            sendMsg = sendMsg.Substring(0, sendMsg.IndexOf("exe",0)+3);
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 506);
            }
        }

        private void OnServerManalItemClick(object sender, RoutedEventArgs e)
        {
            ServerInfo s = this.ServerListView.SelectedItem as ServerInfo;

            string sendMsg = s.Name;
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 507);
            }
            RefleshServerList();
        }

        private void OnServerAutoItemClick(object sender, RoutedEventArgs e)
        {
            ServerInfo s = this.ServerListView.SelectedItem as ServerInfo;

            string sendMsg = s.Name;
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 508);
            }
            RefleshServerList();
        }
    }
}
