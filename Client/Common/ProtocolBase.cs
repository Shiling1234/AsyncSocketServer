using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PublicLibrary;

namespace Client.Common
{
   public abstract class ProtocolBase
    {
        public abstract Stream PacketData(string data);

   
        public void SplitSendData(Socket client, Stream dataStream, int singlePacketLen, int PacketType)
        {
            MemoryStream ms = dataStream as MemoryStream;
            byte[] bytes = new byte[ms.Length];
            ms.Position = 0;
            int len = dataStream.Read(bytes, 0, (int)dataStream.Length);
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
    }

}
