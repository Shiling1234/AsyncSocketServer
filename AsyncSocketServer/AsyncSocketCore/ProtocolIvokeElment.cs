using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using AsyncSocketServer.AsyncSocketProtocol;
using PublicLibrary;

namespace AsyncSocketServer.AsyncSocketCore
{


    enum OperateType
    {
        Message = 1,
        File = 2,
    }

    public class ProtocolIvokeElment
    {
        //public event EventHandler<ObservableCollection<FileObject>> GetFileList;
        //public delegate void MessageHandler(string message);

        public MessageProtocol MessageProtocol;
        public readonly FileManagerProtocol fileManagerProtocol = new FileManagerProtocol();
        public readonly RegistryProtocol registryProtocol = new RegistryProtocol();
        public readonly ProcessProtocol processProtocol = new ProcessProtocol();
        public readonly ServerProtocol serverProtocol = new ServerProtocol();
        public StartUpProtocol StartUpProtocol = new StartUpProtocol();
        public RemoteDestopProtocol remoteDestopProtocol=new RemoteDestopProtocol();
        public AsyncUserToken UserToken;
        byte[] data;
        int offset = 0;

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
                if (UserToken.ReceieveBufferOffset > 5 * sizeof(int))
                {
                    int singlePacketLen = BitConverter.ToInt32(UserToken.ReceieveByteBuffer, 0);
                    int totalLen = BitConverter.ToInt32(UserToken.ReceieveByteBuffer, 4);
                    int ID = BitConverter.ToInt32(UserToken.ReceieveByteBuffer, 8);
                    int maxID = BitConverter.ToInt32(UserToken.ReceieveByteBuffer, 12);
                    int messageType = BitConverter.ToInt32(UserToken.ReceieveByteBuffer, 16);
                    //全部分包一次性被接收完毕
                    if (UserToken.ReceieveBufferOffset >= singlePacketLen)
                    {

                        if (ID == 1)
                        {
                            data = new byte[totalLen - 20 * maxID];
                        }

                        Array.Copy(UserToken.ReceieveByteBuffer, 20, data, offset, singlePacketLen - 20);
                        offset += singlePacketLen - 20;
                        //复制完毕,缓冲池前移动
                        Array.Copy(UserToken.ReceieveByteBuffer, singlePacketLen, UserToken.ReceieveByteBuffer, 0,
                        UserToken.ReceieveBufferOffset - singlePacketLen);
                        //读取完成后将offset前移。
                        UserToken.ReceieveBufferOffset -= singlePacketLen;

                        if (ID == maxID)
                        {
                            offset = 0;
                            // BindProtocol(messageType);
                            if (messageType == 1)
                            {
                                if (this.MessageProtocol == null)
                                {
                                    MessageProtocol = new MessageProtocol();
                                }

                                MessageProtocol.DealData(data);
                            }
                            //获取文件列表
                            if (messageType == 200)
                            {
                                fileManagerProtocol.DealData(data);
                            }
                            //接收文件
                            if (messageType == 201)
                            {
                                fileManagerProtocol.ReceieveFile(data);
                            }
                            if (messageType == 202)
                            {
                                fileManagerProtocol.FileTransferCompleted(data);
                            }
                            if (messageType == 300)
                            {
                                //处理注册表
                                registryProtocol.DealData(data);
                            }
                            if (messageType == 400)
                            {
                                processProtocol.DealData(data);
                            }
                            if (messageType == 500)
                            {
                                serverProtocol.DealData(data);
                            }
                            if (messageType == 600)
                            {
                                StartUpProtocol.DealData(data);
                            }
                            if (messageType == 700)
                            {
                             remoteDestopProtocol.DealData(data);   
                            }
                        }
                    }
                    else
                    {
                        //发送的包最后有余下来的一点字节,也不用处理了，继续等待
                        continueDeal = false;
                    }
                }
                else
                {
                    continueDeal = false;

                }
            }

        }


        /// <summary>
        /// 根据传输数据的首位为当前token绑定协议
        /// </summary>
        /// <param name="ValidData"></param>
        public void BindProtocol(int messageType)
        {

            //switch (messageType)
            //{
            //    //case 0:
            //    //    UserToken.TransportProtocol = new FileManagerProtocol();
            //    //    break;
            //    //case 1:
            //    //    UserToken.TransportProtocol = new FileManagerProtocol();
            //    //    break;
            //    //case 2:
            //    //    UserToken.TransportProtocol = new RegistryProtocol();
            //    //    break;
            //}


        }









    }
}
