using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AsyncSocketServer.AsyncSocketCore;
using PublicLibrary;


namespace AsyncSocketServer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Server server;
        public static string downLoadPath;

        public static  void SplitSendData(Socket client, byte[] bytes, int singlePacketLen, int PacketType)
        {
           
            int len = bytes.Length;
            int packetNum = bytes.Length / singlePacketLen + 1;
            int lastPacketLen = bytes.Length % singlePacketLen;

            for (int i = 0; i < packetNum; i++)
            {
                MessageFormat mf = new MessageFormat();
                mf.singlePacketLen = i == packetNum - 1 ? lastPacketLen + 20 : singlePacketLen + 20;
                mf.totoalLen = bytes.Length + 20 * packetNum;
                mf.ID = i + 1;
                mf.PacketType = PacketType;
                mf.maxID = lastPacketLen == 0 ? packetNum - 1 : packetNum;
                byte[] singlebytes = new byte[mf.singlePacketLen - 20];
                Array.Copy(bytes, i * singlePacketLen, singlebytes, 0, singlebytes.Length);
                byte[] sendata = PublicLibrary.PacketTool.PacketToBytes(mf, singlebytes);
                client.Send(sendata, sendata.Length, SocketFlags.None);
            }
        }



        public static int GetPackNum(byte[] bytes, int singlePacketLen)
        {
            int len = bytes.Length;
            int packetNum = bytes.Length / singlePacketLen + 1;
            return packetNum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="muticast"></param>
        /// <param name="bytes"></param>
        /// <param name="singlePacketLen"></param>
        /// <param name="PacketType"></param>
        /// <param name="packetIndex">当前分包的索引，0<packetIndex<packetNum</param>
        public static void SplitSendDataUdp(UdpClient client,IPEndPoint muticast, byte[] bytes, int singlePacketLen, int PacketType,int packetIndex)
        {

            int len = bytes.Length;
            int packetNum = bytes.Length / singlePacketLen + 1;
            int lastPacketLen = bytes.Length % singlePacketLen;

           
                MessageFormat mf = new MessageFormat();
                mf.singlePacketLen = packetIndex == packetNum - 1 ? lastPacketLen + 20 : singlePacketLen + 20;
                mf.totoalLen = bytes.Length + 20 * packetNum;
                mf.ID = packetIndex + 1;
                mf.PacketType = PacketType;
                mf.maxID = lastPacketLen == 0 ? packetNum - 1 : packetNum;
                byte[] singlebytes = new byte[mf.singlePacketLen - 20];
                Array.Copy(bytes, packetIndex * singlePacketLen, singlebytes, 0, singlebytes.Length);
                byte[] sendata = PublicLibrary.PacketTool.PacketToBytes(mf, singlebytes);
                client.Send(sendata, sendata.Length, muticast);
            //    Thread.Sleep(100);
            
        }

    }
}
