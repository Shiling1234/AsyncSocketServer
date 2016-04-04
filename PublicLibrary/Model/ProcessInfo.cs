using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary.Model
{
    [Serializable]
    public class ProcessInfo : INotifyPropertyChanged
    {
       

        private string m_processName;
        private int m_id;
        private int m_threadCount;
        private int m_handleCount;
        private string m_startTime;
        private int m_basePriority;
        private string m_excutePath;
        
        public String ProcessName
        {

            get
            {
                return m_processName;
            }
            set
            {
                if (value != this.m_processName)
                {
                    m_processName = value;
                    RaisePropertyChanged("ProcessName");
                }
            }
        }
        public String ExcutePath
        {

            get
            {
                return m_excutePath;
            }
            set
            {
                if (value != this.m_excutePath)
                {
                    m_excutePath = value;
                    RaisePropertyChanged("ExcutePath");
                }
            }
        }
        public int PID
        {
            get { return m_id; }
            set
            {
                if (value != this.m_id)
                {
                    m_id = value;
                    RaisePropertyChanged("PID");
                }
            }
        }
        public int ThreadCount
        {
            get { return m_threadCount; }
            set
            {
                if (value != this.m_threadCount)
                {
                    m_threadCount = value;
                    RaisePropertyChanged("ThreadCount");
                }
            }
        }

        public int HandleCount
        {
            get { return m_handleCount ; }
            set
            {
                if (value != this.m_handleCount)
                {
                    m_handleCount = value;
                    RaisePropertyChanged("HandleCount");
                }
            }
        }
        public String StartTime
        {
            get { return m_startTime; }
            set
            {
                if (value != this.m_startTime)
                {
                    m_startTime = value;
                    RaisePropertyChanged("StartTime");
                }
            }
        }

        public int BasePriority
        {
            get { return m_basePriority; }
            set
            {
                if (value != this.m_basePriority)
                {
                    m_basePriority = value;
                    RaisePropertyChanged("BasePriority");
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
