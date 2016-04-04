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
        public void DealData(byte[] ValidData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(ValidData);
            ms.Position = 0;
            List<FileObject> fileInfos = bf.Deserialize(ms) as List<FileObject>;
            if (GetFileList != null)
            {
                GetFileList(this, new ObservableCollection<FileObject>(fileInfos));
            }

        }


        private string savePath = string.Empty;

        private string filePath;
        public void ReceieveFile(byte[] data)
        {

            filePath = App.downLoadPath;
            using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                if (filePath != null)
                {
                    fs.Write(data, 0, data.Length);
                }
            }

        }

        internal void FileTransferCompleted(byte[] bytes)
        {

            FileTransCompleted(this, System.Text.Encoding.Default.GetString(bytes));
        }
    }


}
