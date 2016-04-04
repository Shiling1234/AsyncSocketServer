using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;


namespace 文件属性对话框
{
    class Program
    {
        static void Main(string[] args)
        {
           
            //ArtibuteDialog.ShowFileProperties(@"C:\Users\Administrator\Desktop\1.exe");
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_Service");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("Win32_Service instance");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("PathName: {0}", queryObj["PathName"]);
                }
            }
            catch (ManagementException e)
            {
               // MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
