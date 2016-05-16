using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using AsyncSocketServer.AsyncSocketProtocol;
using PublicLibrary;
using System.ComponentModel;
using AsyncSocketServer.Common;

namespace AsyncSocketServer.AsyncSocketCore
{

    public class ProtocolIvokeElment
    {
        public MessageProtocol MessageProtocol = new MessageProtocol();
        public FileManagerProtocol fileManagerProtocol = new FileManagerProtocol();
        public RegistryProtocol registryProtocol = new RegistryProtocol();
        public ProcessProtocol processProtocol = new ProcessProtocol();
        public ServerProtocol serverProtocol = new ServerProtocol();
        public StartUpProtocol StartUpProtocol = new StartUpProtocol();
        public RemoteDestopProtocol remoteDestopProtocol = new RemoteDestopProtocol();
        public AsyncUserToken UserToken;
        public ProtocolIvokeElment()
        {
            MessageProtocol = new MessageProtocol();
        }

        /// <summary>
        /// 分析半包，粘包，拿到有效数据
        /// </summary>
        public void AnalyzePartMessage()
        {
            bool continueDeal = true;
            while (continueDeal)
            {
                if (UserToken.DynamicBufferManager.DataCount > 5 * sizeof(int))
                {
                    MessageFormat packetMsg=  AnalyzeProtocol();
                    if (UserToken.DynamicBufferManager.DataCount >= packetMsg.singlePacketLen)
                    {
                        //  App.log.InfoFormat("singlePacketLen:{0},totoalLen:{1},ID:{2},maxID:{3},meessageType:{4}", singlePacketLen, totalLen, ID, maxID, messageType);
                        if (packetMsg.ID == 1)
                        {
                            BindProtocol(UserToken, packetMsg.PacketType);
                        }
                         UserToken.DataBufferManager.WriteBuffer(UserToken.DynamicBufferManager.Buffer, 20, packetMsg.singlePacketLen - 20);
                        UserToken.DynamicBufferManager.Clear(packetMsg.singlePacketLen);
                        if (packetMsg.ID == packetMsg.maxID)
                        {
                          //  UserToken.TransportProtocol.DealData(UserToken.DynamicBufferManager.Buffer, (MessageType)packetMsg.PacketType, 20, packetMsg.singlePacketLen - 20);
                           // UserToken.DynamicBufferManager.Clear(packetMsg.singlePacketLen);
                            //   App.log.InfoFormat("收到{0}大小的{1}的数据包,准备解析",UserToken.DataBufferManager.DataCount, ((MessageType)messageType));
                            UserToken.TransportProtocol.DealData(UserToken.DataBufferManager.Buffer, (MessageType)packetMsg.PacketType);
                            UserToken.DataBufferManager.Clear();
                            //   App.log.InfoFormat("数据包解析完成");
                        }
                    }
                    else
                    {
                        continueDeal = false;
                    }
                }
                else
                {
                    continueDeal = false;

                }
            }

        }


    
        public MessageFormat AnalyzeProtocol()
        {
            MessageFormat result = new MessageFormat();
            result.singlePacketLen = BitConverter.ToInt32(UserToken.DynamicBufferManager.Buffer, 0);
            result.totoalLen = BitConverter.ToInt32(UserToken.DynamicBufferManager.Buffer, 4);
            result.ID = BitConverter.ToInt32(UserToken.DynamicBufferManager.Buffer, 8);
            result.maxID = BitConverter.ToInt32(UserToken.DynamicBufferManager.Buffer, 12);
            result.PacketType = BitConverter.ToInt32(UserToken.DynamicBufferManager.Buffer, 16);
            BindProtocol(UserToken, result.PacketType);
            return result;
        }

        /// <summary>
        /// 根据传输数据的首位为当前token绑定协议
        /// </summary>
        /// <param name="ValidData"></param>
        public void BindProtocol(AsyncUserToken userToken, int messageType)
        {
            MessageType mt = (MessageType)messageType;
            switch (mt)
            {
                case MessageType.GetFileList:
                    UserToken.TransportProtocol = fileManagerProtocol;
                    break;
                case MessageType.FileTransferCompleted:
                    UserToken.TransportProtocol = fileManagerProtocol;
                    break;
                case MessageType.ReceieveFile:
                    UserToken.TransportProtocol = fileManagerProtocol;
                    break;
                case MessageType.GetProcessList:
                    UserToken.TransportProtocol = processProtocol;
                    break;
                case MessageType.GetRegistryList:
                    UserToken.TransportProtocol = registryProtocol;
                    break;
                case MessageType.GetRemoteDestop:
                    UserToken.TransportProtocol = remoteDestopProtocol;
                    break;
                case MessageType.GetServerList:
                    UserToken.TransportProtocol = serverProtocol;
                    break;
                case MessageType.GetStartUpList:
                    UserToken.TransportProtocol = StartUpProtocol;
                    break;
            }
        }
    }
}
