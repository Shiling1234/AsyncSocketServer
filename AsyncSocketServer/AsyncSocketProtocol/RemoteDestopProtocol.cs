using AsyncSocketServer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer.AsyncSocketProtocol
{
    public class RemoteDestopProtocol : ProtocolBase
    {
        public event EventHandler<MemoryStream> GetDestopImage;
     

        public void DealData(byte[] data, MessageType msType)
        {
            if (msType == MessageType.GetRemoteDestop)
            {
                ShowRemoteDestop(data);
            }
        }

        private void ShowRemoteDestop(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;

            if (GetDestopImage != null)
            {
                GetDestopImage(this, ms);
            }
        }
    }
}
