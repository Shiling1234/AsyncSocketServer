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

namespace AsyncSocketServer.AsyncSocketProtocol
{
   public class RegistryProtocol:ProtocolBase
    {
        public event EventHandler<ObservableCollection<RegistryInfo>> GetSubRegistryList;
       

        public void DealData(byte[] data, MessageType message)
        {
            ShowRegeditList(data);
        }

        private void ShowRegeditList(byte[] data)
        {
         
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(data);
        ms.Position = 0;
        List<RegistryInfo> registryInfos = bf.Deserialize(ms) as List<RegistryInfo>;
        if (GetSubRegistryList != null)
        {
            GetSubRegistryList(this, new ObservableCollection<RegistryInfo>(registryInfos));
            
        }
    }
    }
}
