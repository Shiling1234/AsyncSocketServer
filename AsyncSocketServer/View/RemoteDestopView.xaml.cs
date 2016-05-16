using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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

        int times = 1;
        private void RemoteDestopView_OnLoaded(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            if (times == 1)
            {
                times++;
                return;

            }
            if (times == 2)
            {
                times++;
                result = MessageBox.Show("是否开启远程控制?", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string sendMsg = "DestopImage";
                    byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendMsg);
                    App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 700);
                    App.server.ProtocolIvokeElment.remoteDestopProtocol.GetDestopImage += remoteDestopProtocol_GetDestopImage;
                }

            }

        }

        void remoteDestopProtocol_GetDestopImage(object sender, System.IO.MemoryStream e)
        {
           
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = e;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { RemoteDestopImg.Source = bitmap; }));           
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

            double hor = imageScrollViewer.HorizontalOffset;
            double ver = imageScrollViewer.VerticalOffset;
            double conHor = imageScrollViewer.ContentHorizontalOffset;
            double conver = imageScrollViewer.ContentVerticalOffset;

            Console.WriteLine(hor + " " + ver + " " + conHor + " " + conver);

            byte[] sendBytes = System.Text.Encoding.Default.GetBytes((e.GetPosition(this).X + hor).ToString() + "|" + (e.GetPosition(this).Y + ver).ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 703);
        }

        private void RemoteDestopImg_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double hor = imageScrollViewer.HorizontalOffset;
            double ver = imageScrollViewer.VerticalOffset;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes((e.GetPosition(this).X + hor).ToString() + "|" + (e.GetPosition(this).Y + ver).ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 704);
        }

        private void RemoteDestopImg_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            double hor = imageScrollViewer.HorizontalOffset;
            double ver = imageScrollViewer.VerticalOffset;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes((e.GetPosition(this).X + hor).ToString() + "|" + (e.GetPosition(this).Y + ver).ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 705);
        }

        private void RemoteDestopImg_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            double hor = imageScrollViewer.HorizontalOffset;
            double ver = imageScrollViewer.VerticalOffset;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes((e.GetPosition(this).X + hor).ToString() + "|" + (e.GetPosition(this).Y + ver).ToString());
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendBytes, 20, 706);
        }

        private void RemoteDestopImg_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void RemoteDestopView_OnKeyDown(object sender, KeyEventArgs e)
        {
            // throw new NotImplementedException();
        }
    }
}
