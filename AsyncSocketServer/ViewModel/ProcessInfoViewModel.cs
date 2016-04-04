using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PublicLibrary.Model;

namespace AsyncSocketServer.ViewModel
{
    public class ProcessInfoViewModel
    {
        public ProcessInfo ProcessInfo { get; set; }

        public ProcessInfoViewModel(int id,string processName,int threadCount,int handleCount,string startTime,int BasePriority)
        {
            ProcessInfo = new ProcessInfo
            {
                PID = id,
                ProcessName = processName,
                ThreadCount = threadCount,
                HandleCount = handleCount,
                StartTime = startTime,
                BasePriority = BasePriority
            };
        }
    }
}
