using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AsyncSocketServer.AsyncSocketCore;
using AsyncSocketServer.Common;
using Microsoft.Win32;
using PublicLibrary;


namespace AsyncSocketServer.AsyncSocketProtocol
{
    public class FileManagerProtocol : ProtocolBase
    {
        public event EventHandler<ObservableCollection<FileObject>> GetFileList;
        public event EventHandler<string> FileTransCompleted;
        public  void DealData(byte[] ValidData,MessageType messageType)
        {
            if (messageType == MessageType.GetFileList)
            {
                ShowFileList(ValidData);
            }
            else if (messageType == MessageType.ReceieveFile)
            {

                ReceieveFile(ValidData);
            }
            else if (messageType == MessageType.FileTransferCompleted)
            {
                MessageBox.Show("transport completed");
            }
        

        }

        private void ShowFileList(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;
            List<FileObject> fileInfos = bf.Deserialize(ms) as List<FileObject>;
            if (GetFileList != null)
            {
                GetFileList(this, new ObservableCollection<FileObject>(fileInfos));
            }
        }

        private string savePath = string.Empty;
        FileStream fs = null;
        private string filePath;
        public void ReceieveFile(byte[] data)
        {

            filePath = App.downLoadPath;
            if (fs == null)
            {
                 fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            }
                if (filePath != null)
                {
                    //112                   
                    fs.Write(data,0,data.Length);
                }
            
        }

        internal void FileTransferCompleted(byte[] bytes)
        {
            fs.Dispose();
            fs.Close();
            FileTransCompleted(this, System.Text.Encoding.Default.GetString(bytes));
        }
    }


}
