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
using Microsoft.Win32;

namespace AsyncSocketServer.PopupWin
{
    /// <summary>
    /// UpdateRegedit.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateRegedit : Window
    {
        public UpdateRegedit()
        {
           
        }
        public delegate void updateUI(string newValue);
        public updateUI UpdateUiDel;
        private string newValue;
        private string oldValue;
        public UpdateRegedit(string value,string oldValue)
        {
            newValue = value;
            this.oldValue = oldValue;    
            InitializeComponent();
            this.oldValueTxtBlock.Text = oldValue;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string sendPath = newValue + "|" + newValueTxtBox.Text;
            byte[] path = Encoding.Default.GetBytes(sendPath);
            //update regedit
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, path, 200, 303);
            if (UpdateUiDel != null)
            {
                UpdateUiDel(newValueTxtBox.Text);
                RegistryKey key=Registry.ClassesRoot;
                
            }
            this.Close();
            
        }
    }
}
