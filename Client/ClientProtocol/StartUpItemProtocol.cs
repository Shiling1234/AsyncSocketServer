using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Common;
using Microsoft.Win32;
using PublicLibrary.Model;

namespace Client.ClientProtocol
{
   public class StartUpItemProtocol:ProtocolBase
    {
        public override Stream PacketData(string data)
        {
            List<StartUpItemInfo> startUpItemInfos = new List<StartUpItemInfo>();
            MemoryStream ms = new MemoryStream();
            if (data == "StartUpItems")
            {
                try
                {
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_StartupCommand");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                    
                        Console.WriteLine("-----------------------------------");
                        startUpItemInfos.Add(new StartUpItemInfo()
                        {
                            Caption = queryObj["Caption"].ToString(),
                            Command = queryObj["Command"].ToString(),
                            Location = queryObj["Location"].ToString()
                        });

                    }
                }
                catch (ManagementException e)
                {
                    //  MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
                }



                
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, startUpItemInfos);
            }

            return ms;
        }
        public void SendRequsetMsg(string msgType)
        {
            Stream s = this.PacketData(msgType);
            this.SplitSendData(App.client, s, 1024 * 1024, 600);
        }

       public void OpenStartupItemDir(string startupInfo)
       {
           Process.Start("Explorer.exe", "/select," + startupInfo);
       }

       public void OpenStartupItemAtrribute(string getStartupAtrribute)
       {
           ArtibuteDialog.ShowFileProperties(getStartupAtrribute);
       }

       internal void ForbiddenStartUpItem(string forbiddenStartUpItem)
       {
           string[] validMsg = forbiddenStartUpItem.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
           MessageBox.Show(validMsg[0]);
           MessageBox.Show(validMsg[1]);
           //HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run     dier
           string root = validMsg[1].Substring(0, validMsg[1].IndexOf('\\'));
           string subDir = validMsg[1].Substring(validMsg[1].IndexOf("\\")+1);
           RegistryKey rootKey=Registry.ClassesRoot;
           switch (root)
           {
               case "HKLM" :
                   rootKey = Registry.LocalMachine;
                   break;
               case  "HKU" :
                   rootKey = Registry.CurrentUser;
                   break;
               default:
                   MessageBox.Show("获取开机启动项位置失败");
                   break;
           }
           RegistryKey subKeys=   rootKey.OpenSubKey(subDir);
       //  string value=  subKeys.GetValue(validMsg[0]).ToString();
           foreach (var sukey in subKeys.GetValueNames())
           {
               MessageBox.Show(sukey);
           }

        //   Console.WriteLine(value);

       }
    }
}
