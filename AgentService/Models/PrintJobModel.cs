using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentService.Models
{
    public class PrintJobModel
    {
        public int JobId { get; set; }
        public string PrinterName { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string Protocol { get; set; }
        public string FileToBePrinted { get; set; }
        public string PrintStatus { get; set; } = " ";
    }
}
