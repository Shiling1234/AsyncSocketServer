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
using System.Windows.Shapes;
using AsyncSocketServer;

namespace Client.PopupWin
{
    /// <summary>
    /// ReNameWin.xaml 的交互逻辑
    /// </summary>
    public partial class ReNameWin : Window
    {
        public delegate void updateUI(string newName);

        public updateUI UpdateUiDel;
        public ReNameWin()
        {
            InitializeComponent();
        }

        private string oldFileName;
        public ReNameWin(string path)
        {
            InitializeComponent();
            oldFileName = path;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newName = newNameTextbox.Text;
            string sendPath = oldFileName + "|" + newName;
            byte[] path = Encoding.Default.GetBytes(sendPath);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, path, 200, 205);
            if (UpdateUiDel != null)
            {
                UpdateUiDel(newName);
            }
            this.Close();
        }
    }
}
