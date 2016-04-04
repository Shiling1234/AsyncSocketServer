using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PublicLibrary;

namespace AsyncSocketServer.ViewModel
{
    public class FileObjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool isSpecial;
        public FileObject FileObject { get; set; }

        public ObservableCollection<FileObjectViewModel> Childen { get; set; }
        public ObservableCollection<FileObject> subFiles = new ObservableCollection<FileObject>();

        /// <summary>
        /// 开始展示所有盘符
        /// </summary>
        public static FileObjectViewModel GetInitialView()
        {

            FileObjectViewModel rootFileObjectViewModel = new FileObjectViewModel();
            rootFileObjectViewModel.Childen = new ObservableCollection<FileObjectViewModel>();
            
            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            foreach (var driveInfo in driveInfos)
            {
                FileObjectViewModel subFileObjectViewModel = new FileObjectViewModel(false, driveInfo.Name,
                    driveInfo.Name);
                //加空项显示展开箭头
                subFileObjectViewModel.FileObject.DirName = driveInfo.Name;
                subFileObjectViewModel.Childen = new ObservableCollection<FileObjectViewModel>();
                subFileObjectViewModel.Childen.Add(new FileObjectViewModel());
                rootFileObjectViewModel.Childen.Add(subFileObjectViewModel);
            }
            return rootFileObjectViewModel;
        }


        public FileObjectViewModel(bool isExpanded, string name, string path)
        {
            FileObject = new FileObject();
            FileObject.FileName = name;
            FileObject.FilePath = path;
            this.isExpanded = isExpanded;
            isSpecial = false;
            Childen = new ObservableCollection<FileObjectViewModel>();
        }

        private void Initialize()
        {
            throw new NotImplementedException();
        }

        private int i = 0;
        private void GetSubFiles(object sender, ObservableCollection<FileObject> e)
        {
            Console.WriteLine("我是{0}结点,调用{1}次", this.FileObject.FileName, i);
            Console.WriteLine(i++);

            ObservableCollection<FileObject> dirs = new ObservableCollection<FileObject>();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                foreach (var file in e)
                {
                    if (file.IsFile == true)
                    {
                        subFiles.Add(file);
                        file.Source = subFiles;
                    }
                    else
                    {
                        dirs.Add(file);
                    }
                }
            }))
            ;


            OnExpanded(dirs);
        }



        public FileObjectViewModel()
        {
            isSpecial = true;
            // TODO: Complete member initialization
            //  App.server.ProtocolIvokeElment.FileManagerProtocol.GetFileList += this.GetSubFiles;
        }

        //viewmodel附加出来的属性
        public bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                Console.WriteLine("我是结点{0},我展开了吗：{1}",FileObject.FileName,value);
                OnPropertyChanged("IsExpanded");
                if (Childen.Count == 1 && Childen[0].isSpecial)
                {
                    App.server.ProtocolIvokeElment.fileManagerProtocol.GetFileList += GetSubFiles;
                    ApplySubFiles(FileObject.FilePath);
                    Console.WriteLine("我是结点{0},请求展开",FileObject.FileName);
                }

            }
        }

        private void ApplySubFiles(string path)
        {
            foreach (var asyncUserToken in App.server.userTokensList)
            {
                App.SplitSendData(asyncUserToken.ConnetSocket, System.Text.Encoding.Default.GetBytes(path), 500, 200);
                Console.WriteLine("我是服务端,请求目录{0}下所有文件,本次发送字节量{1}", path, System.Text.Encoding.Default.GetBytes(path).Length);
            }
        }

        protected void OnExpanded(ObservableCollection<FileObject> dirs)
        {
            //是否有特殊节点

            //将要展开的节点拥有没有列举的子成员（第一次打开）

            //我们需要移除特殊节点，并将子文件夹加入到Children中


            //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Childen.Add(sub)));

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (Childen.Count == 1 && Childen[0].isSpecial)
                {
                    Childen.RemoveAt(0);
                    foreach (var dir in dirs)
                    {
                        FileObjectViewModel subDir = new FileObjectViewModel(false, string.Empty, string.Empty);
                        subDir.FileObject = dir;
                        subDir.Childen.Add(new FileObjectViewModel());

                        Childen.Add(subDir);
                    }

                    App.server.ProtocolIvokeElment.fileManagerProtocol.GetFileList -= GetSubFiles;

                }
            }));
        }



        protected virtual void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
