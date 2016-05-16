using PublicLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
   public class PacketTool
    {
        public static void StartPacket(MessageFormat mf, DynamicBufferManager dymicBufferMgr)
        {         
            dymicBufferMgr.WriteInt(mf.singlePacketLen, false);
            dymicBufferMgr.WriteInt(mf.totoalLen, false);
            dymicBufferMgr.WriteInt(mf.ID, false);
            dymicBufferMgr.WriteInt(mf.maxID, false);
            dymicBufferMgr.WriteInt(mf.PacketType, false);        
        }
        public static void EndtPacket( DynamicBufferManager dymicBufferMgr,byte[] sendBytes,int offset,int count)
        {
            dymicBufferMgr.WriteBuffer(sendBytes,offset,count);
        }

    }
}
