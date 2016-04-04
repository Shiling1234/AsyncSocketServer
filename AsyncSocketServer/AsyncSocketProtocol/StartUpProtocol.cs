using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PublicLibrary.Model;

namespace AsyncSocketServer.AsyncSocketProtocol
{
   public class StartUpProtocol
    {
        public event EventHandler<ObservableCollection<StartUpItemInfo>> GetStartUpItems;
        public void DealData(byte[] ValidData)
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(ValidData);
            ms.Position = 0;
            List<StartUpItemInfo> startUpItemInfos = bf.Deserialize(ms) as List<StartUpItemInfo>;
            if (GetStartUpItems != null)
            {
                GetStartUpItems(this, new ObservableCollection<StartUpItemInfo>(startUpItemInfos));
            }
        }
    }
}
