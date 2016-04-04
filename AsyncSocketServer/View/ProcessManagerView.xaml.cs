using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// ProcessManagerView.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessManagerView : UserControl
    {
        public ProcessManagerView()
        {
            InitializeComponent();
        }

        private bool firstTime = true;

        private void RefleshProcessList()
        {
            App.server.ProtocolIvokeElment.processProtocol.GetProcessList += onGetProcessList;
            string sendMsg = "ProcessList";
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 400);
        }

        private void ProcessManagerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
            {
                firstTime = false;
                return;
            }
            RefleshProcessList();
        }

        private ObservableCollection<ProcessInfo> processList = new ObservableCollection<ProcessInfo>();
        private void onGetProcessList(object sender, System.Collections.ObjectModel.ObservableCollection<PublicLibrary.Model.ProcessInfo> e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.ProcessListView.ItemsSource = e;
                processList = e;
            }));


        }

        private void KillProcessOnPreMouseDown(object sender, MouseButtonEventArgs e)
        {
            ProcessInfo p = ProcessListView.SelectedItem as ProcessInfo;
            string sendMsg = p.PID.ToString();
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 401);
            RefleshProcessList();
        }

        private void RefreshOnPreMouseDown(object sender, MouseButtonEventArgs e)
        {
            RefleshProcessList();
            MessageBox.Show("reflesh success");

        }

        private void GoToProcessDirOnPreMouseDown(object sender, MouseButtonEventArgs e)
        {
            ProcessInfo p = ProcessListView.SelectedItem as ProcessInfo;
            string sendMsg = p.ExcutePath;
            if (sendMsg == null)
            {
                MessageBox.Show("当前系统进程目录不存在");
                return;
            }
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 403);
        }

        private void ReStartProcessPreMouseDown(object sender, MouseButtonEventArgs e)
        {
            ProcessInfo p = ProcessListView.SelectedItem as ProcessInfo;
            string sendMsg = p.PID.ToString()+"|"+p.ExcutePath;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 404);
            RefleshProcessList();
        }

        private void GetArtibutePreMouseDown(object sender, MouseButtonEventArgs e)
        {
            ProcessInfo p = ProcessListView.SelectedItem as ProcessInfo;
            string sendMsg = p.ExcutePath;
            if (sendMsg != null)
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);

                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 405);
            }
        }
    }
}
