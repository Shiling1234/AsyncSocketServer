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
        BufferManager m_bufferManager;

        const int opsToPreAlloc = 2;
        Socket listenSocket;

        SocketAsyncEventArgsPool m_readWritePool;
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
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,//read,write
                receiveBufferSize);

            m_readWritePool = new SocketAsyncEventArgsPool(numConnections);
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
        }



        // 
        public void Init()
        {//初始化池子
            m_bufferManager.InitBuffer();

            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < m_numConnections; i++)
            {

                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();
                //  manager
                m_bufferManager.SetBuffer(readWriteEventArg);
                m_readWritePool.Push(readWriteEventArg);

            }

        }

        public void Start(IPEndPoint localEndPoint)
        {

            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);

            listenSocket.Listen(100);


            StartAccept(null);


           // Console.WriteLine("Press any key to terminate the server process....");
           // Console.ReadKey();
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


            SocketAsyncEventArgs readEventArgs = m_readWritePool.Pop();
            //拿到当前正在监听连接的Socket（回调函数拿Socket的方式）
            ((AsyncUserToken)readEventArgs.UserToken).ConnetSocket = e.AcceptSocket;
            userTokensList.Add((AsyncUserToken)readEventArgs.UserToken);
            // As soon as the client is connected, post a receive to the connection 
            //同步方式的话readEventArgs是不会把当前对象带走的,所以如果异步，那么回调函数e直接拿到，如果同步
            //自己传参
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(readEventArgs);
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
            token.socketAsyncEventArgs = e;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {

                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);
                Console.WriteLine("The server has read a total of {0} bytes", m_totalBytesRead);
                //String message = Encoding.Default.GetString(e.Buffer);
                //Console.WriteLine(message);


                //这块只取出了transferred的字节，其实后边还是乱的，比如第一次发12345，第二次发78，这时候字节
                //其实是78345,此时截取transferred可以得到正确的想要的数据
                //接收到数据后不断的往receieveByte里面送，往后边跟
                //本地的一个大缓冲区
                //临时缓冲区是500，而发送单包是1024*1024，接收缓冲区也是1024*1024，当在1024*1024交接点时,不够这么多，但是
                //再加500却会超了本地缓冲区。 所以本地大缓冲区的字节量应该>=发送单包大小+临时缓冲区大小。
                if (token.ReceieveByteBuffer == null) token.ReceieveByteBuffer = new byte[1024 * 1024 * 2];

                Buffer.BlockCopy(e.Buffer, 0, token.ReceieveByteBuffer, token.ReceieveBufferOffset, e.BytesTransferred);
                //这里不断的接收,所以receieve的缓冲池会不断增大。
                token.ReceieveBufferOffset += e.BytesTransferred;
                //开始分析
                if (ProtocolIvokeElment == null)
                {
                    ProtocolIvokeElment = new ProtocolIvokeElment();
                }



                ProtocolIvokeElment.UserToken = token;
                ProtocolIvokeElment.AnalyzePartMessage();

                //String message = Encoding.Default.GetString(e.Buffer,0,e.BytesTransferred);
                //Console.WriteLine(message);
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
        //public ObservableCollection<FileObject> filelist = new ObservableCollection<FileObject>(); 
        //void ProtocolIvokeElment_GetFileList(object sender,  ObservableCollection<FileObject> e)
        //{

        //    if (AferGetMessage != null)
        //        AferGetMessage(this, e);
        //}

        //public event EventHandler<ObservableCollection<FileObject>> AferGetMessage;



        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {

                // done echoing data back to the client

                AsyncUserToken token = (AsyncUserToken)e.UserToken;

                //e.SetBuffer();
                //token.ConnetSocket.SendAsync()

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


            try
            {
                token.ConnetSocket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed 
            catch (Exception) { }
            token.ConnetSocket.Close();


            Interlocked.Decrement(ref m_numConnectedSockets);
            m_maxNumberAcceptedClients.Release();
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", m_numConnectedSockets);


            m_readWritePool.Push(e);
        }
    }
}
