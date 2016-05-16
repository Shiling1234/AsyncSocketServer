using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using AsyncSocketServer.Common;
using PublicLibrary;
using PublicLibrary.Model;

namespace AsyncSocketServer.AsyncSocketProtocol
{
  
  public  class ProcessProtocol:ProtocolBase
    {
      public event EventHandler<ObservableCollection<ProcessInfo>> GetProcessList;
      public  void DealData(byte[] ValidData,MessageType msType)
        { 
           if (msType == MessageType.GetProcessList)
            {
                ShowProcessList(ValidData);
            }     
      }
        private void ShowProcessList(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;
            List<ProcessInfo> processInfos = bf.Deserialize(ms) as List<ProcessInfo>;
            if (GetProcessList != null)
            {
                GetProcessList(this, new ObservableCollection<ProcessInfo>(processInfos));
            }
        }
    }
}
