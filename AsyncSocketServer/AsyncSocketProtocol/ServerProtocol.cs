using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using AsyncSocketServer.Common;
using PublicLibrary.Model;

namespace AsyncSocketServer.AsyncSocketProtocol
{
    public class ServerProtocol:ProtocolBase
    {
        public event EventHandler<ObservableCollection<ServerInfo>> GetServerList;
      

        public void DealData(byte[] data, MessageType mtType)
        {
            if (mtType == MessageType.GetServerList)
            {
                ShowServerList(data);
            }
        }

        private void ShowServerList(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;
            List<ServerInfo> serverInfos = bf.Deserialize(ms) as List<ServerInfo>;
            if (GetServerList != null)
            {
                GetServerList(this, new ObservableCollection<ServerInfo>(serverInfos));
            }
        }
    }
}
