using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AsyncSocketServer.AsyncSocketProtocol;
using AsyncSocketServer.Common;

namespace AsyncSocketServer.AsyncSocketCore
{
   public class AsyncUserToken
    {
        public Socket ConnetSocket { get; set; }

       public byte[] ReceieveByteBuffer { get; set; }
     //这个buffer尽量大一点,有可能来包太快
       public int ReceieveBufferOffset { get; set; }

     

       //传送过程中一个token的缓冲区大小
       public int BufferSize { get; set; }

       public SocketAsyncEventArgs socketAsyncEventArgs { get; set; }

       public ProtocolBase TransportProtocol { get; set; }



    }
}
