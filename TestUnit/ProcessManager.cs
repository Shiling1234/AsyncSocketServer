using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUnit
{
    class ProcessManager
    {
      public  void GetAllProcess()
        {

            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                Console.WriteLine(process.ProcessName);
            }
        }
    }
}
