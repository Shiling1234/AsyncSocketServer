using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PublicLibrary;
using AsyncSocketServer.Common;

namespace Client.Common
{
   public abstract class ProtocolBase
    {
        public abstract byte[] GenerateMsg(String parameters);
        DynamicBufferManager DynamicBufferManager { get; set; }

        public ProtocolBase()
        {
            DynamicBufferManager = new DynamicBufferManager(1024 * 4);
        }


        public void SplitSendData(Socket client, byte[] bytes, int singlePacketLen, int PacketType)
        {
            //1
            int packetNum = bytes.Length / singlePacketLen + 1;
            //bytes.Length
            int lastPacketLen =bytes.Length % singlePacketLen;
            //1
            int MaxID = lastPacketLen == 0 ? packetNum - 1 : packetNum;
            for (int i = 0; i < packetNum; i++)
            {
                MessageFormat mf = new MessageFormat();
                mf.singlePacketLen = i == packetNum - 1 ? lastPacketLen + 20 : singlePacketLen + 20;
                mf.totoalLen = bytes.Length + 20 * packetNum;
                mf.ID = i + 1;
                mf.PacketType = PacketType;
                mf.maxID = MaxID;
                //最后一包是空字节，不用发，发了也是20个协议字。而且ID会大于MAXID，服务端根本不解析
                if (mf.maxID == 1)
                {
                    lastPacketLen = bytes.Length;
                }
                if (i == mf.maxID)
                {
                    break;
                }
                //先进行构造头协议,然后写入数据
                PacketTool.StartPacket(mf, DynamicBufferManager);
               
                if ((i + 1) == mf.maxID)
                {
                    PacketTool.EndtPacket(DynamicBufferManager, bytes, i * singlePacketLen, lastPacketLen);
                }
                else
                {
                    PacketTool.EndtPacket(DynamicBufferManager, bytes, i * singlePacketLen, singlePacketLen);
                }
                // byte[] singlebytes = new byte[mf.singlePacketLen];
                //  Array.Copy(bytes, i * singlePacketLen, singlebytes, 20, singlebytes.Length);
                App.log.InfoFormat("准备发送：singlePacketLen:{0},totoalLen:{1},ID:{2},maxID:{3},meessageType:{4}", mf.singlePacketLen, mf.totoalLen,mf.ID, mf.maxID, (MessageType)mf.PacketType);

                client.Send(DynamicBufferManager.Buffer, DynamicBufferManager.DataCount, SocketFlags.None);
            //    App.log.InfoFormat("发送完成{0}个字节", DynamicBufferManager.DataCount);
                DynamicBufferManager.Clear();
            }
        }
    }

}
