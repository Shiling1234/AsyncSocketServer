using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary.Model
{
[Serializable]
   public class ServerInfo:INotifyPropertyChanged
    {
  
       private string m_name;
       private int m_id;
       private string m_description;
       private string m_startMode;
       private string m_status;
       private string m_path;

       public string Name
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

       public string Path
       {
           get { return m_path; }
           set
           {
               if (value != this.m_path)
               {
                   m_path = value;
                   RaisePropertyChanged("Path");
               }
           }
       }

       public int ProcessId
       {
           get { return m_id; }
           set
           {
               if (value != this.m_id)
               {
                   m_id = value;
                   RaisePropertyChanged("ProcessId");
               }
           }
       }
       public string Description
       {
           get { return m_description; }
           set
           {
               if (value != this.m_description)
               {
                   m_description = value;
                   RaisePropertyChanged("Description");
               }
           }
       }


       public string StartMode
       {
           get { return m_startMode; }
           set
           {
               if (value != this.m_startMode)
               {
                   m_startMode = value;
                   RaisePropertyChanged("StartMode");
               }
           }
       }

       public string Status
       {
           get { return m_status; }
           set
           {
               if (value != this.m_status)
               {
                   m_status = value;
                   RaisePropertyChanged("Status");
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
