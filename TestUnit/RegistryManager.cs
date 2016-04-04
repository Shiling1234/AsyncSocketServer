using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TestUnit
{
    class RegistryManager
    {
        #region 注册表项打开增删
        void CreateSubKey(String sub = "software\\test")
        {
            
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.CreateSubKey(sub);
        }

        void OpenRegistryKey()
        {
            //当前项如果不存在会抛出异常
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.OpenSubKey("software\\test", true);
        }

        void DeleteRegistryKey()
        {
            RegistryKey key = Registry.LocalMachine;
            key.DeleteSubKey("software\\test", true); //该方法无返回值，直接调用即可
            key.Close();
        } 
        #endregion


    }
}
