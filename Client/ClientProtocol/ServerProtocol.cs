using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Common;
using PublicLibrary.Model;


namespace Client.ClientProtocol
{
    public class ServerProtocol : ProtocolBase
    {
        public override byte[] GenerateMsg(String data)
        {
            List<ServerInfo> serverInfos = new List<ServerInfo>();
            MemoryStream ms = new MemoryStream();
            if (data == "ServerList")
            {
               // ServiceController sc = new ServiceController(serverInfo);
        ServiceController[] scs=    ServiceController.GetServices();
        foreach (ServiceController service in scs)
        {
            Console.WriteLine(service.Status.ToString());
            
        }
                try
                {
                    ManagementObject a=new ManagementClass();
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_Service");

                    foreach (var queryObj in searcher.Get())
                    {
                        
                        ServerInfo singServerInfo=new ServerInfo();
                        singServerInfo.Name = queryObj["Name"].ToString();
                        if (queryObj["Description"]!= null)
                        {
                            singServerInfo.Description = queryObj["Description"].ToString();
                        }
                        singServerInfo.ProcessId = int.Parse(queryObj["ProcessId"].ToString());
                        singServerInfo.StartMode = queryObj["StartMode"].ToString();
                        singServerInfo.Status = new ServiceController(singServerInfo.Name).Status.ToString();
                       // singServerInfo.Status = queryObj["Status"].ToString();
                        singServerInfo.Path = queryObj["PathName"].ToString();
                        serverInfos.Add(singServerInfo);
                    }
           
                }
                catch (ManagementException e)
                {
                    Console.WriteLine
                    ("An error occurred while querying for WMI data: " + e.Message);
                }

                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, serverInfos);
            }

            return ms.ToArray();
        }
        public void SendRequsetMsg(string msgType)
        {
            byte[] bytes = this.GenerateMsg(msgType);
            this.SplitSendData(App.client, bytes, 1024 * 1024, 500);
        }

        internal void StartServer(string serverInfo)
        {
            ServiceController sc = new ServiceController(serverInfo);
            sc.WaitForStatus(ServiceControllerStatus.Stopped);
            try
            {
                sc.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            } 
        }

        internal void StopServer(string stopServer)
        {
            ServiceController sc = new ServiceController(stopServer);
            sc.WaitForStatus(ServiceControllerStatus.Running);

            try
            {
                sc.Stop();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            } 
        }

        internal void PauseServer(string pauseServer)
        {
            ServiceController sc = new ServiceController(pauseServer);
            sc.WaitForStatus(ServiceControllerStatus.Running);
            try
            {
                sc.Pause();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        internal void ContinueServer(string continueServer)
        {
            ServiceController sc = new ServiceController(continueServer);
            sc.WaitForStatus(ServiceControllerStatus.Paused);
            try
            {
                sc.Continue();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        internal void RefreshServer(string refreshServer)
        {
            ServiceController sc = new ServiceController(refreshServer);
            sc.Refresh();
        }

        internal void GetServerAtrribute(string serverAtrribute)
        {
            ArtibuteDialog.ShowFileProperties(serverAtrribute);
        }

        internal void SetServerToManual(string manualServer)
        {
            changeServiceStartMode("admin1508191457", manualServer, "Manual");
        }

        internal void SetServerToAuto(string autolServer)
        {
            changeServiceStartMode("admin1508191457", autolServer, "Automatic");
        }
        public void changeServiceStartMode(string hostname, string serviceName, string startMode)
        {
            try
            {
                ManagementObject classInstance =
                            new ManagementObject(@"\\" + hostname + @"\root\cimv2",
                            "Win32_Service.Name='" + serviceName + "'",
                            null);

                // Obtain in-parameters for the method
                ManagementBaseObject inParams =
                    classInstance.GetMethodParameters("ChangeStartMode");

                // Add the input parameters.
                inParams["StartMode"] = startMode;

                // Execute the method and obtain the return values.
                ManagementBaseObject outParams =
                    classInstance.InvokeMethod("ChangeStartMode", inParams, null);

                // List outParams
                //Console.WriteLine("Out parameters:");
                //richTextBox1.AppendText(DateTime.Now.ToString() + ": ReturnValue: " + outParams["ReturnValue"]);
            }
            catch (ManagementException err)
            {
                //richTextBox1.AppendText(DateTime.Now.ToString() + ": An error occurred while trying to execute the WMI method: " + err.Message);
            }
        }
    }
}
