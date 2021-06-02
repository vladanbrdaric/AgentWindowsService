using AgentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AgentService
{
    public class PingHost
    {
        //public string SendPingSignal(string ipAddress, string fileToBePrinted)
        public PrintJobModel SendPingSignal(PrintJobModel printJob)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(printJob.IpAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {

            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            if (pingable == true)
            {
                printJob.PrintStatus = "Completed";
            }
            else
            {
                printJob.PrintStatus = "Uncompleted";
            }
            return printJob;
        }
    }
}
