using System;
using System.Collections.Generic;
using System.IO;
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

namespace AsyncSocketServer.View
{
    /// <summary>
    /// RemoteDestopView.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteDestopView : UserControl
    {
        public RemoteDestopView()
        {
            InitializeComponent();
        }

        private bool firstTime = true;
        private void RemoteDestopView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (firstTime)
            {
                firstTime = false;
                return;
            }
            MessageBoxResult result = MessageBox.Show("是否开启远程控制?", "tishi", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                string sendMsg = "DestopImage";
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
                App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 700);
                App.server.ProtocolIvokeElment.remoteDestopProtocol.GetDestopImage += remoteDestopProtocol_GetDestopImage;
            }
        }

        void remoteDestopProtocol_GetDestopImage(object sender, System.IO.MemoryStream e)
        {
            using (var stream = e)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { RemoteDestopImg.Source = bitmap; }));

            }
        }

        private void RemoteDestopImg_OnKeyDown(object sender, KeyEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.Key.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 701);
        }

        private void RemoteDestopImg_OnKeyUp(object sender, KeyEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.Key.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 702);
        }

        private void RemoteDestopImg_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.GetPosition(this).X.ToString()+"|"+e.GetPosition(this).Y.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 703);
        }

        private void RemoteDestopImg_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.GetPosition(this).X.ToString() + "|" + e.GetPosition(this).Y.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 704);
        }

        private void RemoteDestopImg_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.GetPosition(this).X.ToString() + "|" + e.GetPosition(this).Y.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 705);
        }

        private void RemoteDestopImg_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(e.GetPosition(this).X.ToString() + "|" + e.GetPosition(this).Y.ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 706);
        }

        private void RemoteDestopImg_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RemoteDestopView_OnKeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
