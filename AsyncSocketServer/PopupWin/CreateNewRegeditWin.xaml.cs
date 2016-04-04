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
    /// CreateNewRegeditWin.xaml 的交互逻辑
    /// </summary>
    public partial class CreateNewRegeditWin : Window
    {
        public delegate void updateUI(string dir,string name,string value);

        public updateUI UpdateUiDel;
        public CreateNewRegeditWin()
        {
            InitializeComponent();
        }
           private string regeditDir;
           public CreateNewRegeditWin(string dir)
        {
            InitializeComponent();
            regeditDir = dir;
        }

           private void Button_Click(object sender, RoutedEventArgs e)
           {
               string name = nameTxtBox.Text ;
               string value = valueTxtBox.Text;
               string sendPath = regeditDir + "|" + name+"|"+value;
               byte[] path = Encoding.Default.GetBytes(sendPath);
               //create new regedit
               App.SplitSendData(App.server.userTokensList[0].ConnetSocket, path, 200, 301);
               if (UpdateUiDel != null)
               {
                   UpdateUiDel(regeditDir,name,value);
               }
               this.Close();
           }
    }
}
