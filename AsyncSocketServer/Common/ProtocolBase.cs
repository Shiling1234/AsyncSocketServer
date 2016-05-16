using AsyncSocketServer.AsyncSocketCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocketServer.Common
{
    public enum MessageType
    {
        Message = 1,
        GetFileList = 200,
        ReceieveFile = 201,
        FileTransferCompleted = 202,
        GetRegistryList = 300,
        GetProcessList = 400,
        GetServerList = 500,
        GetStartUpList = 600,
        GetRemoteDestop = 700
    }
    public interface ProtocolBase
    {
         void DealData(byte[] data,MessageType message);
        
    }
}
