using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary
{
    [Serializable]
   public class FileObject:INotifyPropertyChanged
   {
       private string m_filePath;
        private string m_dirName;
       private string m_fileName;
       private bool m_isfile;
        private string m_fileType;
        private ObservableCollection<FileObject> m_source;

        public ObservableCollection<FileObject> Source
        {
            get { return m_source; }
            set
            {
                if (value != this.m_source)
                {
                    m_source = value;
                    RaisePropertyChanged("Source");
                }
            }
        }
        public String FilePath
        {
            get { return m_filePath; }
            set
            {
                if (value != this.m_filePath)
                {
                    m_filePath = value;
                    RaisePropertyChanged("FilePath");
                }
            }
        }
        public String DirName
        {
            get { return m_dirName; }
            set
            {
                if (value != this.m_dirName)
                {
                    m_dirName = value;
                    RaisePropertyChanged("DirName");
                }
            }
        }
        public String FileName
        {
            get { return  m_fileName; }
            set
            {
                if (value != this.m_fileName)
                {
                    m_fileName = value;
                    RaisePropertyChanged("FileName");
                }
            }
        }
        public bool IsFile
        {
            get { return m_isfile; }
            set
            {
                if (value != this.m_isfile)
                {
                    m_isfile = value;
                    RaisePropertyChanged("IsFile");
                }
            }
        }

  

        public String FileType
        {
            get { return m_fileType; }
            set
            {
                if (value != this.m_fileType)
                {
                    m_fileType = value;
                    RaisePropertyChanged("FileType");
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
