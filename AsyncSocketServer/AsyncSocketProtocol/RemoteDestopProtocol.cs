using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer.AsyncSocketProtocol
{
    public class RemoteDestopProtocol
    {
        public event EventHandler<MemoryStream> GetDestopImage;
        public void DealData(byte[] ValidData)
        {


            MemoryStream ms = new MemoryStream(ValidData);
            ms.Position = 0;
         //   List<StartUpItemInfo> startUpItemInfos = bf.Deserialize(ms) as List<StartUpItemInfo>;
            if (GetDestopImage != null)
            {
                GetDestopImage(this, ms);
            }
        }
    }
}
