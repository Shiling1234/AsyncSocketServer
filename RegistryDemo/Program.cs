using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.Remoting.Channels;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RegistryDemo
{
    class Program
    {
    
        static void Main(string[] args)
        {

         //   Regex ipRegex = new Regex("([0-9]{1,3}\\.)[3][0-9]{1-3}");
           // ^\d+\.\d+\.\d+\.\d+$
Regex ipRegex = new Regex(@"^\d+\.\d+\.\d+\.\d+$");

           bool b=  ipRegex.IsMatch("192.168.4.203");
            Console.WriteLine(b);

            ServiceController sc = new ServiceController();
            //foreach (var VARIABLE in sc.)
            //{
                
            //}
           // sc.Start();//启动
           // sc.Stop();
           // sc.Pause();
           //sc.

            Process.Start("C:\\");
              //  ShowLog("System");
        
            
            //try
            //{
            //    ManagementObjectSearcher searcher = 
            //        new ManagementObjectSearcher("root\\CIMV2", 
            //        "SELECT * FROM Win32_StartupCommand"); 

            //    foreach (ManagementObject queryObj in searcher.Get())
            //    {
            //        Console.WriteLine("-----------------------------------");
            //        Console.WriteLine("Caption: {0}", queryObj["Caption"]);
            //        Console.WriteLine("Caption: {0}", queryObj["Command"]);
            //      //  Console.WriteLine("Caption: {0}", queryObj["Description"]);
            //        Console.WriteLine("Caption: {0}", queryObj["Location"]);
            //    //    Console.WriteLine("Caption: {0}", queryObj["Name"]);
            //     //   Console.WriteLine("Caption: {0}", queryObj["User"]);
            
            //        Console.WriteLine("-----------------------------------");
                   

            //    }
            //}
            //catch (ManagementException e)
            //{
            //  //  MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
            //}

            //const string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            //using (RegistryKey startupKey = Registry.LocalMachine.OpenSubKey(runKey))
            //{
            //    var valueNames = startupKey.GetValueNames();

            //    // Name => File path
            //    Dictionary<string, string> appInfos = valueNames
            //        .Where(valueName => startupKey.GetValueKind(valueName) == RegistryValueKind.String)
            //        .ToDictionary(valueName => valueName, valueName => startupKey.GetValue(valueName).ToString());

            //    foreach (var appinfo in appInfos)
            //    {
            //        Console.WriteLine("{0}:\t{1}",appinfo.Key,appinfo.Value);
            //    }
            //}
          
            //try
            //{
            //    ManagementObjectSearcher searcher =
            //        new ManagementObjectSearcher("root\\CIMV2",
            //        "SELECT * FROM Win32_Service");

            //    foreach (ManagementObject queryObj in searcher.Get())
            //    {
                   
            //        //Console.Write( queryObj["Name"]+"\t");
            //        Console.Write(queryObj["ProcessId"] + "\t");
            //        //Console.Write(queryObj["Description"] + "\t");
            //        //Console.Write(queryObj["StartMode"] + "\t"); 
            //        //Console.Write(queryObj["Status"] + "\t");
            //    }
            //}
            //catch (ManagementException e)
            //{
            //    Console.WriteLine
            //    ("An error occurred while querying for WMI data: " + e.Message);
            //}
        
          //Process[] processes=  Process.GetProcesses();
          //  foreach (Process process in processes)
          //  {
          //      try
          //      {
          //          //Console.Write(process.ProcessName + "\t");
          //          //Console.Write(process.Id + "\t");
          //          //Console.Write(process.Threads.Count + "\t");
          //          //Console.Write(process.HandleCount + "\t");
          //          //Console.Write(process.StartTime + "\t");
          //          //Console.Write(process.BasePriority + "\t");
          //          //更改平台为64
          //          Console.Write(process.MainModule.FileName.ToString());
          //          Console.WriteLine();
          //      }
          //      catch (Exception e)
          //      {
          //          Console.WriteLine(e.Message);
                    
          //      }
               
               
           // }
        
         //RegistryKey performanceKey=   Registry.ClassesRoot;
         // RegistryKey curretnKey=  performanceKey.OpenSubKey("._sln");
         //   Console.WriteLine(curretnKey.Name);
           
         //   foreach (string subkey in curretnKey.GetValueNames())
         //   {
         //     //string value  performanceKey.GetValueNames();
         //    //   Console.WriteLine(subkey);
         //    object o=   curretnKey.GetValue(subkey,"dont have");
         //       curretnKey.GetValueKind(subkey);
                
         //   //    curretnKey.SetValue();
         //    Console.WriteLine(o.ToString());
         //   }
            Console.Read();
        }
        private static void ShowLog(string name)
        {

            EventLog DemonLog = new EventLog(name);
            foreach (EventLogEntry DemonEntry in DemonLog.Entries)
            {
                string type = DemonEntry.EntryType.ToString();
                string data = DemonEntry.TimeWritten.ToShortDateString();
                string time = DemonEntry.TimeWritten.ToLongTimeString();
                string sorce = DemonEntry.Source.ToString();
                string category = DemonEntry.Category.ToString();
                string instanceId = DemonEntry.InstanceId.ToString();
                string usrName = DemonEntry.UserName == null ? "N/A" : DemonEntry.UserName.ToString();
                Console.WriteLine("{0}\t{1}\t{2}\t{4}\t{5}\t{6}\n",type,data,time,sorce,category,instanceId,usrName);
                
                //ListViewItem li = new ListViewItem();
                //li.SubItems[0].Text = DemonEntry.EntryType.ToString();
                //li.SubItems.Add(DemonEntry.TimeWritten.ToShortDateString());
                //li.SubItems.Add(DemonEntry.TimeWritten.ToLongTimeString());
                //li.SubItems.Add(DemonEntry.Source);
                //li.SubItems.Add(DemonEntry.Category);
                //li.SubItems.Add(DemonEntry.InstanceId.ToString());
                //if (DemonEntry.UserName == null)
                //{
                //    li.SubItems.Add("N/A");
                //}
                //else
                //{
                //    li.SubItems.Add(DemonEntry.UserName);
                //}
                //li.SubItems.Add(DemonEntry.MachineName);
               // this.listView1.Items.Add(li);
            }
        }
    }
}
