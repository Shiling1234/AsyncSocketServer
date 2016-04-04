using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary.Model
{
    [Serializable]
    public class StartUpItemInfo:INotifyPropertyChanged
    {
       
        private string m_caption;
        private string m_command;
        private string m_location;
   
        public string Caption
        {
            get { return m_caption; }
            set
            {
                if (value != this.m_caption)
                {
                    m_caption = value;
                    RaisePropertyChanged("Caption");
                }
            }
        }

        public string Command
        {
            get { return m_command; }
            set
            {
                if (value != this.m_command)
                {
                    m_command = value;
                    RaisePropertyChanged("Command");
                }
            }
        }

        public string Location
        {
            get { return m_location; }
            set
            {
                if (value != this.m_location)
                {
                    m_location = value;
                    RaisePropertyChanged("Location");
                }
            }
        }
            

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
