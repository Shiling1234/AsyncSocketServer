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
    /// StartUpItemManager.xaml 的交互逻辑
    /// </summary>
    public partial class StartUpItemManager : UserControl
    {
        public StartUpItemManager()
        {
            InitializeComponent();
        }
        private bool firstTime = true;
        private void StartUpItemManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
            {
                firstTime = false;
                return;
            }
            App.server.ProtocolIvokeElment.StartUpProtocol.GetStartUpItems += StartUpProtocol_GetStartUpItems; ;
            string sendMsg = "StartUpItems";
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 600);
        }

        void StartUpProtocol_GetStartUpItems(object sender, System.Collections.ObjectModel.ObservableCollection<PublicLibrary.Model.StartUpItemInfo> e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.StartUpitemListView.ItemsSource = e;
            }));
        }

        private void OpenFileDirClick(object sender, MouseButtonEventArgs e)
        {
           StartUpItemInfo startUpinfo=   this.StartUpitemListView.SelectedItem as StartUpItemInfo ;
            string path = startUpinfo.Command.Substring(0, startUpinfo.Command.LastIndexOf("exe")+3);
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(path);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 601);
        }

        private void OnOpenItemAtrributeClick(object sender, MouseButtonEventArgs e)
        {
            StartUpItemInfo startUpinfo = this.StartUpitemListView.SelectedItem as StartUpItemInfo;
            string path = startUpinfo.Command.Substring(0, startUpinfo.Command.LastIndexOf("exe") + 3);
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(path);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 602);
        }

        private void OnForbiddenItemClick(object sender, MouseButtonEventArgs e)
        {
            StartUpItemInfo startUpinfo = this.StartUpitemListView.SelectedItem as StartUpItemInfo;
            string path = startUpinfo.Caption+"|"+startUpinfo.Location;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(path);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 603);
        }
    }
}
