using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Common;
using Microsoft.Win32;
using PublicLibrary;

namespace Client.ClientProtocol
{
    public class RegistryProtocol : ProtocolBase
    {

        RegistryHelper registryHelper = new RegistryHelper();
        List<RegistryInfo> registryInfos = new List<RegistryInfo>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">某一注册表项全称 </param>
        /// <returns></returns>
        public override System.IO.Stream PacketData(string data)
        {
            string[] roots = new string[5]
            {
            "HKEY_CLASSES_ROOT","HKEY_CURRENT_USER","HKEY_LOCAL_MACHINE",
            "HKEY_USERS","HKEY_CURRENT_CONFIG"
            };
            RegistryKey registryKey;
            //请求的是根目录
            if (roots.Contains(data))
            {
                switch (data)
                {
                    case "HKEY_CLASSES_ROOT":
                        registryKey = Registry.ClassesRoot;
                        break;
                    case "HKEY_CURRENT_USER":
                        registryKey = Registry.CurrentUser;
                        break;
                    case "HKEY_LOCAL_MACHINE":
                        registryKey = Registry.LocalMachine;
                        break;
                    case "HKEY_USERS":
                        registryKey = Registry.Users;
                        break;
                    case "HKEY_CURRENT_CONFIG":
                        registryKey = Registry.CurrentConfig;
                        break;
                    default:
                        registryKey=Registry.ClassesRoot;
                        Console.WriteLine("请求注册表项错误");
                        break;
                        
                }

                foreach (var subKey in registryKey.GetSubKeyNames())
                {
                    RegistryInfo registryInfo = new RegistryInfo();
                    registryInfo.Root = registryKey.Name;
                    try
                    {

                   
                    RegistryKey singleRegistryKey = registryKey.OpenSubKey(subKey);
                    registryInfo.Name = singleRegistryKey.Name;
                    if (registryInfo.Content == null)
                    {
                        registryInfo.Content = new Dictionary<string, string>();
                    }
                    foreach (var valueName in singleRegistryKey.GetValueNames())
                    {
                        string value = singleRegistryKey.GetValue(valueName).ToString();
                      
                        registryInfo.Content.Add(valueName, value);
                    }
                    registryInfos.Add(registryInfo);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
                //返成流
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, registryInfos);
                return ms;



            }
            else
            {

                //非根目录
                //  HKEY_CLASSES_ROOT\ADODB.Error\CLSID
                int index = data.IndexOf("\\");
                string root = data.Substring(0, index);
                switch (root)
                {
                    case "HKEY_CLASSES_ROOT":
                        registryKey = Registry.ClassesRoot;
                        break;
                    case "HKEY_CURRENT_USER":
                        registryKey = Registry.CurrentUser;
                        break;
                    case "HKEY_LOCAL_MACHINE":
                        registryKey = Registry.LocalMachine;
                        break;
                    case "HKEY_USERS":
                        registryKey = Registry.Users;
                        break;
                    case "HKEY_CURRENT_CONFIG":
                        registryKey = Registry.CurrentConfig;
                        break;
                    default:
                        Console.WriteLine("请求注册表项有误");
                        registryKey = Registry.ClassesRoot;
                        break;
                }

                RegistryKey selectedRegistryKey = registryKey.OpenSubKey(data.Substring(index + 1));
                foreach (var subRegistry in selectedRegistryKey.GetSubKeyNames())
                {
                    RegistryInfo registryInfo = new RegistryInfo();
                    registryInfo.Root = root;
                  
                    RegistryKey singKey = selectedRegistryKey.OpenSubKey(subRegistry);
                    registryInfo.Name = singKey.Name;
                    foreach (var valueName in singKey.GetValueNames())
                    {
                        string value = singKey.GetValue(valueName).ToString();
                        registryInfo.Content = new Dictionary<string, string>();
                        registryInfo.Content.Add(valueName, value);
                    }
                    registryInfos.Add(registryInfo);
                }
                //序列化
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, registryInfos);

                return ms;


            }
        }

        public void SendRegistry(string registryKey)
        {
            Stream s = this.PacketData(registryKey);
            this.SplitSendData(App.client, s, 1024*1024, 300);
        }

        internal void CreateNewRegedit(string newKey)
        {
            string[] msg = newKey.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string root = msg[0].Substring(0, msg[0].IndexOf('\\'));
            string subDir = msg[0].Substring(msg[0].IndexOf('\\') + 1);
            RegistryKey registryKey = Registry.ClassesRoot;
            switch (root)
            {
                case "HKEY_CLASSES_ROOT":
                    registryKey = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    registryKey = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    registryKey = Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    registryKey = Registry.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    registryKey = Registry.CurrentConfig;
                    break;
                    default:
               registryKey=Registry.ClassesRoot;
                Console.WriteLine("请求注册表项错误");
                    MessageBox.Show("新建出错~");
                  break;

            }
       //  RegistryKey subKey=  registryKey.OpenSubKey(subDir);
            RegistryKey uaes = registryKey.OpenSubKey(subDir,
                RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl); 
          
           uaes.SetValue(msg[1],msg[2]);
           //subKey.se
            uaes.Close();
        }

        internal void DeleteRegedit(string delregistry)
        {
            string[] msg = delregistry.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string root = msg[0].Substring(0, msg[0].IndexOf('\\'));
            string subDir = msg[0].Substring(msg[0].IndexOf('\\') + 1);
            RegistryKey registryKey = Registry.ClassesRoot;
            switch (root)
            {
                case "HKEY_CLASSES_ROOT":
                    registryKey = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    registryKey = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    registryKey = Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    registryKey = Registry.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    registryKey = Registry.CurrentConfig;
                    break;
                default:
                    registryKey = Registry.ClassesRoot;
                    Console.WriteLine("请求注册表项错误");
                    MessageBox.Show("新建出错~");
                    break;

            }
            RegistryKey uaes = registryKey.OpenSubKey(subDir,
               RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl); 
          uaes.DeleteValue(msg[1]);
         
        }

        internal void UpdateRegedit(string updateregistry)
        {
            string[] msg = updateregistry.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string root = msg[0].Substring(0, msg[0].IndexOf('\\'));
            string subDir = msg[0].Substring(msg[0].IndexOf('\\') + 1);
            RegistryKey registryKey = Registry.ClassesRoot;
            switch (root)
            {
                case "HKEY_CLASSES_ROOT":
                    registryKey = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    registryKey = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    registryKey = Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    registryKey = Registry.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    registryKey = Registry.CurrentConfig;
                    break;
                default:
                    registryKey = Registry.ClassesRoot;
                    Console.WriteLine("请求注册表项错误");
                    MessageBox.Show("新建出错~");
                    break;

            }
            RegistryKey uaes = registryKey.OpenSubKey(subDir,
               RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
            uaes.SetValue(msg[1],msg[2]);
        }

        internal void RenameRegedit(string renameregistry)
        {
            string[] msg = renameregistry.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string root = msg[0].Substring(0, msg[0].IndexOf('\\'));
            string subDir = msg[0].Substring(msg[0].IndexOf('\\') + 1);
            RegistryKey registryKey = Registry.ClassesRoot;
            switch (root)
            {
                case "HKEY_CLASSES_ROOT":
                    registryKey = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    registryKey = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    registryKey = Registry.LocalMachine;
                    break;
                case "HKEY_USERS":
                    registryKey = Registry.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    registryKey = Registry.CurrentConfig;
                    break;
                default:
                    registryKey = Registry.ClassesRoot;
                    Console.WriteLine("请求注册表项错误");
                    MessageBox.Show("新建出错~");
                    break;

            }
            RegistryKey uaes = registryKey.OpenSubKey(subDir,
               RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
            uaes.DeleteValue(msg[1]);
            uaes.SetValue(msg[3],msg[2]);
        }
    }
}
