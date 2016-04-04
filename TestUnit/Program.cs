using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUnit
{
    class Program
    {
        static void Main(string[] args)
        {
        //String [] test=    Directory.GetFileSystemEntries(@"f:\1");
        //    Console.WriteLine(test.Length);

            //System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            //foreach (System.IO.DriveInfo di in drives)
            //    di.DriveType==dr
            //   Console.WriteLine(di.Name);
            //new ProcessManager().GetAllProcess();
            //int i = 2846/220;
            //Console.WriteLine(i);
         //   Console.ReadKey();
            //int a = 2046/1023;
            //Console.WriteLine(sizeof(int));
            //Console.Read();
        //    string path = @"C:\NVIDIA\DisplayDriver\355.60";
        //    //Console.WriteLine( Path.GetDirectoryName(path));
        ////    Path.GetDirectoryName(path);
        //    Console.WriteLine(path.Substring(path.LastIndexOf('\\')+1));
        // //   path.Substring(path.LastIndexOf("\"));   
        //    Console.Read();
          String []files=  Directory.GetFileSystemEntries(".");
            foreach (var file in files)
            {
                //if (Path.GetExtension(file) == "")
                //{
                //    Console.WriteLine(file);
              
                //}
                //else
                //{
                //    Console.WriteLine(Path.GetExtension(file));
                //}
                if (Directory.Exists(file))
                {
                    Console.WriteLine("目录：{0}",file);
                }
                else
                {
                    Console.WriteLine("文件：{0}", file);
                }

            }
            Console.Read();
        }
    }
}
