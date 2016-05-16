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

        public DynamicBufferManager DynamicBufferManager { get; set; }
        private byte[] receieveBuffer { get; set; }
        public DynamicBufferManager DataBufferManager { get; set; }
        public SocketAsyncEventArgs socketAsyncEventArgs { get; set; }

        public ProtocolBase TransportProtocol { get; set; }

        public AsyncUserToken(int bufferSize)
        {
            DynamicBufferManager = new DynamicBufferManager(1024 * 4);
            DataBufferManager = new DynamicBufferManager(1024 * 4);
            socketAsyncEventArgs = new SocketAsyncEventArgs();
            socketAsyncEventArgs.UserToken = this;
            receieveBuffer = new byte[bufferSize];
            socketAsyncEventArgs.SetBuffer(receieveBuffer, 0, receieveBuffer.Length);


        }

    }
}
