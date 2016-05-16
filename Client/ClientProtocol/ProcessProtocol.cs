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
using PublicLibrary.Model;

namespace Client.ClientProtocol
{
    public class ProcessProtocol : ProtocolBase
    {

        public override byte[] GenerateMsg(String data)
        {
            List<ProcessInfo> processInfos = new List<ProcessInfo>();
            MemoryStream ms = new MemoryStream();
            if (data == "ProcessList")
            {

                try
                {
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_Process");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        ProcessInfo p=new ProcessInfo();
                        p.BasePriority = Convert.ToInt32(queryObj["Priority"]);
                        p.HandleCount = Convert.ToInt32(queryObj["HandleCount"]);
                        p.PID = Convert.ToInt32(queryObj["ProcessId"]);
                        p.ProcessName = queryObj["Name"].ToString();
                        try
                        {
                            p.StartTime = Process.GetProcessById(p.PID).StartTime.ToString();
                        }
                        catch (Exception)
                        {

                            Console.WriteLine("无权限读取");
                        }

                        p.ThreadCount = Convert.ToInt32(queryObj["ThreadCount"]);
                        if (queryObj["ExecutablePath"] != null)
                        {
                            p.ExcutePath = queryObj["ExecutablePath"].ToString();
                        }
                        processInfos.Add(p);
                    }
                }
                catch (ManagementException e)
                {
                    MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
                }
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, processInfos);
            }
            return ms.ToArray();
        }
        public void SendRequsetMsg(string msgType)
        {
            byte[] bytes = this.GenerateMsg(msgType);
            this.SplitSendData(App.client, bytes, 1024 * 1024, 400);
        }

        internal void KillProcess(string killProcessId)
        {
            try
            {
                Process p = Process.GetProcessById(int.Parse(killProcessId));
                p.Kill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        public void RefreshProcess(string refreshProcessId)
        {
            try
            {
                Process p = Process.GetProcessById(int.Parse(refreshProcessId));
                p.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }   
        }

        public void GotoProcessDir(string porcessDir)
        {
            Process.Start("Explorer.exe", "/select," + porcessDir);
        }

        public void RestartProcess(string restartPorcess)
        {
            try
            {
                string[] msg = restartPorcess.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                Process p = Process.GetProcessById(int.Parse(msg[0]));
                
                p.Kill();
                Process startProcess=new Process();
                startProcess.StartInfo = new ProcessStartInfo(msg[1]);
                startProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }   
        }

        public void OpenProcessAtributeDialg(string openArtibuteDia)
        {
            try
            {
                ArtibuteDialog.ShowFileProperties(openArtibuteDia);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}