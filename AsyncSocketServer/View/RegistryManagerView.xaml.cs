using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using AsyncSocketServer.PopupWin;
using AsyncSocketServer.ViewModel;
using Client.PopupWin;
using PublicLibrary;

namespace AsyncSocketServer.View
{
    /// <summary>
    /// RegistryManagerView.xaml 的交互逻辑
    /// </summary>
    public partial class RegistryManagerView : UserControl
    {
        public RegistryManagerView()
        {
            InitializeComponent();
        }

        private ObservableCollection<SingleRegeditInfo> regeditInfos;
        private void RegistryItemOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistryInfo selectRegistry = ((sender as TextBlock).DataContext as RegistryInfoViewModel).RegistryInfoInstance;
            regeditInfos = new ObservableCollection<SingleRegeditInfo>();
            if (selectRegistry.Content != null)
            {
                foreach (var keyValue in selectRegistry.Content)
                {
                    regeditInfos.Add(new SingleRegeditInfo() { Key = keyValue.Key, Value = keyValue.Value, Dir = selectRegistry.Name });
                }
                detailListView.ItemsSource = regeditInfos;
            }
        }

        private void CreateRegeditItem(object sender, RoutedEventArgs e)
        {
            SingleRegeditInfo singleRegedit = detailListView.SelectedItem as SingleRegeditInfo;
            string dir = singleRegedit.Dir;
            Console.WriteLine("需要在{0}新建一项");
            CreateNewRegeditWin cnRegeditWin = new CreateNewRegeditWin(dir);
            cnRegeditWin.UpdateUiDel += this.UpdateUi;

            cnRegeditWin.ShowDialog();
        }

        private void UpdateUi(string dir, string name, string value)
        {
            regeditInfos.Add(new SingleRegeditInfo() { Key = name, Dir = dir, Value = value });
        }

        private void DeleteRegeditItem(object sender, RoutedEventArgs e)
        {
            SingleRegeditInfo singleRegedit = detailListView.SelectedItem as SingleRegeditInfo;
            string path = singleRegedit.Dir + "|" + singleRegedit.Key;
            byte[] sendpath = Encoding.Default.GetBytes(path);
            //Delete give regedit
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, sendpath, 200, 302);
            regeditInfos.Remove(singleRegedit);
        }


        private void ReNameRegeditItem(object sender, RoutedEventArgs e)
        {
            singleRegedit = detailListView.SelectedItem as SingleRegeditInfo;
            string path = singleRegedit.Dir + "|" + singleRegedit.Key+"|"+singleRegedit.Value;
            RegeditRenameWin regeditRenameWin=new RegeditRenameWin(path);
            regeditRenameWin.RegeditRename += RegeditRename;
            regeditRenameWin.ShowDialog();
        }

        private void RegeditRename(string newName)
        {
            singleRegedit.Key = newName;
        }

        private SingleRegeditInfo singleRegedit;
        private void UpdateRegeditItem(object sender, RoutedEventArgs e)
        {
            singleRegedit = detailListView.SelectedItem as SingleRegeditInfo;
            string path = singleRegedit.Dir + "|" + singleRegedit.Key;
            UpdateRegedit upWin = new UpdateRegedit(path,singleRegedit.Value);
            upWin.UpdateUiDel += UpdateValue;
            upWin.ShowDialog();
        }

        private void UpdateValue(string newValue)
        {
            singleRegedit.Value = newValue;
        }
    }

    public class SingleRegeditInfo : INotifyPropertyChanged
    {
        private string m_key;
        private string m_value;
        private string m_dir;
        public String Key
        {
            get { return m_key; }
            set
            {
                if (value != this.m_key)
                {
                    m_key = value;
                    RaisePropertyChanged("Key");
                }
            }
        }
        public String Value
        {
            get { return m_value; }
            set
            {
                if (value != this.m_value)
                {
                    m_value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        public String Dir
        {
            get { return m_dir; }
            set
            {
                if (value != this.m_dir)
                {
                    m_dir = value;
                    RaisePropertyChanged("Dir");
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
