using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary
{
    public static class PacketTool
    {
        /// <summary>
        /// 将要发送的字节数组进行格式化
        /// </summary>
        /// <param name="mf">定义包头</param>
        /// <param name="primateBytes">原始字节数据</param>
        /// <returns></returns>
        public static byte[] PacketToBytes(MessageFormat mf, byte[] primateBytes)
        {
            byte[] singlePacketLen = BitConverter.GetBytes(mf.singlePacketLen);
            byte[] totoalLen = BitConverter.GetBytes(mf.totoalLen);
            byte[] ID = BitConverter.GetBytes(mf.ID);
            byte[] maxID = BitConverter.GetBytes(mf.maxID);
            byte[] PacketType = BitConverter.GetBytes(mf.PacketType);
            byte[] sendDataBytes = new byte[mf.singlePacketLen];
            Array.Copy(singlePacketLen, 0, sendDataBytes, 0, 4);
            Array.Copy(totoalLen, 0, sendDataBytes, 4, 4);
            Array.Copy(ID, 0, sendDataBytes, 8, 4);
            Array.Copy(maxID, 0, sendDataBytes, 12, 4);
            Array.Copy(PacketType, 0, sendDataBytes, 16, 4);

            Array.Copy(primateBytes, 0, sendDataBytes, 20, primateBytes.Length);
            return sendDataBytes;
        }
    }
}
