using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Client.Common;
using Path = System.Windows.Shapes.Path;
using PublicLibrary;

namespace Client.ClientProtocol
{
    class FileProtocol : ProtocolBase
    {
        public override Stream PacketData(String path)
        {
            List<FileObject> fileInfoList = new List<FileObject>();
            if (path == "root")
            {
                System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
                foreach (System.IO.DriveInfo di in drives)
                {
                    if (di.DriveType != DriveType.CDRom)
                    {
                        FileObject file = new FileObject();
                        file.IsFile = false;
                        file.FilePath = di.Name;
                        fileInfoList.Add(file);
                      
                    }
                }

            }
            else
            {
                String[] files;
                try
                {
                    files = Directory.GetFileSystemEntries(path);
                }
                catch
                {
                    files = new string[0];

                }
                foreach (String file in files)
                {
                    bool isFile = IsFile(file);
                    FileObject fileInfo = new FileObject();
                    if (isFile)
                    {
                        fileInfo.FileName = System.IO.Path.GetFileName(file);

                        fileInfo.FilePath = file;
                        fileInfo.IsFile = true;
                    }
                    else
                    {
                        fileInfo.DirName = file.Substring(path.LastIndexOf('\\') + 1);
                        fileInfo.FilePath = file;
                        fileInfo.IsFile = false;
                    }
                    fileInfoList.Add(fileInfo);
                  
                }
            }
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, fileInfoList);

            return ms;


        }

        public bool IsFile(string filepath)
        {
            if (Directory.Exists(filepath))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async void SendFile(string path)
        {
            byte[] bytes = new byte[1024 * 1024];
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                int len;
                while ((len = fs.Read(bytes, 0, bytes.Length)) > 0)
                {
                    byte[] sendBytes = new byte[len];
                    Buffer.BlockCopy(bytes, 0, sendBytes, 0, len);
                    App.SplitSendData(App.client, sendBytes, 1024 * 1024, 201);
                }


            }
            string sendFileOver = "文件" + path + "传送完毕";
            byte[] fileOver = System.Text.Encoding.Default.GetBytes(sendFileOver);
            App.SplitSendData(App.client, fileOver, 1024, 202);
        }

        public void RunFile(string file)
        {
            try
            {
                Process.Start(file);
            }
            catch (Exception fileOpenException)
            {
                MessageBox.Show("file run error,error message :{0}", fileOpenException.Message);
            }


        }

        public void OpenFileDir(string file)
        {
            string openDir = System.IO.Path.GetDirectoryName(file);
            Process.Start(openDir);
        }

        public void DeleteFile(string file)
        {
           File.Delete(file);
        }

        public void ReNameFile(string file)
        {
            string[] oldAndNewFile = file.Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries);
            string oldFileName = oldAndNewFile[0];
            string newFileName = System.IO.Path.GetDirectoryName(oldFileName) + "\\" + oldAndNewFile[1];
            File.Move(oldFileName,newFileName);
        }
    }
}
