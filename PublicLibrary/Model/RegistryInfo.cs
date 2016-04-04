using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PublicLibrary
{
    [Serializable]
   public class RegistryInfo:INotifyPropertyChanged
    {
      //  private RegistryKey m_root;   //不可序列化的类
        private string m_root;//父节点
        private string m_name;  //全路径
        private Dictionary<string, string>  m_content;//当前结点下的键值对 


        public string Root
        {
            get { return m_root; }
            set
            {
                if (value != this.m_root)
                {
                    m_root = value;
                    RaisePropertyChanged("Root");
                }
            }
        }

        public String Name
        {
            get { return m_name; }
            set
            {
                if (value != this.m_name)
                {
                    m_name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public Dictionary<string,string> Content
        {
            get { return m_content; }
            set
            {
                if (value != this.m_content)
                {
                    m_content = value;
                    RaisePropertyChanged("Content");
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
