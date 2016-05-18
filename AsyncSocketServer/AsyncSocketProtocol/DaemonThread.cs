using AsyncSocketServer.AsyncSocketCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncSocketServer.AsyncSocketProtocol
{
    class DaemonThread
    {
        private Thread m_thread;
        private Server m_asyncSocketServer;
        public DaemonThread(Server asyncSocketServer)
        {
            m_asyncSocketServer = asyncSocketServer;
            m_thread = new Thread(DaemonThreadStart);
         
            m_thread.Start();
        }

        public void DaemonThreadStart()
        {
            while (m_thread.IsAlive)
            {


                AsyncUserToken[] userTokenArray = new AsyncUserToken[m_asyncSocketServer.userTokensList.Count];
                m_asyncSocketServer.userTokensList.CopyTo(userTokenArray);
                for (int i = 0; i < userTokenArray.Length; i++)
                {
                    if (!m_thread.IsAlive)
                        break;
                    try
                    {
                        //if ((DateTime.Now - userTokenArray[i].ActiveDateTime).Milliseconds > m_asyncSocketServer.SocketTimeOutMS) //超时Socket断开
                        //{
                        //    lock (userTokenArray[i])
                        //    {
                        //        m_asyncSocketServer.CloseClientSocket(userTokenArray[i]);
                        //    }
                        //}

                        if (userTokenArray[i].ConnetSocket.Connected == false)
                        {
                            MessageBox.Show("客户端断开连接");
                        }
                    }
                    catch (Exception E)
                    {
                        App.log.ErrorFormat("Daemon thread check timeout socket error, message: {0}", E.Message);
                        App.log.Error(E.StackTrace);
                    }
                }

                for (int i = 0; i < 60 * 100 / 10; i++) //每分钟检测一次
                {
                    if (!m_thread.IsAlive)
                        break;
                    Thread.Sleep(10);
                }
            }
        }

        public void Close()
        {
            m_thread.Abort();
            m_thread.Join();
        }
    }
}
