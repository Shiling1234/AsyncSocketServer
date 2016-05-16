using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PublicLibrary;

namespace AsyncSocketServer.AsyncSocketCore
{
    public class Server
    {
        public ProtocolIvokeElment ProtocolIvokeElment = new ProtocolIvokeElment();
        private int m_numConnections;
        private int m_receiveBufferSize;
       

        const int opsToPreAlloc = 2;
        Socket listenSocket;

        AsyncUserTokenPool m_asyncUserTokenPool;
        int m_totalBytesRead;
        int m_numConnectedSockets;
        private Semaphore m_maxNumberAcceptedClients;
        public List<AsyncUserToken> userTokensList = new List<AsyncUserToken>();
        public Server(int numConnections, int receiveBufferSize)
        {
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;
            m_numConnections = numConnections;//最大可连接数
            m_receiveBufferSize = receiveBufferSize;// buffer size to use for each socket I/O operation 
            // allocate buffers such that the maximum number of sockets can have one outstanding read and  
            //write posted to the socket simultaneously  
            //初始化的时候buffer是设置的e.buffer设置为两倍，因为读写两种操作
            m_asyncUserTokenPool = new AsyncUserTokenPool(numConnections);
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
        }



        // 
        public void Init()
        {//初始化池子
    

            AsyncUserToken asyncUserToken;

            for (int i = 0; i < m_numConnections; i++)
            {

                asyncUserToken = new AsyncUserToken(m_receiveBufferSize);
                asyncUserToken.socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_asyncUserTokenPool.Push(asyncUserToken);
             

            }

        }

        public void Start(IPEndPoint localEndPoint)
        {

            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);

            listenSocket.Listen(100);


            StartAccept(null);
        }


        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                //socket必须被清除，这样才能让上下文对象可以被重用
                acceptEventArg.AcceptSocket = null;
            }

            m_maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                //同步
                ProcessAccept(acceptEventArg);
            }
        }


        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            //连接数增加
            Interlocked.Increment(ref m_numConnectedSockets);
            Console.WriteLine("Client connection accepted. There are {0} clients connected to the server",
                m_numConnectedSockets);
            AsyncUserToken userToken = m_asyncUserTokenPool.Pop();
            
            //拿到当前正在监听连接的Socket（回调函数拿Socket的方式）
            userToken.ConnetSocket = e.AcceptSocket;
            userTokensList.Add(userToken);
            // As soon as the client is connected, post a receive to the connection 
            //同步方式的话readEventArgs是不会把当前对象带走的,所以如果异步，那么回调函数e直接拿到，如果同步
            //自己传参
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(userToken.socketAsyncEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(userToken.socketAsyncEventArgs);
            }
            StartAccept(e);
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }


        private void ProcessReceive(SocketAsyncEventArgs e)
        {

            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {

                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);          
                token.DynamicBufferManager.WriteBuffer(e.Buffer, 0, e.BytesTransferred);    
                ProtocolIvokeElment.UserToken = token;
                  ProtocolIvokeElment.AnalyzePartMessage();
                bool willRaiseEvent = token.ConnetSocket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

       

        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {


                AsyncUserToken token = (AsyncUserToken)e.UserToken;

            }
            else
            {
                Console.WriteLine("close");
                CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;
            token.DynamicBufferManager.Clear();
            token.socketAsyncEventArgs = null;         
            try
            {
                token.ConnetSocket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed 
            catch (Exception ex) {
                App.log.Error(ex);         
            } 
            Interlocked.Decrement(ref m_numConnectedSockets);
            m_maxNumberAcceptedClients.Release();         
            App.log.InfoFormat("A client has been disconnected from the server.There are {0}" +
            "clients connected to the server", m_numConnectedSockets);
            m_asyncUserTokenPool.Push(token);
        }
    }
}
