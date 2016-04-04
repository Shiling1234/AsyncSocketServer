using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PublicLibrary.Model;

namespace AsyncSocketServer.ViewModel
{
   public class StartUpItemInfoViewModel
   {
       public StartUpItemInfo StartUpItemInfo { get ; set; }

       public StartUpItemInfoViewModel(string caption,string command,string location)
       {
           this.StartUpItemInfo = new StartUpItemInfo()
           {
                Caption =caption,
                Command = command,
                Location = location
           };

       }

   }
}
