﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncSocketServer.Common;

namespace AsyncSocketServer.AsyncSocketProtocol
{
    public class MessageProtocol:ProtocolBase
    {
        public  string DealData(byte[] ValidData,MessageType messType)
        {
            String message = System.Text.Encoding.Default.GetString(ValidData);
            Console.WriteLine(message);
            return message;
        }

        void ProtocolBase.DealData(byte[] data, MessageType message)
        {
            throw new NotImplementedException();
        }
    }
}
