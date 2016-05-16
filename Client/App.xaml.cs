using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using PublicLibrary;
using log4net;
using System.IO;

namespace Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        public static  Socket client;

        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected override void OnStartup(StartupEventArgs e)
        {
            if (File.Exists("logs\\log.txt")) { File.Delete("logs\\log.txt"); }
            log4net.Config.XmlConfigurator.Configure();
            base.OnStartup(e);
            log.Info("==Startup=====================>>>");
        }
        protected override void OnExit(ExitEventArgs e)
        {
            log.Info("<<<========================End==");
            base.OnExit(e);
        }


        public static void SplitSendData(Socket client, byte[] bytes, int singlePacketLen, int PacketType)
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
               // mf.maxID = packetNum;
                mf.maxID = lastPacketLen == 0 ? packetNum - 1 : packetNum;
                if (i == mf.maxID)
                {
                    break;
                }
                byte[] singlebytes = new byte[mf.singlePacketLen - 20];
                Array.Copy(bytes, i * singlePacketLen, singlebytes, 0, singlebytes.Length);
                byte[] sendata = PublicLibrary.PacketTool.PacketToBytes(mf, singlebytes);
                client.Send(sendata, sendata.Length, SocketFlags.None);
            }
        }
    }
}
