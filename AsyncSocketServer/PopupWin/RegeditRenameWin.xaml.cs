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

namespace AsyncSocketServer.PopupWin
{
    /// <summary>
    /// RegeditRenameWin.xaml 的交互逻辑
    /// </summary>
    public partial class RegeditRenameWin : Window
    {
        private string fullPath;

        public RegeditRenameWin()
        {
            InitializeComponent();
        }
        public  delegate  void regeditRename(string newName);

        public regeditRename RegeditRename;
        public RegeditRenameWin(string path)
        {
            // TODO: Complete member initialization
            this.fullPath = path;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newName = newNameTextbox.Text;
            string sendPath = fullPath + "|" + newName;
            byte[] path = Encoding.Default.GetBytes(sendPath);
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, path, 200, 304);
            if (RegeditRename != null)
            {
                RegeditRename(newName);
            }
            this.Close();
        }
    }
}
