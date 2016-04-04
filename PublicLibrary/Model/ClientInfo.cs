using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary.Model
{
    public class ClientInfo : INotifyPropertyChanged
    {
      private string m_ip;

      public String Ip
      {

          get
          {
              return m_ip;
          }
          set
          {
              if (value != this.m_ip)
              {
                  m_ip = value;
                  RaisePropertyChanged("Ip");
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
