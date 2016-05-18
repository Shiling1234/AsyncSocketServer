using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    class DaemonThread
    {
        private Thread m_thread;
        private Socket client;

        public DaemonThread(Socket client)
        {
            this.client = client;
            m_thread = new Thread(DaemonThreadStart);
            m_thread.Start();
        }

        public void DaemonThreadStart()
        {
            while (m_thread.IsAlive)
            {
                //AsyncSocketUserToken[] userTokenArray = null;
                //m_asyncSocketServer.AsyncSocketUserTokenList.CopyList(ref userTokenArray);
                //for (int i = 0; i < userTokenArray.Length; i++)
                //{
                //    if (!m_thread.IsAlive)
                //        break;
                //    try
                //    {
                //        if ((DateTime.Now - userTokenArray[i].ActiveDateTime).Milliseconds > m_asyncSocketServer.SocketTimeOutMS) //超时Socket断开
                //        {
                //            lock (userTokenArray[i])
                //            {
                //                m_asyncSocketServer.CloseClientSocket(userTokenArray[i]);
                //            }
                //        }
                //    }
                //    catch (Exception E)
                //    {
                //        Program.Logger.ErrorFormat("Daemon thread check timeout socket error, message: {0}", E.Message);
                //        Program.Logger.Error(E.StackTrace);
                //    }

                if (client.Connected == false)
                {
                    MessageBox.Show("客户端连接服务端失败");
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

