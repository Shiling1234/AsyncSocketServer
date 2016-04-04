using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AsyncSocketServer.ViewModel;
using Client.PopupWin;
using Microsoft.Win32;
using PublicLibrary;
using Path = System.IO.Path;

namespace AsyncSocketServer.View
{
    /// <summary>
    /// FileSystemView.xaml 的交互逻辑
    /// </summary>
    public partial class FileSystemView : UserControl
    {
        public FileSystemView()
        {
            InitializeComponent();
           
        }


        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock selectedTextBlock = sender as TextBlock;
            FileObjectViewModel clickFileObjectViewModel = selectedTextBlock.DataContext as FileObjectViewModel;
            //去获取一下子项的内容
            if (clickFileObjectViewModel.IsExpanded == false)
            {
                clickFileObjectViewModel.IsExpanded = true;
            }
            detailListView.ItemsSource = clickFileObjectViewModel.subFiles;



        }

        private void DownLoadItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.server.ProtocolIvokeElment.fileManagerProtocol.FileTransCompleted += OntransferCompleted;
            ContextMenu contextMenu = (sender as MenuItem).Parent as ContextMenu;
            FileObject file = detailListView.SelectedItem as FileObject;
            Console.WriteLine(file.FilePath);
            byte[] path = System.Text.Encoding.Default.GetBytes(file.FilePath);
            bool? operate = null;
            SaveFileDialog sf = new SaveFileDialog();
            operate = sf.ShowDialog();
            if (operate.HasValue)
            {
                if (((bool)operate) == true)
                {
                    App.downLoadPath = sf.FileName;
                }
            }
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, path, 20, 201);

            Console.WriteLine("我是服务端,本次发送内容为文件请求,请求文件为{0},字节量大小为{1}", file.FilePath, path.Length);
        }

        private void OntransferCompleted(object sender, string e)
        {
            MessageBox.Show(e);
            App.server.ProtocolIvokeElment.fileManagerProtocol.FileTransCompleted -= OntransferCompleted;
        }


        private void RunItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileObject file = detailListView.SelectedItem as FileObject;
            Console.WriteLine(file.FilePath);
            byte[] path = System.Text.Encoding.Default.GetBytes(file.FilePath);
            //202   run file
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket,path,200,202);
        }

        private void GetFileDirItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileObject file = detailListView.SelectedItem as FileObject;
            Console.WriteLine(file.FilePath);
            byte[] path = System.Text.Encoding.Default.GetBytes(file.FilePath);
            //203   get file dir
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, path, 200, 203);
        }

        private void DeleteItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            file = detailListView.SelectedItem as FileObject;
            int index = detailListView.SelectedIndex;
      
            
            Console.WriteLine(file.FilePath);
            byte[] path = System.Text.Encoding.Default.GetBytes(file.FilePath);
            //204   delete the file
            App.SplitSendData(App.server.userTokensList[0].ConnetSocket, path, 200, 204);
           file.Source.RemoveAt(index);

        }
        //写个委托把新名字传回去

        private FileObject file;
        private void ReNameItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            file = detailListView.SelectedItem as FileObject;
            Console.WriteLine(file.FilePath);
            byte[] path = System.Text.Encoding.Default.GetBytes(file.FilePath);
            ReNameWin re=new ReNameWin(file.FilePath);
            re.UpdateUiDel += this.UpdateUi;
            
            re.ShowDialog();
            
            //205   rename the file

        }

        private void UpdateUi(string newname)
        {
            file.FileName = newname;
            file.FilePath = Path.GetDirectoryName(file.FilePath) + "\\" + newname;
        }
    }
}
