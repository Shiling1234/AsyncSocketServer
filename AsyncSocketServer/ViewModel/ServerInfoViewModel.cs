using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PublicLibrary.Model;

namespace AsyncSocketServer.ViewModel
{
    public class ServerInfoViewModel
    {
        public ServerInfo ServerInfo { get; set; }
        public ServerInfoViewModel(string name,int id,string description,string startMode,string status)
        {
            ServerInfo=new ServerInfo()
            {
                Description = description,
                Name = name,
                ProcessId = id,
                StartMode = startMode,
                Status = status
            };
        }
    }
}
