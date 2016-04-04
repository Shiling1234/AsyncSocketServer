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
using Microsoft.Win32;
using PublicLibrary;

namespace AsyncSocketServer.ViewModel
{
    public class RegistryInfoViewModel : INotifyPropertyChanged
    {

        public RegistryInfo RegistryInfoInstance { get; set; }
    

        public ObservableCollection<RegistryInfoViewModel> Childen { get; set; }
        public ObservableCollection<RegistryInfoViewModel> content = new ObservableCollection<RegistryInfoViewModel>();

        /// <summary>
        /// 开始展示所有盘符
        /// </summary>
        public static RegistryInfoViewModel GetInitialView()
        {

            RegistryInfoViewModel rootRegistryInfoViewModel = new RegistryInfoViewModel();
            rootRegistryInfoViewModel.Childen = new ObservableCollection<RegistryInfoViewModel>();
          
            string[] rootNames = {"HKEY_CLASSES_ROOT","HKEY_CURRENT_USER","HKEY_LOCAL_MACHINE","HKEY_USERS","HKEY_CURRENT_CONFIG"};
           
            foreach (var rootName in rootNames)
            {
                RegistryInfoViewModel subRegistryInfoViewModel = new RegistryInfoViewModel(false, rootName, string.Empty);
                   
                //加空项显示展开箭头
               
                subRegistryInfoViewModel.Childen = new ObservableCollection<RegistryInfoViewModel>();
                subRegistryInfoViewModel.Childen.Add(new RegistryInfoViewModel());
                rootRegistryInfoViewModel.Childen.Add(subRegistryInfoViewModel);
            }
            return rootRegistryInfoViewModel;
        }

        public bool isSpecial { get; set; }

        public RegistryInfoViewModel(bool isExpanded, string name, string root)
        {
            RegistryInfoInstance = new RegistryInfo();
            RegistryInfoInstance.Name = name;
            RegistryInfoInstance.Root = root;
            this.isExpanded = isExpanded;
            isSpecial = false;
            Childen = new ObservableCollection<RegistryInfoViewModel>();
        }

        private void Initialize()
        {
            throw new NotImplementedException();
        }

        private int i = 0;
        private void GetSubRegistry(object sender, ObservableCollection<RegistryInfo> e)
        {
            Console.WriteLine("我是{0}结点,调用{1}次", this.RegistryInfoInstance.Name, i);
            Console.WriteLine(i++);
            OnExpanded(e);
        }



        public RegistryInfoViewModel()
        {
            isSpecial = true;
            // TODO: Complete member initialization
          
        }

        //viewmodel附加出来的属性
        public bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                Console.WriteLine("我是结点{0},我展开了吗：{1}",RegistryInfoInstance.Name,value);
                OnPropertyChanged("IsExpanded");
                if (Childen.Count == 1 && Childen[0].isSpecial)
                {
                    App.server.ProtocolIvokeElment.registryProtocol.GetSubRegistryList += GetSubRegistry;
                    ApplySubRegistry(RegistryInfoInstance.Name);
                    Console.WriteLine("我是结点{0},请求展开",RegistryInfoInstance.Name);
                }

            }
        }

        private void ApplySubRegistry(string path)
        {
            foreach (var asyncUserToken in App.server.userTokensList)
            {
                App.SplitSendData(asyncUserToken.ConnetSocket, System.Text.Encoding.Default.GetBytes(path), 500,300);
                Console.WriteLine("我是服务端,请求目录{0}下所有文件,本次发送字节量{1}", path, System.Text.Encoding.Default.GetBytes(path).Length);
            }
        }

        protected void OnExpanded(ObservableCollection<RegistryInfo> registrys)
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
                    foreach (var registry in registrys)
                    {
                        RegistryInfoViewModel subRegistryViewModel = new RegistryInfoViewModel(false, string.Empty, string.Empty);
                        subRegistryViewModel.RegistryInfoInstance = registry;
                        subRegistryViewModel.Childen.Add(new RegistryInfoViewModel());

                        Childen.Add(subRegistryViewModel);
                    }

                    App.server.ProtocolIvokeElment.registryProtocol.GetSubRegistryList -= GetSubRegistry;

                }
            }));
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
